using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Excess.Compiler.Tests.TestRuntime
{
    using Spawner = Func<object[], ConcurrentObject>;

    public class Node
    {
        //cache events to avoid allocations
        private readonly ConcurrentQueue<Event> _cache = new ConcurrentQueue<Event>();
        private int _cacheHits;
        private int _cacheMisses;

        private readonly ConcurrentQueue<Event> _queue = new ConcurrentQueue<Event>();

        private CancellationTokenSource _stop = new CancellationTokenSource();
        private int _stopCount = 1;
        private readonly int _threads;
        private readonly IDictionary<string, Spawner> _types;

        public Node(int threads, IDictionary<string, Spawner> types)
        {
            _threads = threads;
            _types = types;

            Debug.Assert(_threads > 0);
            createThreads(_threads);
        }

        public T Spawn<T>(params object[] args) where T : ConcurrentObject, new()
        {
            var result = new T();
            result.startRunning(this, args);
            return result;
        }

        public ConcurrentObject Spawn(string type, params object[] args)
        {
            var caller = null as Func<object[], ConcurrentObject>;
            if (!_types.TryGetValue(type, out caller))
                throw new InvalidOperationException(type + " is not defined");

            var result = caller(args);
            result.startRunning(this, args);
            return result;
        }

        public void Start(ConcurrentObject @object, params object[] args)
        {
            @object.startRunning(this, args);
        }

        public void Queue(ConcurrentObject who, Action what, Action<Exception> failure)
        {
            //_queue.Enqueue(new Event
            //{
            //    Tries = 0,
            //    Target = who,
            //    What = what,
            //    Failure = failure
            //});
            _queue.Enqueue(queueEvent(0, who, what, failure));
        }

        public void Stop()
        {
            _stopCount--;
            if (_stopCount > 0)
                return;

            _stop.Cancel();
            Thread.Sleep(1);
        }

        public void WaitForCompletion()
        {
            _stop.Token.WaitHandle.WaitOne();
        }

        public void Restart()
        {
            Debug.Assert(_stop.Token.IsCancellationRequested);
            _stop = new CancellationTokenSource();
            createThreads(_threads);
        }

        public void StopCount(int stopCount)
        {
            _stopCount = stopCount;
        }

        private void createThreads(int threads)
        {
            var cancellation = _stop.Token;
            for (var i = 0; i < threads; i++)
            {
                var thread = new Thread(() =>
                {
                    while (true)
                    {
                        Event message;
                        _queue.TryDequeue(out message);
                        if (cancellation.IsCancellationRequested)
                            break;

                        if (message == null)
                        {
                            Thread.Sleep(1);
                            continue;
                        }

                        message.Target.__enter(message.What, message.Failure);
                        _cache.Enqueue(message);
                    }
                });

                thread.Priority = ThreadPriority.AboveNormal;
                thread.Start();
            }
        }

        private Event queueEvent(int tries, ConcurrentObject target, Action action, Action<Exception> failure)
        {
            var result = null as Event;
            if (_cache.TryDequeue(out result))
            {
                _cacheHits++;
                result.Tries = tries;
                result.Target = target;
                result.What = action;
                result.Failure = failure;
                return result;
            }

            _cacheMisses++;
            return new Event
            {
                Tries = tries,
                Target = target,
                What = action,
                Failure = failure
            };
        }

        private class Event
        {
            public int Tries { get; set; }
            public ConcurrentObject Target { get; set; }
            public Action What { get; set; }
            public Action<Exception> Failure { get; set; }
        }
    }

    public class ConcurrentObject
    {
        private readonly Random __rand = new Random(); //td: test only

        private int _busy;

        private readonly Dictionary<string, List<Action>> _listeners = new Dictionary<string, List<Action>>();
        protected Node _node;

        protected Node Node { get { return _node; } }

        internal void startRunning(Node node, object[] args)
        {
            Debug.Assert(_busy == 0);
            _node = node;
            __start(args);
        }

        protected virtual void __start(object[] args)
        {
        }

        protected T spawn<T>(params object[] args) where T : ConcurrentObject, new()
        {
            return _node.Spawn<T>(args);
        }

        protected internal void __enter(Action what, Action<Exception> failure)
        {
            var was_busy = Interlocked.CompareExchange(ref _busy, 1, 0) == 1;
            if (was_busy)
            {
                _node.Queue(this, what, failure);
            }
            else
            {
                try
                {
                    what();
                }
                catch (Exception ex)
                {
                    if (failure != null)
                        failure(ex);
                    else
                        throw;
                }
                finally
                {
                    Interlocked.CompareExchange(ref _busy, 0, 1);
                }
            }
        }

        protected void __advance(IEnumerator<Expression> thread)
        {
            if (!thread.MoveNext())
                return;

            var expr = thread.Current;
            expr.Continuator = thread;
            if (expr.Start != null)
                expr.Start(expr);
        }

        protected void __listen(string signal, Action callback)
        {
            List<Action> actions;
            if (!_listeners.TryGetValue(signal, out actions))
            {
                actions = new List<Action>();
                _listeners[signal] = actions;
            }

            actions.Add(callback);
        }

        protected void __dispatch(string signal)
        {
            List<Action> actions;
            if (_listeners.TryGetValue(signal, out actions))
            {
                foreach (var action in actions)
                {
                    try
                    {
                        action();
                    }
                    catch
                    {
                    }
                }

                actions.Clear();
            }
        }

        protected bool __awaiting(string signal)
        {
            return _listeners
                .Where(kvp => kvp.Key == signal
                              && kvp.Value.Any())
                .Any();
        }

        protected double rand()
        {
            return __rand.NextDouble();
        }

        protected double rand(double from, double to)
        {
            return from + (to - from)*__rand.NextDouble();
        }
    }

    public class Expression
    {
        private readonly List<Exception> _exceptions = new List<Exception>();
        public Action<Expression> Start { get; set; }
        public IEnumerator<Expression> Continuator { get; set; }
        public Action<Expression> End { get; set; }
        public Exception Failure { get; set; }

        protected void __complete(bool success, Exception failure)
        {
            Debug.Assert(Continuator != null);
            Debug.Assert(End != null);

            IEnumerable<Exception> allFailures = _exceptions;
            if (failure != null)
                allFailures = allFailures.Union(new[] {failure});

            if (!success)
            {
                Debug.Assert(allFailures.Any());

                if (allFailures.Count() == 1)
                    Failure = allFailures.First();
                else
                    Failure = new AggregateException(allFailures);
            }

            End(this);
        }

        protected bool tryUpdate(bool? v1, bool? v2, ref bool? s1, ref bool? s2, Exception ex)
        {
            Debug.Assert(!v1.HasValue || !v2.HasValue);

            if (ex != null)
            {
                Debug.Assert((v1.HasValue && !v1.Value) || (v2.HasValue && !v2.Value));
                _exceptions.Add(ex);
            }

            if (v1.HasValue)
            {
                if (s1.HasValue)
                    return false;

                s1 = v1;
            }
            else
            {
                Debug.Assert(v2.HasValue);
                if (s2.HasValue)
                    return false;

                s2 = v2;
            }

            return true;
        }
    }
}
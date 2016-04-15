﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Concurrent.Runtime
{
    using Spawner = Func<object[], Object>;

    public class Node
    {
        private readonly ConcurrentQueue<Event> _queue = new ConcurrentQueue<Event>();

        private readonly CancellationTokenSource _stop = new CancellationTokenSource();

        private readonly Dictionary<string, Func<object[], Object>> _types = new Dictionary<string, Func<object[], Object>>();

        public Node(int threads, IDictionary<string, Spawner> types)
        {
            Debug.Assert(threads > 0);
            createThreads(threads);
        }

        public T Spawn<T>(params object[] args) where T : Object, new()
        {
            var result = new T();
            result._node = this;
            result.__start(args);
            return result;
        }

        public Object Spawn(string type, params object[] args)
        {
            var caller = null as Func<object[], Object>;
            lock (_types)
            {
                _types.TryGetValue(type, out caller);
            }

            if (caller == null)
                throw new InvalidOperationException(type + " is not defined");

            return caller(args);
        }

        public void Queue(Object who, Func<object> what, Action<object> success, Action<Exception> failure)
        {
            _queue.Enqueue(new Event
            {
                Tries = 0,
                Target = who,
                What = what,
                Success = success,
                Failure = failure
            });
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

                        message.Target.__run(message.What, message.Success, message.Failure);
                    }
                });

                thread.Priority = ThreadPriority.AboveNormal;
                thread.Start();
            }
        }

        private class Event
        {
            public int Tries { get; set; }
            public Object Target { get; set; }
            public Func<object> What { get; set; }
            public Action<object> Success { get; set; }
            public Action<Exception> Failure { get; set; }
        }
    }

    public class Object
    {
        private int _busy;

        private readonly Dictionary<string, List<Action>> _listeners = new Dictionary<string, List<Action>>();

        internal Node _node;

        protected internal virtual void __start(params object[] args)
        {
            Debug.Assert(_busy == 0);
        }

        protected T spawn<T>(params object[] args) where T : Object, new()
        {
            return _node.Spawn<T>(args);
        }

        internal void __run(Action what, Action<object> success, Action<Exception> failure)
        {
            __run(() =>
            {
                what();
                return null;
            }, success, failure);
        }

        internal void __run(Func<object> what, Action<object> success, Action<Exception> failure)
        {
            var was_busy = Interlocked.CompareExchange(ref _busy, 1, 0) == 1;
            if (was_busy)
            {
                _node.Queue(this, what, success, failure);
            }
            else
            {
                try
                {
                    var result = what();
                    if (success != null)
                        try
                        {
                            success(result);
                        }
                        catch
                        {
                        }
                }
                catch (Exception ex)
                {
                    if (failure != null)
                        try
                        {
                            failure(ex);
                        }
                        catch
                        {
                        }
                }

                Interlocked.CompareExchange(ref _busy, 0, 1);
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
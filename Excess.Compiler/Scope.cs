using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;

namespace Excess.Compiler
{
    public class Scope : DynamicObject
    {
        private readonly Scope _parent;

        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public Scope(Scope parent)
        {
            _parent = parent;
        }


        public ICompilerService<TToken, TNode, TModel> GetService<TToken, TNode, TModel>()
        {
            var result = Get<ICompilerService<TToken, TNode, TModel>>();
            if (result == null && _parent != null)
                result = _parent.GetService<TToken, TNode, TModel>();

            return result;
        }

        public IDocument<TToken, TNode, TModel> GetDocument<TToken, TNode, TModel>()
        {
            var result = Get<IDocument<TToken, TNode, TModel>>();
            if (result == null && _parent != null)
                result = _parent.GetDocument<TToken, TNode, TModel>();

            return result;
        }

        public Scope CreateScope<TToken, TNode, TModel>(TNode node)
        {
            var service = GetService<TToken, TNode, TModel>();
            if (service == null)
                return null;

            var id = service.GetExcessId(node);
            if (id < 0)
                return null;

            var repository = GetScopeRepository();
            Debug.Assert(repository != null);

            Scope result;
            if (!repository.TryGetValue(id, out result))
            {
                result = new Scope(this); //TODO: find parent scope
                repository[id] = result;
            }

            return result;
        }

        public Scope Parent()
        {
            return _parent;
        }

        public Scope GetScope<TToken, TNode, TModel>(TNode node)
        {
            var service = GetService<TToken, TNode, TModel>();
            if (service == null)
                return null;

            var id = service.GetExcessId(node);
            if (id < 0)
                return null;

            var repository = GetScopeRepository();
            Debug.Assert(repository != null);

            Scope result;
            if (repository.TryGetValue(id, out result))
                return result;

            return null;
        }

        private IDictionary<int, Scope> GetScopeRepository()
        {
            var result = Get<IDictionary<int, Scope>>();
            if (result == null && _parent != null)
                result = _parent.GetScopeRepository();

            return result;
        }

        //DynamicObject
        public dynamic Context()
        {
            return this;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            _values.TryGetValue(binder.Name, out result);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _values[binder.Name] = value;
            return true;
        }

        public T Get<T>() where T : class
        {
            var thash = typeof(T).GetHashCode().ToString();
            return Get<T>(thash);
        }

        public T Get<T>(string id) where T : class
        {
            object result;
            if (_values.TryGetValue(id, out result))
                return (T) result;

            return default(T);
        }

        public object Get(string id)
        {
            object result;
            if (_values.TryGetValue(id, out result))
                return result;

            return null;
        }

        public T Find<T>() where T : class
        {
            var thash = typeof(T).GetHashCode().ToString();
            return Find<T>(thash);
        }

        public T Find<T>(string id) where T : class
        {
            object result;
            if (_values.TryGetValue(id, out result))
                return (T) result;

            return _parent?.Find<T>(id);
        }

        public object Find(string id)
        {
            object result;
            return _values.TryGetValue(id, out result)
                ? result
                : _parent?.Find(id);
        }

        public void Set(string id, object value)
        {
            _values[id] = value;
        }

        public void Set<T>(T t) where T : class
        {
            var id = typeof(T).GetHashCode().ToString();
            _values[id] = t;
        }

        public bool Has(string id)
        {
            return _values.ContainsKey(id);
        }
    }
}
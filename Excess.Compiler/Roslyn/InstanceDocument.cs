using System;
using System.Collections.Generic;
using System.Diagnostics;
using Excess.Compiler.Core;
using Microsoft.CodeAnalysis;

namespace Excess.Compiler.Roslyn
{
    public class RoslynInstanceDocument : RoslynDocument, IInstanceDocument<SyntaxNode>
    {
        private readonly InstanceDocumentBase<SyntaxNode> _instance = new InstanceDocumentBase<SyntaxNode>();

        private readonly Func<string, IDictionary<string, object>, ICollection<Connection>, Scope, bool> _parser;

        public RoslynInstanceDocument(Func<string, IDictionary<string, object>, ICollection<Connection>, Scope, bool> parser, Scope scope = null) : base(scope)
        {
            _parser = parser;
        }

        public void Change(Func<string, object, Scope, bool> match, IInstanceTransform<SyntaxNode> transform)
        {
            _instance.Change(match, transform);
        }

        public void Change(Func<IDictionary<string, Tuple<object, SyntaxNode>>, Scope, SyntaxNode> transform)
        {
            _instance.Change(transform);
        }

        protected override void ApplyLexical()
        {
            Debug.Assert(_parser != null);
            var instances = new Dictionary<string, object>();
            var connections = new List<Connection>();
            if (_parser(Text, instances, connections, Scope))
            {
                Scope.InitInstance();
                Root = _instance.Transform(instances, connections, Scope);
            }
        }
    }
}
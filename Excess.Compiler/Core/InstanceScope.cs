using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Excess.Compiler.Core
{
    public static class ScopeExtensions
    {
        public static void AddInstanceInitializer(this Scope scope, SyntaxNode node)
        {
            var instanceInitializer = scope.Find<List<SyntaxNode>>("_instanceInitializer");
            Debug.Assert(instanceInitializer != null);

            instanceInitializer.Add(node);
        }

        public static List<SyntaxNode> GetInstanceInitializers(this Scope scope)
        {
            var instanceInitializer = scope.Find<List<SyntaxNode>>("_instanceInitializer");
            Debug.Assert(instanceInitializer != null);

            return instanceInitializer;
        }

        public static void AddInstanceDeclaration(this Scope scope, SyntaxNode node)
        {
            var instanceDeclaration = scope.Find<List<SyntaxNode>>("_instanceDeclaration");
            Debug.Assert(instanceDeclaration != null);

            instanceDeclaration.Add(node);
        }

        public static List<SyntaxNode> GetInstanceDeclarations(this Scope scope)
        {
            var instanceDeclaration = scope.Find<List<SyntaxNode>>("_instanceDeclaration");
            Debug.Assert(instanceDeclaration != null);

            return instanceDeclaration;
        }

        internal static void InitInstance(this Scope scope)
        {
            Debug.Assert(scope.Find<List<SyntaxNode>>("_instanceDeclaration") == null);

            scope.Set("_instanceDeclaration", new List<SyntaxNode>());
            scope.Set("_instanceInitializer", new List<SyntaxNode>());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Excess.Compiler.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using CSharp = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Excess.Compiler.Roslyn
{
    public class RoslynDocument : BaseDocument<SyntaxToken, SyntaxNode, SemanticModel>
    {
        private readonly string _documentId;

        private readonly List<Diagnostic> _errors = new List<Diagnostic>();

        public string LexicalText { get; internal set; }

        public RoslynDocument(Scope scope) : base(scope)
        {
            Scope.Set<IDocument<SyntaxToken, SyntaxNode, SemanticModel>>(this);
            Scope.InitDocumentScope();
        }

        public RoslynDocument(Scope scope, string text, string id = null) : base(scope)
        {
            Text = text;
            Scope.Set<IDocument<SyntaxToken, SyntaxNode, SemanticModel>>(this);
            Scope.InitDocumentScope();

            _documentId = id;
        }

        public FileLinePositionSpan OriginalPosition(Location location)
        {
            var tree = location.SourceTree;
            if (tree == null)
            {
                return default(FileLinePositionSpan);
            }

            var errorNode = tree.GetRoot().FindNode(location.SourceSpan);
            if (errorNode == null)
            {
                return default(FileLinePositionSpan);
            }

            var nodeId = RoslynCompiler.NodeMark(errorNode);
            if (nodeId == null)
            {
                return default(FileLinePositionSpan);
            }

            var originalNode = RoslynCompiler.FindNode(Original, nodeId);
            if (originalNode == null)
            {
                return default(FileLinePositionSpan);
            }

            location = originalNode.SyntaxTree.GetLocation(originalNode.Span);
            return location.GetMappedLineSpan();
        }

        public void AddError(string id, string message, SyntaxNode node)
        {
            var location = Location.Create(Root.SyntaxTree, node.Span);
            var descriptor = new DiagnosticDescriptor(id, message, message, "Excess", DiagnosticSeverity.Error, true);

            var error = Diagnostic.Create(descriptor, location);
            _errors.Add(error);
        }

        public void AddError(string id, string message, int offset, int length)
        {
            var location = Location.Create(Root.SyntaxTree, new TextSpan(offset, length));
            var descriptor = new DiagnosticDescriptor(id, message, message, "Excess", DiagnosticSeverity.Error, true);

            var error = Diagnostic.Create(descriptor, location);
            _errors.Add(error);
        }

        public IEnumerable<Diagnostic> GetErrors()
        {
            return _errors;
        }

        protected override void NotifyOriginal(string newText)
        {
            LexicalText = newText;
        }

        protected override SyntaxNode UpdateRoot(SyntaxNode root)
        {
            var newTree = root.SyntaxTree.WithFilePath(_documentId);
            return newTree.GetRoot();
        }

        public override bool HasErrors()
        {
            if (_errors.Any())
            {
                return true;
            }

            return Root != null &&
                   Root.GetDiagnostics()
                       .Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        }

        protected override SyntaxNode Transform(SyntaxNode node, Dictionary<int, Func<SyntaxNode, Scope, SyntaxNode>> transformers)
        {
            var nodes = new Dictionary<SyntaxNode, Func<SyntaxNode, Scope, SyntaxNode>>();
            foreach (var transformer in transformers)
            {
                var tNode = node
                    .GetAnnotatedNodes(RoslynCompiler.NodeIdAnnotation + transformer.Key)
                    .First();

                Debug.Assert(tNode != null); //td: cases
                nodes[tNode] = transformer.Value;
            }

            IEnumerable<SyntaxNode> toReplace = nodes.Keys;
            return node.ReplaceNodes(toReplace, (oldNode, newNode) =>
            {
                Func<SyntaxNode, Scope, SyntaxNode> handler;
                if (nodes.TryGetValue(oldNode, out handler))
                {
                    var result = handler(newNode, Scope);
                    return result;
                }

                return newNode;
            });
        }

        protected override SyntaxNode Transform(SyntaxNode node, Dictionary<int, Func<SyntaxNode, SyntaxNode, SemanticModel, Scope, SyntaxNode>> transformers)
        {
            Debug.Assert(Model != null);

            var nodes = new Dictionary<SyntaxNode, Func<SyntaxNode, SyntaxNode, SemanticModel, Scope, SyntaxNode>>();
            foreach (var transformer in transformers)
            {
                var tNode = node
                    .GetAnnotatedNodes(RoslynCompiler.NodeIdAnnotation + transformer.Key)
                    .First();

                Debug.Assert(tNode != null); //td: cases

                if (Mapper != null)
                {
                    tNode = Mapper.SemanticalMap(tNode);
                }

                nodes[tNode] = transformer.Value;
            }

            if (Mapper != null)
            {
                node = Mapper.SemanticalMap(node);
            }

            IEnumerable<SyntaxNode> toReplace = nodes.Keys;
            return node.ReplaceNodes(toReplace, (oldNode, newNode) =>
            {
                Func<SyntaxNode, SyntaxNode, SemanticModel, Scope, SyntaxNode> handler;
                if (nodes.TryGetValue(oldNode, out handler))
                {
                    var result = handler(oldNode, newNode, Model, Scope);
                    return result;
                }

                return newNode;
            });
        }

        protected override SyntaxNode AddModules(SyntaxNode root, IEnumerable<string> modules)
        {
            var compilationUnit = (CompilationUnitSyntax) root;
            return compilationUnit
                .WithUsings(CSharp.List(
                    compilationUnit.Usings.Union(
                        modules
                            .Select(module => CSharp.UsingDirective(
                                CSharp.ParseName(module))))));
        }

        protected override SyntaxNode SyntacticalTransform(SyntaxNode node, Scope scope, IEnumerable<Func<SyntaxNode, Scope, SyntaxNode>> transformers)
        {
            var rewriter = new SyntaxRewriter(transformers, scope);
            return rewriter.Visit(node);
        }

        protected override SyntaxNode GetRoot()
        {
            return Root;
        }

        protected override void SetRoot(SyntaxNode node)
        {
            Root = node;
        }


        protected override void ApplySyntactical()
        {
            base.ApplySyntactical();

            var additionalTypes = Scope.GetAdditionalTypes();
            if (additionalTypes != null && additionalTypes.Any())
            {
                var namespaces = Root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();

                if (!namespaces.Any())
                {
                    Root = (Root as CompilationUnitSyntax)
                        .AddMembers(additionalTypes.ToArray());
                }
                else
                {
                    Root = Root.ReplaceNodes(namespaces, (on, nn) => nn.AddMembers(additionalTypes.ToArray()));
                }
            }
        }
    }
}
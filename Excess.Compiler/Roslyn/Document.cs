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
		private readonly string _documentID;

		private readonly List<Diagnostic> _errors = new List<Diagnostic>();

		public string LexicalText { get; internal set; }

		public RoslynDocument(Scope scope) : base(scope)
		{
			_scope.set<IDocument<SyntaxToken, SyntaxNode, SemanticModel>>(this);
			_scope.InitDocumentScope();
		}

		public RoslynDocument(Scope scope, string text, string id = null) : base(scope)
		{
			Text = text;
			_scope.set<IDocument<SyntaxToken, SyntaxNode, SemanticModel>>(this);
			_scope.InitDocumentScope();

			_documentID = id;
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

			var nodeID = RoslynCompiler.NodeMark(errorNode);
			if (nodeID == null)
			{
				return default(FileLinePositionSpan);
			}

			var originalNode = RoslynCompiler.FindNode(_original, nodeID);
			if (originalNode == null)
			{
				return default(FileLinePositionSpan);
			}

			location = originalNode.SyntaxTree.GetLocation(originalNode.Span);
			return location.GetMappedLineSpan();
		}

		public void AddError(string id, string message, SyntaxNode node)
		{
			var location = Location.Create(_root.SyntaxTree, node.Span);
			var descriptor = new DiagnosticDescriptor(id, message, message, "Excess", DiagnosticSeverity.Error, true);

			var error = Diagnostic.Create(descriptor, location);
			_errors.Add(error);
		}

		public void AddError(string id, string message, int offset, int length)
		{
			var location = Location.Create(_root.SyntaxTree, new TextSpan(offset, length));
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
			var newTree = root.SyntaxTree.WithFilePath(_documentID);
			return newTree.GetRoot();
		}

		public override bool HasErrors()
		{
			if (_errors.Any())
			{
				return true;
			}

			return _root != null &&
			       _root
				       .GetDiagnostics()
				       .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
				       .Any();
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
					var result = handler(newNode, _scope);
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
					var result = handler(oldNode, newNode, Model, _scope);
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
			return _root;
		}

		protected override void SetRoot(SyntaxNode node)
		{
			_root = node;
		}


		protected override void ApplySyntactical()
		{
			base.ApplySyntactical();

			var additionalTypes = _scope.GetAdditionalTypes();
			if (additionalTypes != null && additionalTypes.Any())
			{
				var namespaces = _root
					.DescendantNodes()
					.OfType<NamespaceDeclarationSyntax>();

				if (!namespaces.Any())
				{
					_root = (_root as CompilationUnitSyntax)
						.AddMembers(additionalTypes.ToArray());
				}
				else
				{
					_root = _root
						.ReplaceNodes(namespaces,
							(on, nn) => nn.AddMembers(additionalTypes.ToArray()));
				}
			}
		}
	}
}
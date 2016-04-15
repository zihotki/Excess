using System.Collections.Generic;
using System.Linq;
using Excess.Compiler.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Excess.Compiler.Roslyn
{
	using CSharp = SyntaxFactory;

	public class RoslynLexicalAnalysis : BaseLexicalAnalysis<SyntaxToken, SyntaxNode, SemanticModel>
	{
		protected override SyntaxNode Normalize(SyntaxNode root, Scope scope)
		{
			IEnumerable<SyntaxNode> rootStatements = null;
			IEnumerable<SyntaxNode> rootMembers = null;
			IEnumerable<SyntaxNode> rootTypes = null;

			var node = root;
			var normalized = false;
			if (_normalizeStatements != null)
			{
				node = NormalizeCode(root, out rootStatements, out normalized);
			}

			if (!normalized && (_normalizeMembers != null || _normalizeTypes != null))
			{
				node = NormalizePass(root, out rootMembers, out rootTypes, out normalized);
			}

			if (normalized)
			{
				if (_normalizeStatements != null && rootStatements != null && rootStatements.Any())
				{
					node = _normalizeStatements(node, rootStatements, scope);
				}

				if (_normalizeMembers != null)
				{
					if (node != root)
					{
						node = NormalizePass(node, out rootMembers, out rootTypes, out normalized);
						root = node;
					}

					if (rootMembers != null && rootMembers.Any())
					{
						node = _normalizeMembers(node, rootMembers, scope);
					}
				}

				if (_normalizeTypes != null)
				{
					if (node != root)
					{
						node = NormalizePass(node, out rootMembers, out rootTypes, out normalized);
					}

					if (rootTypes != null && rootTypes.Any())
					{
						node = _normalizeTypes(node, rootTypes, scope);
					}
				}
			}

			if (_normalizeThen != null)
			{
				node = _normalizeThen(node, scope);
			}

			return node;
		}

		private SyntaxNode NormalizeCode(SyntaxNode root, out IEnumerable<SyntaxNode> rootStatements, out bool normalized)
		{
			normalized = true;
			rootStatements = null;

			//td: line numbers!
			var newTree = CSharp.ParseSyntaxTree("void m() {" + root.ToFullString() + "}", new CSharpParseOptions(kind: SourceCodeKind.Script));
			try
			{
				var block = newTree
					.GetRoot()
					.ChildNodes()
					.OfType<MethodDeclarationSyntax>()
					.Single()
					.Body;

				if (block.ChildNodes().All(child => child is StatementSyntax))
				{
					rootStatements = new List<StatementSyntax>(block.ChildNodes().OfType<StatementSyntax>());
					return block;
				}
			}
			catch
			{
			}

			normalized = false;
			return root;
		}

		private SyntaxNode NormalizePass(SyntaxNode root, out IEnumerable<SyntaxNode> rootMembers, out IEnumerable<SyntaxNode> rootTypes, out bool normalized)
		{
			rootMembers = null;
			rootTypes = null;
			normalized = false;

			var tree = root.SyntaxTree;
			var codeErrors = root.GetDiagnostics().Where(error => error.Id == "CS1022").
				OrderBy(error => error.Location.SourceSpan.Start).GetEnumerator();

			Diagnostic currError = null;
			var currErrorPos = 0;

			if (codeErrors.MoveNext())
			{
				currError = codeErrors.Current;
			}

			BlockSyntax statementBlock;

			var statements = new List<StatementSyntax>();
			var members = new List<MemberDeclarationSyntax>();
			var types = new List<MemberDeclarationSyntax>();
			var toRemove = new List<TextSpan>();
			foreach (var child in root.ChildNodes())
			{
				if (child is IncompleteMemberSyntax)
				{
					continue;
				}

				if (child is FieldDeclarationSyntax)
				{
					//case: code variable?
					var field = (FieldDeclarationSyntax) child;
					if (!field.Modifiers.Any())
					{
						//td: !!! variable initialization
						continue;
					}
				}

				if (child is MethodDeclarationSyntax)
				{
					//case: bad method?
					var method = (MethodDeclarationSyntax) child;
					if (method.Body == null)
					{
						continue;
					}
				}

				if (child is MemberDeclarationSyntax)
				{
					var foundError = false;
					if (currError != null)
					{
						if (child.SpanStart > currError.Location.SourceSpan.Start)
						{
							var errorSpan = new TextSpan(currErrorPos, child.SpanStart - currErrorPos);
							var errorSource = tree.GetText().GetSubText(errorSpan);

							statementBlock = (BlockSyntax) SyntaxFactory.ParseStatement("{" + errorSource + "}");
							statements.AddRange(statementBlock.Statements);

							toRemove.Add(errorSpan);

							foundError = true;
							currError = null;
							while (codeErrors.MoveNext())
							{
								var nextError = codeErrors.Current;
								if (nextError.Location.SourceSpan.Start > child.Span.End)
								{
									currError = nextError;
									break;
								}
							}
						}
					}

					currErrorPos = child.Span.End;
					var toAdd = child as MemberDeclarationSyntax;

					if (foundError)
					{
						toAdd = toAdd.ReplaceTrivia(child.GetLeadingTrivia(), (oldTrivia, newTrivia) => { return CSharp.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, string.Empty); });
					}

					if (toAdd is TypeDeclarationSyntax || toAdd is EnumDeclarationSyntax)
					{
						types.Add(toAdd);
					}
					else if (!(toAdd is NamespaceDeclarationSyntax))
					{
						members.Add(toAdd);
					}
				}
				else
				{
					//any other top level construct indicates completeness
					return root;
				}
			}

			if (currError != null)
			{
				var errorSpan = new TextSpan(currErrorPos, tree.GetRoot().FullSpan.End - currErrorPos);
				var errorSource = tree.GetText().GetSubText(errorSpan);
				statementBlock = (BlockSyntax) SyntaxFactory.ParseStatement("{" + errorSource + "}");
				statements.AddRange(statementBlock.Statements);

				toRemove.Add(errorSpan);
			}

			normalized = statements.Any() || members.Any() || !types.Any();

			if (!normalized)
			{
				return root; //nothing to se here
			}

			rootMembers = members;
			rootTypes = types;

			if (!toRemove.Any())
			{
				return root; //nothing to remove
			}

			return root.RemoveNodes(
				root
					.ChildNodes()
					.Where(node => Contained(node, toRemove)),
				SyntaxRemoveOptions.KeepEndOfLine);
		}

		private static bool Contained(SyntaxNode node, List<TextSpan> text)
		{
			foreach (var inner in text)
			{
				if (inner.Contains(node.FullSpan))
				{
					return true;
				}
			}

			return false;
		}
	}
}
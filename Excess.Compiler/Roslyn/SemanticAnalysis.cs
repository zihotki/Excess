using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Excess.Compiler.Roslyn
{
	public class RoslynSemanticAnalysis : ISemanticAnalysis<SyntaxToken, SyntaxNode, SemanticModel>,
		IDocumentInjector<SyntaxToken, SyntaxNode, SemanticModel>
	{
		private readonly List<ErrorHandler> _errors = new List<ErrorHandler>();

		public void Apply(IDocument<SyntaxToken, SyntaxNode, SemanticModel> document)
		{
			document.Change(HandleErrors);
		}

		public ISemanticAnalysis<SyntaxToken, SyntaxNode, SemanticModel> Error(string error, Action<SyntaxNode, SemanticModel, Scope> handler)
		{
			_errors.Add(new ErrorHandler {ErrorId = error, Handler = handler});
			return this;
		}

		public ISemanticAnalysis<SyntaxToken, SyntaxNode, SemanticModel> Error(string error, Action<SyntaxNode, Scope> handler)
		{
			_errors.Add(new ErrorHandler {ErrorId = error, Handler = (node, model, scope) => handler(node, scope)});
			return this;
		}

		private SyntaxNode HandleErrors(SyntaxNode root, SemanticModel model, Scope scope)
		{
			var errors = model.GetDiagnostics();
			foreach (var error in errors)
			{
				var id = error.Id;
				foreach (var handler in _errors)
				{
					if (handler.ErrorId == id)
					{
						var node = root.FindNode(error.Location.SourceSpan);
						handler.Handler(node, model, scope);
					}
				}
			}

			return root; //unmodified
		}

		private class ErrorHandler
		{
			public string ErrorId { get; set; }
			public Action<SyntaxNode, SemanticModel, Scope> Handler { get; set; }
		}
	}
}
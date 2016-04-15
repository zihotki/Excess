using System;

namespace Excess.Compiler
{
    public interface ISemanticAnalysis<TToken, TNode, TModel>
    {
        ISemanticAnalysis<TToken, TNode, TModel> Error(string error, Action<TNode, Scope> handler);
        ISemanticAnalysis<TToken, TNode, TModel> Error(string error, Action<TNode, TModel, Scope> handler);
    }
}
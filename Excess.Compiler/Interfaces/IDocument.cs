using System;
using System.Collections.Generic;

namespace Excess.Compiler
{
    public interface IDocument<TToken, TNode, TModel>
    {
        string Text { get; set; }
        CompilerStage Stage { get; }
        TNode SyntaxRoot { get; set; }
        TModel Model { get; set; }
        Scope Scope { get; }
        //IMappingService<TNode> Mapper { get; set; }

        void Change(Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>> transform, string kind = null);
        TToken Change(TToken token, Func<TNode, Scope, TNode> transform, string kind = null);
        IEnumerable<TToken> Change(IEnumerable<TToken> tokens, Func<TNode, Scope, TNode> transform, string kind = null);
        TToken Change(TToken token, Func<TNode, TNode, TModel, Scope, TNode> transform, string kind = null);
        IEnumerable<TToken> Change(IEnumerable<TToken> tokens, Func<TNode, TNode, TModel, Scope, TNode> transform, string kind = null);
        void Change(Func<TNode, Scope, TNode> transform, string kind = null);
        TNode Change(TNode node, Func<TNode, Scope, TNode> transform, string kind = null);
        void Change(Func<TNode, TModel, Scope, TNode> transform, string kind = null);
        TNode Change(TNode node, Func<TNode, TNode, TModel, Scope, TNode> transform, string kind = null);

        bool ApplyChanges(CompilerStage stage = CompilerStage.Finished);
        bool HasErrors();
        bool HasSemanticalChanges();
    }
}
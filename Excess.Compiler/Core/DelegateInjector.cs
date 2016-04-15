using System;

namespace Excess.Compiler.Core
{
    public class DelegateInjector<TToken, TNode, TModel> : ICompilerInjector<TToken, TNode, TModel>
    {
        private readonly Action<ICompiler<TToken, TNode, TModel>> _delegate;

        public DelegateInjector(Action<ICompiler<TToken, TNode, TModel>> @delegate)
        {
            _delegate = @delegate;
        }

        public void Apply(ICompiler<TToken, TNode, TModel> compiler)
        {
            _delegate(compiler);
        }
    }
}
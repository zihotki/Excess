using System.Collections.Generic;

namespace Excess.Compiler.Core
{
    public class CompositeInjector<TToken, TNode, TModel> : ICompilerInjector<TToken, TNode, TModel>
    {
        private readonly IEnumerable<ICompilerInjector<TToken, TNode, TModel>> _injectors;

        public CompositeInjector(IEnumerable<ICompilerInjector<TToken, TNode, TModel>> injectors)
        {
            _injectors = injectors;
        }

        public void Apply(ICompiler<TToken, TNode, TModel> compiler)
        {
            foreach (var injector in _injectors)
            {
                injector.Apply(compiler);
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Excess.Compiler;
using Excess.Compiler.Core;
using Excess.Entensions.XS;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Excess.RuntimeProject
{
    using CSharp = SyntaxFactory;
    using Injector = ICompilerInjector<SyntaxToken, SyntaxNode, SemanticModel>;
    using DelegateInjector = DelegateInjector<SyntaxToken, SyntaxNode, SemanticModel>;
    using CompositeInjector = CompositeInjector<SyntaxToken, SyntaxNode, SemanticModel>;

    internal class ConcurrentRuntime : ConsoleRuntime
    {
        private static readonly Injector _concurrent = new DelegateInjector(compiler => XSLang.Apply(compiler));

        private static readonly Injector _main = new DelegateInjector(compiler =>
        {
            compiler
                .Lexical()
                .Normalize()
                .With(MoveToRun);

            compiler
                .Environment()
                .Dependency<Expression>("System.Linq");
        });

        private static readonly CompilationUnitSyntax _app = CSharp.ParseCompilationUnit(@"
            public class application
            {
                Node _node;
                public void main()
                {
                    _node = new Node(3);
                    _node.StopOnEmpty();

                    run();

                    var tokenSource = new CancellationTokenSource();
                    try
                    {
                        CancellationToken token = tokenSource.Token;
                        Task.Delay(30000, token)
                            .ContinueWith((t) => _node.stop());

                        _node.waitForCompletion();
                        tokenSource.Cancel();
                    }
                    catch (OperationCanceledException)
                    {
                    }    
                }

                private void run()
                {
                }

                private void start(IConcurrentObject obj, params object[] args)
                {
                    _node.run(obj as ConcurrentObject, args);
                }

                private T start<T>(IConcurrentObject obj, params object[] args)
                {
                    return _node.run<T>(obj as ConcurrentObject, args);
                }

                private T spawn<T>() where T : IConcurrentObject, new ()
                {
                    return _node.spawn<T>();
                }

                private IEnumerable<T> spawn<T>(int count)  where T : IConcurrentObject, new ()
                {
                    return _node.spawnMany<T>(count);
                }
            }");

        public ConcurrentRuntime(IPersistentStorage storage) : base(storage)
        {
        }

        private static SyntaxNode MoveToRun(SyntaxNode root, IEnumerable<SyntaxNode> statements, Scope scope)
        {
            return _app
                .ReplaceNodes(_app
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(m => m.Identifier.ToString() == "run"),
                    (on, nn) => nn.WithBody(CSharp.Block(
                        statements.Select(sn => (StatementSyntax) sn))));
        }

        protected override ICompilerInjector<SyntaxToken, SyntaxNode, SemanticModel> GetInjector(string file)
        {
            var xs = XSLang.Create();
            if (file == "application")
                return new CompositeInjector(new[] {_main, _concurrent, xs});

            return new CompositeInjector(new[] {_concurrent, xs});
        }
    }
}
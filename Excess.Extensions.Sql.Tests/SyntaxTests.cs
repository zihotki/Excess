using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Excess.Extensions.Sql.Tests
{
    public class SyntaxTests
    {
        private void Build(string text)
        {
            errors = null;

            var compilation = new Roslyn.Compilation(null);
            var injector = new CompositeInjector<SyntaxToken, SyntaxNode, SemanticModel>(new[]
            {
                new DelegateInjector<SyntaxToken, SyntaxNode, SemanticModel>(compiler => compiler
                    .Environment()
                        .dependency<console>("Excess.Compiler.Tests.TestRuntime")
                        //.dependency<object>(new[] {
                        //    "System",
                        //    "System.Collections",
                        //    "System.Collections.Generic" })
                        .dependency(new[] {
                            "System.Threading",
                            "System.Threading.Tasks",
                            "System.Diagnostics",
                        })),

                new DelegateInjector<SyntaxToken, SyntaxNode, SemanticModel>(compiler =>
                    Extensions
                    .Concurrent.Extension
                        .Apply(compiler))
            });

            compilation.addDocument("concurrent-test", text, injector);

            Assembly assembly = compilation.build();
            if (assembly == null)
            {
                errors = compilation.errors();

                //debug
                StringBuilder errorLines = new StringBuilder();
                foreach (var error in errors)
                {
                    errorLines.AppendLine(error.ToString());
                }

                var errorString = errorLines.ToString();
                return null;
            }

            var exportTypes = new Dictionary<string, Spawner>();
            foreach (var type in assembly.GetTypes())
            {
                if (type.BaseType != typeof(ConcurrentObject))
                    continue;

                var useParameterLess = type.GetConstructors().Length == 0;
                if (!useParameterLess)
                    useParameterLess = type.GetConstructor(new Type[] { }) != null;

                var typeName = type.ToString();
                exportTypes[typeName] = (args) =>
                {
                    if (useParameterLess)
                        return (ConcurrentObject)Activator.CreateInstance(type);

                    var ctor = type.GetConstructor(args
                        .Select(arg => arg.GetType())
                        .ToArray());

                    if (ctor != null)
                        return (ConcurrentObject)ctor.Invoke(args);

                    throw new InvalidOperationException("unable to find a constructor");
                };
            }

            return new Node(threads, exportTypes);
        }

        [Fact]
        public void Test1()
        {
            
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Excess.Compiler.Core;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Excess.Extensions.Sql.Tests
{
    public class SyntaxTests
    {
        private IEnumerable<string> Build(string text)
        {
            var errors = new List<string>();

            var compilation = new Compiler.Roslyn.Compilation(null);
            var injector = new CompositeInjector<SyntaxToken, SyntaxNode, SemanticModel>(new[]
            {
                new DelegateInjector<SyntaxToken, SyntaxNode, SemanticModel>(compiler => compiler
                    .Environment()
                        .Dependency(new[] {
                            "System",
                            "System.Collections",
                            "System.Collections.Generic",
                            "System.Diagnostics",
                        })),

                new DelegateInjector<SyntaxToken, SyntaxNode, SemanticModel>(compiler =>
                    SqlExtension.Apply(compiler))
            });

            compilation.addDocument("sql-test", text, injector);

            var assembly = compilation.build();
            if (assembly == null)
            {
                errors = compilation.errors().Select(x => x.ToString()).ToList();
            }


            return errors;
            /*var exportTypes = new Dictionary<string, Spawner>();
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
            }*/
        }

        [Fact]
        public void Test1()
        {
            var txt = @"
public class Query
{
    public static void Filter()
    {
        Sql()
        {
            Select 
        };		
    }
}";

            var errors = Build(txt);

            Assert.Empty(errors);
        }

        [Fact]
        public void Test2()
        {
            var txt = @"
public class Client
{
    public string Name {get; set;}
    public string Surname {get; set;}
    public DateTime BirthDate {get; set;}
    public int Height {get; set;}
}

public class Query
{
    public static void Filter()
    {
        var filterValue = ""Peter"";

        var query = Sql()
        {
            Select Client.Name, Client.Surname, Client.Height From Client
            Where Name = filterValue
                Order By Name
        };		
    }
}";

            var errors = Build(txt);

            Assert.Empty(errors);
        }

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Excess.Compiler.Core;
using Excess.Compiler.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharp = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Excess.Compiler.Tests
{
    [TestClass]
    public class Instance
    {
        private static readonly CompilationUnitSyntax UsageApp = CSharp.ParseCompilationUnit(@"
            internal class BaseRuntime : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler PropertyChanged;

                private void NotifyPropertyChanged(String info)
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(info));
                    }
                }            

                public int input 
                {
                    get; 
                    set 
                    {
                        _input = value;
                        NotifyPropertyChanged(""input"");
                    }
                }

                public int output 
                {
                    get; 
                    set 
                    {
                        _output = value;
                        NotifyPropertyChanged(""output"");
                    }
                }
            }

            internal class RuntimeFoo : BaseRuntime
            {
                public RuntimeFoo(params int[] ags)
                {
                }
            }

            internal class RuntimeBar : BaseRuntime
            {
                public RuntimeBar(params string[] ags)
                {
                }
            }

            public class Program
            {
                void main()
                {
                    BaseRuntime[] values = new [] {};
                    var input = 0;
                    var output = 0;
                    foreach(var val in value)
                    {
                        input += val.input;
                        output += val.output;
                    }        
                }
            }
        ");

        private static readonly Template FooInputInit = Template.ParseStatement(@"
            _0.PropertyChanged += (sender, args) => 
            {
                if (args.PropertyName == __1)
                    __2;
            }
        ");

        private static readonly Template instanceCreation = Template.ParseStatement(@"
            var _0 = new _1();
        ");

        [TestMethod]
        public void InstanceUsage()
        {
            var compiler = new RoslynCompiler();
            var instance = compiler.Instance();

            //code extension
            instance
                .Match<InstanceFoo>()
                .Input(new InstanceConnector {Id = "input"}, FooInput)
                .Output(new InstanceConnector {Id = "output"})
                .Then(TransformFoo)
                .Match<InstanceBar>()
                .Input(new InstanceConnector {Id = "input"})
                .Output(new InstanceConnector {Id = "output"}, transform: BarOutput)
                .Then(TransformBar)
                .Then(TransformInstances);

            SyntaxTree tree;
            string text;
            var doc = new RoslynInstanceDocument(InstanceTestParser);
            tree = compiler.CompileInstance(doc, out text);
            Assert.IsTrue(tree
                .GetRoot()
                .DescendantNodes()
                .OfType<ParenthesizedLambdaExpressionSyntax>()
                .Count() == 2); //must have added a PropertyChanged handler
        }

        private static SyntaxNode TransformInstances(IDictionary<string, Tuple<object, SyntaxNode>> instances, Scope scope)
        {
            var main = UsageApp
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(method => method.Identifier.ToString() == "main")
                .First();

            var result = UsageApp
                .ReplaceNodes(new[] {main},
                    (on, nn) =>
                    {
                        var newStatements = scope
                            .GetInstanceDeclarations()
                            .Select(instance => (StatementSyntax) instance);

                        var initStatements = scope
                            .GetInstanceInitializers()
                            .Select(instance => (StatementSyntax) instance);

                        return nn
                            .WithBody(nn.Body
                                .WithStatements(CSharp.List(
                                    newStatements
                                        .Union(
                                            initStatements)
                                        .Union(
                                            nn.Body.Statements))));
                    });

            return result
                .ReplaceNodes(result
                    .DescendantNodes()
                    .OfType<ImplicitArrayCreationExpressionSyntax>(),
                    (on, nn) =>
                    {
                        return nn
                            .WithInitializer(nn.Initializer
                                .WithExpressions(CSharp.SeparatedList(
                                    instances
                                        .Keys
                                        .Select(key => CSharp.IdentifierName(key) as ExpressionSyntax))));
                    });
        }

        private static void FooInput(InstanceConnector input, object source, object target, Scope scope)
        {
            var bar = source as InstanceBar;
            var foo = target as InstanceFoo;

            Assert.IsNotNull(source);
            Assert.IsNotNull(target);

            foo.Callers.Add(bar);
        }

        private static void BarOutput(InstanceConnector output, InstanceConnection<SyntaxNode> connection, Scope scope)
        {
            Assert.IsNotNull(connection.OutputNode);
            Assert.IsNotNull(connection.InputNode);
            Assert.IsInstanceOfType(connection.InputNode, typeof(ExpressionSyntax));
            Assert.IsInstanceOfType(connection.OutputNode, typeof(ExpressionSyntax));

            scope.AddInstanceInitializer(FooInputInit.Get(
                connection.Source,
                CSharp.ParseExpression('"' + connection.Input.Id + '"'),
                CSharp.BinaryExpression(SyntaxKind.EqualsExpression,
                    (ExpressionSyntax) connection.InputNode,
                    (ExpressionSyntax) connection.OutputNode)));
        }

        private static SyntaxNode TransformBar(string id, object value, IEnumerable<InstanceConnection<SyntaxNode>> connections, Scope scope)
        {
            var decl = instanceCreation.Get(id, "RuntimeBar");
            scope.AddInstanceDeclaration(decl
                .ReplaceNodes(decl
                    .DescendantNodes()
                    .OfType<ParameterListSyntax>(),
                    (on, nn) =>
                    {
                        var values = (value as InstanceFoo).Values;
                        return nn
                            .WithParameters(CSharp.SeparatedList(
                                values.Select(val => CSharp.Parameter(CSharp.Literal(val)))));
                    }));

            foreach (var connection in connections)
            {
                if (connection.Source == id)
                    connection.OutputNode = CSharp.ParseExpression(id + "." + connection.Output.Id);
                else
                {
                    Assert.IsTrue(connection.Target == id);
                    connection.InputNode = CSharp.ParseExpression(id + "." + connection.Input.Id);
                }
            }

            return null;
        }

        private static SyntaxNode TransformFoo(string id, object value, IEnumerable<InstanceConnection<SyntaxNode>> connections, Scope scope)
        {
            var decl = instanceCreation.Get(id, "RuntimeFoo");
            scope.AddInstanceDeclaration(decl
                .ReplaceNodes(decl
                    .DescendantNodes()
                    .OfType<ParameterListSyntax>(),
                    (on, nn) =>
                    {
                        var values = (value as InstanceFoo).Values;
                        return nn
                            .WithParameters(CSharp.SeparatedList(
                                values.Select(val => CSharp.Parameter(CSharp.Literal(val)))));
                    }));

            foreach (var connection in connections)
            {
                if (connection.Input == null)
                {
                    scope.AddError("test01", "unregistered connection", connection.OutputModelNode);
                    continue;
                }

                if (connection.Output == null)
                {
                    scope.AddError("test01", "unregistered connection", connection.InputModelNode);
                    continue;
                }

                if (connection.Source == id)
                    connection.OutputNode = CSharp.ParseExpression(id + "." + connection.Output.Id);
                else
                {
                    Assert.IsTrue(connection.Target == id);
                    connection.InputNode = CSharp.ParseExpression(id + "." + connection.Input.Id);
                }
            }

            return null;
        }

        private static bool InstanceTestParser(string text, IDictionary<string, object> instances, ICollection<Connection> connections, Scope scope)
        {
            instances["foo"] = new InstanceFoo {Id = "foo", Values = new[] {1, 2, 3}};
            instances["bar"] = new InstanceBar {Id = "bar", Values = new[] {"1", "2", "3"}};

            connections.Add(new Connection {Source = "foo", Target = "bar", InputConnector = "input", OutputConnector = "output"});
            connections.Add(new Connection {Source = "bar", Target = "foo", InputConnector = "input", OutputConnector = "output"});
            return true;
        }

        private class InstanceFoo
        {
            public string Id { get; set; }
            public int[] Values { get; set; }
            public List<InstanceBar> Callers { get; }

            public InstanceFoo()
            {
                Callers = new List<InstanceBar>();
            }
        }

        private class InstanceBar
        {
            public string Id { get; set; }
            public string[] Values { get; set; }
        }
    }
}
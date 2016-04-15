﻿using System.Collections.Generic;
using System.Linq;
using Excess.Compiler.Roslyn;
using Excess.Extensions.Concurrent;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excess.Compiler.Tests
{
    [TestClass]
    public class ConcurrentUsage
    {
        [TestMethod]
        public void BasicOperators()
        {
            var compiler = new RoslynCompiler();
            Extension.Apply(compiler);

            SyntaxTree tree = null;
            string text = null;

            tree = compiler.ApplySemanticalPass(@"
                concurrent class SomeClass 
                { 
                    void main() 
                    {
                        A | (B & C()) >> D(10);
                    }

                    public void A();
                    public void B();
                    public void F();
                    public void G();
                    
                    private string C()
                    {
                        if (2 > 1)
                            return ""SomeValue"";

                        F & G;

                        if (1 > 2)
                            return ""SomeValue"";
                        return ""SomeOtherValue"";
                    }

                    private int D(int v)
                    {
                        return v + 1;
                    }
                }", out text);

            Assert.IsTrue(tree.GetRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Count(method =>
                    new[]
                    {
                        "__concurrentmain",
                        "__concurrentA",
                        "__concurrentB",
                        "__concurrentC",
                        "__concurrentF",
                        "__concurrentG"
                    }
                        .Contains(method
                            .Identifier
                            .ToString())) == 6); //must have created concurrent methods

            Assert.IsFalse(tree.GetRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Any(method => method
                    .Identifier
                    .ToString() == "__concurrentD")); //but not for D
        }

        [TestMethod]
        public void BasicAssigment()
        {
            var compiler = new RoslynCompiler();
            Extension.Apply(compiler);

            SyntaxTree tree = null;
            string text = null;

            tree = compiler.ApplySemanticalPass(@"
                concurrent class SomeClass 
                { 
                    int E;
                    void main() 
                    {
                        string B;
                        A | (B = C()) & (E = D(10));
                    }

                    public void A();
                    public void F();
                    public void G();
                    
                    private string C()
                    {
                        F & G;

                        return ""SomeValue"";
                    }

                    private int D(int v)
                    {
                        return v + 1;
                    }
                }", out text);

            Assert.IsTrue(tree.GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(@class => @class.Identifier.ToString() == "__expr1")
                .Single()
                .Members
                .OfType<FieldDeclarationSyntax>()
                .Count(field => new[] {"B", "E"}
                    .Contains(field
                        .Declaration
                        .Variables[0]
                        .Identifier.ToString())) == 2); //must have added fields to the expression object

            Assert.IsTrue(tree.GetRoot()
                .DescendantNodes()
                .OfType<AssignmentExpressionSyntax>()
                .Count(assignment => new[] {"B", "E"}
                    .Contains(assignment
                        .Left
                        .ToString())) == 2); //must have added assignments from fields to the expression object
        }

        [TestMethod]
        public void BasicTryCatch()
        {
            var compiler = new RoslynCompiler();
            Extension.Apply(compiler);

            SyntaxTree tree = null;
            string text = null;

            tree = compiler.ApplySemanticalPass(@"
                concurrent class SomeClass 
                { 
                    public void A();
                    public void B();

                    void main() 
                    {
                        try
                        {
                            int someValue = 10;
                            int someOtherValue = 11;

                            A | B;

                            someValue++;

                            B >> A;

                            someOtherValue++;
                        }
                        catch
                        {
                        }
                    }
                }", out text);

            Assert.IsTrue(tree.GetRoot()
                .DescendantNodes()
                .OfType<TryStatementSyntax>()
                .Count() == 2); //must have added a a try statement
        }

        [TestMethod]
        public void BasicProtection()
        {
            var compiler = new RoslynCompiler();
            Extension.Apply(compiler);

            SyntaxTree tree = null;
            string text = null;

            tree = compiler.ApplySemanticalPass(@"
                concurrent class VendingMachine 
                { 
                    public    void coin();
                    protected void choc();
                    protected void toffee();

                    void main() 
                    {
                        for (;;)
                        {
                            coin >> (choc | toffee);
                        }
                    }
                }", out text);

            Assert.IsTrue(tree.GetRoot()
                .DescendantNodes()
                .OfType<ThrowStatementSyntax>()
                .SelectMany(thrw => thrw
                    .DescendantNodes()
                    .OfType<LiteralExpressionSyntax>())
                .Select(s => s.ToString())
                .Count(s => new[] {"\"choc\"", "\"toffee\""}
                    .Contains(s)) == 2); //must have added checks for choc and toffee
        }

        [TestMethod]
        public void BasicAwait()
        {
            var compiler = new RoslynCompiler();
            Extension.Apply(compiler);

            SyntaxTree tree = null;
            string text = null;

            tree = compiler.ApplySemanticalPass(@"
                concurrent class SomeClass
                { 
                    public void A();
                    public void B();

                    void main() 
                    {
                        await A;
                        int val = await C();
                        val++;
                    }

                    private int C()
                    {
                        await B;
                        return 10;
                    }
                }", out text);

            Assert.IsTrue(tree.GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(invocation => invocation
                    .Expression
                    .ToString() == "__listen")
                .Count(invocation => new[] {"\"A\"", "\"B\""}
                    .Contains(invocation
                        .ArgumentList
                        .Arguments[0]
                        .Expression.ToString())) == 2); //must have listened to both signals
        }

        [TestMethod]
        public void BasicProtectionRuntime()
        {
            var errors = null as IEnumerable<Diagnostic>;
            var node = TestRuntime.Concurrent
                .Build(@"
                    concurrent class VendingMachine 
                    { 
                        public    void coin();
                        protected void choc();
                        protected void toffee();

                        void main() 
                        {
                            for (;;)
                            {
                                coin >> (choc | toffee);
                            }
                        }
                    }", out errors);

            //must not have compilation errors
            Assert.IsNull(errors);

            var vm = node.Spawn("VendingMachine");

            TestRuntime.Concurrent.Fails(vm, "choc");
            TestRuntime.Concurrent.Fails(vm, "toffee");

            TestRuntime.Concurrent.Succeeds(vm, "coin", "choc");
            TestRuntime.Concurrent.Succeeds(vm, "coin", "toffee");

            node.Stop();
        }
    }
}
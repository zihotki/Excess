using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Antlr4.Runtime;
using Excess.Compiler;
using Excess.Compiler.Core;
using Excess.Compiler.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharp = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Excess.RuntimeProject
{
    using Injector = ICompilerInjector<SyntaxToken, SyntaxNode, SemanticModel>;
    using DelegateInjector = DelegateInjector<SyntaxToken, SyntaxNode, SemanticModel>;
    using CompositeInjector = CompositeInjector<SyntaxToken, SyntaxNode, SemanticModel>;

    internal class ExtensionRuntime : BaseRuntime, IExtensionRuntime
    {
        private static readonly Injector References = new DelegateInjector(compiler =>
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            compiler
                .Environment()
                .Dependency<Injector>(new[]
                {
                    "Excess.Compiler",
                    "Excess.Compiler.Core",
                    "Excess.Compiler.Roslyn"
                })
                .Dependency<Expression>("System.Linq")
                .Dependency<SyntaxNode>("Microsoft.CodeAnalysis")
                .Dependency<CSharpSyntaxNode>(new[]
                {
                    "Microsoft.CodeAnalysis.CSharp",
                    "Microsoft.CodeAnalysis.CSharp.Syntax"
                })
                .Dependency(string.Empty, Path.Combine(assemblyPath, "System.Runtime.dll"))
                .Dependency(string.Empty, Path.Combine(assemblyPath, "System.Threading.Tasks.dll"))
                .Dependency<ParserRuleContext>("Antlr4.Runtime")
                .Dependency<CodeCompileUnit>("System.CodeDom");
        });

        private static readonly Injector Extension = new DelegateInjector(compiler =>
        {
            compiler
                .Lexical()
                .Normalize()
                .With(MoveToApply, then: CreateExtensionClass);
        });

        private static readonly CompilationUnitSyntax ExtensionClass = CSharp.ParseCompilationUnit(@"
            internal partial class Extension
            {
                public static void Apply(ICompiler<SyntaxToken, SyntaxNode, SemanticModel> compiler)
                {
                    var lexical = compiler.Lexical();
                    var syntax = compiler.Syntax();
                    var semantics = compiler.Semantics();
                    var environment = compiler.Environment();

                }
            }");

        private static readonly Injector Transform = new DelegateInjector(compiler =>
        {
            compiler
                .Lexical()
                .Normalize()
                .With(members: MoveToClass, then: CreateTransformClass);
        });

        private static readonly CompilationUnitSyntax TransformClass = CSharp.ParseCompilationUnit(@"
            using CSharp = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
            internal partial class Extension
            {
            }");

        private static readonly string[] ExpressionTypes =
        {
            "ExpressionContext",
            "AssignmentExpressionContext",
            "UnaryExpressionContext",
            "LogicalAndExpressionContext",
            "AdditiveExpressionContext",
            "ConditionalExpressionContext",
            "LogicalOrExpressionContext",
            "InclusiveOrExpressionContext",
            "ExclusiveOrExpressionContext",
            "AndExpressionContext",
            "EqualityExpressionContext",
            "ConstantContext",
            "CastExpressionContext",
            "RelationalExpressionContext",
            "ShiftExpressionContext",
            "MultiplicativeExpressionContext",
            "MemberAccessContext",
            "ParenthesizedContext",
            "MemberPointerAccessContext",
            "SimpleExpressionContext",
            "IndexContext",
            "CallContext",
            "PostfixIncrementContext",
            "PostfixDecrementContext",
            "IdentifierContext",
            "StringLiteralContext",
            "ArgumentExpressionListContext",
            "UnaryOperatorContext",
            "AssignmentOperatorContext",
            "ConstantExpressionContext"
        };

        private static readonly string NodeTransformFunction = @"
private static SyntaxNode {0}({1} value, Func<ParserRuleContext, Scope, SyntaxNode> compile, Scope scope)
{{
    throw new NotImplementedException();
}}
";

        private static readonly string TransformFunction = @"
private static SyntaxNode Transform(SyntaxNode oldNode, SyntaxNode newNode, Scope scope, LexicalExtension<SyntaxToken> extension)
{
    return newNode;
}
";

        private static readonly string LexicalHeader = @"
lexical
    .grammar<{0}Grammar, ParserRuleContext>(""{0}"", ExtensionKind.Code)
";

        private Assembly _assembly;

        private RoslynCompiler _compiler;

        static ExtensionRuntime()
        {
            _tools[".g4"] = new AntlrTool();
        }

        public ExtensionRuntime(IPersistentStorage storage) : base(storage)
        {
        }

        public string DebugExtension(string text)
        {
            Debug.Assert(_compiler != null);

            string rText;
            var tree = _compiler.ApplySemanticalPass(text, out rText);

            return tree.GetRoot().NormalizeWhitespace().ToString();
        }

        public bool GenerateGrammar(out string extension, out string transform)
        {
            extension = null;
            transform = null;

            var grammar = _compilation.GetFileByExtension(".g4");
            if (grammar == null)
                return false;

            var grammarName = Path.GetFileNameWithoutExtension(grammar);
            var listenerName = grammarName + "BaseListener.cs";
            var listener = _compilation.GetCSharpFile(listenerName);

            if (listener == null)
                return false;

            var methods = listener
                .GetRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(method => method
                    .ParameterList
                    .Parameters
                    .Count == 1);

            var types = methods
                .Select(method => method
                    .ParameterList
                    .Parameters[0]
                    .Type
                    .ToString())
                .GroupBy(x => x)
                .Select(y => y.First());

            var parserName = grammarName + "Parser";

            var extensionText = new StringBuilder();
            var transformText = new StringBuilder();

            extensionText.AppendFormat(LexicalHeader, grammarName);

            //track default extenions
            var expressionExtensionText = new StringBuilder();
            var expressionTransformText = new StringBuilder();

            var expressionCount = 0;
            foreach (var type in types)
            {
                if (!type.Contains(parserName))
                    continue;

                var ext = extensionText;
                var xform = transformText;
                var context = type.Split('.')[1];
                if (ExpressionTypes.Contains(context))
                {
                    ext = expressionExtensionText;
                    xform = expressionTransformText;

                    expressionCount++;
                }

                ext.AppendFormat("\t\t.Transform<{0}>({1})\n", type, context);
                xform.AppendFormat(NodeTransformFunction, context, type);
            }

            if (expressionCount == ExpressionTypes.Length)
                extensionText.AppendFormat("\t\t.Transform<{0}.ExpressionContext>(AntlrExpression.Parse)\n", parserName);
            else
            {
                extensionText.Append(expressionExtensionText);
                transformText.Append(expressionTransformText);
            }

            extensionText.Append("\t\t.then(Transform);");
            transformText.Append(TransformFunction);

            extension = extensionText.ToString();
            transform = transformText.ToString();
            return true;
        }

        private static SyntaxNode CreateExtensionClass(SyntaxNode root, Scope scope)
        {
            if (root.DescendantNodes().OfType<ClassDeclarationSyntax>().All(@class => @class.Identifier.ToString() != "Extension"))
                return ExtensionClass;

            return root;
        }

        private static SyntaxNode MoveToApply(SyntaxNode root, IEnumerable<SyntaxNode> statements, Scope scope)
        {
            return ExtensionClass
                .ReplaceNodes(ExtensionClass
                    .DescendantNodes()
                    .OfType<BlockSyntax>(),
                    (on, nn) =>
                    {
                        return nn.AddStatements(statements
                            .OfType<StatementSyntax>()
                            .ToArray());
                    });
        }


        private static SyntaxNode CreateTransformClass(SyntaxNode root, Scope scope)
        {
            if (!root
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(@class => @class.Identifier.ToString() == "Extension")
                .Any())
                return TransformClass;

            return root;
        }

        private static SyntaxNode MoveToClass(SyntaxNode root, IEnumerable<SyntaxNode> members, Scope scope)
        {
            return TransformClass.
                ReplaceNodes(TransformClass
                    .DescendantNodes()
                    .OfType<ClassDeclarationSyntax>(),
                    (on, nn) => nn.WithMembers(CSharp
                        .List(members
                            .Select(member => (MemberDeclarationSyntax) member))));
        }

        protected override ICompilerInjector<SyntaxToken, SyntaxNode, SemanticModel> GetInjector(string file)
        {
            var xs = base.GetInjector(file);
            if (file == "extension")
                return new CompositeInjector(new[] {References, Extension, xs});

            if (file == "Transform")
                return new CompositeInjector(new[] {References, Transform, xs});

            return new CompositeInjector(new[] {References, xs});
        }

        public override string DefaultFile()
        {
            return "extension";
        }

        private string KeywordString(IEnumerable<string> keywords)
        {
            var result = new StringBuilder();
            foreach (var k in keywords)
            {
                result.Append(" ");
                result.Append(k);
            }

            return result.Length > 0
                ? result.ToString()
                : " ";
        }

        protected override void DoRun(Assembly asm, out dynamic client)
        {
            if (_assembly != asm)
            {
                _assembly = asm;

                var type = _assembly.GetType("ExtensionPlugin");
                var result = (Injector) type.InvokeMember("Create", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static, null, null, null);
                if (result == null)
                    throw new InvalidOperationException("Corrupted extension");

                _compiler = new RoslynCompiler();
                result.Apply(_compiler);
            }

            client = new
            {
                debuggerDlg = "/App/Main/dialogs/dslDebugger.html",
                debuggerCtrl = "dslDebuggerCtrl",
                debuggerData = new
                {
                    keywords = KeywordString(_compiler.Environment().Keywords())
                }
            };
        }

        public override IEnumerable<TreeNodeAction> FileActions(string file)
        {
            if (file == "extension")
                return new[] {new TreeNodeAction {id = "add-extension-item", icon = "fa-plus-circle"}};

            if (Path.GetExtension(file) == ".g4")
                return new[] {new TreeNodeAction {id = "generate-grammar", icon = "fa-flash"}};

            return base.FileActions(file);
        }
    }
}
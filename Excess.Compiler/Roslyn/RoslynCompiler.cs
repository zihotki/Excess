using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Excess.Compiler.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Excess.Compiler.Roslyn
{
    using CSharp = SyntaxFactory;

    public class RoslynCompiler : CompilerBase<SyntaxToken, SyntaxNode, SemanticModel>
    {
        //declarations
        public static TypeSyntax @void = CSharp.PredefinedType(CSharp.Token(SyntaxKind.VoidKeyword));
        public static TypeSyntax @object = CSharp.PredefinedType(CSharp.Token(SyntaxKind.ObjectKeyword));
        public static TypeSyntax @double = CSharp.PredefinedType(CSharp.Token(SyntaxKind.DoubleKeyword));
        public static TypeSyntax @int = CSharp.PredefinedType(CSharp.Token(SyntaxKind.IntKeyword));
        public static TypeSyntax @string = CSharp.PredefinedType(CSharp.Token(SyntaxKind.StringKeyword));
        public static TypeSyntax boolean = CSharp.PredefinedType(CSharp.Token(SyntaxKind.BoolKeyword));
        public static TypeSyntax dynamic = CSharp.ParseTypeName("dynamic");
        public static TypeSyntax exception = CSharp.ParseTypeName("Exception");


        //modifiers
        public static SyntaxTokenList @public = CSharp.TokenList(CSharp.Token(SyntaxKind.PublicKeyword));

        public static SyntaxTokenList @private = CSharp.TokenList(CSharp.Token(SyntaxKind.PrivateKeyword));
        public static SyntaxTokenList @static = CSharp.TokenList(CSharp.Token(SyntaxKind.StaticKeyword));
        public static SyntaxToken @out = CSharp.Token(SyntaxKind.OutKeyword);

        //constants
        public static ExpressionSyntax @null = CSharp.ParseExpression("null");
        public static SyntaxToken nullToken = CSharp.ParseToken("null");
        public static ExpressionSyntax @true = CSharp.ParseExpression("true");
        public static ExpressionSyntax @false = CSharp.ParseExpression("false");

        //tokens
        public static SyntaxToken semicolon = CSharp.ParseToken(";");


        //node marking
        private static int _seed;

        public static string NodeIdAnnotation = "xs-node";

        public RoslynCompiler(ICompilerEnvironment environment, Scope scope = null) :
            base(new RoslynLexicalAnalysis(),
                new RoslynSyntaxAnalysis(),
                new RoslynSemanticAnalysis(),
                environment,
                new InstanceAnalisysBase<SyntaxToken, SyntaxNode, SemanticModel>(),
                scope)
        {
            _scope.set<ICompilerService<SyntaxToken, SyntaxNode, SemanticModel>>(new CompilerService());
            _scope.set(environment);
        }

        public RoslynCompiler(Scope scope) : this(new RoslynEnvironment(scope, null), scope)
        {
        }

        public RoslynCompiler() : this(new Scope(null))
        {
        }

        protected override IDocument<SyntaxToken, SyntaxNode, SemanticModel> CreateDocument()
        {
            var result = new RoslynDocument(_scope);
            _scope.set<IDocument<SyntaxToken, SyntaxNode, SemanticModel>>(result);

            ApplyLexical(result);
            return result;
        }

        public static ExpressionSyntax Constant(object value)
        {
            return SyntaxFactory.ParseExpression(value.ToString());
        }

        private void ApplyLexical(RoslynDocument document)
        {
            var handler = _lexical as IDocumentInjector<SyntaxToken, SyntaxNode, SemanticModel>;
            Debug.Assert(handler != null);

            handler.Apply(document);
        }

        //out of interface methods, used for testing
        public RoslynDocument CreateDocument(string text)
        {
            return new RoslynDocument(_scope, text);
        }

        public ExpressionSyntax CompileExpression(string expr)
        {
            var document = new RoslynDocument(_scope, expr);
            var handler = _lexical as IDocumentInjector<SyntaxToken, SyntaxNode, SemanticModel>;
            _scope.set<IDocument<SyntaxToken, SyntaxNode, SemanticModel>>(document);

            handler.Apply(document);
            document.ApplyChanges(CompilerStage.Lexical);

            return CSharp.ParseExpression(document.LexicalText);
        }

        public SyntaxNode ApplyLexicalPass(string text, out string newText)
        {
            var document = new RoslynDocument(_scope, text);
            var handler = _lexical as IDocumentInjector<SyntaxToken, SyntaxNode, SemanticModel>;
            _scope.set<IDocument<SyntaxToken, SyntaxNode, SemanticModel>>(document);

            handler.Apply(document);
            document.ApplyChanges(CompilerStage.Lexical);

            newText = document.LexicalText;
            return document.SyntaxRoot;
        }

        public string ApplyLexicalPass(string text)
        {
            string result;
            ApplyLexicalPass(text, out result);
            return result;
        }

        public SyntaxTree ApplySyntacticalPass(string text, out string result)
        {
            var document = new RoslynDocument(_scope, text); //we actually dont touch our own state during these calls
            var lHandler = _lexical as IDocumentInjector<SyntaxToken, SyntaxNode, SemanticModel>;
            var sHandler = _syntax as IDocumentInjector<SyntaxToken, SyntaxNode, SemanticModel>;

            lHandler.Apply(document);
            sHandler.Apply(document);

            document.ApplyChanges(CompilerStage.Syntactical);

            result = document.SyntaxRoot.NormalizeWhitespace().ToFullString();
            return document.SyntaxRoot.SyntaxTree;
        }

        public SyntaxTree ApplySyntacticalPass(string text)
        {
            string useless;
            return ApplySyntacticalPass(text, out useless);
        }

        public SyntaxTree ApplySemanticalPass(string text)
        {
            string useless;
            return ApplySemanticalPass(text, out useless);
        }

        public SyntaxTree ApplySemanticalPass(string text, out string result)
        {
            var document = new RoslynDocument(_scope, text);
            return ApplySemanticalPass(document, out result);
        }

        public SyntaxTree ApplySemanticalPass(RoslynDocument document, out string result)
        {
            var lHandler = _lexical as IDocumentInjector<SyntaxToken, SyntaxNode, SemanticModel>;
            var sHandler = _syntax as IDocumentInjector<SyntaxToken, SyntaxNode, SemanticModel>;
            var ssHandler = _semantics as IDocumentInjector<SyntaxToken, SyntaxNode, SemanticModel>;

            lHandler.Apply(document);
            sHandler.Apply(document);
            ssHandler.Apply(document);

            document.ApplyChanges(CompilerStage.Syntactical);
            var tree = document.SyntaxRoot.SyntaxTree;

            var compilation = CSharpCompilation.Create("semantical-pass", new[] {tree}, new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Dictionary<int, int>).Assembly.Location)
            }, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            while (true)
            {
                document.Model = compilation.GetSemanticModel(tree);
                if (document.ApplyChanges(CompilerStage.Semantical))
                {
                    break;
                }

                var oldTree = tree;
                tree = document.SyntaxRoot.SyntaxTree;
                compilation = compilation.ReplaceSyntaxTree(oldTree, tree);
            }

            result = document.SyntaxRoot.NormalizeWhitespace().ToFullString();
            return document.SyntaxRoot.SyntaxTree;
        }

        public SyntaxTree CompileInstance(RoslynInstanceDocument document, out string result)
        {
            var iHandler = _instance as IDocumentInjector<SyntaxToken, SyntaxNode, SemanticModel>;
            iHandler.Apply(document);

            return ApplySemanticalPass(document, out result);
        }


        public static Func<SyntaxNode, Scope, SyntaxNode> AddMember(MemberDeclarationSyntax member)
        {
            return (node, scope) =>
            {
                Debug.Assert(node is TypeDeclarationSyntax);

                if (node is ClassDeclarationSyntax)
                {
                    return (node as ClassDeclarationSyntax).AddMembers(member);
                }

                Debug.Assert(false); //td: case
                return node;
            };
        }

        public static Func<SyntaxNode, Scope, SyntaxNode> AddStatement(StatementSyntax statement, SyntaxNode before = null, SyntaxNode after = null)
        {
            return (node, scope) =>
            {
                var block = node as BlockSyntax;
                Debug.Assert(block != null);

                var newStatements = InsertStatement(block, statement, before, after);
                return block
                    .WithStatements(CSharp.List(
                        newStatements));
            };
        }

        private static IEnumerable<StatementSyntax> InsertStatement(BlockSyntax block, StatementSyntax statement, SyntaxNode before, SyntaxNode after)
        {
            Debug.Assert(before != null || after != null);

            var target = before != null
                ? NodeMark(before)
                : NodeMark(after);

            Debug.Assert(target != null);

            foreach (var st in block.Statements)
            {
                var id = NodeMark(st);
                if (id == target)
                {
                    if (before != null)
                    {
                        yield return statement;
                        yield return st;
                    }
                    else
                    {
                        yield return st;
                        yield return statement;
                    }
                }
                else
                {
                    yield return st;
                }
            }
        }

        public static string UniqueId()
        {
            return (++_seed).ToString();
        }

        public static SyntaxNode MarkNode(SyntaxNode node, string id)
        {
            return node
                .WithoutAnnotations(NodeIdAnnotation)
                .WithoutAnnotations(NodeIdAnnotation + id)
                .WithAdditionalAnnotations(
                    new SyntaxAnnotation(NodeIdAnnotation + id),
                    new SyntaxAnnotation(NodeIdAnnotation, id));
        }

        public static SyntaxNode TrackNode(SyntaxNode node)
        {
            var id = NodeMark(node);
            if (id != null)
            {
                return node;
            }

            return MarkNode(node, UniqueId());
        }

        public static string NodeMark(SyntaxNode node)
        {
            var annotation = node
                .GetAnnotations(NodeIdAnnotation)
                .FirstOrDefault();

            if (annotation != null)
            {
                return annotation.Data;
            }

            return null;
        }

        public static SyntaxToken MarkToken(SyntaxToken token, string id)
        {
            return token
                .WithoutAnnotations(NodeIdAnnotation)
                .WithAdditionalAnnotations(new SyntaxAnnotation(NodeIdAnnotation, id));
        }

        public static string TokenMark(SyntaxToken token)
        {
            var annotation = token.GetAnnotations(NodeIdAnnotation).FirstOrDefault();
            if (annotation != null)
            {
                return annotation.Data;
            }

            return null;
        }

        public static SyntaxToken MarkToken(SyntaxToken token, string mark, object value)
        {
            var result = value == null
                ? new SyntaxAnnotation(mark)
                : new SyntaxAnnotation(mark, value.ToString());

            return token
                .WithoutAnnotations(mark)
                .WithAdditionalAnnotations(result);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var tokens = CSharp.ParseTokens(text);
            foreach (var token in tokens)
            {
                if (token.Kind() != SyntaxKind.EndOfFileToken)
                {
                    yield return token;
                }
            }
        }

        public static string TokensToString(IEnumerable<SyntaxToken> tokens)
        {
            var result = new StringBuilder();
            foreach (var token in tokens)
            {
                result.Append(token.ToFullString());
            }

            return result.ToString();
        }

        public static ParameterListSyntax ParseParameterList(IEnumerable<SyntaxToken> parameters)
        {
            var parameterString = TokensToString(parameters); //td: mapping
            return CSharp.ParseParameterList(parameterString);
        }

        public static bool IsLexicalIdentifier(SyntaxToken token)
        {
            return IsLexicalIdentifier(token.Kind());
        }

        public static bool IsLexicalIdentifier(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.IdentifierToken:
                case SyntaxKind.BoolKeyword:
                case SyntaxKind.ByteKeyword:
                case SyntaxKind.SByteKeyword:
                case SyntaxKind.ShortKeyword:
                case SyntaxKind.UShortKeyword:
                case SyntaxKind.IntKeyword:
                case SyntaxKind.UIntKeyword:
                case SyntaxKind.LongKeyword:
                case SyntaxKind.ULongKeyword:
                case SyntaxKind.DoubleKeyword:
                case SyntaxKind.FloatKeyword:
                case SyntaxKind.DecimalKeyword:
                case SyntaxKind.StringKeyword:
                case SyntaxKind.CharKeyword:
                case SyntaxKind.VoidKeyword:
                case SyntaxKind.ObjectKeyword:
                case SyntaxKind.NullKeyword:
                    return true;
            }

            return false;
        }

        public static Func<SyntaxNode, Scope, SyntaxNode> RemoveStatement(SyntaxNode statement)
        {
            var statementId = NodeMark(statement);
            return (node, scope) =>
            {
                var code = (BlockSyntax) node;
                return code.RemoveNode(FindNode(code.Statements, statementId), SyntaxRemoveOptions.KeepTrailingTrivia);
            };
        }

        public static Func<SyntaxNode, Scope, SyntaxNode> RemoveMember(SyntaxNode member)
        {
            var memberId = NodeMark(member);
            return (node, scope) =>
            {
                var clazz = (ClassDeclarationSyntax) node;
                clazz = clazz.RemoveNode(FindNode(clazz.Members, memberId), SyntaxRemoveOptions.KeepTrailingTrivia);
                return clazz;
            };
        }

        public static SyntaxNode FindNode<T>(SyntaxList<T> nodes, string id) where T : SyntaxNode
        {
            foreach (var node in nodes)
            {
                if (NodeMark(node) == id)
                {
                    return node;
                }
            }

            return null;
        }

        public static SyntaxNode FindNode(SyntaxNode root, string id)
        {
            return root
                .GetAnnotatedNodes(NodeIdAnnotation + id)
                .FirstOrDefault();
        }

        public static SyntaxNode FindNode(SyntaxNode root, SyntaxNode tracked)
        {
            var id = NodeMark(tracked);
            if (id == null)
            {
                return null;
            }

            return FindNode(root, id);
        }

        public static SyntaxNode FindNode<T>(SyntaxList<T> nodes, SyntaxNode tracked) where T : SyntaxNode
        {
            var id = NodeMark(tracked);
            if (id == null)
            {
                return null;
            }

            return FindNode(nodes, id);
        }

        public static StatementSyntax NextStatement(BlockSyntax code, StatementSyntax statement)
        {
            var found = false;
            foreach (var stment in code.Statements)
            {
                if (found)
                {
                    return stment;
                }

                found = stment == statement;
            }

            return null;
        }

        public static bool HasVisibilityModifier(SyntaxTokenList modifiers)
        {
            foreach (var modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.InternalKeyword:
                        return true;
                }
            }

            return false;
        }

        public static Func<SyntaxNode, Scope, SyntaxNode> AddInitializers(StatementSyntax initializer)
        {
            return (node, scope) =>
            {
                var decl = (ClassDeclarationSyntax) node;

                var found = false;
                var result = decl.ReplaceNodes(decl
                    .DescendantNodes()
                    .OfType<ConstructorDeclarationSyntax>(), (oldConstuctor, newConstuctor) =>
                    {
                        found = true;
                        return newConstuctor.WithBody(SyntaxFactory.Block().
                            WithStatements(SyntaxFactory.List(
                                newConstuctor.Body.Statements.Union(
                                    new[] {initializer}))));
                    });

                if (!found)
                {
                    result = result.WithMembers(SyntaxFactory.List(
                        result.Members.Union(
                            new MemberDeclarationSyntax[]
                            {
                                SyntaxFactory.ConstructorDeclaration(decl.Identifier.ToString()).
                                    WithBody(SyntaxFactory.Block().
                                        WithStatements(SyntaxFactory.List(new[]
                                        {
                                            initializer
                                        })))
                            })));
                }

                return result;
            };
        }

        public static Func<SyntaxNode, Scope, SyntaxNode> ExplodeBlock(SyntaxNode newNode)
        {
            return (node, scope) =>
            {
                var block = node as BlockSyntax;
                return block
                    .WithStatements(CSharp.List(
                        ExplodeStatements(block.Statements, newNode, (BlockSyntax) newNode)));
            };
        }

        public static Func<SyntaxNode, Scope, SyntaxNode> ExplodeStatement(SyntaxNode toReplace, BlockSyntax statements)
        {
            return (node, scope) =>
            {
                var block = node as BlockSyntax;
                return block
                    .WithStatements(CSharp.List(
                        ExplodeStatements(block.Statements, toReplace, statements)));
            };
        }

        private static IEnumerable<StatementSyntax> ExplodeStatements(SyntaxList<StatementSyntax> statements, SyntaxNode toReplace, BlockSyntax block)
        {
            var nodeID = NodeMark(toReplace);
            foreach (var statement in statements)
            {
                var innerID = NodeMark(statement);
                if (innerID == nodeID)
                {
                    foreach (var inner in block.Statements)
                    {
                        yield return inner;
                    }
                }
                else
                {
                    yield return statement;
                }
            }
        }

        public static Func<SyntaxNode, Scope, SyntaxNode> ReplaceNode(SyntaxNode newNode)
        {
            return (node, scope) => ReplaceExcessId(newNode, node);
        }

        public static SyntaxNode ReplaceExcessId(SyntaxNode node, SyntaxNode before)
        {
            var oldId = NodeMark(before);
            Debug.Assert(oldId != null);

            return MarkNode(node, oldId);
        }

        public static SyntaxNode UpdateExcessId(SyntaxNode node, SyntaxNode before)
        {
            var oldId = NodeMark(before);
            Debug.Assert(oldId != null);

            var newId = NodeMark(node);
            if (newId == null)
            {
                return MarkNode(node, oldId); //mark substitutions
            }

            return node;
        }

        public static TypeSyntax ConstantType(ExpressionSyntax value)
        {
            switch (value.Kind())
            {
                case SyntaxKind.NumericLiteralExpression:
                {
                    var valueStr = value.ToString();

                    int val;
                    double dval;
                    if (int.TryParse(valueStr, out val))
                    {
                        return @int;
                    }
                    if (double.TryParse(valueStr, out dval))
                    {
                        return @double;
                    }

                    break;
                }

                case SyntaxKind.StringLiteralExpression:
                    return @string;

                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    return boolean;
            }

            return null;
        }

        public static TypeSyntax GetReturnType(BlockSyntax code, SemanticModel model)
        {
            var cfa = model.AnalyzeControlFlow(code);
            if (!cfa.ReturnStatements.Any())
            {
                return @void;
            }

            ITypeSymbol rt = null;
            foreach (var rs in cfa.ReturnStatements)
            {
                var rss = (ReturnStatementSyntax) rs;
                var type = model.GetSpeculativeTypeInfo(rss.Expression.SpanStart, rss.Expression, SpeculativeBindingOption.BindAsExpression).Type;

                if (type == null)
                {
                    continue;
                }

                if (type.TypeKind == TypeKind.Error)
                {
                    rt = null;
                    break;
                }

                if (rt == null)
                {
                    rt = type;
                }
                else if (rt != type)
                {
                    rt = null;
                    break;
                }
            }

            if (rt == null)
            {
                return dynamic;
            }

            return CSharp.ParseTypeName(rt.Name);
        }

        //td: !!! refactor the marking
        public static SyntaxNode UnMark(SyntaxNode node)
        {
            return node.ReplaceNodes(node.DescendantNodesAndSelf(), (oldNode, newNode) =>
            {
                var id = NodeMark(oldNode);
                return newNode
                    .WithoutAnnotations(NodeIdAnnotation)
                    .WithoutAnnotations(NodeIdAnnotation + id);
            });
        }

        public static SyntaxNode Mark(SyntaxNode node)
        {
            return node.ReplaceNodes(node.DescendantNodesAndSelf(), (oldNode, newNode) => { return MarkNode(newNode, UniqueId()); });
        }

        public static bool IsVisible(MemberDeclarationSyntax member)
        {
            var modifiers = default(SyntaxTokenList);
            if (member is MethodDeclarationSyntax)
            {
                modifiers = (member as MethodDeclarationSyntax).Modifiers;
            }
            else if (member is PropertyDeclarationSyntax)
            {
                modifiers = (member as PropertyDeclarationSyntax).Modifiers;
            }
            else if (member is FieldDeclarationSyntax)
            {
                modifiers = (member as FieldDeclarationSyntax).Modifiers;
            }
            else if (member is ConstructorDeclarationSyntax)
            {
                modifiers = (member as ConstructorDeclarationSyntax).Modifiers;
            }
            else if (member is EnumDeclarationSyntax)
            {
                modifiers = (member as EnumDeclarationSyntax).Modifiers;
            }
            else
            {
                throw new NotImplementedException();
            }

            return modifiers
                .Any(modifier => modifier.Kind() == SyntaxKind.PublicKeyword
                                 || modifier.Kind() == SyntaxKind.InternalKeyword);
        }

        public static ExpressionSyntax Quoted(string value)
        {
            return CSharp.ParseExpression('"' + value + '"');
        }

        public static SyntaxNode ReplaceAssignment(SyntaxNode node, SyntaxNode newNode, out bool isAssignment)
        {
            isAssignment = false;
            if (node is LocalDeclarationStatementSyntax)
            {
                isAssignment = true;

                var decl = node as LocalDeclarationStatementSyntax;
                return decl
                    .WithDeclaration(decl.Declaration
                        .WithVariables(CSharp.SeparatedList(new[]
                        {
                            decl.Declaration.Variables[0]
                                .WithInitializer(decl.Declaration.Variables[0].Initializer
                                    .WithValue((ExpressionSyntax) newNode))
                        })));
            }

            if (node is BinaryExpressionSyntax)
            {
                var expr = node as BinaryExpressionSyntax;
                if (expr.Kind() == SyntaxKind.SimpleAssignmentExpression)
                {
                    isAssignment = true;
                    return expr.WithRight((ExpressionSyntax) newNode);
                }
            }

            return newNode;
        }

        //symbols
        public static ITypeSymbol SymbolType(SemanticModel model, SyntaxNode node)
        {
            if (node is CastExpressionSyntax)
            {
                return SymbolType(model
                    .GetSymbolInfo((node as CastExpressionSyntax)
                        .Type)
                    .Symbol);
            }

            if (node is InvocationExpressionSyntax)
            {
                return SymbolType(model, (node as InvocationExpressionSyntax).Expression);
            }

            return SymbolType(model.GetSymbolInfo(node).Symbol);
        }

        public static TypeSyntax SymbolTypeSyntax(SemanticModel model, SyntaxNode node)
        {
            var type = SymbolType(model, node);
            if (type == null)
            {
                return null;
            }

            return CSharp.ParseTypeName(type.Name);
        }

        public static TypeSyntax SymbolTypeSyntax(ISymbol symbol)
        {
            var type = SymbolType(symbol);
            if (type == null)
            {
                return null;
            }

            return CSharp.ParseTypeName(type.Name);
        }

        public static ITypeSymbol SymbolType(ISymbol symbol)
        {
            if (symbol == null)
            {
                return null;
            }

            switch (symbol.Kind)
            {
                case SymbolKind.Local:
                    return ((ILocalSymbol) symbol).Type;
                case SymbolKind.Field:
                    return ((IFieldSymbol) symbol).Type;
                case SymbolKind.Property:
                    return ((IPropertySymbol) symbol).Type;
                case SymbolKind.Method:
                    return ((IMethodSymbol) symbol).ReturnType;
                case SymbolKind.Parameter:
                    return ((IParameterSymbol) symbol).Type;
                case SymbolKind.NamedType:
                    return (ITypeSymbol) symbol;
                default:
                {
                    return null;
                }
            }
        }

        public static Dictionary<SyntaxNode, SyntaxNode> Track(SyntaxTree syntaxTree, Dictionary<SyntaxNode, SyntaxNode> nodes)
        {
            var result = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var node in nodes)
            {
                if (node.Key.SyntaxTree == syntaxTree)
                {
                    result[node.Key] = node.Value;
                }
                else
                {
                    var nn = Track(syntaxTree, node.Key);
                    if (nn != null)
                    {
                        result[nn] = node.Value;
                    }
                }
            }

            return result;
        }

        public static SyntaxNode Track(SyntaxTree syntaxTree, SyntaxNode node)
        {
            var mark = NodeMark(node);
            if (mark == null)
            {
                return null;
            }

            return FindNode(syntaxTree.GetRoot(), mark);
        }
    }
}
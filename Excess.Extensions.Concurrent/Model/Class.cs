using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Excess.Compiler;
using Excess.Compiler.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Excess.Extensions.Concurrent.Model
{
    using CSharp = SyntaxFactory;
    using Roslyn = RoslynCompiler;

    internal class Class
    {
        private readonly List<MemberDeclarationSyntax> _add = new List<MemberDeclarationSyntax>();

        private readonly List<MemberDeclarationSyntax> _remove = new List<MemberDeclarationSyntax>();

        private Dictionary<SyntaxNode, SyntaxNode> _replace = new Dictionary<SyntaxNode, SyntaxNode>();

        private readonly Dictionary<string, int> _signals = new Dictionary<string, int>();

        private readonly List<TypeDeclarationSyntax> _types = new List<TypeDeclarationSyntax>();

        public string Name { get; set; }
        public bool HasMain { get; set; }
        public IDictionary<int, Signal> Signals { get; }
        public Scope Scope { get; private set; }

        public Class(string name, Scope scope)
        {
            Scope = scope;
            Signals = new Dictionary<int, Signal>();
            Name = name;
        }

        public void AddMember(MemberDeclarationSyntax member)
        {
            _add.Add(member);
        }

        public bool IsSignal(MethodDeclarationSyntax node)
        {
            var name = node.Identifier.ToString();
            if (name == "__concurrentmain")
                return true;

            var pattern = "__concurrent";
            return _signals
                .Any(s => pattern + s.Key == name);
        }

        public bool hasSignal(string name)
        {
            return _signals
                .Any(s => s.Key == name);
        }

        public void RemoveMember(MemberDeclarationSyntax member)
        {
            _remove.Add(member);
        }

        public void Replace(SyntaxNode oldNode, SyntaxNode newNode)
        {
            _replace[oldNode] = newNode;
        }

        public Signal AddSignal(string name, bool isPublic)
        {
            if (_signals.ContainsKey(name))
                throw new InvalidOperationException("duplicate concurrent signal");

            var id = _signals.Count;
            _signals[name] = id;

            var signal = new Signal(id, name, isPublic);
            Signals[id] = signal;
            return signal;
        }

        public Signal AddSignal(string name, TypeSyntax returnType, bool isPublic)
        {
            var signal = AddSignal(name, isPublic);
            signal.ReturnType = returnType;
            return signal;
        }

        public Signal AddSignal()
        {
            var signal = AddSignal(Roslyn.UniqueId(), false);
            signal.Internal = true;
            return signal;
        }

        public Signal GetSignal(string name)
        {
            var result = 0;
            if (_signals.TryGetValue(name, out result))
                return Signals[result];
            return null;
        }

        public void AddType(TypeDeclarationSyntax type)
        {
            _types.Add(type);
        }

        public ClassDeclarationSyntax Update(ClassDeclarationSyntax @class)
        {
            _replace = RoslynCompiler.Track(@class.SyntaxTree, _replace);

            var result = @class
                .ReplaceNodes(_replace.Keys, (on, nn) => _replace[on])
                .RemoveNodes(_remove, SyntaxRemoveOptions.KeepNoTrivia)
                .AddMembers(_add
                    .Union(_types)
                    .Select(add => (MemberDeclarationSyntax) RoslynCompiler.TrackNode(add))
                    .ToArray());

            _remove.Clear();
            _add.Clear();
            _types.Clear();
            _replace.Clear();
            return result;
        }

        public bool hasMember(string name)
        {
            return _add
                .Any(member =>
                {
                    if (member is MethodDeclarationSyntax)
                        return (member as MethodDeclarationSyntax).Identifier.ToString() == name;
                    if (member is PropertyDeclarationSyntax)
                        return (member as PropertyDeclarationSyntax).Identifier.ToString() == name;
                    if (member is FieldDeclarationSyntax)
                        return (member as FieldDeclarationSyntax)
                            .Declaration
                            .Variables[0]
                            .Identifier.ToString() == name;

                    return false;
                });
        }

        public bool isQueueInvocation(InvocationExpressionSyntax invocation, bool asynch, ExpressionSyntax success, out StatementSyntax result)
        {
            result = null;
            var signal = invocation.Expression as MemberAccessExpressionSyntax;
            if (signal == null)
                return false;

            if (!(signal.Expression is IdentifierNameSyntax))
                return false;

            var signalName = signal.Expression.ToString();
            if (asynch && !hasSignal(signalName))
                return false;

            if (!asynch)
            {
                if (!invocation
                    .Ancestors()
                    .Where(node => node is ClassDeclarationSyntax)
                    .First()
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(method => method
                        .Identifier.ToString()
                        .Equals(signalName))
                    .Any())
                    return false;
            }

            var queueName = "__queue" + signalName;
            switch (signal.Name.ToString())
            {
                case "enqueue":
                    Debug.Assert(asynch);
                    if (!hasMember(queueName))
                        AddMember(Templates
                            .SignalQueueMember
                            .Get<MemberDeclarationSyntax>(queueName));

                    result = Templates
                        .Enqueue
                        .Get<StatementSyntax>(queueName, success);
                    break;
                case "dequeue":
                    result = asynch
                        ? Templates
                            .DequeueAsynch
                            .Get<StatementSyntax>(queueName, queueName + "_Top")
                        : Templates
                            .DequeueSynch
                            .Get<StatementSyntax>(queueName, queueName + "_Top");
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
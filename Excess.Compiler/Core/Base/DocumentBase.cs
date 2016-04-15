using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Excess.Compiler.Core
{
    public abstract class BaseDocument<TToken, TNode, TModel> : IDocument<TToken, TNode, TModel>
    {
        private readonly List<Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>>> _lexical =
            new List<Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>>>();

        private readonly List<ChangeDto> _lexicalChanges = new List<ChangeDto>();

        private readonly List<Func<TNode, TModel, Scope, TNode>> _semantical = new List<Func<TNode, TModel, Scope, TNode>>();

        private readonly List<ChangeDto> _semanticalChanges = new List<ChangeDto>();

        private readonly List<Func<TNode, Scope, TNode>> _syntactical = new List<Func<TNode, Scope, TNode>>();

        private readonly List<ChangeDto> _syntacticalChanges = new List<ChangeDto>();
        private readonly Dictionary<string, Func<TNode, Scope, TNode>> _syntacticalPass = new Dictionary<string, Func<TNode, Scope, TNode>>();

        protected ICompilerService<TToken, TNode, TModel> Compiler;
        protected TNode Original;

        protected TNode Root;
        
        private int _semanticalTries;

        protected BaseDocument(Scope scope)
        {
            Scope = new Scope(scope);
            Compiler = Scope.GetService<TToken, TNode, TModel>();
            Stage = CompilerStage.Started;

            //setup the node repository
            //td: per document?
            Scope.Set<IDictionary<int, Scope>>(new Dictionary<int, Scope>());
        }

        public CompilerStage Stage { get; private set; }

        public string Text { get; set; }

        public TNode SyntaxRoot { get { return GetRoot(); } set { SetRoot(value); } }

        public TModel Model { get; set; }

        public Scope Scope { get; }

        public IMappingService<TNode> Mapper { get; set; }


        public void Change(Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>> transform, string kind)
        {
            if (kind != null)
            {
                throw new NotImplementedException();
            }

            _lexical.Add(transform);
        }

        public TToken Change(TToken token, Func<TNode, Scope, TNode> transform, string kind)
        {
            int tokenId;
            var result = Compiler.MarkToken(token, out tokenId);

            _lexicalChanges.Add(new ChangeDto
            {
                Id = tokenId,
                Kind = kind,
                Transform = transform
            });

            return result;
        }

        public IEnumerable<TToken> Change(IEnumerable<TToken> tokens, Func<TNode, Scope, TNode> transform, string kind)
        {
            int id;
            var result = Compiler.MarkTokens(tokens, out id);
            _lexicalChanges.Add(new ChangeDto
            {
                Id = id,
                Kind = kind,
                Transform = transform
            });

            return result;
        }

        public TToken Change(TToken token, Func<TNode, TNode, TModel, Scope, TNode> transform, string kind = null)
        {
            int tokenId;
            var result = Compiler.MarkToken(token, out tokenId);

            _lexicalChanges.Add(new ChangeDto
            {
                Id = tokenId,
                Kind = kind,
                SemanticalTransform = transform
            });

            return result;
        }

        public IEnumerable<TToken> Change(IEnumerable<TToken> tokens, Func<TNode, TNode, TModel, Scope, TNode> transform, string kind = null)
        {
            int id;
            var result = Compiler.MarkTokens(tokens, out id);
            _lexicalChanges.Add(new ChangeDto
            {
                Id = id,
                Kind = kind,
                SemanticalTransform = transform
            });

            return result;
        }

        public void Change(Func<TNode, Scope, TNode> transform, string kind)
        {
            if (kind != null)
            {
                //known kinds
                switch (kind)
                {
                    case "normalize":
                    {
                        _lexicalChanges.Add(new ChangeDto
                        {
                            Kind = kind,
                            Transform = transform
                        });
                        break;
                    }
                    default:
                    {
                        _syntacticalPass[kind] = transform;
                        break;
                    }
                }
            }
            else
            {
                _syntactical.Add(transform);
            }
        }

        public TNode Change(TNode node, Func<TNode, Scope, TNode> transform, string kind = null)
        {
            int nodeId;
            var result = Compiler.MarkNode(node, out nodeId);

            if (Stage <= CompilerStage.Syntactical)
            {
                _syntacticalChanges.Add(new ChangeDto
                {
                    Id = nodeId,
                    Kind = kind,
                    Transform = transform
                });
            }
            else
            {
                Debug.Assert(Stage == CompilerStage.Semantical);
                _semanticalChanges.Add(new ChangeDto
                {
                    Id = nodeId,
                    Kind = kind,
                    SemanticalTransform = (oldNode, newNode, model, scope) => transform(newNode, scope)
                });
            }

            return result;
        }

        public void Change(Func<TNode, TModel, Scope, TNode> transform, string kind = null)
        {
            Debug.Assert(kind == null); //td: 

            _semantical.Add(transform);
        }

        public TNode Change(TNode node, Func<TNode, TNode, TModel, Scope, TNode> transform, string kind)
        {
            int nodeId;
            var result = Compiler.MarkNode(node, out nodeId);

            _semanticalChanges.Add(new ChangeDto
            {
                Id = nodeId,
                Kind = kind,
                SemanticalTransform = transform
            });

            return result;
        }

        public bool ApplyChanges()
        {
            return ApplyChanges(CompilerStage.Finished);
        }

        public bool ApplyChanges(CompilerStage stage)
        {
            if (stage < Stage)
            {
                return true;
            }

            var oldStage = Stage;
            Stage = stage;

            if (oldStage < CompilerStage.Lexical && stage >= CompilerStage.Lexical)
            {
                ApplyLexical();
            }

            if (oldStage < CompilerStage.Syntactical && stage >= CompilerStage.Syntactical)
            {
                ApplySyntactical();
                _semanticalTries = 0;
            }

            if (oldStage <= CompilerStage.Semantical && stage >= CompilerStage.Semantical)
            {
                var result = ApplySemantical();
                if (result)
                {
                    _semanticalTries = 0;
                }
                else
                {
                    _semanticalTries++;
                }
                return result;
            }

            return true; //finished
        }

        public bool HasSemanticalChanges()
        {
            if (_semanticalTries > 5)
            {
                return false; //shortcut
            }

            if (_semanticalTries == 0 && _semantical.Count > 0)
            {
                return true;
            }

            return _semanticalChanges.Count > 0;
        }

        public abstract bool HasErrors();

        protected abstract TNode GetRoot();
        protected abstract void SetRoot(TNode node);

        protected virtual void ApplyLexical()
        {
            Debug.Assert(Root == null);

            //apply the lexical pass
            var tokens = Compiler.ParseTokens(Text);
            foreach (var lexical in _lexical)
            {
                tokens = lexical(tokens, new Scope(Scope));
            }

            //build modified text for syntactic parsing
            string resultText;
            var annotations = new Dictionary<string, SourceSpan>();
            Root = CalculateNewText(tokens, annotations, out resultText);

            //allow extensions writers to perform one last rewrite before the syntactical 
            Root = ApplyNodeChanges(Root, "lexical-extension", CompilerStage.Lexical);

            //move any pending changes to the appropriate pass
            _syntacticalChanges.AddRange(_lexicalChanges
                .Where(change => change.Transform != null));

            _semanticalChanges.AddRange(_lexicalChanges
                .Where(change => change.SemanticalTransform != null));
        }

        private TNode CalculateNewText(IEnumerable<TToken> tokens, Dictionary<string, SourceSpan> annotations, out string modifiedText)
        {
            //td: !! mapping info
            var newText = new StringBuilder();
            string currId = null;
            foreach (var token in tokens)
            {
                string excessId;
                var toInsert = Compiler.TokenToString(token, out excessId);

                //store the actual position in the transformed stream of any tokens pending processing
                if (excessId != currId)
                {
                    if (excessId != null)
                    {
                        annotations[excessId] = new SourceSpan(newText.Length, toInsert.Length);
                    }

                    currId = excessId;
                }
                else if (excessId != null)
                {
                    //augment span
                    var span = annotations[excessId];
                    span.Length += toInsert.Length;
                }
                else
                {
                    currId = null;
                }

                newText.Append(toInsert);
            }

            //parse
            modifiedText = newText.ToString();
            var root = Compiler.Parse(modifiedText);

            //assign ids to the nodes found
            root = Compiler.MarkTree(root);
            Original = root;

            ProcessAnnotations(root, annotations);

            //allow for preprocessing of the original 
            NotifyOriginal(modifiedText);

            //apply any scheduled normalization
            var normalizers = Poll(_lexicalChanges, "normalize");
            foreach (var normalizer in normalizers)
            {
                root = normalizer.Transform(root, new Scope(Scope));
            }

            //allow for preprocessing
            Original = UpdateRoot(Original);
            root = UpdateRoot(root);
            return root;
        }

        private void ProcessAnnotations(TNode root, Dictionary<string, SourceSpan> annotations)
        {
            foreach (var annotation in annotations)
            {
                var aNode = Compiler.Find(root, annotation.Value);
                Debug.Assert(aNode != null);

                //change the id of the change from token to node
                var annotationId = int.Parse(annotation.Key);
                foreach (var change in _lexicalChanges)
                {
                    if (change.Id == annotationId)
                    {
                        change.Id = Compiler.GetExcessId(aNode);
                        Debug.Assert(change.Id >= 0);
                    }
                }
            }
        }

        protected virtual void NotifyOriginal(string newText)
        {
        }

        protected virtual TNode UpdateRoot(TNode root)
        {
            return root;
        }

        private TNode ApplyNodeChanges(TNode root, CompilerStage stage)
        {
            return ApplyNodeChanges(root, null, stage);
        }

        private void AddTransformer(IDictionary<int, Func<TNode, TNode, TModel, Scope, TNode>> transformers, ChangeDto change)
        {
            var transform = change.SemanticalTransform;
            if (transform == null)
            {
                Debug.Assert(change.Transform != null);
                transform = (oldNode, newNode, model, scope) => change.Transform(oldNode, scope);
            }

            Func<TNode, TNode, TModel, Scope, TNode> existing;
            if (transformers.TryGetValue(change.Id, out existing))
            {
                var newTransform = transform;
                transform = (oldNode, newNode, model, scope) =>
                {
                    var result = existing(oldNode, newNode, model, scope);
                    return newTransform(oldNode, result, model, scope);
                };
            }

            transformers[change.Id] = transform;
        }

        private void AddTransformer(IDictionary<int, Func<TNode, Scope, TNode>> transformers, ChangeDto change)
        {
            var transform = change.Transform;

            Func<TNode, Scope, TNode> existing;
            if (transformers.TryGetValue(change.Id, out existing))
            {
                var newTransform = transform;
                transform = (node, scope) =>
                {
                    var result = existing(node, scope);
                    return newTransform(result, scope);
                };
            }

            transformers[change.Id] = transform;
        }

        private TNode ApplyNodeChanges(TNode node, string kind, CompilerStage stage)
        {
            List<ChangeDto> changeList;
            switch (stage)
            {
                case CompilerStage.Lexical:
                    changeList = _lexicalChanges;
                    break;
                case CompilerStage.Syntactical:
                    changeList = _syntacticalChanges;
                    break;
                case CompilerStage.Semantical:
                    changeList = _semanticalChanges;
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (!changeList.Any())
            {
                return node;
            }

            IEnumerable<ChangeDto> changes;
            if (kind != null)
            {
                changes = Poll(changeList, kind);
            }
            else
            {
                changes = new List<ChangeDto>(changeList);
                changeList.Clear(); //take all for the stage
            }

            var changeDtos = changes as ChangeDto[] ?? changes.ToArray();
            if (changes == null || !changeDtos.Any())
            {
                return node;
            }

            if (stage == CompilerStage.Semantical)
            {
                var transformers = new Dictionary<int, Func<TNode, TNode, TModel, Scope, TNode>>();
                foreach (var change in changeDtos)
                {
                    AddTransformer(transformers, change);
                    //if (change.SemanticalTransform != null)
                    //    transformers[change.ID] = change.SemanticalTransform; duplicates IDS
                    //else
                    //{
                    //    Debug.Assert(change.Transform != null);
                    //    transformers[change.ID] = (oldNode, newNode, model, scope) => change.Transform(oldNode, scope);
                    //}
                }

                return Transform(node, transformers);
            }
            else
            {
                var transformers = new Dictionary<int, Func<TNode, Scope, TNode>>();
                foreach (var change in changeDtos)
                {
                    if (change.Transform != null)
                    {
                        AddTransformer(transformers, change);
                    }

                    //transformers[change.ID] = change.Transform;
                }

                return Transform(node, transformers);
            }
        }

        protected IEnumerable<ChangeDto> Poll(List<ChangeDto> changes, string kind)
        {
            for (var i = changes.Count - 1; i >= 0; i--)
            {
                var change = changes[i];
                if (change.Kind == kind)
                {
                    yield return change;
                    changes.RemoveAt(i);
                }
            }
        }

        protected abstract TNode Transform(TNode root, Dictionary<int, Func<TNode, Scope, TNode>> transformers);
        protected abstract TNode Transform(TNode root, Dictionary<int, Func<TNode, TNode, TModel, Scope, TNode>> transformers);

        protected virtual void ApplySyntactical()
        {
            Debug.Assert(Root != null);
            Root = Pass("syntactical-extensions", Root, Scope);

            if (_syntactical.Any())
            {
                Root = SyntacticalTransform(Root, Scope, _syntactical);
            }

            do
            {
                Root = ApplyNodeChanges(Root, CompilerStage.Syntactical);
            } while (_syntacticalChanges.Any());

            //add modules before going into semantics
            var environment = Scope.Find<ICompilerEnvironment>();
            if (environment != null)
            {
                Root = AddModules(Root, environment.Modules());
            }
        }

        protected abstract TNode AddModules(TNode root, IEnumerable<string> modules);
        protected abstract TNode SyntacticalTransform(TNode node, Scope scope, IEnumerable<Func<TNode, Scope, TNode>> transformers);

        protected TNode Pass(string kind, TNode node, Scope scope)
        {
            Func<TNode, Scope, TNode> transform;
            if (_syntacticalPass.TryGetValue(kind, out transform))
            {
                return transform(node, scope);
            }

            return node;
        }

        protected virtual bool ApplySemantical()
        {
            if (_semanticalTries > 5)
            {
                return true; //shortcut
            }

            var oldRoot = Root;

            Root = ApplyNodeChanges(Root, CompilerStage.Semantical);

            foreach (var semantical in _semantical)
            {
                Root = semantical(Root, Model, Scope);
            }

            return oldRoot.Equals(Root);
        }

        protected class ChangeDto
        {
            public int Id { get; set; }
            public string Kind { get; set; }
            public Func<TNode, Scope, TNode> Transform { get; set; }
            public Func<TNode, TNode, TModel, Scope, TNode> SemanticalTransform { get; set; }
        }
    }
}
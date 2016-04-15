using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Excess.Compiler.Roslyn
{
	public class RoslynEnvironment : ICompilerEnvironment
	{
		private readonly List<string> _keywords = new List<string>();
		private readonly List<string> _modules = new List<string>();

		private dynamic _path;


		private readonly List<MetadataReference> _references = new List<MetadataReference>();
		private readonly Scope _root;

		private readonly IPersistentStorage _storage;

		public RoslynEnvironment(Scope root, IPersistentStorage storage)
		{
			_root = root;
			_storage = storage;
		}

		public ICompilerEnvironment Dependency<T>(string module)
		{
			return Dependency<T>(
				string.IsNullOrEmpty(module)
					? null
					: new[] {module});
		}

		public ICompilerEnvironment Dependency<T>(IEnumerable<string> modules)
		{
			MetadataReference reference = MetadataReference.CreateFromFile(typeof (T).Assembly.Location);
			Debug.Assert(reference != null);

			_references.Add(reference);

			if (modules != null)
			{
				AddModules(modules);
			}

			return this;
		}

		public ICompilerEnvironment Dependency(string module, string path = null)
		{
			return Dependency(
				string.IsNullOrEmpty(module)
					? null
					: new[] {module},
				path);
		}

		public ICompilerEnvironment Dependency(IEnumerable<string> modules, string path = null)
		{
			if (path != null)
			{
				MetadataReference reference = MetadataReference.CreateFromFile(path);
				Debug.Assert(reference != null);

				_references.Add(reference);
			}

			if (modules != null)
			{
				AddModules(modules);
			}

			return this;
		}

		public ICompilerEnvironment Keyword(string word)
		{
			_keywords.Add(word);
			return this;
		}

		public ICompilerEnvironment Global<T>() where T : class, new()
		{
			_root.set(new T());
			return this;
		}

		public IEnumerable<string> Modules()
		{
			return _modules;
		}

		public IEnumerable<string> Keywords()
		{
			return _keywords;
		}

		public dynamic Path()
		{
			return _path;
		}

		public IPersistentStorage Storage()
		{
			return _storage;
		}

		public void SetPath(dynamic path)
		{
			_path = path;
		}

		private void AddModules(IEnumerable<string> modules)
		{
			foreach (var module in modules)
			{
				if (!_modules.Contains(module))
				{
					_modules.Add(module);
				}
			}
		}

		internal IEnumerable<MetadataReference> GetReferences()
		{
			return _references;
		}
	}
}
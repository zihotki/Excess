using System.Collections.Generic;

namespace Excess.Compiler
{
    public interface ICompilerEnvironment
    {
        ICompilerEnvironment Dependency<T>(string module);
        ICompilerEnvironment Dependency<T>(IEnumerable<string> modules);
        ICompilerEnvironment Dependency(string module, string path = null);
        ICompilerEnvironment Dependency(IEnumerable<string> modules, string path = null);

        ICompilerEnvironment Keyword(string word);

        ICompilerEnvironment Global<T>() where T : class, new();

        IEnumerable<string> Modules();
        IEnumerable<string> Keywords();
        dynamic Path();
        IPersistentStorage Storage();
    }
}
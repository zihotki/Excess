using Excess.Compiler;
using Excess.RuntimeProject;

namespace Excess
{
    public interface IProjectManager
    {
        IRuntimeProject CreateRuntime(string projectType, string projectName, dynamic config, dynamic path, IPersistentStorage storage);
    }
}
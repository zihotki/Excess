using Excess.Compiler;
using Excess.RuntimeProject;

namespace Excess
{
    public interface IUserServices
    {
        string userId();
    }

    public interface ITranslationService
    {
        string translate(string text);
    }

    public interface IProjectManager
    {
        IRuntimeProject createRuntime(string projectType, string projectName, dynamic config, dynamic path, IPersistentStorage storage);
    }
}
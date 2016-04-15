namespace Excess.Compiler
{
    public interface IPersistentStorage
    {
        int AddFile(string name, string contents, bool hidden);
        int CachedId(string name);
        void CachedId(string name, int id);
    }
}
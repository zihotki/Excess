using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excess.RuntimeProject
{
    public enum NotificationKind
    {
        Started,
        System,
        Application,
        Error,
        Finished,
    }

    public class Notification
    {
        public NotificationKind Kind { get; set; }
        public string Message { get; set; }
    }

    public interface INotifier
    {
        void notify(Notification notification);
    }

    public class Error
    {
        public string File { get; set; }
        public int Line { get; set; }
        public int Character { get; set; }
        public string Message { get; set; }
    }

    public interface IRuntimeProject
    {
        bool Busy();
        IEnumerable<Error> Compile();
        IEnumerable<Error> Run(INotifier notifier, out dynamic client);
        void Add(string file, int id, string contents);
        void Modify(string file, string contents);
        string DefaultFile();
        string FileContents(string file);
        int FileId(string file);
        IEnumerable<TreeNodeAction> FileActions(string file);
        void setFilePath(dynamic path);
    }

    public interface IExtensionRuntime
    {
        string debugExtension(string text);
        bool generateGrammar(out string extension, out string transform);
    }
}

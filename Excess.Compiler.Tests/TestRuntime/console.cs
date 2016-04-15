using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Excess.Compiler.Tests.TestRuntime
{
    public class console
    {
        private static readonly List<string> _items = new List<string>();
        private static readonly object _consoleLock = new object();

        public static void write(object message)
        {
            Debug.WriteLine(message);
            lock (_consoleLock)
            {
                if (message == null)
                    _items.Add("null");
                else
                    _items.Add(message.ToString()); //DateTime.Now.ToString("mm:ss.ff") + ": " + 
            }
        }

        public static string[] items()
        {
            string[] result;
            lock (_consoleLock)
            {
                result = _items.ToArray();
            }

            return result;
        }

        public static string[] clear()
        {
            string[] result;
            lock (_consoleLock)
            {
                result = _items.ToArray();
                _items.Clear();
            }

            return result;
        }

        public static string last()
        {
            string result;
            lock (_consoleLock)
            {
                result = _items.Last();
            }

            return result;
        }
    }
}
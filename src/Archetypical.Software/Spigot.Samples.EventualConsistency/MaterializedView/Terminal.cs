using System.Collections.Concurrent;

namespace Spigot.Samples.EventualConsistency.MaterializedView
{
    public class Terminal
    {
        public string TerminalNumber { get; internal set; }

        public Terminal(string number)
        {
            TerminalNumber = number;
        }

        public ConcurrentDictionary<string, Gate> Gates { get; set; } = new ConcurrentDictionary<string, Gate>();
    }
}
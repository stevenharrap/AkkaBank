using System.Collections.Immutable;

namespace AkkaBank.BasicBank.Messages.Console
{
    public class ConsoleOutput
    {
        public ImmutableArray<string> Output { get; }
        public bool Clear { get; }
        public bool Boxed { get; }
        public int Padding { get; }

        public ConsoleOutput(string output) : this(new[] { output }) { }

        public ConsoleOutput(string[] output, bool clear = false, bool boxed = false, int padding = 2)
        {
            Output = output.ToImmutableArray();
            Clear = clear;
            Boxed = boxed;
            Padding = padding;
        }
    }
}
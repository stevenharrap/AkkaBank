using System.Collections.Immutable;

namespace AkkaBank.BasicBank.Messages.Console
{
    public class ConsoleOutputMessage
    {
        public ImmutableArray<string> Output { get; }
        public bool Clear { get; }
        public bool Boxed { get; }
        public int Padding { get; }

        public ConsoleOutputMessage(string output) : this(new[] { output }) { }

        public ConsoleOutputMessage(string[] output, bool clear = false, bool boxed = false, int padding = 2)
        {
            Output = output.ToImmutableArray();
            Clear = clear;
            Boxed = boxed;
            Padding = padding;
        }
    }
}
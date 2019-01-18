namespace AkkaBank.BasicBank.Messages.Console
{
    public class ConsoleOutputMessage
    {
        public string Output { get; }
        public bool Clear { get; }

        public ConsoleOutputMessage(string output, bool clear = false)
        {
            Output = output;
            Clear = clear;
        }
    }
}
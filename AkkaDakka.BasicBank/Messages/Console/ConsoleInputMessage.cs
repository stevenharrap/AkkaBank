namespace AkkaBank.BasicBank.Messages.Console
{
    public class ConsoleInputMessage
    {
        public string Input { get; }

        public ConsoleInputMessage(string input)
        {
            Input = input;
        }
    }
}
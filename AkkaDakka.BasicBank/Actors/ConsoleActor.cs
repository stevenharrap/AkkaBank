using System;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using Akka.Actor;
using AkkaBank.BasicBank.Messages.Console;

namespace AkkaBank.BasicBank.Actors
{
    public class ConsoleActor : ReceiveActor
    {
        private CancellationTokenSource _consoleCancellation;

        public ConsoleActor()
        {
            Receive<ConsoleOutputMessage>(message => HandleConsoleOutput(message));
            Receive<string>(message => HandleConsoleOutput(new ConsoleOutputMessage(message)));
            Receive<CloseConsole>(message => HandleCloseConsole(message));
        }

        protected override void PreStart()
        {
            //Note: this does not fix the problem!
            _consoleCancellation = new CancellationTokenSource();
            var parent = Context.Parent;
            Task.Run(() =>
            {
                while (!_consoleCancellation.IsCancellationRequested)
                {
                    var input = Console.ReadLine();
                    parent.Tell(new ConsoleInputMessage(input));
                }
            }, _consoleCancellation.Token);

            Console.WriteLine("BLEEP BLEEP. SIMPLE BANK CONSOLE ONLINE.");
        }

        private void HandleCloseConsole(CloseConsole message)
        {
            _consoleCancellation.Cancel();
            new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }

        private void HandleConsoleOutput(ConsoleOutputMessage message)
        {
            if (message.Clear)
            {
                Console.Clear();
            }

            Console.Write(message.Output);
        }
    }
}

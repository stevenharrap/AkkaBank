using System;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using Akka.Actor;
using AkkaBank.BasicBank.Messages.Console;
using System.Linq;

namespace AkkaBank.BasicBank.Actors
{
    public class ConsoleActor : ReceiveActor
    {
        private CancellationTokenSource _consoleCancellation;

        public ConsoleActor()
        {
            Receive<ConsoleOutput>(message => HandleConsoleOutput(message));
            Receive<string>(message => HandleConsoleOutput(new ConsoleOutput(message)));
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
                    parent.Tell(new ConsoleInput(input));
                }
            }, _consoleCancellation.Token);
                        
            Console.WriteLine("BLEEP BLEEP. SIMPLE BANK CONSOLE ONLINE.");

            //Pretend that it takes some time to start the ATM console.
            Task.Delay(2000).GetAwaiter().GetResult();
        }

        private void HandleCloseConsole(CloseConsole message)
        {
            _consoleCancellation.Cancel();
            new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }

        private void HandleConsoleOutput(ConsoleOutput message)
        {
            if (message.Clear)
            {
                Console.Clear();
            }

            if (!message.Boxed)
            {
                foreach (var line in message.Output)
                {
                    Console.WriteLine(line);
                }
                return;
            }

            var maxLineLength = message.Output.Max(line => line.Length);
            Console.WriteLine(Stars(maxLineLength + (message.Padding * 2) + 2));
            Console.WriteLine($"*{Space(maxLineLength + (message.Padding * 2))}*");
            Console.WriteLine($"*{Space(maxLineLength + (message.Padding * 2))}*");

            foreach (var line in message.Output)
            {
                Console.WriteLine($"*{Space(message.Padding)}{line}{Space(maxLineLength - line.Length + message.Padding)}*");
            }

            Console.WriteLine($"*{Space(maxLineLength + (message.Padding * 2))}*");
            Console.WriteLine($"*{Space(maxLineLength + (message.Padding * 2))}*");
            Console.WriteLine(Stars(maxLineLength + (message.Padding * 2) + 2));

            string Space(int n) => new string(' ', n);
            string Stars(int n) => new string('*', n);
        }
    }
}

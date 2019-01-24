using System;
using Akka.Actor;
using AkkaBank.BasicBank.Actors;
using AkkaBank.BasicBank.Messages.Account;
using AkkaBank.BasicBank.Messages.Bank;

namespace AkkaBank.ConsoleDirect
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("my-actor-system");
            var bankActor = actorSystem.ActorOf(Props.Create(() => new AccountActor()), "basic-bank-account");
            var input = string.Empty;

            while (input != "x")
            {
                Console.WriteLine("[x] Exit, [d] deposit, [w] withdraw");
                input = Console.ReadLine();
                var balance = 0;

                switch (input)
                {
                    case "d":
                        Console.WriteLine("Enter amount to deposit...");
                        if (int.TryParse(Console.ReadLine(), out var save))
                        {
                            balance = (bankActor.Ask<ReceiptMessage>(new DepositMoneyMessage(save)).GetAwaiter().GetResult()).Balance;
                        }
                        break;

                    case "w":
                        Console.WriteLine("Enter amount to withdraw...");
                        if (int.TryParse(Console.ReadLine(), out var spend))
                        {
                            balance = (bankActor.Ask<ReceiptMessage>(new WithdrawMoneyMessage(spend)).GetAwaiter().GetResult()).Balance;
                        }
                        break;
                }

                Console.WriteLine($"Your balance is: ${balance}");
            }
        }
    }
}

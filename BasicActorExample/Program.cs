using System;
using Akka.Actor;
using AkkaDakka.BasicBank.Actors;
using AkkaDakka.BasicBank.Messages;

namespace BasicActorExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("my-actor-system");
            var bankActor = actorSystem.ActorOf(Props.Create(() => new BankActor()), "basic-bank");
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
                            balance = (bankActor.Ask<BalanceMessage>(new DepositMoneyMessage(save)).GetAwaiter().GetResult()).Amount;
                        }
                        break;

                    case "w":
                        Console.WriteLine("Enter amount to withdraw...");
                        if (int.TryParse(Console.ReadLine(), out var spend))
                        {
                            balance = (bankActor.Ask<BalanceMessage>(new WithdrawMoneyMessage(spend)).GetAwaiter().GetResult()).Amount;
                        }
                        break;
                }

                Console.WriteLine($"Your balance is: ${balance}");
            }
        }
    }
}

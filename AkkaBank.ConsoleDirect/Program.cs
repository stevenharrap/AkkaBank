using System;
using Akka.Actor;
using AkkaBank.BasicBank.Actors;
using AkkaBank.BasicBank.Messages.Account;

namespace AkkaBank.ConsoleDirect
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // keep me safe somewhere
            var actorSystem = ActorSystem.Create("my-actor-system");
            // Create an account actor for our one customer (but we can only see it as an IActorRef)
            // The address of the actor will be "akka://my-actor-system/user/basic-bank-account"
            var accountActor = actorSystem.ActorOf(Props.Create(() => new AccountActor()), "basic-bank-account");
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
                            //We're asking the account to process this and respond to us
                            //So we're blocked till it responds... hope it was built well!
                            balance = (accountActor.Ask<ReceiptResponse>(new DepositRequest(save)).GetAwaiter().GetResult()).Balance;
                        }
                        break;

                    case "w":
                        Console.WriteLine("Enter amount to withdraw...");
                        if (int.TryParse(Console.ReadLine(), out var spend))
                        {
                            balance = (accountActor.Ask<ReceiptResponse>(new WithdrawRequest(spend)).GetAwaiter().GetResult()).Balance;
                        }
                        break;
                }

                Console.WriteLine($"Your balance is: ${balance}");
            }
        }
    }
}

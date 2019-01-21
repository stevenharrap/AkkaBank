using Akka.Actor;
using AkkaBank.BasicBank.Actors;
using AkkaBank.BasicBank.Messages.Bank;

namespace AkkaBank.ConsoleAtmV1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("my-actor-system");

            var account = actorSystem.ActorOf(Props.Create(() => new AccountActor()), "mrs-smith-account");
            var atmV1 = actorSystem.ActorOf(Props.Create(() => new AtmV1Actor()), "simple-bank-atm");

            atmV1.Tell(new AccoutAcctorMessage(account));

            while (true)
            {
            }
        }
    }
}

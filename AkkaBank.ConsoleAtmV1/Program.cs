using Akka.Actor;
using AkkaBank.BasicBank.Actors;

namespace AkkaBank.ConsoleAtmV1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("my-actor-system");

            var account = actorSystem.ActorOf(Props.Create(() => new AccountActor()), "mrs-smith-account");
            var atm = actorSystem.ActorOf(Props.Create(() => new AtmV1Actor()), "simple-bank-atm");

            atm.Tell(new BasicBank.Messages.Atm.AccountActor(account));

            while (true)
            {
            }
        }
    }
}

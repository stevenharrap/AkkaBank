using Akka.Actor;
using AkkaBank.BasicBank.Actors;

namespace AkkaBank.ConsoleAtm
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("my-actor-system");

            actorSystem.ActorOf(Props.Create(() => new AccountActor()), "the-bank-account");
            actorSystem.ActorOf(Props.Create(() => new AtmActor()), "simple-bank-atm");

            while (true)
            {
            }
        }
    }
}

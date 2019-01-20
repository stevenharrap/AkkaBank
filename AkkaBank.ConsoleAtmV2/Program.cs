using Akka.Actor;
using AkkaBank.BasicBank.Actors;
using AkkaBank.BasicBank.Messages.Bank;

namespace AkkaBank.ConsoleAtmV2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("my-actor-system");

            var atmV1 = actorSystem.ActorOf(Props.Create(() => new AtmV2Actor()), "simple-bank-atm");            

            while (true)
            {
            }
        }
    }
}

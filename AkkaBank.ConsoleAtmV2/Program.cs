using System.Threading.Tasks;
using Akka.Actor;
using AkkaBank.BasicBank.Actors;
using AkkaBank.BasicBank.Messages.Bank;

namespace AkkaBank.ConsoleAtmV2
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("my-actor-system");

            var bank = actorSystem.ActorOf(Props.Create(() => new BankActor()), "simple-bank");
            await Task.Delay(1000);
            bank.Tell(new CreateAccountMessage(123, "Billy White"));
            bank.Tell(new CreateAccountMessage(456, "Sally Brown"));
            bank.Tell(new CreateAccountMessage(789, "Wally Green"));

            //var atmV2 = actorSystem.ActorOf(Props.Create(() => new AtmV2Actor()), "simple-bank-atm");

            //await Task.Delay(2000);

            //atmV2.Tell(new BankActorMessage(bank));

            while (true)
            {
                await Task.Delay(10);
            }
        }
    }
}

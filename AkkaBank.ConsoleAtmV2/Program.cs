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
            bank.Tell(new CreateCustomerRequestMessage(new Customer(123, "Billy White")));
            bank.Tell(new CreateCustomerRequestMessage(new Customer(456, "Sally Brown")));
            bank.Tell(new CreateCustomerRequestMessage(new Customer(789, "Wally Green")));

            var atmV2 = actorSystem.ActorOf(Props.Create(() => new AtmV2Actor()), "simple-bank-atm");
            atmV2.Tell(new BankActorMessage(bank));

            while (true)
            {
                await Task.Delay(10);
            }
        }
    }
}

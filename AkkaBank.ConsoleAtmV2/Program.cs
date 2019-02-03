using System.Threading.Tasks;
using Akka.Actor;
using AkkaBank.BasicBank.Actors;
using AkkaBank.BasicBank.Messages.Bank;
using AkkaBank.ConsoleNode;

namespace AkkaBank.ConsoleAtmV2
{
    internal class Program : ConsoleNodeBase
    {
        private static async Task Main(string[] args)
        {            
            var actorSystem = ActorSystem.Create("my-actor-system");

            var bank = actorSystem.ActorOf(Props.Create(() => new BankV1Actor()), "simple-bank");
            bank.Tell(new CreateCustomerRequest(new Customer(123, "Billy White")));
            bank.Tell(new CreateCustomerRequest(new Customer(456, "Sally Brown")));
            bank.Tell(new CreateCustomerRequest(new Customer(789, "Wally Green")));

            var atm = actorSystem.ActorOf(Props.Create(() => new AtmV2Actor()), "simple-bank-atm");
            atm.Tell(new SetBank(bank));

            while (true)
            {
                await Task.Delay(10);
            }
        }
    }
}

using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using AkkaBank.BasicBank.Actors;
using AkkaBank.BasicBank.Messages.Bank;
using AkkaBank.ConsoleNode;

namespace AkkaBank.ConsoleAtmV2
{
    internal class Program : ConsoleNodeBase
    {
        private static async Task Main(string[] args)
        {
            //Necessary for examples further on.
            var config = ConfigurationFactory.ParseString(SingleHocon);
            var actorSystem = ActorSystem.Create(ClusterName, config);

            var bank = actorSystem.ActorOf(Props.Create(() => new BasicBank.Actors.BankActor()), "simple-bank");
            bank.Tell(new CreateCustomerRequest(new Customer(123, "Billy White")));
            bank.Tell(new CreateCustomerRequest(new Customer(456, "Sally Brown")));
            bank.Tell(new CreateCustomerRequest(new Customer(789, "Wally Green")));

            var atmV2 = actorSystem.ActorOf(Props.Create(() => new AtmV2Actor()), "simple-bank-atm");
            atmV2.Tell(new BasicBank.Messages.Bank.BankActor(bank));

            while (true)
            {
                await Task.Delay(10);
            }
        }
    }
}

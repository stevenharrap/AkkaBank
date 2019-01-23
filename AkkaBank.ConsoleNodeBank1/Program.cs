using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using AkkaBank.BasicBank.Actors;
using AkkaBank.ConsoleNode;

namespace AkkaBank.ConsoleNodeBank1
{
    internal class Program : ConsoleNodeBase
    {
        public static async Task Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(BankHocon);
            var actorSystem = ActorSystem.Create(ClusterName, config);

            var bank = actorSystem.ActorOf(ClusterSingletonManager.Props(
                Props.Create(() => new BankActor()),
                settings: ClusterSingletonManagerSettings.Create(actorSystem).WithRole("bank")), 
                BankActorName);

            bank.Tell(new CreateCustomerRequestMessage(new Customer(123, "Billy White")));
            bank.Tell(new CreateCustomerRequestMessage(new Customer(456, "Sally Brown")));
            bank.Tell(new CreateCustomerRequestMessage(new Customer(789, "Wally Green")));

            while (true)
            {
                await Task.Delay(10);
            }
        }        
    }
}
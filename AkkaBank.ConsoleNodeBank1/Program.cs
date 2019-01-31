using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using AkkaBank.BasicBank.Messages.Bank;
using AkkaBank.ConsoleNode;

namespace AkkaBank.ConsoleNodeBank1
{
    internal class Program : ConsoleNodeBase
    {
        public static async Task Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(BankHocon);
            var actorSystem = ActorSystem.Create(ClusterName, config);
            var clusterSystem = Cluster.Get(actorSystem);

            clusterSystem.RegisterOnMemberUp(() =>
            {
                actorSystem.ActorOf(ClusterSingletonManager.Props(
                        Props.Create(() => new BasicBank.Actors.BankV2Actor()),
                        settings: ClusterSingletonManagerSettings.Create(actorSystem).WithRole(BankRoleName)),
                    BankActorName);

                var bankProxy = actorSystem.ActorOf(ClusterSingletonProxy.Props(
                        singletonManagerPath: $"/user/{BankActorName}",
                        settings: ClusterSingletonProxySettings.Create(actorSystem).WithRole(BankRoleName)),
                    name: $"{BankActorName}-proxy");

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("BANK NODE UP!");

                Console.WriteLine("ADD Billy White.");
                bankProxy.Tell(new CreateCustomerRequest(new Customer(123, "Billy White")));
                Console.WriteLine("ADD Sally Brown.");
                bankProxy.Tell(new CreateCustomerRequest(new Customer(456, "Sally Brown")));
                Console.WriteLine("ADD Wally Green.");
                bankProxy.Tell(new CreateCustomerRequest(new Customer(789, "Wally Green")));
                Console.ResetColor();

                
            });

            while (true)
            {
                await Task.Delay(10);
            }
        }        
    }
}
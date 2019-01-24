using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using AkkaBank.BasicBank.Actors;
using AkkaBank.ConsoleNode;

namespace AkkaBank.ConsoleNodeAtm1
{
    internal class Program : ConsoleNodeBase
    {
        public static async Task Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(AtmHocon);
            var actorSystem = ActorSystem.Create(ClusterName, config);

            var atmV2 = actorSystem.ActorOf(Props.Create(() => new AtmV2Actor()), "atm1");

            var bankProxy = actorSystem.ActorOf(ClusterSingletonProxy.Props(
                    singletonManagerPath: $"/user/{BankActorName}",
                    settings: ClusterSingletonProxySettings.Create(actorSystem).WithRole(BankRoleName)),
                name: $"{BankActorName}-proxy");

            atmV2.Tell(new BankActorMessage(bankProxy));

            while (true)
            {
                await Task.Delay(10);
            }
        }
    }
}
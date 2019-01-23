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
       private const string BankActorName = "simple-bank";

        public static async Task Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(AtmHocon);
            var actorSystem = ActorSystem.Create(ClusterName, config);

            var atmV2 = actorSystem.ActorOf(Props.Create(() => new AtmV2Actor()), "atm1");

            var bank = actorSystem.ActorOf(ClusterSingletonProxy.Props(
                singletonManagerPath: $"/user/{BankActorName}",
                settings: ClusterSingletonProxySettings.Create(actorSystem).WithRole("bank")),
                name: $"{BankActorName}-proxy");

            atmV2.Tell(new BankActorMessage(bank));

            while (true)
            {
                await Task.Delay(10);
            }
        }
    }
}
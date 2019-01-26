using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using AkkaBank.BasicBank.Actors;
using AkkaBank.BasicBank.Messages.Bank;
using AkkaBank.ConsoleNode;


namespace AkkaBank.ConsoleNodeBankAdmin
{
    internal class Program : ConsoleNodeBase
    {
        public static async Task Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(AtmHocon);
            var actorSystem = ActorSystem.Create(ClusterName, config);
            var clusterSystem = Cluster.Get(actorSystem);

            clusterSystem.RegisterOnMemberUp(() =>
            {
                var bankAdmin = actorSystem.ActorOf(Props.Create(() => new BankAdminActor()), "bank-admin");
                var bankProxy = actorSystem.ActorOf(ClusterSingletonProxy.Props(
                        singletonManagerPath: $"/user/{BankActorName}",
                        settings: ClusterSingletonProxySettings.Create(actorSystem).WithRole(BankRoleName)),
                    name: $"{BankActorName}-proxy");

                bankAdmin.Tell(new BasicBank.Messages.Bank.BankActor(bankProxy));
            });

            while (true)
            {
                await Task.Delay(10);
            }
        }
    }
}
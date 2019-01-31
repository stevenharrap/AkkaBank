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
            var clusterSystem = Cluster.Get(actorSystem);

            clusterSystem.RegisterOnMemberUp(() =>
            {
                var atm = actorSystem.ActorOf(Props.Create(() => new AtmV3Actor()), "atm-angus-street");
                var bankProxy = actorSystem.ActorOf(ClusterSingletonProxy.Props(
                        singletonManagerPath: $"/user/{BankActorName}",
                        settings: ClusterSingletonProxySettings.Create(actorSystem).WithRole(BankRoleName)),
                    name: $"{BankActorName}-proxy");

                atm.Tell(new BasicBank.Messages.Bank.BankActor(bankProxy));
            });            

            while (true)
            {
                await Task.Delay(10);
            }
        }
    }
}
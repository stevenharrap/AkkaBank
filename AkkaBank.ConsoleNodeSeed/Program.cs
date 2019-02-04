using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Configuration;
using AkkaBank.ConsoleNode;

namespace AkkaBank.ConsoleNodeSeed
{
    internal class Program : ConsoleNodeBase
    {
        public static async Task Main(string[] args)
        {
            //The "Hocon" is a type of configuration string for Akka. 
            //Standalone actor systems don't need it but clusters need some guidance.
            var config = ConfigurationFactory.ParseString(SeedHocon);
            var actorSystem = ActorSystem.Create(ClusterName, config);
            var clusterSystem = Cluster.Get(actorSystem);

            //When the cluster is viable and the node is accepted into the cluster this will be fired
            clusterSystem.RegisterOnMemberUp(() =>
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("SEED NODE UP!");
                Console.ResetColor();
            });

            while (true)
            {
                await Task.Delay(10);
            }
        }        
    }
}
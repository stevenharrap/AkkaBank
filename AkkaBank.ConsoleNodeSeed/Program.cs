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

namespace AkkaBank.ConsoleNodeSeed
{
    internal class Program : ConsoleNodeBase
    {
        public static async Task Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(SeedHocon);
            ActorSystem.Create(ClusterName, config);
            
            while (true)
            {
                await Task.Delay(10);
            }
        }        
    }
}
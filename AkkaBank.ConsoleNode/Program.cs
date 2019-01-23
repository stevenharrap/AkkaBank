using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Configuration;
using AkkaBank.BasicBank.Actors;

namespace AkkaBank.ConsoleNode
{
    internal class Program
    {
        private const string ClusterName = "basic-bank-cluster";
        private const string BankActorName = "simple-bank";

        public static async Task Main(string[] args)
        {
            //if (args == null || !args.Any())
            //{
            //    Console.WriteLine("no arguments!");
            //    return;
            //}

            //if (args.First() == "bank")
            //{
            //    await StartBankNode();
            //    return;
            //}

            //if (args.First().StartsWith("atm"))
            //{
            //    await StartAtmNode(args.First());
            //    return;
            //}

            await StartAtmNode("hhh");
            Console.WriteLine(args.First());
            Console.ReadLine();
        }

        private static async Task StartBankNode()
        {
            var hocon = $@"akka {{
                actor.provider = cluster
                remote {{
                    dot-netty.tcp {{
                        port = 8081 # this will be a seed node
                        hostname = localhost
                    }}
                }}
                cluster {{
                    seed-nodes = [""akka.tcp://{ClusterName}@localhost:8081""]
                    roles = [""bank""] # roles this member is in
                }}
            }}";

            var config = ConfigurationFactory.ParseString(hocon);
            var actorSystem = ActorSystem.Create(ClusterName, config);

            var bank = actorSystem.ActorOf(Props.Create(() => new BankActor()), BankActorName);
            bank.Tell(new CreateCustomerRequestMessage(new Customer(123, "Billy White")));
            bank.Tell(new CreateCustomerRequestMessage(new Customer(456, "Sally Brown")));
            bank.Tell(new CreateCustomerRequestMessage(new Customer(789, "Wally Green")));

            while (true)
            {
                await Task.Delay(10);
            }

        }

        private static async Task StartAtmNode(string atmName)
        {
            var hocon = $@"akka {{
                actor.provider = cluster
                remote {{
                    dot-netty.tcp {{
                        port = 0
                        hostname = localhost
                    }}
                }}
                cluster {{
                    seed-nodes = [""akka.tcp://{ClusterName}@localhost:8081""]
                    roles = [""atm""] # roles this member is in
                    role.[""bank""].min-nr-of-members = 1 # atm role needs a minimum of 1 bank in the cluster
                }}
            }}";

            var config = ConfigurationFactory.ParseString(hocon);
            var actorSystem = ActorSystem.Create(ClusterName, config);

            var atmV2 = actorSystem.ActorOf(Props.Create(() => new AtmV2Actor()), atmName);
            //use an actor singletone
            var bank = await actorSystem.ActorSelection($"akka://{ClusterName}/user/{BankActorName}")
                .ResolveOne(TimeSpan.FromSeconds(10));
            atmV2.Tell(new BankActorMessage(bank));

            while (true)
            {
                await Task.Delay(10);
            }
        }
    }
}
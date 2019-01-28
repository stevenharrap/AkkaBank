using System;

namespace AkkaBank.ConsoleNode
{
    public class ConsoleNodeBase
    {
        public const string BankActorName = "simple-bank";
        public const string ClusterName = "basic-bank-cluster";
        public const string BankRoleName = "bank-role";

        public static readonly string SingleHocon = $@"akka {{
                actor.provider = cluster
                extensions = [""Akka.Cluster.Tools.PublishSubscribe.DistributedPubSubExtensionProvider,Akka.Cluster.Tools""]
                remote {{
                    dot-netty.tcp {{
                        port = 8081
                        hostname = localhost
                    }}
                }}
                cluster {{
                    seed-nodes = [""akka.tcp://{ClusterName}@localhost:8081""]
                }}
            }}";

        // Only a seed node can define the minimum cluster configuration.
        public static readonly string SeedHocon = $@"akka {{
                actor.provider = cluster
                extensions = [""Akka.Cluster.Tools.PublishSubscribe.DistributedPubSubExtensionProvider,Akka.Cluster.Tools""]
                remote {{
                    dot-netty.tcp {{
                        port = 8081
                        hostname = localhost
                    }}
                }}
                cluster {{
                    seed-nodes = [""akka.tcp://{ClusterName}@localhost:8081""]
                    roles = [""seed-role""]
                    role.[""{BankRoleName}""].min-nr-of-members = 1
                }}
            }}";

        public static readonly string BankHocon = $@"akka {{
                actor.provider = cluster
                extensions = [""Akka.Cluster.Tools.PublishSubscribe.DistributedPubSubExtensionProvider,Akka.Cluster.Tools""]
                remote {{
                    dot-netty.tcp {{
                        port = 0
                        hostname = localhost
                    }}
                }}
                cluster {{
                    seed-nodes = [""akka.tcp://{ClusterName}@localhost:8081""]
                    roles = [""{BankRoleName}""]
                }}
            }}";

        public static readonly string AtmHocon = $@"akka {{
                actor.provider = cluster
                extensions = [""Akka.Cluster.Tools.PublishSubscribe.DistributedPubSubExtensionProvider,Akka.Cluster.Tools""]
                remote {{
                    dot-netty.tcp {{
                        port = 0
                        hostname = localhost
                    }}
                }}
                cluster {{
                    seed-nodes = [""akka.tcp://{ClusterName}@localhost:8081""]
                    roles = [""atm""]                    
                }}
            }}";        
    }
}

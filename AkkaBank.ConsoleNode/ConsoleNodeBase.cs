using System;

namespace AkkaBank.ConsoleNode
{
    public class ConsoleNodeBase
    {
        public const string BankActorName = "simple-bank";
        public const string ClusterName = "basic-bank-cluster";
        public const string BankRoleName = "bank-role";

        public static readonly string AtmHocon = $@"akka {{
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
                    role.[""{BankRoleName}""].min-nr-of-members = 1 # atm role needs a minimum of 1 bank in the cluster
                }}
            }}";

        public static readonly string BankHocon = $@"akka {{
                actor.provider = cluster
                remote {{
                    dot-netty.tcp {{
                        port = 8081 # this will be a seed node
                        hostname = localhost
                    }}
                }}
                cluster {{
                    seed-nodes = [""akka.tcp://{ClusterName}@localhost:8081""]
                    roles = [""{BankRoleName}""] # roles this member is in
                }}
            }}";
    }
}

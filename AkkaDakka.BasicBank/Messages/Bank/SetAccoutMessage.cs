using Akka.Actor;

namespace AkkaBank.BasicBank.Messages.Bank
{
    public class SetAccoutMessage
    {
        public IActorRef BankAccout { get; }

        public SetAccoutMessage(IActorRef bankAccount)
        {
            BankAccout = bankAccount;
        }
    }
}

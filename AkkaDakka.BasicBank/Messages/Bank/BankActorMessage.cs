using Akka.Actor;

namespace AkkaBank.BasicBank.Messages.Bank
{
    public class BankActorMessage
    {
        public IActorRef Bank { get; }

        public BankActorMessage(IActorRef bank)
        {
            Bank = bank;
        }
    }
}
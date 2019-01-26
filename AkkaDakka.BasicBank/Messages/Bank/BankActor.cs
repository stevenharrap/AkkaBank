using Akka.Actor;

namespace AkkaBank.BasicBank.Messages.Bank
{
    public class BankActor
    {
        public IActorRef Bank { get; }

        public BankActor(IActorRef bank)
        {
            Bank = bank;
        }
    }
}
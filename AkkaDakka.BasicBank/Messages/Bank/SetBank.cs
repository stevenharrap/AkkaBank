using Akka.Actor;

namespace AkkaBank.BasicBank.Messages.Bank
{
    public class SetBank
    {
        public IActorRef Bank { get; }

        public SetBank(IActorRef bank)
        {
            Bank = bank;
        }
    }
}
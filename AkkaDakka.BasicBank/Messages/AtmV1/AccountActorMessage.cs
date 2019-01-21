using Akka.Actor;

namespace AkkaBank.BasicBank.Messages.AtmV1
{
    public class AccountActorMessage
    {
        public IActorRef Account { get; }

        public AccountActorMessage(IActorRef account)
        {
            Account = account;
        }
    }
}
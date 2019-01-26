using Akka.Actor;

namespace AkkaBank.BasicBank.Messages.AtmV1
{
    public class AccountActor
    {
        public IActorRef Account { get; }

        public AccountActor(IActorRef account)
        {
            Account = account;
        }
    }
}
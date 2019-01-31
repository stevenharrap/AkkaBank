using Akka.Actor;

namespace AkkaBank.BasicBank.Messages.Atm
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
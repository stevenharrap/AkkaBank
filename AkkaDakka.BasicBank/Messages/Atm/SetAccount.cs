using Akka.Actor;

namespace AkkaBank.BasicBank.Messages.Atm
{
    public class SetAccount
    {
        public IActorRef Account { get; }

        public SetAccount(IActorRef account)
        {
            Account = account;
        }
    }
}
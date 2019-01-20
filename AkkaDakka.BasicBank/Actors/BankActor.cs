using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Routing;

namespace AkkaBank.BasicBank.Actors
{
    public class CreateAccountMessage
    {
        public int AccountNumber { get; }

        public string AccountName { get; }

        public CreateAccountMessage(int accountNumber, string accountName)
        {
            AccountNumber = accountNumber;
            AccountName = accountName;
        }
    }

    public class GetAccountMessage
    {
        public int AccountNumber { get; }

        public GetAccountMessage(int accountNumber)
        {
            AccountNumber = accountNumber;
        }
    }

    public class AccountMesage
    {
        public IActorRef Account { get; }

        public AccountMesage(IActorRef account)
        {
            Account = account;
        }
    }

    public class BankActor : ReceiveActor
    {
        private IActorRef _bankAccountsRouter;

        public BankActor()
        {
            Receive<CreateAccountMessage>(message => _bankAccountsRouter.Tell(message));
            Receive<GetAccountMessage>(message => _bankAccountsRouter.Tell(message));
        }

        protected override void PreStart()
        {
            _bankAccountsRouter = Context.ActorOf(
                Props.Create<BankAccountsActor>().WithRouter(new ConsistentHashingPool(5)), "bank-accounts-router");
        }
    }

    public class BankAccountsActor : ReceiveActor
    {
        private readonly Dictionary<int, IActorRef> _accounts = new Dictionary<int, IActorRef>();

        public BankAccountsActor() { }

        private void HandleCreateAccount(CreateAccountMessage message)
        {
            var account = Context.ActorOf(Props.Create(() => new AccountActor()), $"account-{message.AccountNumber}");
            _accounts.Add(message.AccountNumber, account);
        }

        private void HandleGetAccount(GetAccountMessage message)
        {
            Sender.Tell(new AccountMesage(_accounts[message.AccountNumber]));
        }
    }
}
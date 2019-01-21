using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Routing;

namespace AkkaBank.BasicBank.Actors
{
    public class CreateAccountMessage : IConsistentHashable
    {
        public int AccountNumber { get; }

        public object ConsistentHashKey => AccountNumber;

        public string AccountName { get; }

        public CreateAccountMessage(int accountNumber, string accountName)
        {
            AccountNumber = accountNumber;
            AccountName = accountName;
        }
    }

    public class GetAccountMessage : IConsistentHashable
    {
        public int AccountNumber { get; }

        public object ConsistentHashKey => AccountNumber;

        public GetAccountMessage(int accountNumber)
        {
            AccountNumber = accountNumber;
        }
    }

    public class BankActorMessage
    {
        public IActorRef Bank { get; }

        public BankActorMessage(IActorRef bank)
        {
            Bank = bank;
        }
    }

    public class AccountActorMessage
    {
        public IActorRef Account { get; }
        public bool Ok { get; }
        public string Error { get; }

        public AccountActorMessage(IActorRef account)
        {
            Account = account;
            Ok = true;
        }

        public AccountActorMessage(string error)
        {
            Error = error;
            Ok = false;
        }
    }

    public class BankActor : ReceiveActor
    {
        private IActorRef _bankAccountsRouter;

        public BankActor()
        {
            Receive<CreateAccountMessage>(message => _bankAccountsRouter.Tell(message, Sender));
            Receive<GetAccountMessage>(message => _bankAccountsRouter.Tell(message, Sender));
        }

        protected override void PreStart()
        {
            _bankAccountsRouter = Context.ActorOf(
                Props.Create<AccountsManagerActor>().WithRouter(new ConsistentHashingPool(5)), "bank-accounts-router");
        }
    }

    public class AccountsManagerActor : ReceiveActor
    {
        private readonly Dictionary<int, IActorRef> _accounts = new Dictionary<int, IActorRef>();

        public AccountsManagerActor()
        {
            Receive<CreateAccountMessage>(HandleCreateAccount);
            Receive<GetAccountMessage>(HandleGetAccount);
        }

        private void HandleCreateAccount(CreateAccountMessage message)
        {
            if (_accounts.ContainsKey(message.AccountNumber))
            {
                Sender.Tell(new AccountActorMessage("The account already exists."));
                return;
            }

            var account = Context.ActorOf(Props.Create(() => new AccountActor()), $"account-{message.AccountNumber}");
            _accounts.Add(message.AccountNumber, account);

            Sender.Tell(new AccountActorMessage(account));
        }


        private void HandleGetAccount(GetAccountMessage message)
        {
            if (_accounts.TryGetValue(message.AccountNumber, out var accountActor))
            {
                Sender.Tell(new AccountActorMessage(accountActor));
                return;
            }

            Sender.Tell(new AccountActorMessage("No account found."));
        }
    }
}
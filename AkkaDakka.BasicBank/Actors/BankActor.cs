using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.Routing;
using AkkaBank.BasicBank.Messages.Bank;

namespace AkkaBank.BasicBank.Actors
{
    public class BankActor : ReceiveActor
    {
        private IActorRef _bankAccountsRouter;

        public BankActor()
        {
            Receive<CreateCustomerRequestMessage>(message =>
            {
                _bankAccountsRouter.Tell(message, Sender);
            });
            Receive<GetCustomerRequstMessage>(message =>
            {
                _bankAccountsRouter.Tell(message, Sender);
            });
        }

        protected override void PreStart()
        {
            _bankAccountsRouter = Context.ActorOf(
                Props.Create<CustomersManagerActor>().WithRouter(new ConsistentHashingPool(5)), "accounts-manager-router");
        }
    }

    public class CustomersManagerActor : ReceiveActor
    {      
        private readonly Dictionary<int, CustomerAccount> _accounts = new Dictionary<int, CustomerAccount>();

        public CustomersManagerActor()
        {
            Receive<CreateCustomerRequestMessage>(HandleCreateCustomerRequest);
            Receive<GetCustomerRequstMessage>(HandleGetCustomerRequest);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 10,
                withinTimeRange: TimeSpan.FromMinutes(1),
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case NegativeAccountBalanceException nabe:
                            return Directive.Resume;
                        default:
                            return Directive.Escalate;
                    }
                });
        }

        private void HandleCreateCustomerRequest(CreateCustomerRequestMessage message)
        {
            GetCustomerResponseMessage response;

            if (_accounts.ContainsKey(message.Customer.CustomerNumber))
            {
                response = new GetCustomerResponseMessage("The account already exists.");
            }
            else
            {
                var account = Context.ActorOf(Props.Create(() => new AccountActor()), $"account-{message.Customer.CustomerNumber}");
                var customerAccount = new CustomerAccount(message.Customer, account);
                _accounts.Add(message.Customer.CustomerNumber, customerAccount);
                response = new GetCustomerResponseMessage(customerAccount);
            }

            if (!Sender.IsNobody())
            {
                Sender.Tell(response);
            }            
        }

        private void HandleGetCustomerRequest(GetCustomerRequstMessage message)
        {
            //Pretend that it takes some time to find an account.
            Task.Delay(2000).GetAwaiter().GetResult();

            if (_accounts.TryGetValue(message.CustomerNumber, out var customerAccount))
            {
                Sender.Tell(new GetCustomerResponseMessage(customerAccount));
                return;
            }

            Sender.Tell(new GetCustomerResponseMessage("No account found."));
        }        
    }
}
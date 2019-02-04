using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using AkkaBank.BasicBank.Messages.Bank;

namespace AkkaBank.BasicBank.Actors
{
    public class BankV1Actor : ReceiveActor
    {
        private IActorRef _bankAccountsRouter;

        public BankV1Actor()
        {
            Receive<CreateCustomerRequest>(message =>
            {
                _bankAccountsRouter.Tell(message, Sender);
            });
            Receive<GetCustomerRequest>(message =>
            {
                _bankAccountsRouter.Tell(message, Sender);
            });
        }

        protected override void PreStart()
        {
            // Create a router of CustomerManagerV1Actors.
            // Each child will take care of a selection of accounts based on the ID in the message
            // Different types of messages with the same ID value will always end up at the same child CustomerManager
            _bankAccountsRouter = Context.ActorOf(
                Props.Create<CustomerManagerV1Actor>().WithRouter(new ConsistentHashingPool(5)), "customer-manager-router");
        }
    }

    /// <summary>
    /// The CustomerManager handles the access to a selection of accounts based on the ID in each message
    /// </summary>
    public class CustomerManagerV1Actor : ReceiveActor
    {      
        private readonly Dictionary<int, CustomerAccount> _customerAccounts = new Dictionary<int, CustomerAccount>();

        public CustomerManagerV1Actor()
        {
            Receive<CreateCustomerRequest>(HandleCreateCustomerRequest);
            Receive<GetCustomerRequest>(HandleGetCustomerRequest);
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

        private void HandleCreateCustomerRequest(CreateCustomerRequest message)
        {
            GetCustomerResponse response;

            if (_customerAccounts.ContainsKey(message.Customer.CustomerNumber))
            {
                response = new GetCustomerResponse("The account already exists.");
            }
            else
            {
                var account = Context.ActorOf(Props.Create(() => new AccountActor()), $"account-{message.Customer.CustomerNumber}");
                var customerAccount = new CustomerAccount(message.Customer, account);
                _customerAccounts.Add(message.Customer.CustomerNumber, customerAccount);
                response = new GetCustomerResponse(customerAccount);
            }

            if (!Sender.IsNobody())
            {
                Sender.Tell(response);
            }            
        }

        private void HandleGetCustomerRequest(GetCustomerRequest message)
        {
            //Pretend that it takes some time to find an account.
            Task.Delay(2000).GetAwaiter().GetResult();

            if (_customerAccounts.TryGetValue(message.CustomerNumber, out var customerAccount))
            {
                Sender.Tell(new GetCustomerResponse(customerAccount));
                return;
            }

            Sender.Tell(new GetCustomerResponse("No account found."));
        }
    }
}
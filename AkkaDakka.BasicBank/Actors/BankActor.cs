using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.Routing;

namespace AkkaBank.BasicBank.Actors
{
    public class CreateCustomerRequestMessage : IConsistentHashable
    {     
        public Customer Customer { get; }

        public object ConsistentHashKey => Customer.CustomerNumber;        

        public CreateCustomerRequestMessage(Customer customer)
        {
            Customer = customer;
        }
    }    

    public class GetCustomerRequstMessage : IConsistentHashable
    {
        public int CustomerNumber { get; }

        public object ConsistentHashKey => CustomerNumber;

        public GetCustomerRequstMessage(int customerNumber)
        {
            CustomerNumber = customerNumber;
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

    public class GetCustomerResponseMessage
    {
        public CustomerAccount CustomerAccount { get; }

        public bool Ok { get; }
        public string Error { get; }

        public GetCustomerResponseMessage(CustomerAccount customerAccount)
        {
            CustomerAccount = customerAccount;
            Ok = true;
        }

        public GetCustomerResponseMessage(string error)
        {
            Error = error;
            Ok = false;
        }
    }

    public class Customer
    {
        public int CustomerNumber { get; }
        public string CustomerName { get; }

        public Customer(int customerNumber, string customerName)
        {
            CustomerNumber = customerNumber;
            CustomerName = customerName;
        }
    }

    public class CustomerAccount
    {
        public Customer Customer { get; }
        public IActorRef Account { get; }

        public CustomerAccount(Customer customer, IActorRef account)
        {
            Customer = customer;
            Account = account;
        }
    }

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
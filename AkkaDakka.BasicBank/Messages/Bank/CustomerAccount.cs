using Akka.Actor;
using Newtonsoft.Json;

namespace AkkaBank.BasicBank.Messages.Bank
{
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
}
using Akka.Routing;

namespace AkkaBank.BasicBank.Messages.Bank
{
    public class CreateCustomerRequest : IConsistentHashable
    {     
        public Customer Customer { get; }

        public object ConsistentHashKey => Customer.CustomerNumber;        

        public CreateCustomerRequest(Customer customer)
        {
            Customer = customer;
        }
    }    
}
using Akka.Routing;

namespace AkkaBank.BasicBank.Messages.Bank
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
}
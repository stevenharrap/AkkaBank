using Akka.Routing;

namespace AkkaBank.BasicBank.Messages.Bank
{
    public class GetCustomerRequest : IConsistentHashable
    {
        public int CustomerNumber { get; }

        public object ConsistentHashKey => CustomerNumber;

        public GetCustomerRequest(int customerNumber)
        {
            CustomerNumber = customerNumber;
        }
    }
}
using Akka.Routing;

namespace AkkaBank.BasicBank.Messages.Bank
{
    public class GetCustomerRequstMessage : IConsistentHashable
    {
        public int CustomerNumber { get; }

        public object ConsistentHashKey => CustomerNumber;

        public GetCustomerRequstMessage(int customerNumber)
        {
            CustomerNumber = customerNumber;
        }
    }
}
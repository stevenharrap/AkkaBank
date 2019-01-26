using Akka.Routing;

namespace AkkaBank.BasicBank.Messages.Bank
{
    public class GetCustomerRequst : IConsistentHashable
    {
        public int CustomerNumber { get; }

        public object ConsistentHashKey => CustomerNumber;

        public GetCustomerRequst(int customerNumber)
        {
            CustomerNumber = customerNumber;
        }
    }
}
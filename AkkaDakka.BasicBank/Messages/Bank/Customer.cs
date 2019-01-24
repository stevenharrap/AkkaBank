namespace AkkaBank.BasicBank.Messages.Bank
{
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
}
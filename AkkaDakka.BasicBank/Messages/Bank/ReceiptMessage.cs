namespace AkkaBank.BasicBank.Messages.Bank
{
    public class ReceiptMessage
    {
        public int Balance { get; }

        public ReceiptMessage(int balance)
        {
            Balance = balance;
        }
    }
}
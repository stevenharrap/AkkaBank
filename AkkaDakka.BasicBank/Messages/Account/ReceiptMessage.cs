namespace AkkaBank.BasicBank.Messages.Account
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
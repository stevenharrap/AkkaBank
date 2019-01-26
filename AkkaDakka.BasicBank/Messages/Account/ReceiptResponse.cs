namespace AkkaBank.BasicBank.Messages.Account
{
    public class ReceiptResponse
    {
        public int Balance { get; }

        public ReceiptResponse(int balance)
        {
            Balance = balance;
        }
    }
}
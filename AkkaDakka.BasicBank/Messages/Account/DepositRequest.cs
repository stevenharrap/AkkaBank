namespace AkkaBank.BasicBank.Messages.Account
{
    public class DepositRequest
    {
        public int Amount { get; }

        public DepositRequest(int amount)
        {
            Amount = amount;
        }
    }
}

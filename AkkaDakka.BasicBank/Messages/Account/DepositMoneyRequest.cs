namespace AkkaBank.BasicBank.Messages.Account
{
    public class DepositMoneyRequest
    {
        public int Amount { get; }

        public DepositMoneyRequest(int amount)
        {
            Amount = amount;
        }
    }
}

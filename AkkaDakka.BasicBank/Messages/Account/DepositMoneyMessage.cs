namespace AkkaBank.BasicBank.Messages.Account
{
    public class DepositMoneyMessage
    {
        public int Amount { get; }

        public DepositMoneyMessage(int amount)
        {
            Amount = amount;
        }
    }
}

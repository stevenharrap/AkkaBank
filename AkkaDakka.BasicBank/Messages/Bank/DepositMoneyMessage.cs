namespace AkkaBank.BasicBank.Messages.Bank
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

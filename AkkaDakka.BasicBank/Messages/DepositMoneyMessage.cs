namespace AkkaDakka.BasicBank.Messages
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

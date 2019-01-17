namespace AkkaDakka.BasicBank.Messages
{
    public class BalanceMessage
    {
        public int Amount { get; }

        public BalanceMessage(int amount)
        {
            Amount = amount;
        }
    }
}

namespace AkkaDakka.BasicBank.Messages
{
    public class WithdrawMoneyMessage
    {
        public int Amount { get; }

        public WithdrawMoneyMessage(int amount)
        {
            Amount = amount;
        }
    }
}

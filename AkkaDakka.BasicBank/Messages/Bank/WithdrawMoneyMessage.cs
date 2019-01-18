namespace AkkaBank.BasicBank.Messages.Bank
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

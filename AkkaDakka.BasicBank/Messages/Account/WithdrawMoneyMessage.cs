namespace AkkaBank.BasicBank.Messages.Account
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

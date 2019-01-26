namespace AkkaBank.BasicBank.Messages.Account
{
    public class WithdrawMoneyRequest
    {
        public int Amount { get; }

        public WithdrawMoneyRequest(int amount)
        {
            Amount = amount;
        }
    }
}

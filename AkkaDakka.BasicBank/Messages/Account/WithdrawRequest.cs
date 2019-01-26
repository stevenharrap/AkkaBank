namespace AkkaBank.BasicBank.Messages.Account
{
    public class WithdrawRequest
    {
        public int Amount { get; }

        public WithdrawRequest(int amount)
        {
            Amount = amount;
        }
    }
}

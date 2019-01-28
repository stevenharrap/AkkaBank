using AkkaBank.BasicBank.Messages.Bank;

namespace AkkaBank.BasicBank.Messages.BankAdmin
{
    public class AccountFeeResponse
    {
        public CustomerAccount CustomerAccount { get; }
        public int FeeCharged { get; }
        public int Balance { get; }

        public AccountFeeResponse(CustomerAccount customerAccount, int feeCharged, int balance)
        {
            CustomerAccount = customerAccount;
            FeeCharged = feeCharged;
            Balance = balance;
        }
    }
}

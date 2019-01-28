using Akka.Actor;
using AkkaBank.BasicBank.Messages.Bank;
namespace AkkaBank.BasicBank.Messages.BankAdmin
{
    public class AccountFeeRequest
    {
        public int Fee { get; }
        public CustomerAccount CustomerAccount { get;  }
        public IActorRef BankAdmin { get; }

        public AccountFeeRequest(int fee, CustomerAccount customerAccount, IActorRef bankAdmin)
        {
            Fee = fee;
            CustomerAccount = customerAccount;
            BankAdmin = bankAdmin;
        }
    }
}

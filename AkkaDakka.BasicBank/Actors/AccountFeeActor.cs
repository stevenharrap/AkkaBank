using Akka.Actor;
using AkkaBank.BasicBank.Messages.Account;
using AkkaBank.BasicBank.Messages.BankAdmin;

namespace AkkaBank.BasicBank.Actors
{
    public class AccountFeeActor : ReceiveActor
    {
        private AccountFeeRequest _toCharge;

        public AccountFeeActor()
        {
            Become(WaitingForCustomerAccount);
        }

        #region States

        private void WaitingForCustomerAccount()
        {
            Receive<AccountFeeRequest>(HandleAccountRequest);
        }

        private void WaitingForBalance()
        {
            Receive<ReceiptResponse>(HandleAccountBalance);
        }

        private void WaitingForWithdrawal()
        {
            Receive<ReceiptResponse>(HandleAccountWithdrawal);
        }

        #endregion

        #region Handlers

        private void HandleAccountRequest(AccountFeeRequest message)
        {
            _toCharge = message;
            _toCharge.CustomerAccount.Account.Tell(new BalanceRequest());
            Become(WaitingForBalance);
        }

        private void HandleAccountBalance(ReceiptResponse message)
        {
            if (message.Balance < _toCharge.Fee)
            {
                _toCharge.BankAdmin.Tell(new AccountFeeResponse(_toCharge.CustomerAccount, 0, message.Balance));
                _toCharge = null;
                Become(WaitingForCustomerAccount);
                return;
            }

            _toCharge.CustomerAccount.Account.Tell(new WithdrawRequest(_toCharge.Fee));
            // we should really send that cash to another account!
            Become(WaitingForWithdrawal);
        }

        private void HandleAccountWithdrawal(ReceiptResponse message)
        {            
            _toCharge.BankAdmin.Tell(new AccountFeeResponse(_toCharge.CustomerAccount, _toCharge.Fee, message.Balance));
            _toCharge = null;
            Become(WaitingForCustomerAccount);
        }

        #endregion
    }
}

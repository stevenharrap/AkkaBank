using System;
using Akka.Actor;
using AkkaBank.BasicBank.Messages.Account;

namespace AkkaBank.BasicBank.Actors
{
    public class AccountActor : ReceiveActor
    {
        private int _balance = 0;

        public AccountActor()
        {
            Receive<WithdrawRequest>(HandleWithdrawMoney);
            Receive<DepositRequest>(HandleDepositMoney);
            Receive<BalanceRequest>(HandleBalanceMoney);
        }        

        private void HandleDepositMoney(DepositRequest message)
        {
            _balance += message.Amount;

            Sender.Tell(new ReceiptResponse(_balance));
        }

        private void HandleWithdrawMoney(WithdrawRequest message)
        {
            if (_balance - message.Amount < 0)
            {
                throw new NegativeAccountBalanceException();
            }

            _balance -= message.Amount;

            Sender.Tell(new ReceiptResponse(_balance));
        }

        private void HandleBalanceMoney(BalanceRequest message)
        {
            Sender.Tell(new ReceiptResponse(_balance));
        }
    }

    public class NegativeAccountBalanceException : Exception
    {
    }
}
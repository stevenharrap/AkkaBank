using System;
using Akka.Actor;
using AkkaBank.BasicBank.Messages.Account;
using AkkaBank.BasicBank.Messages.Bank;

namespace AkkaBank.BasicBank.Actors
{
    public class AccountActor : ReceiveActor
    {
        private int _balance = 0;

        public AccountActor()
        {
            Receive<WithdrawMoneyRequest>(message => HandleWithdrawMoney(message));
            Receive<DepositMoneyRequest>(message => HandleDepositMoney(message));
        }

        private void HandleDepositMoney(DepositMoneyRequest message)
        {
            _balance += message.Amount;

            Sender.Tell(new ReceiptResponse(_balance));
        }

        private void HandleWithdrawMoney(WithdrawMoneyRequest message)
        {
            if (_balance - message.Amount < 0)
            {
                throw new NegativeAccountBalanceException();
            }

            _balance -= message.Amount;

            Sender.Tell(new ReceiptResponse(_balance));
        }
    }

    public class NegativeAccountBalanceException : Exception
    {
    }
}
using System;
using Akka.Actor;
using AkkaBank.BasicBank.Messages.Bank;

namespace AkkaBank.BasicBank.Actors
{
    public class AccountActor : ReceiveActor
    {
        private int _balance = 0;

        public AccountActor()
        {
            Receive<WithdrawMoneyMessage>(message => HandleWithdrawMoney(message));
            Receive<DepositMoneyMessage>(message => HandleDepositMoney(message));
        }

        private void HandleDepositMoney(DepositMoneyMessage message)
        {
            _balance += message.Amount;

            Sender.Tell(new ReceiptMessage(_balance));
        }

        private void HandleWithdrawMoney(WithdrawMoneyMessage message)
        {
            if (_balance - message.Amount < 0)
            {
                throw new NegativeAccountBalanceException();
            }

            _balance -= message.Amount;

            Sender.Tell(new ReceiptMessage(_balance));
        }
    }

    public class NegativeAccountBalanceException : Exception
    {
    }
}
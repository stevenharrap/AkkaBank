using Akka.Actor;
using AkkaDakka.BasicBank.Messages;

namespace AkkaDakka.BasicBank.Actors
{
    public class BankActor : ReceiveActor
    {
        private int _account = 0;

        public BankActor()
        {
            Receive<WithdrawMoneyMessage>(message => HandleWithdrawMoney(message));
            Receive<DepositMoneyMessage>(message => HandleDepositMoney(message));
        }

        private void HandleDepositMoney(DepositMoneyMessage message)
        {
            _account += message.Amount;

            Sender.Tell(new BalanceMessage(_account));
        }

        private void HandleWithdrawMoney(WithdrawMoneyMessage message)
        {
            _account -= message.Amount;

            Sender.Tell(new BalanceMessage(_account));
        }
    }
}
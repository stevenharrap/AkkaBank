 using System;
using Akka.Actor;
using AkkaBank.BasicBank.Messages.Bank;
using AkkaBank.BasicBank.Messages.Console;

namespace AkkaBank.BasicBank.Actors
{
    public class AtmV2Actor : ReceiveActor
    {
        private IActorRef _console;
        private IActorRef _bankAccount;

        public AtmV2Actor()
        {
            Become(WaitingForAccountState);            
        }
        
        protected override void PreStart()
        {
            _console = Context.ActorOf(Props.Create(() => new ConsoleActor()), "atm-console");
            _console.Tell(MakeAccountScreenMessage());
        }

        protected override void Unhandled(object message)
        {
            _console.Tell("BEEP BEEP BEEP. UNEXPECTED INPUT!");
        }

        #region States

        private void WaitingForAccountState()
        {
            Receive<SetAccoutMessage>(message => HandleSetAccount(message));
        }

        private void MainMenuState()
        {
            Receive<ConsoleInputMessage>(message => HandleMainMenuInput(message));
        }

        private void WithdrawalState()
        {
            Receive<ConsoleInputMessage>(message => HandleWithdrawalInput(message));
        }

        private void DepositState()
        {
            Receive<ConsoleInputMessage>(message => HandleDepositInput(message));
        }

        private void WaitingForReceiptState()
        {
            Receive<ReceiptMessage>(message => HandleReceipt(message));
        }

        #endregion

        #region Handlers

        private void HandleSetAccount(SetAccoutMessage message)
        {
            _bankAccount = message.BankAccout;
            Become(MainMenuState);
            _console.Tell(MakeMainMenuScreenMessage());
        }

        private void HandleMainMenuInput(ConsoleInputMessage message)
        {
            switch (message.Input)
            {
                case "w":
                    Become(WithdrawalState);
                    _console.Tell(
                        new ConsoleOutputMessage(
                            "****************************************\n" +
                            "*                                      *\n" +
                            "*                                      *\n" +
                            "*         WITHDRAWAL!!!.               *\n" +
                            "*                                      *\n" +
                            "*                                      *\n" +
                            "****************************************\n" +
                            "PLEASE ENTER AMOUNT:", true));
                    break;

                case "d":
                    Become(DepositState);
                    _console.Tell(
                        new ConsoleOutputMessage(
                            "****************************************\n" +
                            "*                                      *\n" +
                            "*                                      *\n" +
                            "*         DEPOSIT!!!.                  *\n" +
                            "*                                      *\n" +
                            "*                                      *\n" +
                            "****************************************\n" +
                            "PLEASE ENTER AMOUNT:", true));
                    break;

                default:
                    _console.Tell(MakeMainMenuScreenMessage());
                    _console.Tell("What!? Try again...");
                    break;
            }
        }

        private void HandleDepositInput(ConsoleInputMessage message)
        {
            if (int.TryParse(message.Input, out var amount))
            {
                _bankAccount.Tell(new DepositMoneyMessage(amount));
                _console.Tell("Please wait.. taking to the bank.\n");
                Become(WaitingForReceiptState);
                return;
            }

            _console.Tell("That's not money! Try again:");
        }

        private void HandleWithdrawalInput(ConsoleInputMessage message)
        {
            if (int.TryParse(message.Input, out var amount))
            {
                _bankAccount.Tell(new WithdrawMoneyMessage(amount));
                _console.Tell("Please wait.. taking to the bank.\n");
                Become(WaitingForReceiptState);
                return;
            }

            _console.Tell("That's not money! Try again:");
        }

        private void HandleReceipt(ReceiptMessage message)
        {
            _console.Tell("Your transaction is complete!...\n");
            _console.Tell($"The balance of your account is: ${message.Balance}\n");

            _bankAccount = null;
            Become(WaitingForAccountState);

            Context.System.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(10),
                _console,
                MakeAccountScreenMessage(),                
                Self);            
        }

        #endregion

        #region Screens        

        private ConsoleOutputMessage MakeMainMenuScreenMessage()
        {
            const string MainMenuScreen =
                "****************************************\n" +
                "*                                      *\n" +
                "*                                      *\n" +
                "*         WELCOME TO BASIC BANK.       *\n" +
                "*                                      *\n" +
                "*         [w] WITHDRAWAL               *\n" +
                "*         [d] DEPOSIT                  *\n" +
                "*                                      *\n" +
                "*                                      *\n" +
                "*                                      *\n" +
                "****************************************\n";

            return new ConsoleOutputMessage(MainMenuScreen, true);            
        }

        private ConsoleOutputMessage MakeAccountScreenMessage()
        {
            const string AccountScreen =
                "****************************************\n" +
                "*                                      *\n" +
                "*                                      *\n" +
                "*         WELCOME TO BASIC BANK.       *\n" +
                "*                                      *\n" +
                "*         PLEASE ENTER YOU ACC.        *\n" +
                "*                                      *\n" +
                "*                                      *\n" +
                "*                                      *\n" +
                "****************************************\n";

            return new ConsoleOutputMessage(AccountScreen, true);
        }

        #endregion
    }
}
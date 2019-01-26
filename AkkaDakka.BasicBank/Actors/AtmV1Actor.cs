 using System;
using Akka.Actor;
using AkkaBank.BasicBank.Messages.Account;
using AkkaBank.BasicBank.Messages.AtmV1;
using AkkaBank.BasicBank.Messages.Bank;
using AkkaBank.BasicBank.Messages.Console;

namespace AkkaBank.BasicBank.Actors
{
    public class AtmV1Actor : ReceiveActor
    {
        private IActorRef _console;
        private IActorRef _bankAccount;        

        public AtmV1Actor()
        {
            Become(WaitingForAccountState);
        }
        
        protected override void PreStart()
        {
            _console = Context.ActorOf(Props.Create(() => new ConsoleActor()), "atm-console");            
        }

        protected override void Unhandled(object message)
        {
            _console.Tell("BEEP BEEP BEEP. UNEXPECTED INPUT!");
        }

        #region States

        private void WaitingForAccountState()
        {
            Receive((Action<Messages.AtmV1.AccountActor>) this.HandleSetAccount);
        }

        private void MainMenuState()
        {
            Receive<ConsoleInput>(HandleMainMenuInput);
        }

        private void WithdrawalState()
        {
            Receive<ConsoleInput>(HandleWithdrawalInput);
        }

        private void DepositState()
        {
            Receive<ConsoleInput>(HandleDepositInput);
        }

        private void WaitingForReceiptState()
        {
            Receive<ReceiptResponse>(HandleReceipt);
        }

        #endregion

        #region Handlers

        private void HandleSetAccount(Messages.AtmV1.AccountActor message)
        {
            _bankAccount = message.Account;
            Become(MainMenuState);
            _console.Tell(MakeMainMenuScreenMessage());
        }

        private void HandleMainMenuInput(ConsoleInput message)
        {
            switch (message.Input)
            {
                case "w":
                    Become(WithdrawalState);
                    _console.Tell(
                        new ConsoleOutput(
                            new[] {
                                "WITHDRAWAL!!!",
                                "PLEASE ENTER AMOUNT..."
                            },
                            clear: true,
                            boxed: true,
                            padding: 10));
                    break;

                case "d":
                    Become(DepositState);
                    _console.Tell(
                        new ConsoleOutput(
                            new[] {
                                "DEPOSIT!!!",
                                "PLEASE ENTER AMOUNT..."
                            },
                            clear: true,
                            boxed: true,
                            padding: 10));
                    break;

                default:
                    _console.Tell(MakeMainMenuScreenMessage());
                    _console.Tell("What!? Try again...");
                    break;
            }
        }

        private void HandleDepositInput(ConsoleInput message)
        {
            if (int.TryParse(message.Input, out var amount))
            {
                _bankAccount.Tell(new DepositMoneyRequest(amount));
                _console.Tell("Please wait.. taking to the bank.\n");
                Become(WaitingForReceiptState);
                return;
            }

            _console.Tell("That's not money! Try again:");
        }

        private void HandleWithdrawalInput(ConsoleInput message)
        {
            if (int.TryParse(message.Input, out var amount))
            {
                _bankAccount.Tell(new WithdrawMoneyRequest(amount));
                _console.Tell("Please wait.. taking to the bank.\n");
                Become(WaitingForReceiptState);
                return;
            }

            _console.Tell("That's not money! Try again:");
        }

        private void HandleReceipt(ReceiptResponse message)
        {
            _console.Tell("Your transaction is complete!...\n");
            _console.Tell($"The balance of your account is: ${message.Balance}\n");

            Context.System.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(10),
                _console,
                MakeMainMenuScreenMessage(),                
                Self);

            Become(MainMenuState);
        }

        #endregion

        #region Screens        

        private ConsoleOutput MakeMainMenuScreenMessage()
        {            
            return new ConsoleOutput(
                new[] {
                    "WELCOME TO BASIC BANK",
                    string.Empty,
                    "[w] WITHDRAWAL",
                    "[d] DEPOSIT"
                },
                clear: true,
                boxed: true,
                padding: 10);
        }

        #endregion
    }
}
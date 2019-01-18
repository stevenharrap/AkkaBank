using System;
using Akka.Actor;
using AkkaBank.BasicBank.Messages;
using AkkaBank.BasicBank.Messages.Bank;
using AkkaBank.BasicBank.Messages.Console;

namespace AkkaBank.BasicBank.Actors
{
    public class AtmActor : ReceiveActor
    {
        private IActorRef _console;
        private IActorRef _bankAccount;

        private const string MainMenu =
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

        public AtmActor()
        {
            Become(MainMenuState);
        }
        
        protected override void PreStart()
        {
            _console = Context.ActorOf(Props.Create(() => new ConsoleActor()), "atm-console");
            _bankAccount = Context.ActorSelection("akka://my-actor-system/user/the-bank-account")
                .ResolveOne(TimeSpan.FromSeconds(5))
                .GetAwaiter()
                .GetResult();

            ShowWelcome();
        }

        protected override void Unhandled(object message)
        {
            _console.Tell("BEEP BEEP BEEP. UNEXPECTED INPUT!");
        }

        #region States

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
                    _console.Tell(new ConsoleOutputMessage(MainMenu, true));
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

            Context.System.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(10),
                _console,
                new ConsoleOutputMessage(MainMenu, true),
                Self);

            Become(MainMenuState);
        }

        #endregion

        private void ShowWelcome()
        {
            _console.Tell(new ConsoleOutputMessage(MainMenu, true));
        }
    }
}
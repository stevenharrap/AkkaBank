 using System;
using Akka.Actor;
 using Akka.Cluster.Tools.PublishSubscribe;
 using AkkaBank.BasicBank.Messages.Account;
using AkkaBank.BasicBank.Messages.AtmV2;
using AkkaBank.BasicBank.Messages.Bank;
 using AkkaBank.BasicBank.Messages.BankAdmin;
 using AkkaBank.BasicBank.Messages.Console;

namespace AkkaBank.BasicBank.Actors
{
    public class AtmV2Actor : ReceiveActor
    {
        private IActorRef _console;
        private IActorRef _bank;
        private CustomerAccount _customerAccount;

        public AtmV2Actor()
        {
            var mediator = DistributedPubSub.Get(Context.System).Mediator;
            mediator.Tell(new Subscribe("advert", Self));

            Become(WaitingForBankState);
        }
        
        protected override void PreStart()
        {
            _console = Context.ActorOf(Props.Create(() => new ConsoleActor()), "atm-console");
        }

        protected override void Unhandled(object message)
        {
            switch (message)
            {
                case ConsoleInput cim:
                    _console.Tell("BEEP BEEP BEEP. UNEXPECTED CONSOLE INPUT!");
                    break;

                //possibly log other message types
            }            
        }

        #region States

        private void WaitingForBankState()
        {
            Receive((Action<Messages.Bank.BankActor>) this.HandleBankActor);
        }

        private void WaitingForCustomerNumberState()
        {
            Receive<ConsoleInput>(HandleCustomerNumberInput);
            Receive<Advertisement>(HandleAdvertisement);
        }

        private void WaitingForCustomerState()
        {
            Receive<GetCustomerResponse>(HandleCustomerResponse);
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
            Receive<ReceiptTimedOut>(HandleTransactionTimedOut);
        }

        #endregion

        #region Handlers

        private void HandleBankActor(Messages.Bank.BankActor message)
        {
            _bank = message.Bank;
            Become(WaitingForCustomerNumberState);
            _console.Tell(MakeWelcomeScreenMessage());
        }

        private void HandleAdvertisement(Advertisement message)
        {
            _console.Tell(MakeWelcomeScreenMessage(message));
        }

        private void HandleCustomerNumberInput(ConsoleInput message)
        {
            if (int.TryParse(message.Input, out var accountNumber))
            {
                _bank.Tell(new GetCustomerRequst(accountNumber));
                _console.Tell("Please wait.. taking to the bank.\n");
                Become(WaitingForCustomerState);
                return;
            }

            _console.Tell("That's not an account number! Try again:");
        }

        private void HandleTransactionTimedOut(ReceiptTimedOut message)
        {
            _customerAccount = null;
            Become(WaitingForCustomerNumberState);
            _console.Tell(MakeWelcomeScreenMessage());
        }

        private void HandleCustomerResponse(GetCustomerResponse message)
        {
            if (message.Ok)
            {
                _customerAccount = message.CustomerAccount;
                _console.Tell(MakeMainMenuScreenMessage());
                Become(MainMenuState);
                return;
            }

            _console.Tell("Unknown account!");
            Become(WaitingForCustomerNumberState);

            Context.System.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(7),
                _console,
                MakeWelcomeScreenMessage(),
                Self);
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
                _customerAccount.Account.Tell(new DepositMoneyRequest(amount));
                _console.Tell("Please wait.. taking to the bank.\n");
                Context.System.Scheduler.ScheduleTellOnce(
                    TimeSpan.FromSeconds(6),
                    Self,
                    new ReceiptTimedOut(),
                    Self);

                Become(WaitingForReceiptState);
                return;
            }

            _console.Tell("That's not money! Try again:");
        }

        private void HandleWithdrawalInput(ConsoleInput message)
        {
            if (int.TryParse(message.Input, out var amount))
            {
                _customerAccount.Account.Tell(new WithdrawMoneyRequest(amount));
                _console.Tell("Please wait.. taking to the bank.\n");
                Context.System.Scheduler.ScheduleTellOnce(
                    TimeSpan.FromSeconds(6),
                    Self,
                    new ReceiptTimedOut(),
                    Self);

                Become(WaitingForReceiptState);
                return;
            }

            _console.Tell("That's not money! Try again:");
        }

        private void HandleReceipt(ReceiptResponse message)
        {
            _console.Tell("Your transaction is complete!...\n");
            _console.Tell($"The balance of your account is: ${message.Balance}\n");

            _customerAccount = null;
            Become(WaitingForCustomerNumberState);

            Context.System.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(7),
                _console,
                MakeWelcomeScreenMessage(),                
                Self);
        }

        #endregion

        #region Screens        

        private ConsoleOutput MakeMainMenuScreenMessage()
        {
            return new ConsoleOutput(
                new[] {
                    $"Hi {_customerAccount.Customer.CustomerName},",
                    "WELCOME TO BASIC BANK.",
                    string.Empty,
                    "[w] WITHDRAWAL",
                    "[d] DEPOSIT"
                },
                clear: true,
                boxed: true,
                padding: 10);
        }

        private ConsoleOutput MakeWelcomeScreenMessage(Advertisement advert = null)
        {          
            return new ConsoleOutput(
                new[] {
                    "WELCOME TO BASIC BANK.",
                    advert == null 
                    ? string.Empty
                    : advert.Blurb,
                    "PLEASE ENTER YOU ACC."
                },
                clear: true,
                boxed: true,
                padding: 10);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using AkkaBank.BasicBank.Messages.Bank;
using AkkaBank.BasicBank.Messages.BankAdmin;
using AkkaBank.BasicBank.Messages.Console;

namespace AkkaBank.BasicBank.Actors
{
    public class BankAdminActor : ReceiveActor, IWithUnboundedStash
    {
        private IActorRef _console;
        private IActorRef _bank;

        private readonly string[] _adverts = {
            "BUY MORE PANTS!",
            "BUY MORE SHIRTS!",
            "BUY MORE HATS!"
        };

        private int _advertId = 0;

        public IStash Stash { get; set; }

        public BankAdminActor()
        {
            Become(WaitingForBankState);
        }

        protected override void PreStart()
        {
            _console = Context.ActorOf(Props.Create(() => new ConsoleActor()), "atm-console");
        }

        #region States

        private void WaitingForBankState()
        {
            Receive((Action<Messages.Bank.BankActor>) this.HandleBankActor);
        }

        private void WaitingForMenuInput()
        {
            Receive<ConsoleInput>(HandleMainMenuInput);
        }

        private void WaitingForCustomerAccounts()
        {
            Receive<GetCustomerResponse>(message => {
                _console.Tell($"Received {message.CustomerAccount.Customer.CustomerName}");
                Stash.Stash();
            });
            Receive<ProcessCustomerAccounts>(message =>
            {
                Become(ProccessingCustomerAccounts);
                Stash.UnstashAll();
            });
        }

        private void ProccessingCustomerAccounts()
        {
            Receive<GetCustomerResponse>(HandleProcessCustomerAccount);
        }

        #endregion

        #region Handlers

        private void HandleBankActor(Messages.Bank.BankActor message)
        {
            _bank = message.Bank;
            _console.Tell(MakeMainMenuScreenMessage());
            Become(WaitingForMenuInput);
        }

        private void HandleMainMenuInput(ConsoleInput message)
        {
            var mediator = DistributedPubSub.Get(Context.System).Mediator;

            switch (message.Input)
            {
                case "a":
                {
                    _console.Tell("sending advertisement");                    
                    mediator.Tell(new Publish("advert", new Advertisement(_adverts[_advertId])));
                    _advertId = _advertId == _adverts.Length - 1 ? 0 : _advertId + 1;
                    break;
                }
                case "b":
                    _console.Tell("billing account fees");
                    Become(WaitingForCustomerAccounts);
                    mediator.Tell(new Publish("request-customer-accounts", new GetCustomersRequest()));
                    Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(10), Self, new ProcessCustomerAccounts(), Self);
                    break;

                default:
                    _console.Tell(MakeMainMenuScreenMessage());
                    _console.Tell("What!? Try again...");
                    break;
            }
        }

        private void HandleProcessCustomerAccount(GetCustomerResponse message)
        {
            _console.Tell($"Processing {message.CustomerAccount.Customer.CustomerName}");
        }

        #endregion  

        #region Screens        

        private ConsoleOutput MakeMainMenuScreenMessage()
        {            
            return new ConsoleOutput(
                new[] {
                    "BASIC BANK ADMIN.",
                    string.Empty,
                    "[a] ADVERTISE",
                    "[b] BILL ACCOUNT FEES"
                },
                clear: true,
                boxed: true,
                padding: 10);
        }

        #endregion
    }
}

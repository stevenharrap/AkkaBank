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
    public class BankAdminActor : ReceiveActor
    {
        private IActorRef _console;
        private IActorRef _bank;

        private readonly string[] _adverts = {
            "BUY MORE PANTS!",
            "BUY MORE SHIRTS!",
            "BUY MORE HATS!"
        };

        private int _advertId = 0;

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
            switch (message.Input)
            {
                case "a":
                {
                    _console.Tell("sending advertisement");
                    var mediator = DistributedPubSub.Get(Context.System).Mediator;
                    mediator.Tell(new Publish("advert", new Advertisement(_adverts[_advertId])));
                    _advertId = _advertId == _adverts.Length - 1 ? 0 : _advertId + 1;
                    break;
                }
                case "b":
                    _console.Tell("billing account fees");
                    break;

                default:
                    _console.Tell(MakeMainMenuScreenMessage());
                    _console.Tell("What!? Try again...");
                    break;
            }
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

using System;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Routing;
using AkkaBank.BasicBank.Messages.Bank;
using AkkaBank.BasicBank.Messages.BankAdmin;
using AkkaBank.BasicBank.Messages.Console;
using Phobos.Actor;

namespace AkkaBank.BasicBank.Actors
{
    public class BankAdminActor : ReceiveActor, IWithUnboundedStash
    {
        private IActorRef _console;
        private IActorRef _bank;
        private IActorRef _accountFeeRouter;

        private readonly string[] _adverts = {
            "BUY MORE PANTS!",
            "BUY MORE SHIRTS!",
            "BUY MORE HATS!"
        };
        private int _advertId = 0;
        private int _fee = 5;
        private int _charged = 0;

        public IStash Stash { get; set; }

        public BankAdminActor()
        {
            Become(WaitingForBankState);
        }

        protected override void PreStart()
        {
            _console = Context.ActorOf(Props.Create(() => new ConsoleActor()), "atm-console");
            _accountFeeRouter = Context.ActorOf(
                Props.Create<AccountFeeActor>()
                .WithRouter(new RoundRobinPool(5, new DefaultResizer(1, 10))));
        }        

        #region States

        private void WaitingForBankState()
        {
            Receive((Action<Messages.Bank.SetBank>) this.HandleBankActor);
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
            Receive<GetCustomerResponse>(HandleCustomerAccount);
            Receive<AccountFeeResponse>(HandleAccountFeeResponse);
        }
        
        #endregion

        #region Handlers

        private void HandleBankActor(Messages.Bank.SetBank message)
        {
            _bank = message.Bank;
            _console.Tell(MakeMainMenuScreenMessage());
            Become(WaitingForMenuInput);
        }

        private void HandleMainMenuInput(ConsoleInput message)
        {
            var mediator = DistributedPubSub.Get(Context.System).Mediator;
            _console.Tell(MakeMainMenuScreenMessage());
            switch (message.Input)
            {
                case "a":                   
                    _console.Tell("sending advertisement");                    
                    mediator.Tell(new Publish("advert", new Advertisement(_adverts[_advertId])));
                    _advertId = _advertId == _adverts.Length - 1 ? 0 : _advertId + 1;
                    var timing = Context.GetInstrumentation().Monitor.CreateTiming("process-time");
                    timing.Record(42);
                    break;
                
                case "b":
                    _console.Tell("billing account fees");
                    Become(WaitingForCustomerAccounts);
                    mediator.Tell(new Publish("request-customer-accounts", new GetCustomersRequest()));
                    Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(10), Self, new ProcessCustomerAccounts(), Self);
                    break;

                default:
                    _console.Tell("What!? Try again...");
                    break;
            }
        }

        private void HandleCustomerAccount(GetCustomerResponse message)
        {
            _console.Tell($"Processing {message.CustomerAccount.Customer.CustomerName}");
            _accountFeeRouter.Tell(new AccountFeeRequest(_fee, message.CustomerAccount, Self));
            _charged++;
        }

        private void HandleAccountFeeResponse(AccountFeeResponse message)
        {
            _console.Tell(message.FeeCharged > 0
                ? $"{message.CustomerAccount.Customer.CustomerName} charged {message.FeeCharged}, Remaing balance: {message.Balance}"
                : $"{message.CustomerAccount.Customer.CustomerName} has insufficent funds. Balance: {message.Balance}");
            _charged--;

            if (_charged == 0)
            {
                _console.Tell("All customer accounts billed.");
                Become(WaitingForMenuInput);
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

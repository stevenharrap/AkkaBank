using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using AkkaDakka.BasicBank.Messages;

namespace AkkaDakka.BasicBank
{
    public class StartSessionMessage { }

    public class TellerActor : ReceiveActor
    {
        public TellerActor()
        {
            Receive<BalanceMessage>(message => HandleBalance(message));
        }

        private void HandleBalance(BalanceMessage message)
        {
            Console.WriteLine($"Your balance is: ${message.Amount}");
        }

        
    }
}

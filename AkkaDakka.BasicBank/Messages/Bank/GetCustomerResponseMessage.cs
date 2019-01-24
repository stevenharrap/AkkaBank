﻿using Newtonsoft.Json;

namespace AkkaBank.BasicBank.Messages.Bank
{
    public class GetCustomerResponseMessage
    {
        [JsonProperty]
        public CustomerAccount CustomerAccount { get; private set; }

        [JsonProperty]
        public bool Ok { get; private set; }

        [JsonProperty]
        public string Error { get; private set; }

        private GetCustomerResponseMessage() { }

        public GetCustomerResponseMessage(CustomerAccount customerAccount)
        {
            CustomerAccount = customerAccount;
            Ok = true;
        }

        public GetCustomerResponseMessage(string error)
        {
            Error = error;
            Ok = false;
        }
    }
}
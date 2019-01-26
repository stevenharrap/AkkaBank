using Newtonsoft.Json;

namespace AkkaBank.BasicBank.Messages.Bank
{
    public class GetCustomerResponse
    {
        [JsonProperty]
        public CustomerAccount CustomerAccount { get; private set; }

        [JsonProperty]
        public bool Ok { get; private set; }

        [JsonProperty]
        public string Error { get; private set; }

        private GetCustomerResponse() { }

        public GetCustomerResponse(CustomerAccount customerAccount)
        {
            CustomerAccount = customerAccount;
            Ok = true;
        }

        public GetCustomerResponse(string error)
        {
            Error = error;
            Ok = false;
        }
    }
}
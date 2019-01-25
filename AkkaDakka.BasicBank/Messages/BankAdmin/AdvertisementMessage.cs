namespace AkkaBank.BasicBank.Messages.BankAdmin
{
    public class AdvertisementMessage
    {
        public string Blurb { get; }

        public AdvertisementMessage(string blurb)
        {
            Blurb = blurb;
        }
    }
}

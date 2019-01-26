namespace AkkaBank.BasicBank.Messages.BankAdmin
{
    public class Advertisement
    {
        public string Blurb { get; }

        public Advertisement(string blurb)
        {
            Blurb = blurb;
        }
    }
}

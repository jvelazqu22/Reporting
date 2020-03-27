namespace iBank.Services.Implementation.Classes
{
    public class UdidInformation
    {
        public UdidInformation()
        {
            RecKey = 0;
            UdidNumber = 0;
            UdidText = string.Empty;
        }

        public int RecKey { get; set; }

        public int? UdidNumber { get; set; }

        public string UdidText { get; set; }
    }
}

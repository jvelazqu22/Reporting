namespace iBank.Services.Orm.Classes
{
    public class ClientsTLInformation
    {
        public ClientsTLInformation()
        {
            RecordNo = 0;
            Agency = string.Empty;
            ClientId = string.Empty;
            ClientName = string.Empty;
        }

        public int RecordNo { get; set; }
        public string Agency { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
    }
}

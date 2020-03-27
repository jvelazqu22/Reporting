namespace iBank.Services.Orm.Classes
{
    public class BroadcastEmailMasterInfo
    {
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }

        public BroadcastEmailMasterInfo()
        {
            SenderEmail = "";
            SenderName = "";
        }
    }
}

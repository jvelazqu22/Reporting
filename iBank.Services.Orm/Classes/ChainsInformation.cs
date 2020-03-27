namespace iBank.Services.Orm.Classes
{
    public class ChainsInformation
    {
        public ChainsInformation()
        {
            ChainCode = string.Empty;
            ChainDescription = string.Empty;
            ChainClass = string.Empty;
            ChainParent = string.Empty;
        }

        public string ChainCode { get; set; }
        public string ChainDescription { get; set; }
        public string ChainClass { get; set; }
        public string ChainParent { get; set; }
    }
}

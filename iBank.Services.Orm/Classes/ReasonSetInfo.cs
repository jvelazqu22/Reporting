namespace iBank.Services.Orm.Classes
{
    public class ReasonSetInfo
    {
        public ReasonSetInfo()
        {
            Agency = string.Empty;
            DefLang = string.Empty;
            ReasSetDesc = string.Empty;
            ReasSetNbr = 0;
        }
        public string Agency { get; set; }
        public string DefLang { get; set; }
        public string ReasSetDesc { get; set; }
        public int ReasSetNbr { get; set; }
    }
}

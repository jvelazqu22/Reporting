using Domain.Interfaces;
namespace Domain.Models.ReportPrograms.XmlExtractReport
{

    public class UdidRawData : IRecKey
    {
        public UdidRawData()
        {
            RecLoc = string.Empty;
            Agency = string.Empty;
            UdidText = string.Empty;
        }
        public int RecKey { get; set; }
        public string RecLoc { get; set; }
        public string Agency { get; set; }
        public int Udidno { get; set; }
        public string UdidText { get; set; }
    }
}

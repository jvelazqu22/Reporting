using Domain.Helper;

namespace Domain.Models.BroadcastServer
{
    public class BroadcastServerInformation
    {
        public string ReportLogoDirectory { get; set; }
        public BroadcastServerFunction ServerFunction { get; set; }
        public int ServerNumber { get; set; }
        public bool IsOfflineServer
        {
            get
            {
                return true;
            }
        }
        public bool ProcessHotBroadcast { get; set; }
        public string ReportOutputDirectory { get; set; }
        public string SenderEmailAddress { get; set; }
        public string SenderName { get; set; }
        public string IbankBaseUrl { get; set; }
        public string CrystalReportDirectory { get; set; }
    }
}

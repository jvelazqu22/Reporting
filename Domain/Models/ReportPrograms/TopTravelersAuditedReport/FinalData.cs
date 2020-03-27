using System;

namespace Domain.Models.ReportPrograms.TopTravelersAuditedReport
{
    public class FinalData
    {
        public string Passname { get; set; } = string.Empty;
        public decimal Approved { get; set; } = 0m;
        public decimal Notifyonly { get; set; } = 0m;
        public decimal Declined { get; set; } = 0m;
        public decimal Expired { get; set; } = 0m;
        public decimal Trips { get; set; } = 0m;
        public decimal Bookvolume { get; set; } = 0m;
        public decimal Avgcost { get { return Trips  == 0 ? 0 : Math.Round(Bookvolume/Trips,2); } }
    }
}

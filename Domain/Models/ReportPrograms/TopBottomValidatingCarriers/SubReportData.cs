using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ReportPrograms.TopBottomValidatingCarriers
{
    public class SubReportData
    {
        public SubReportData()
        {
            HomeCtry = string.Empty;
            Trips = 0;
            Amt = 0m;
            Avgcost = 0m;
            Trips2 = 0;
            Amt2 = 0m;
            Avgcost2 = 0m;
        }
        public string HomeCtry { get; set; }
        public int Trips { get; set; }
        public decimal Amt { get; set; }
        public decimal Avgcost { get; set; }
        public int Trips2 { get; set; }
        public decimal Amt2 { get; set; }
        public decimal Avgcost2 { get; set; }
    }
}

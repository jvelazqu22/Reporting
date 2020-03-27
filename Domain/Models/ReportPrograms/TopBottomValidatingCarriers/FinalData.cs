using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ReportPrograms.TopBottomValidatingCarriers
{
    public class FinalData
    {
        public string CtryCode { get; set; } = string.Empty;
        public string HomeCtry { get; set; } = string.Empty;
        public string ValCarr { get; set; } = string.Empty;
        public string Carrdesc { get; set; }
        public int Trips { get; set; } = 0;
        public decimal Amt { get; set; } = 0m;
        public decimal Avgcost { get; set; } = 0m;
        public int SubTrips { get; set; } = 0;
        public decimal SubAmt { get; set; } = 0m;
        public int Trips2 { get; set; } = 0;
        public decimal Amt2 { get; set; } = 0m;
        public decimal Avgcost2 { get; set; } = 0m;
        public int SubTrips2 { get; set; } = 0;
        public decimal SubAmt2 { get; set; } = 0m;

    }
}

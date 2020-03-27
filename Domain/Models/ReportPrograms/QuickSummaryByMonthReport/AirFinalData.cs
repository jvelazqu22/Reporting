using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ReportPrograms.QuickSummaryByMonthReport
{
    public class AirFinalData
    {
        public int RptYear { get; set; }
        public int RptMonth { get; set; }
        public string RptMthText { get; set; }
        public int PlusMin { get; set; }
        public decimal AirChg { get; set; }
        public string ReasCode { get; set; }
        public string SavingCode { get; set; }
        public decimal StndChg { get; set; }
        public decimal Savings { get; set; }
        public decimal OffrdChg { get; set; }
        public decimal NegoSvngs { get; set; }
        public decimal LostAmt { get; set; }
    }
}

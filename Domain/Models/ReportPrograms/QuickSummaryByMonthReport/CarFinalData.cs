using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ReportPrograms.QuickSummaryByMonthReport
{
    public class CarFinalData
    {
        public int RptYear { get; set; }
        public int RptMonth { get; set; }
        public string RptMthText { get; set; }
        public int CPlusMin { get; set; }
        public int Days { get; set; }
        public decimal ABookRat { get; set; }
        public string ReasCoda { get; set; }
        public decimal AExcprat { get; set; }
        public int CarRents { get; set; }
        public int CarDays { get; set; }
        public decimal CarVolume { get; set; }
        public int CarExcepts { get; set; }
        public decimal CarLost { get; set; }

    }
}

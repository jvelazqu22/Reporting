using System;
using System.Collections.Generic;

namespace Domain.Models.ReportPrograms.PassengersOnPlaneReport
{
    public class GroupedData
    {
        public string Acct { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string Airline { get; set; }
        public string Fltno { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Mode { get; set; }
        public DateTime? Rdepdate { get; set; }
        public string Orgdesc { get; set; }
        public string Destdesc { get; set; }
        public IEnumerable<FinalData> Pax { get; set; }

        public bool InRange { get; set; } = true;
    }
}

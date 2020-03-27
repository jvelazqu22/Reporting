using System;

namespace Domain.Models.ReportPrograms.SameCityReport
{
    public class FinalData
    {
        public DateTime Rarrdate { get; set; }
        public decimal Sgrpcnt { get; set; }
        public string Airline { get; set; }
        public string Arrtime { get; set; }
        public string Carrier { get; set; }
        public string Class { get; set; }
        public string Destcity { get; set; }
        public string Destinat { get; set; }
        public string Fltno { get; set; }
        public string Invoice { get; set; }
        public string Orgcity { get; set; }
        public string Origin { get; set; }
        public string Passfrst { get; set; }
        public string Passlast { get; set; }
        public string Recloc { get; set; }
        public DateTime Sorttime { get; set; }
        public string Ticket { get; set; }
        public int Plusmin { get; set; }
    }
}

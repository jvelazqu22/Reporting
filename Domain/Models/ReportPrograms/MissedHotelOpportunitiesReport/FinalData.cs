using System;

namespace Domain.Models.ReportPrograms.MissedHotelOpportunitiesReport
{
    public class FinalData
    {
        public int Reckey { get; set; } = 0;
        public string Homectry { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty; 
        public DateTime Invdate { get; set; } = DateTime.MinValue;
        public string Agentid { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Trantype { get; set; } = string.Empty;
        public DateTime Tripstart { get; set; } = DateTime.MinValue;
        public DateTime Tripend { get; set; } = DateTime.MinValue;
        public string Itinerary { get; set; } = string.Empty;
        public decimal Tripduratn { get; set; } = 0m;
        public string Hotelbkd { get; set; } = string.Empty;
    }
}

using System;

namespace Domain.Models.ReportPrograms.TravelDetail
{
    public class TravDet2FinalData
    {
        public decimal HotelCost { get; set; } = 0m;

        public decimal CarCost { get; set; } = 0m;

        public DateTime InDate { get; set; } = DateTime.MinValue;
        public DateTime InvDate { get; set; } = DateTime.MinValue;
        public DateTime RdepDate { get; set; } = DateTime.MinValue;
        public decimal AirChg { get; set; } = 0m;
        public decimal BookRate { get; set; } = 0m;
        public decimal LostAmt { get; set; } = 0m;
        public decimal OffrdChg { get; set; } = 0m;
        public int Days { get; set; } = 0;
        public int PlusMin { get; set; } = 0;
        public int RecKey { get; set; } = 0;
        public int Rooms { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string AcctDesc { get; set; } = string.Empty;
        public bool AirIndicat { get; set; } = false;
        public string Airline { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string DestDesc { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string ExchInfo { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public string OrgDesc { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string PassFrst { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public string ReasCode { get; set; } = string.Empty;
        public string ReasCodV { get; set; } = string.Empty;
        public string RecLoc { get; set; } = string.Empty;
        public string RecType { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string TypeCode { get; set; } = string.Empty;
        public string Vendor { get; set; } = string.Empty;
    }
}

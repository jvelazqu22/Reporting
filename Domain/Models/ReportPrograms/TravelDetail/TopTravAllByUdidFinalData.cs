using System;

namespace Domain.Models.ReportPrograms.TravelDetail
{
    public class TopTravAllByUdidFinalData
    {
        public string passlast { get; set; }
        public string passfrst { get; set; }
        public string acct { get; set; }
        public string acctdesc { get; set; }
        public int tripcount { get; set; }
        public decimal airchg { get; set; } = 0m;
        public int Airco2 { get; set; }
        public int railcount { get; set; }
        public decimal railchg { get; set; }
        public int cardays { get; set; }
        public decimal carcost { get; set; }
        public int Carco2 { get; set; }
        public int hotnights { get; set; }
        public decimal hotelcost { get; set; }
        public int HotelCo2 { get; set; }
        public decimal tripcost { get { return carcost + hotelcost + airchg + railchg; } }
        public int TotCO2 { get; set; }
        public int daysonroad { get; set; }
        public DateTime TripStart { get; set; }
        public DateTime TripEnd { get; set; }
        public DateTime DepDate { get; set; }
        public DateTime ArrDate { get; set; }
        public int PlusMin { get; set; }
        public  string RecLoc { get; set; }
        public int RecKey { get; set; }
        public string homectry { get; set; }
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Udidtext { get; set; } = string.Empty;

    }
}

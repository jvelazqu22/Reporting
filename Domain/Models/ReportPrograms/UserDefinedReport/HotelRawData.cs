using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class HotelRawData : IRecKey, ICarbonCalculationsHotel
    {
        public HotelRawData()
        {
            HotCurrTyp = string.Empty;
            AirCurrTyp = string.Empty;
            RecKey = 0;
            Pseudocity = string.Empty;
            Agentid = string.Empty;
            Agency = string.Empty;
            Recloc = string.Empty;
            Invdate = DateTime.MinValue;
            Bookdate = DateTime.MinValue;
            Chaincod = string.Empty;
            Metro = string.Empty;
            Hotelnam = string.Empty;
            Hotcity = string.Empty;
            Hotstate = string.Empty;
            Datein = DateTime.MinValue;
            Dateout = DateTime.MinValue;
            Bookrate = 0m;
            Moneytype = string.Empty;
            Roomtype = string.Empty;
            Hexcprat = 0m;
            Guarante = string.Empty;
            Reascodh = string.Empty;
            Hotphone = string.Empty;
            Confirmno = string.Empty;
            Hotpropid = string.Empty;
            SeqNo = 0;
            Seqctr = 0;
            Numguests = 0;
            Compamt = 0m;
            HPlusMin = 0;
            Hcommissn = 0m;
            Nights = 0;
            Rooms = 0;
            Hoteladdr1 = string.Empty;
            Hoteladdr2 = string.Empty;
            Hotelzip = string.Empty;
            Hotcountry = string.Empty;
            Hotsvgcode = string.Empty;
            Hotstdrate = 0m;
            Hottrantyp = string.Empty;
            Emailaddr = string.Empty;
            Gds = string.Empty;
            Hotctrycod = string.Empty;
            Hotcitycod = string.Empty;
            Hotvendcod = string.Empty;
            Hotbktype = string.Empty;
            Smartctrh = 0;
            Comisableh = string.Empty;
            Hotratetyp = string.Empty;
            Hotsegstat = string.Empty;
            Hotvoiddat = DateTime.MinValue;
            Trdtrxnumh = 0;
        }

        public string HotCurrTyp { get; set; }
        public string AirCurrTyp { get; set; }
        public int RecKey { get; set; }
        public string Pseudocity { get; set; }
        public string Agentid { get; set; }
        public string Agency { get; set; }
        public string Recloc { get; set; }

        [ExchangeDate2]
        public DateTime? Invdate { get; set; }

        [ExchangeDate3]
        public DateTime? Bookdate { get; set; }

        public string Chaincod { get; set; }
        public string Metro { get; set; }
        public string Hotelnam { get; set; }
        public string Hotcity { get; set; }
        public string Hotstate { get; set; }
        public DateTime? Datein { get; set; }

        [ExchangeDate1]
        public DateTime CheckOut
        {
            get { return Datein.GetValueOrDefault().AddDays(Nights); }
        }

        public DateTime? Dateout { get; set; }

        [Currency(RecordType = RecordType.Hotel)]
        public decimal Bookrate { get; set; }

        [HotelCurrency]
        public string Moneytype { get; set; }

        public string Roomtype { get; set; }

        [Currency(RecordType = RecordType.Hotel)]
        public decimal Hexcprat { get; set; }

        public string Guarante { get; set; }
        public string Reascodh { get; set; }
        public string Hotphone { get; set; }
        public string Confirmno { get; set; }
        public bool Hinvbyagcy { get; set; }
        public string Hotpropid { get; set; }
        public int SeqNo { get; set; }
        public int Seqctr { get; set; }
        public int Numguests { get; set; }

        [Currency(RecordType = RecordType.Hotel)]
        public decimal? Compamt { get; set; }

        public int HPlusMin { get; set; }

        [Currency(RecordType = RecordType.Hotel)]
        public decimal? Hcommissn { get; set; }

        public int Nights { get; set; }
        public int Rooms { get; set; }
        public string Hoteladdr1 { get; set; }
        public string Hoteladdr2 { get; set; }
        public string Hotelzip { get; set; }
        public string Hotcountry { get; set; }
        public string Hotsvgcode { get; set; }

        [Currency(RecordType = RecordType.Hotel)]
        public decimal Hotstdrate { get; set; }

        public string Hottrantyp { get; set; }
        public string Emailaddr { get; set; }
        public string Gds { get; set; }
        public string Hotctrycod { get; set; }
        public string Hotcitycod { get; set; }
        public string Hotvendcod { get; set; }
        public string Hotbktype { get; set; }
        public int Smartctrh { get; set; }
        public string Comisableh { get; set; }
        public string Hotratetyp { get; set; }
        public string Hotsegstat { get; set; }
        public DateTime? Hotvoiddat { get; set; }
        public int Trdtrxnumh { get; set; }

        public decimal HotelCo2 { get; set; }
    }
}

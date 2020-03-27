using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class MarketSegmentRawData : IRecKey
    {
        public int RecKey { get; set; }
        public string Agency { get; set; }
        public string Recloc { get; set; }
        public string Invoice { get; set; }

        [AirCurrency]
        public string Moneytype { get; set; }

        [ExchangeDate1]
        public DateTime? Invdate { get; set; }

        public string Pseudocity { get; set; }
        public string Agentid { get; set; }

        [ExchangeDate2]
        public DateTime? Bookdate { get; set; }

        public int Segnum { get; set; }
        public string Segorigin { get; set; }
        public string Segdest { get; set; }
        public string Mktseg { get; set; }
        public string Mktsegboth { get; set; }
        public string Mktctry { get; set; }
        public string Mktreg { get; set; }
        public string MktCtry2 { get; set; }
        public string MktReg2 { get; set; }
        public int Miles { get; set; }
        public int Stops { get; set; }
        public string Mode { get; set; }
        public string Airline { get; set; }
        public string Fltno { get; set; }
        public DateTime? Sdepdate { get; set; }
        public DateTime? Sarrdate { get; set; }
        public string Sdeptime { get; set; }
        public string Sarrtime { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Segfare { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Segmiscamt { get; set; }

        public string DitCode { get; set; }
        public string Class { get; set; }
        public string Classcat { get; set; }
        public int Splusmin { get; set; }
        public int Flduration { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? Seggrsfare { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? Segprofare { get; set; }

        public string Firstfltno { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? Grossamt { get; set; }

        public string Prdairline { get; set; }
        public string Prdclass { get; set; }
        public string Prdclscat { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? Segcommisn { get; set; }

        public string Firstaline { get; set; }
        public string Prdfbase { get; set; }
        public string Samealine { get; set; }
        public string Samefbase { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? Sfaretax { get; set; }

        public int? Connectime { get; set; }
        public string Sfltstatus { get; set; }

        public string Segtransid { get; set; }
    }
}

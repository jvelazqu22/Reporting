using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class MiscRawData : IRecKey
    {
        public MiscRawData()
        {
            RecKey = 0;
            Agency = string.Empty;
            Recloc = string.Empty;
            Invoice = string.Empty;
            Invdate = DateTime.MinValue;
            Pseudocity = string.Empty;
            Agentid = string.Empty;
            Bookdate = DateTime.MinValue;
            Segtype = string.Empty;
            Msorigin = string.Empty;
            Msdestinat = string.Empty;
            Msorgctry = string.Empty;
            Msdestctry = string.Empty;
            Vendorcode = string.Empty;
            Svcidnbr = string.Empty;
            Msdepdate = DateTime.MinValue;
            Msdeptime = string.Empty;
            Msarrdate = DateTime.MinValue;
            Msarrtime = string.Empty;
            Msseqno = 0;
            Class = string.Empty;
            Msplusmin = 0;
            Mstrantype = string.Empty;
            Segamt = 0m;
            Moneytype = string.Empty;
            Msexcprate = 0m;
            Msstndrate = 0m;
            Mslosscode = string.Empty;
            Mssvgcode = string.Empty;
            Tax1 = 0m;
            Tax2 = 0m;
            Tax3 = 0m;
            Tax4 = 0m;
            Cabintype = string.Empty;
            Confirmno = string.Empty;
            Cabinseat = string.Empty;
            Mxchaincod = string.Empty;
            Mealdesc = string.Empty;
            Nitecount = 0;
            Opt = string.Empty;
            Arrivermks = string.Empty;
            Departrmks = string.Empty;
            Mxtourcode = string.Empty;
            Mxtourname = string.Empty;
            Trnsfrrmks = string.Empty;
            Mxvendname = string.Empty;
            Mxsgstatus = string.Empty;
            Tourcount = 0;
        }

        public int RecKey { get; set; }
        public string Agency { get; set; }
        public string Recloc { get; set; }
        public string Invoice { get; set; }

        [ExchangeDate1]
        public DateTime? Invdate { get; set; }

        public string Pseudocity { get; set; }
        public string Agentid { get; set; }

        [ExchangeDate2]
        public DateTime? Bookdate { get; set; }

        public string Segtype { get; set; }
        public string Msorigin { get; set; }
        public string Msdestinat { get; set; }
        public string Msorgctry { get; set; }
        public string Msdestctry { get; set; }
        public string Vendorcode { get; set; }
        public string Svcidnbr { get; set; }
        public DateTime? Msdepdate { get; set; }
        public string Msdeptime { get; set; }
        public DateTime? Msarrdate { get; set; }
        public string Msarrtime { get; set; }
        public int Msseqno { get; set; }
        public string Class { get; set; }
        public int Msplusmin { get; set; }
        public string Mstrantype { get; set; }

        [Currency(RecordType = RecordType.MiscSeg)]
        public decimal Segamt { get; set; }

        [MiscSegCurrency]
        public string Moneytype { get; set; }

        [Currency(RecordType = RecordType.MiscSeg)]
        public decimal Msexcprate { get; set; }

        [Currency(RecordType = RecordType.MiscSeg)]
        public decimal Msstndrate { get; set; }

        public string Mslosscode { get; set; }
        public string Mssvgcode { get; set; }

        [Currency(RecordType = RecordType.MiscSeg)]
        public decimal Tax1 { get; set; }

        [Currency(RecordType = RecordType.MiscSeg)]
        public decimal Tax2 { get; set; }

        [Currency(RecordType = RecordType.MiscSeg)]
        public decimal Tax3 { get; set; }

        [Currency(RecordType = RecordType.MiscSeg)]
        public decimal Tax4 { get; set; }

        public string Cabintype { get; set; }
        public string Confirmno { get; set; }
        public string Cabinseat { get; set; }
        public string Mxchaincod { get; set; }
        public string Mealdesc { get; set; }
        public int Nitecount { get; set; }
        public string Opt { get; set; }
        public string Arrivermks { get; set; }
        public string Departrmks { get; set; }
        public string Mxtourcode { get; set; }
        public string Mxtourname { get; set; }
        public string Trnsfrrmks { get; set; }
        public string Mxvendname { get; set; }
        public string Mxsgstatus { get; set; }
        public int? Tourcount { get; set; }
    }
}

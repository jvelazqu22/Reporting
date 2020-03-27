using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class MiscSegSharedRawData :IRecKey
    {        
        public int RecKey { get; set; }
        public string Agency { get; set; }
        public string Recloc { get; set; }

        [ExchangeDate1]
        public string Invoice { get; set; }

        public DateTime? Invdate { get; set; }
        public string Pseudocity { get; set; }
        public string Agentid { get; set; }

        [ExchangeDate2]
        public DateTime? Bookdate { get; set; }

        [MiscSegCurrency]
        public string Moneytype { get; set; }

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

        public string Prodcode { get; set; }
        public int Numvehics { get; set; }
        public int Numadults { get; set; }
        public int Numchild { get; set; }
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
        public int Tourcount { get; set; }
        public decimal Baseprice1 { get; set; }
        public string Baseprice2 { get; set; }
        public int Nbrrooms { get; set; }

        [Currency(RecordType = RecordType.MiscSeg)]
        public decimal Mscommisn { get; set; }

        public string Msorgcode { get; set; }
        public string Msdestcode { get; set; }
        public int Mssegnum { get; set; }
        public int Msduration { get; set; }
        public string Shipname { get; set; }
        public string Cabincateg { get; set; }
        public string Cabinnbr { get; set; }
        public string Cabindeck { get; set; }
        public string Pgmid { get; set; }
        public string Spclinfo { get; set; }
        public string Regionid { get; set; }
        public string Msratetype { get; set; }
    }
}

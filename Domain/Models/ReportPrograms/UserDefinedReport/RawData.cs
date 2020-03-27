using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{

    //This is basically "trip" data.
    public class RawData : IRecKey
    {
        public RawData()
        {
            Agency = string.Empty;
            RecKey = 0;
            Recloc = string.Empty;
            Branch = string.Empty;
            Agentid = string.Empty;
            Pseudocity = string.Empty;
            Acct = string.Empty;
            Invoice = string.Empty;
            Invdate = DateTime.MinValue;
            Bookdate = DateTime.MinValue;
            Domintl = string.Empty;
            Ticket = string.Empty;
            Passlast = string.Empty;
            Passfrst = string.Empty;
            Break1 = string.Empty;
            Break2 = string.Empty;
            Break3 = string.Empty;
            Break4 = string.Empty;
            Airchg = 0m;
            Credcard = string.Empty;
            Cardnum = string.Empty;
            Ccnumber2 = string.Empty;
            Arrdate = DateTime.MinValue;
            Mktfare = 0m;
            DepDate = DateTime.MinValue;
            Tripstart = DateTime.MinValue;
            Tripend = DateTime.MinValue;
            Stndchg = 0m;
            Offrdchg = 0m;
            Reascode = string.Empty;
            Savingcode = string.Empty;
            Valcarr = string.Empty;
            Tickettype = string.Empty;
            Faretax = 0m;
            Basefare = 0m;
            Corpacct = string.Empty;
            Sourceabbr = string.Empty;
            Tax4 = 0m;
            Tax3 = 0m;
            Tax2 = 0m;
            Tax1 = 0m;
            Origticket = string.Empty;
            Exchange = false;
            Moneytype = string.Empty;
            Iatanbr = string.Empty;
            Chgindcatr = string.Empty;
            Trantype = string.Empty;
            PlusMin = 0;
            Svcfee = 0m;
            Acommisn = 0m;
            Refundable = string.Empty;
            Tourcode = string.Empty;
            Cancelcode = string.Empty;
            Pnrcrdtgmt = DateTime.MinValue;
            Parsestamp = DateTime.MinValue;
            Valcarmode = string.Empty;
            Origvalcar = string.Empty;
            Phone = string.Empty;
            Bktool = string.Empty;
            Bkagent = string.Empty;
            Tkagent = string.Empty;
            Gds = string.Empty;
            Changedby = string.Empty;
            Changstamp = DateTime.MinValue;
            Agcontact = string.Empty;
            Emailaddr = string.Empty;
            Lastupdate = DateTime.MinValue;
            Invctrycod = string.Empty;
            Netremit = string.Empty;
            Primprodcd = string.Empty;
            Clientid = string.Empty;
            Formofpay = string.Empty;
            Passseqno = 0;
            Trpvendcod = string.Empty;
            Invamt = 0m;
            Invcommis = 0m;
            BkPseudo = string.Empty;
            TkPseudo = string.Empty;
            SmartCtrTr = 0;
            BkAgtName = string.Empty;
            TkAgtName = string.Empty;
            TrpRetDate = DateTime.MinValue;
            ReturnFlt = string.Empty;
            RetExchInd = string.Empty;
            OrigInvoic = string.Empty;
            OrigInvDat = DateTime.MinValue;
            OrigCurr = string.Empty;
            ExchRate = 0m;
            TktTypeInd = string.Empty;
            Discount = string.Empty;
            DocStatus = string.Empty;
            ExchCode = string.Empty;
            TrpHotSeq = 0;
            TrpCarSeq = 0;
            TrpTourSeq = 0;
            TrpOthSeq = 0;
            TrpAuxSeq = 0;
            VoidDate = DateTime.MinValue;
            ValCarrNo = string.Empty;
            VendRevGrp = string.Empty;
            VendName = string.Empty;
            Origin = string.Empty;
            Destinat = string.Empty;
            Airline = string.Empty;
            DitCode = string.Empty;
            Ccnumber2 = string.Empty;
            Mode = string.Empty;
            Fees = 0m;
            Maxagentid = string.Empty;
            Atktstatus = string.Empty;
            Farebasis = string.Empty;
            Predomcarr = string.Empty;
            Trpctypair = string.Empty;
            Tktnbrsuff = string.Empty;
            Fopindic = string.Empty;
            Numsegs = 0;
            TrpODmiles = 0;
            TrPrdClass = string.Empty;
            TrpPrdDest = string.Empty;
            TrpPrdOrig = string.Empty;
            PrdCarComp = string.Empty;
            PrdFareBas = string.Empty;
            PrdHchain = string.Empty;
            SeqNo = 0;
            HasCarData = false;
            HasHotelData = false;
        }

        public string Agency { get; set; }
        public string DitCode { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Mode { get; set; }
        public string Airline { get; set; }
        public int RecKey { get; set; }
        public int SeqNo { get; set; }
        public DateTime? RDepDate { get; set; }
        public DateTime? RArrDate { get; set; }
        public string Recloc { get; set; }
        public string Branch { get; set; }
        public string Agentid { get; set; }
        public string Pseudocity { get; set; }
        public string Acct { get; set; }
        public string Invoice { get; set; }

        [ExchangeDate1]
        public DateTime? Invdate { get; set; }

        [ExchangeDate2]
        public DateTime? Bookdate { get; set; }

        public string Domintl { get; set; }
        public string Ticket { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string Break4 { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; }

        public string Credcard { get; set; }
        public string Cardnum { get; set; }
        public string Ccnumber2 { get; set; }
        public DateTime? Arrdate { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Mktfare { get; set; }

        public DateTime? DepDate { get; set; }
        public DateTime? Tripstart { get; set; }
        public DateTime? Tripend { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Stndchg { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; }

        public string Reascode { get; set; }
        public string Savingcode { get; set; }
        public string Valcarr { get; set; }
        public string Tickettype { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Faretax { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Basefare { get; set; }

        public string Corpacct { get; set; }
        public string Sourceabbr { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Tax4 { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Tax3 { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Tax2 { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Tax1 { get; set; }

        public string Origticket { get; set; }
        public bool Exchange { get; set; }

        [AirCurrency]
        public string Moneytype { get; set; }

        public string Iatanbr { get; set; }
        public string Chgindcatr { get; set; }
        public string Trantype { get; set; }
        public int PlusMin { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? Svcfee { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? Acommisn { get; set; }

        public string Refundable { get; set; }
        public string Tourcode { get; set; }
        public string Cancelcode { get; set; }
        public DateTime? Pnrcrdtgmt { get; set; }
        public DateTime? Parsestamp { get; set; }
        public string Valcarmode { get; set; }
        public string Origvalcar { get; set; }
        public string Phone { get; set; }
        public string Bktool { get; set; }
        public string Bkagent { get; set; }
        public string Tkagent { get; set; }
        public string Gds { get; set; }
        public string Changedby { get; set; }
        public DateTime? Changstamp { get; set; }
        public string Agcontact { get; set; }
        public string Emailaddr { get; set; }
        public DateTime? Lastupdate { get; set; }
        public string Invctrycod { get; set; }
        public string Netremit { get; set; }
        public string Primprodcd { get; set; }
        public string Clientid { get; set; }
        public string Formofpay { get; set; }
        public int? Passseqno { get; set; }
        public string Trpvendcod { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? Invamt { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? Invcommis { get; set; }

        public string BkPseudo { get; set; }
        public string TkPseudo { get; set; }
        public int? SmartCtrTr { get; set; }
        public string BkAgtName { get; set; }
        public string TkAgtName { get; set; }
        public DateTime? TrpRetDate { get; set; }
        public string ReturnFlt { get; set; }
        public string RetExchInd { get; set; }
        public string OrigInvoic { get; set; }
        public DateTime? OrigInvDat { get; set; }
        public string OrigCurr { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal? ExchRate { get; set; }
        public string TktTypeInd { get; set; }
        public string Discount { get; set; }
        public string DocStatus { get; set; }
        public string ExchCode { get; set; }
        public int? TrpHotSeq { get; set; }
        public int? TrpCarSeq { get; set; }
        public int? TrpTourSeq { get; set; }
        public int? TrpOthSeq { get; set; }
        public int? TrpAuxSeq { get; set; }
        public DateTime? VoidDate { get; set; }
        public string ValCarrNo { get; set; }
        public string VendRevGrp { get; set; }
        public string VendName { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal? Fees { get; set; }
        public string Maxagentid { get; set; }
        public string Atktstatus { get; set; }
        public string Farebasis { get; set; }
        public string Predomcarr { get; set; }
        public string Trpctypair { get; set; }
        public string Tktnbrsuff { get; set; }
        public string Fopindic { get; set; }
        public int? Numsegs { get; set; }
        public int? TrpODmiles { get; set; }
        public string TrPrdClass { get; set; }
        public string TrpPrdDest { get; set; }
        public string TrpPrdOrig { get; set; }
        public string PrdCarComp { get; set; }
        public string PrdFareBas { get; set; }
        public string PrdHchain { get; set; }

        //flags to indicate whether this records has associated data in Car or Hotel records
        public bool HasHotelData { get; set; }
        public bool HasCarData { get; set; }
    }
}

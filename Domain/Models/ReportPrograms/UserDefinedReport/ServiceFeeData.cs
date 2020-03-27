using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class ServiceFeeData : IRecKey
    {
        public ServiceFeeData()
        {
            RecKey = 0;
            Svcfee = 0m;
            Descript = string.Empty;
            Agency = string.Empty;
            Mco = string.Empty;
            Cardnum = string.Empty;
            TranDate = DateTime.MinValue;
            SfTranType = string.Empty;
            Moneytype = string.Empty;
            STax1 = 0m;
            STax2 = 0m;
            STax3 = 0m;
            STax4 = 0m;
            Iatanbr = string.Empty;
            Svccode = string.Empty;
            Vendorcode = string.Empty;
            Seqctr = 0;
        }

        public int RecKey { get; set; }

        [Currency(RecordType = RecordType.SvcFee)]
        public decimal Svcfee { get; set; }

        public string Descript { get; set; }

        public string SDescript
        {
            get { return Descript; }
        } // This is what the field is called in the columns table
        public string Agency { get; set; }
        public string Mco { get; set; }

        public string Smco
        {
            get { return Mco; }
        } // This is what the field is called in the columns table
        public string Cardnum { get; set; }
        public string Scardnum
        {
            get { return Cardnum; }
        } // This is what the field is called in the columns table

        [ExchangeDate1]
        public DateTime? TranDate { get; set; }

        public DateTime? STranDate
        {
            get { return TranDate; }
        } // This is what the field is called in the columns table
        public string SfTranType { get; set; }

        [FeeCurrency]
        public string Moneytype { get; set; }

        public string SMoneytyp
        {
            get { return Moneytype; }
        } // This is what the field is called in the columns table

        [Currency(RecordType = RecordType.SvcFee)]
        public decimal STax1 { get; set; }

        [Currency(RecordType = RecordType.SvcFee)]
        public decimal STax2 { get; set; }

        [Currency(RecordType = RecordType.SvcFee)]
        public decimal STax3 { get; set; }

        [Currency(RecordType = RecordType.SvcFee)]
        public decimal STax4 { get; set; }

        public string Iatanbr { get; set; }

        public string SIatanbr
        {
            get { return Iatanbr; }
        } // This is what the field is called in the columns table
        public string Svccode { get; set; }
        public string Vendorcode { get; set; }
        public int SeqNo { get; set; }

        public int Seqctr { get; set; }
    }
}

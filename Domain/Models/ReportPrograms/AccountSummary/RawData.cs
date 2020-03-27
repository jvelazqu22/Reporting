using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AccountSummary
{
    public class RawData: IRecKey
    {

        public RawData()
        {
            Acct = string.Empty;
            AcctDesc = string.Empty;
            Account = string.Empty;
            AgentId = string.Empty;
            Branch = string.Empty;
            Pseudocity = string.Empty;
        }

        public int RecKey { get; set; }
        public string Acct { get; set; }
        public string AcctDesc { get; set; }
        public int PyTrips { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal PyAmt { get; set; }

        public short? plusmin { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? airchg { get; set; }

        public int CyTrips { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal CyAmt { get; set; }

        public int VarTrips
        {
            get { return CyTrips - PyTrips; }
        }

        public decimal VarAmt
        {
            get { return CyAmt - PyAmt; }
        }

        //Grouping fields
        public string Account { get; set; }
        public string AgentId { get; set; }
        public string Branch { get; set; }
        public string Pseudocity { get; set; }

        //fields that may be needed for currency conversion
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }

        [ExchangeDate2]
        public DateTime? BookDate { get; set; }

        [AirCurrency]
        public string MoneyType { get; set; }
    }
}

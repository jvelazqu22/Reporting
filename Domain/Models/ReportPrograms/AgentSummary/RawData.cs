using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AgentSummary
{
    public class RawData: IRecKey
    {
        [ExchangeDate1]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Agentid { get; set; } = string.Empty;
        public int Plusmin { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal Commission { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;
        public int RecKey { get; set; } = 0;
        public int Transacts { get; set; } = 0;
    }
}

using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AgentProductivity
{
    public class RawData : IRecKey
    {
        [ExchangeDate1]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string AgentID { get; set; } = string.Empty;
        public string Valcarr { get; set; } = string.Empty;
        public string ValcarMode { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;
        public int Trips { get; set; } = 0;
        public int RecKey { get; set; } = 0;
    }
}

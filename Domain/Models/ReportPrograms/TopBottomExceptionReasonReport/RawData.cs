using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TopBottomExceptionReasonReport
{
    public class RawData : IRecKey
    {
        public string Type { get; set; } = string.Empty;
        public string Reascode { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public int Plusmin { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;
        public decimal Basefare { get; set; } = 0m;
        public string Acct { get; set; } = string.Empty;
        public int RecKey { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        [ExchangeDate1]
        public DateTime? Invdate { get; set; }
        [ExchangeDate2]
        public DateTime Bookdate { get; set; }

        public decimal LowChg
        {
            get
            {
                return Offrdchg > 0 && Airchg < 0
                    ? 0 - Offrdchg
                    : Offrdchg == 0 ? Airchg : Offrdchg;
            }
        }
    }
}

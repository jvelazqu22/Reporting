using Domain.Helper;

namespace Domain.Models.ReportPrograms.AccountSummary
{
    [Exportable]
    public class FinalData
    {
        public FinalData()
        {
            Acct = string.Empty;
            AcctDesc = string.Empty;
        }

        public string Acct { get; set; }
        public string AcctDesc { get; set; }
        public int PyTrips { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal PyAmt { get; set; }

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
    }
}

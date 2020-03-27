using System;

namespace Domain.Models.ReportPrograms.ConcurrentSegmentsBookedReport
{
    public class RecsFromRawDataListWhereSegsIsLessThanThree
    {
        public int RecKey { get; set; }
        public string RecLoc { get; set; }
        public string Ticket { get; set; }
        public string Acct { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Bktool { get; set; }
        public DateTime? Bookdate { get; set; }
        public string Gds { get; set; }
        public decimal Airchg { get; set; }
        public DateTime? FirstDate { get; set; }
        public DateTime? LastDate { get; set; }
        public string Airline { get; set; }
        public DateTime? RDepDate { get; set; }
        public DateTime? RArrDate { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public int SeqNo { get; set; }
        public string ClassCode { get; set; }
        public int Segs { get; set; }
    }
}

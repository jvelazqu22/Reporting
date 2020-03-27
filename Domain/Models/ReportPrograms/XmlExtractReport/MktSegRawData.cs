using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.XmlExtractReport
{
    public class MktSegRawData : IRecKey
    {
        MktSegRawData()
        {
            RecKey = 0;
            SegNum = 0;
            SegOrigin = string.Empty;
            SegDest = string.Empty;
            MktSeg = string.Empty;
            MktSegBoth = string.Empty;
            MktCtry = string.Empty;
            MktReg = string.Empty;
            MktCtry2 = string.Empty;
            MktReg2 = string.Empty;
            Miles = 0;
            Stops = 0;
            Mode = string.Empty;
            Airline = string.Empty;
            FltNo = string.Empty;
            SDepdate = DateTime.MinValue;
            SDeptime = string.Empty;
            SArrdate = DateTime.MinValue;
            SArrtime = string.Empty;
            SegFare = 0.0m;
            SegMiscAmt = 0.0m;
            DitCode = string.Empty;
            Class = string.Empty;
            ClassCat = string.Empty;
            SPlusmin = 0;
        }
        public int RecKey { get; set; }
        public int SegNum { get; set; }
        public string SegOrigin { get; set; }
        public string SegDest { get; set; }
        public string MktSeg { get; set; }
        public string MktSegBoth { get; set; }
        public string MktCtry { get; set; }
        public string MktReg { get; set; }
        public string MktCtry2 { get; set; }
        public string MktReg2 { get; set; }
        public int Miles { get; set; }
        public int Stops { get; set; }
        public string Mode { get; set; }
        public string Airline { get; set; }
        public string FltNo { get; set; }
        public DateTime? SDepdate { get; set; }
        public string SDeptime { get; set; }
        public DateTime? SArrdate { get; set; }
        public string SArrtime { get; set; }
        public decimal SegFare { get; set; }
        public decimal SegMiscAmt { get; set; }
        public string DitCode { get; set; }
        public string Class { get; set; }
        public string ClassCat { get; set; }
        public int SPlusmin { get; set; }
    }
}

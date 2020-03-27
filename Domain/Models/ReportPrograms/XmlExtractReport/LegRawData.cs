using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.XmlExtractReport
{
    public class LegRawData : IRouteWhere
    {
        public LegRawData()
        {
            RecKey = 0;
            Recloc = string.Empty;
            Agency = string.Empty;
            Passlast = string.Empty;
            Passfrst = string.Empty;
            Origin = string.Empty;
            Destinat = string.Empty;
            Airline = string.Empty;
            Fltno = string.Empty;
            RDepDate = DateTime.MinValue;
            Deptime = string.Empty;
            RArrDate = DateTime.MinValue;
            Arrtime = string.Empty;
            Class = string.Empty;
            Classcat = string.Empty;
            Connect = string.Empty;
            Mode = string.Empty;
            SeqNo = 0;
            Miles = 0;
            Actfare = 0m;
            Miscamt = 0m;
            Farebase = string.Empty;
            DitCode = string.Empty;
            Agentid = string.Empty;
            Pseudocity = string.Empty;
            Branch = string.Empty;
            Rplusmin = 0;
            Tktdesig = string.Empty;
            Segnum = 0;
            Seat = string.Empty;
            Stops = string.Empty;
            Segstatus = string.Empty;
            Emailaddr = string.Empty;
            Gds = string.Empty;
        }
        public int RecKey { get; set; }
        public string Recloc { get; set; }
        public string Agency { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Airline { get; set; }
        public string Fltno { get; set; }
        public DateTime? RDepDate { get; set; }
        public string Deptime { get; set; }
        public DateTime? RArrDate { get; set; }
        public string Arrtime { get; set; }
        public string Class { get; set; }
        public string Classcat { get; set; }
        public string Connect { get; set; }
        public string Mode { get; set; }
        public int SeqNo { get; set; }
        public int Miles { get; set; }
        public decimal Actfare { get; set; }
        public decimal Miscamt { get; set; }
        public string Farebase { get; set; }
        public string DitCode { get; set; }
        public string Agentid { get; set; }
        public string Pseudocity { get; set; }
        public string Branch { get; set; }
        public int Rplusmin { get; set; }
        public string Tktdesig { get; set; }
        public int Segnum { get; set; }
        public string Seat { get; set; }
        public string Stops { get; set; }
        public string Segstatus { get; set; }
        public string Emailaddr { get; set; }
        public string Gds { get; set; }
    }

}

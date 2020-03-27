using System;

namespace Domain.Models.ReportPrograms.UserDefinedLayout
{
    public class RawData
    {
        public RawData()
        {
            Reportkey = 0;
            Crname = string.Empty;
            Crtitle = string.Empty;
            Crsubtit = string.Empty;
            Crtype = string.Empty;
            Lastused = DateTime.MinValue;
            Colname = string.Empty;
            Colorder = 0;
            Sort = 0;
            Pagebreak = false;
            Subtotal = false;
            Udidhdg1 = string.Empty;
            Udidhdg2 = string.Empty;
            Udidwidth = 0;
            Udidtype = 0;
            Horalign = string.Empty;
            Grpbreak = 0;
            Lastname = string.Empty;
            Firstname = string.Empty;
        }
        public int Reportkey { get; set; }
        public string Crname { get; set; }
        public string Crtitle { get; set; }
        public string Crsubtit { get; set; }
        public string Crtype { get; set; }
        public DateTime Lastused { get; set; }
        public string Colname { get; set; }
        public int Colorder { get; set; }
        public int Sort { get; set; }
        public bool Pagebreak { get; set; }
        public bool Subtotal { get; set; }
        public string Udidhdg1 { get; set; }
        public string Udidhdg2 { get; set; }
        public int Udidwidth { get; set; }
        public int Udidtype { get; set; }
        public string Horalign { get; set; }
        public int Grpbreak { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
    }
}

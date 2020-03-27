namespace Domain.Models.ReportPrograms.TravelerbyCountryReport
{
    public class FinalData
    {
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public decimal Tickets { get; set; } = 0m;
        public int Dispticks { get; set; } = 0;
        public int Totdays { get; set; } = 0;
        public int Longstay { get; set; } = 0;
        public decimal Ctryticks { get; set; } = 0m;
        public int Ctrydays { get; set; } = 0;
        public int Ctrylong { get; set; } = 0;
    }
}

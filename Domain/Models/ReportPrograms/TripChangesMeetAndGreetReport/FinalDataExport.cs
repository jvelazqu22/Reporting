using System;

namespace Domain.Models.ReportPrograms.TripChangesMeetAndGreetReport
{
    //The CSV and Excel Export use strings for some fields whereas Crystal/PDF export uses DateTime for the same fields. So I decided to split the final data classes for clarity
    public class FinalDataExport : FinalData
    {
        public FinalDataExport()
        {
            Rarrdate = DateTime.Now;
        }

        public string Origin { get; set; } = string.Empty;
        public string LastOrigin { get; set; } = string.Empty;
        public new string Changstamp { get; set; } = string.Empty;
        public string Changedesc2 { get; set; } = string.Empty;
        public string Changstamp2 { get; set; } = string.Empty;
        public string Changedesc3 { get; set; } = string.Empty;
        public string Changstamp3 { get; set; } = string.Empty;
    }
}

namespace Domain.Models.ReportPrograms.TripChangesAir
{
    public class FinalDataExport : FinalData
    {
        public FinalDataExport()
        {
            Changedesc = string.Empty;
            Changstamp = string.Empty;
            Changedesc2 = string.Empty;
            Changstamp2 = string.Empty;
            Changedesc3 = string.Empty;
            Changstamp3 = string.Empty;
        }

        public new string Rdepdate { get; set; }
        public string DepdateDisplay => Depdate.ToShortDateString() ?? "";
        public new string Changstamp { get; set; }
        public string Changedesc2 { get; set; }
        public string Changstamp2 { get; set; }
        public string Changedesc3 { get; set; }
        public string Changstamp3 { get; set; }

    }
}

namespace Domain.Models.ReportPrograms.TopBottomDestinations
{
    public class FinalData
    {
        public string Cat1 { get; set; } = string.Empty;
        public string Cat2 { get; set; } = string.Empty;
        public string Catdesc1 { get; set; } = string.Empty;
        public string Catdesc2 { get; set; } = string.Empty;
        public string OrgDestTemp { get; set; } = string.Empty;
        public int Trips { get; set; } = 0;
        public decimal Volume { get; set; } = 0m;
        public decimal Avgcost { get { return Trips == 0 ? 0 : Volume/Trips; } }
        public int Ytdtrips { get; set; } = 0;
        public decimal Ytdvolume { get; set; } = 0m;
        public decimal Ytdavgcost { get { return Ytdtrips == 0 ? 0 : Ytdvolume / Ytdtrips; } }
        public int Subtrips { get; set; } = 0;
        public decimal Subvolume { get; set; } = 0m;
        public decimal SubAvgcost { get { return Subtrips == 0 ? 0 : Subvolume / Subtrips; } }
        public decimal Subytdtrps { get; set; } = 0m;
        public decimal Subytdvol { get; set; } = 0m;
        public decimal SubYtdavgcost { get { return Subytdtrps == 0 ? 0 : Subytdvol / Subytdtrps; } }
        public string Mode { get; set; } = string.Empty;
    }
}

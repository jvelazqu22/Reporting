using System;

namespace Domain.Models.ViewModels
{
    public class LoadLogViewModelDisplayData
    {
        public int llrecno { get; set; }
        public DateTime loaddate { get; set; }
        public string loadtype { get; set; }
        public string sourceabbr { get; set; }
        public string sourceever { get; set; }
        public string gds_bo { get; set; }
        public string loadmsg { get; set; }
        public int triprecs { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
    }
}

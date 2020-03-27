using System.Collections.Generic;

namespace Domain.Models.ReportPrograms.MarketReport
{
    public class Carrier
    {
        public List<string> Carriers { get; set; } = new List<string>();
        public string Description { get; set; }
        public List<string> ExpandedCarriers { get; set; } = new List<string>();
    }
}

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.PassengersOnPlaneReport
{
    public class GantData : IRecKey
    {
        public GantData()
        {
            UdidText = string.Empty;
        }
        public int RecKey { get; set; }
        public string UdidText { get; set; }
    }
}

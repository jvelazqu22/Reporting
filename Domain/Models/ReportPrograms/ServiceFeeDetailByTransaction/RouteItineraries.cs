using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ServiceFeeDetailByTransaction
{
    public class RouteItineraries : IRouteItineraryInformation, IRecKey
    {
        public int RecKey { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
    }
}

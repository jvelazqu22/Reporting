using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ServiceFeeDetailByTripReport
{
    public class RouteItineraries : IRouteItineraryInformation
    {
        public int RecKey { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Connect { get; set; }
        public string Mode { get; set; }
        public string OrigOrigin { get; set; }
        public string OrigDest { get; set; }
    }
}

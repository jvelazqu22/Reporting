using Domain.Helper;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class LegSegTemp : IAirMileage
    {
       
        public int RecKey { get; set; } = 0;
        public string Connect { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string ClassCat { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
        public string Mode { get; set; } = string.Empty;
        public string Farebase { get; set; } = string.Empty;
        //from ibTripsDerivedData/hibTripsDerivedData table
        public string DerivedSegRouting { get; set; } = string.Empty;
        public string DerivedLegRouting { get; set; } = string.Empty;
        public string DerivedTransId { get; set; } = string.Empty;
        //prepare for later use on  TripClass and TripClassCat
        public string DerivedTripClass { get; set; } = string.Empty;
        public string DerivedTripClassCat { get; set; } = string.Empty;
    }
}

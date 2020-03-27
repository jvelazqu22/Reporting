using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.XmlExtract;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Shared.XMLReport.Elements
{
    public static class RailElement
    {
        public static XElement Build(XNamespace xmlns, LegRawData railLeg)
        {
            var railSegment = new XElement(xmlns + "RailSegment");

            railSegment.Add(new XElement(xmlns + "DepartureCityRailCode", railLeg.Origin.Trim().Left(3)));
            railSegment.Add(new XElement(xmlns + "DepartureTime", DateTimeFormater.FormatDate(railLeg.RDepDate, "-") + "T" + DateTimeFormater.FormatTime(railLeg.Deptime)));
            railSegment.Add(new XElement(xmlns + "ArrivalCityRailCode", railLeg.Destinat.Trim().Left(3)));
            railSegment.Add(new XElement(xmlns + "ArrivalTime", DateTimeFormater.FormatDate(railLeg.RArrDate, "-") + "T" + DateTimeFormater.FormatTime(railLeg.Arrtime)));
            railSegment.Add(new XElement(xmlns + "RailLineCode", railLeg.Airline.Trim().Left(3)));

            return railSegment;
        }
    }
}

using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.XmlExtract;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Shared.XMLReport.Elements
{
    public static class AirElement
    {
        public static XElement Build(XNamespace _xmlns, LegRawData airLeg)
        {
            var airSegment = new XElement(_xmlns + "AirSegment");

            airSegment.Add(new XElement(_xmlns + "AirlineCode", airLeg.Airline.Trim().Left(3)));
            airSegment.Add(new XElement(_xmlns + "FlightNumber", airLeg.Fltno.Trim().Left(10)));
            airSegment.Add(new XElement(_xmlns + "DepartureIATACode", airLeg.Origin.Trim().Left(3)));
            airSegment.Add(new XElement(_xmlns + "DepartureTime", DateTimeFormater.FormatDate(airLeg.RDepDate, "-") + "T" + DateTimeFormater.FormatTime(airLeg.Deptime)));
            airSegment.Add(new XElement(_xmlns + "ArrivalIATACode", airLeg.Destinat.Trim().Left(3)));
            airSegment.Add(new XElement(_xmlns + "ArrivalTime", DateTimeFormater.FormatDate(airLeg.RArrDate, "-") + "T" + DateTimeFormater.FormatTime(airLeg.Arrtime)));
            if (!string.IsNullOrEmpty(airLeg.Segstatus.Trim()))
            {
                airSegment.Add(new XElement(_xmlns + "ActionStatusCode", airLeg.Segstatus.Trim()));
            }

            return airSegment;
        }
    }
}

using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.XmlExtract;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Shared.XMLReport.Elements
{
    public static class TravelerElement
    {
        public static XElement Build(XNamespace _xmlns, RawData trip)
        {
            var traveler = new XElement(_xmlns + "Traveler");

            traveler.Add(new XElement(_xmlns + "FirstName", ValueConverter.ConvertValue(trip.Passfrst, false).PadRight(50).Left(50).Trim()));
            traveler.Add(new XElement(_xmlns + "LastName", ValueConverter.ConvertValue(trip.Passlast, false).PadRight(50).Left(50).Trim()));
            if (!string.IsNullOrEmpty(trip.Emailaddr.Trim()))
            {
                traveler.Add(new XElement(_xmlns + "EmailAddress", ValueConverter.ConvertValue(trip.Emailaddr, false)));
            }
            if (!string.IsNullOrEmpty(trip.Phone.Trim()))
            {
                traveler.Add(new XElement(_xmlns + "PhoneNumber", ValueConverter.ConvertValue(trip.Phone, false)));
            }

            return traveler;
        }
    }
}

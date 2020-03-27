using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.XmlExtract;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Shared.XMLReport.Elements
{
    public static class TravelerNameElement
    {
        public static XElement Build(XNamespace xmlns, RawData otherTraveler)
        {
            var traveler2 = new XElement(xmlns + "Traveler");

            traveler2.Add(new XElement(xmlns + "FirstName", ValueConverter.ConvertValue(otherTraveler.Passfrst, false).PadRight(50).Left(50).Trim()));
            traveler2.Add(new XElement(xmlns + "LastName", ValueConverter.ConvertValue(otherTraveler.Passlast, false).PadRight(50).Left(50).Trim()));

            return traveler2;
        }
    }
}

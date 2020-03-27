using System;
using Domain.Models.ReportPrograms.XmlExtractReport;
using System.Xml.Linq;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    public class ElementBuilder
    {
        private XNamespace _xmlns;
        public ElementBuilder(XNamespace xmlns)
        {
            _xmlns = xmlns;
        }

        public XElement BuildTravelerElement(RawData otherTraveler)
        {
            var traveler2 = new XElement(_xmlns + "Traveler");

            traveler2.Add(new XElement(_xmlns + "FirstName", ValueConverter.ConvertValue(otherTraveler.Passfrst, false).PadRight(50).Left(50).Trim()));
            traveler2.Add(new XElement(_xmlns + "LastName", ValueConverter.ConvertValue(otherTraveler.Passlast, false).PadRight(50).Left(50).Trim()));
            return traveler2;
        }
        public XElement GetSimpleXElement(string name, string val)
        {
            return string.IsNullOrEmpty(val.Trim())
                ? new XElement(_xmlns + name)
                : new XElement(_xmlns + name, val);
        }

 
    }
}

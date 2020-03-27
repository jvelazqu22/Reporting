using Domain.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    class TravelerInfoBuilder
    {
        private XNamespace _xmlns;
        private XMLReportHelper _xmlReportHelper;
        private ElementBuilder _elementBuilder;
        private ReportGlobals _globals;

        public TravelerInfoBuilder(XMLReportHelper xmlReportHelper, ElementBuilder elementBuilder, ReportGlobals globals)
        {
            _xmlns = xmlReportHelper.xmlns;
            _globals = globals;
            _xmlReportHelper = xmlReportHelper;
            _elementBuilder = elementBuilder;
        }

        public XElement Build(IXmlTravelerInfo row)
        {
            var travelerInfo = new XElement(_xmlns + "TravelerInfo");
            var traveler = new XElement(_xmlns + "Traveler");


            var currentTag = _xmlReportHelper.GetTag("TravelerName", "trv");
            if (currentTag != null)
            {
                var travelerName = new XElement(_xmlns + currentTag.ActiveName);
                travelerName.Add(new XElement(_xmlns + "FirstName", ValueConverter.ConvertValue(row.Passfrst, currentTag.Mask)));
                travelerName.Add(new XElement(_xmlns + "LastName", ValueConverter.ConvertValue(row.Passlast, currentTag.Mask)));
                traveler.Add(travelerName);
            }


            //*SPECIAL HANDLING: emailaddress IS NOT A TAG, BUT A PROPERTY OF THE<EMail> NODE. 
            //*IF THE TAG IS RENAMED, THEN INSERT EMAIL ADDRESS AS A REGULAR ELEMENT.
            //* NORMAL: < EMail EmailAddress = 'me@somewhere.com' ></ EMail >
            //*WHEN TAG IS RENAMED:
            //* < MyEmailAddrTag > me@somewhere.com </ MyEmailAddrTag >
            currentTag = _xmlReportHelper.GetTag("EMail", "trv");
            if (currentTag != null)
            {
                if (currentTag.IsRenamed)
                {
                    traveler.Add(new XElement(_xmlns + currentTag.ActiveName, ValueConverter.ConvertValue(row.Emailaddr, currentTag.Mask)));
                }
                else
                {
                    //* "NORMAL" - TAG NOT RENAMED
                    traveler.Add(new XElement(_xmlns + currentTag.ActiveName,
                        new XAttribute("EmailAddress", ValueConverter.ConvertValue(row.Emailaddr, currentTag.Mask))));
                }
            }

            currentTag = _xmlReportHelper.GetTag("Telephone", "trv");
            if (currentTag != null)
            {
                if (currentTag.IsRenamed)
                {
                    traveler.Add(new XElement(_xmlns + currentTag.ActiveName, ValueConverter.ConvertValue(row.Phone, currentTag.Mask)));
                }
                else
                {
                    //* "NORMAL" - TAG NOT RENAMED
                    traveler.Add(new XElement(_xmlns + currentTag.ActiveName,
                        new XAttribute("PhoneNumber", ValueConverter.ConvertValue(row.Phone, currentTag.Mask))));
                }
            }
            //*01 / 12 / 06 - WRITE THE TICKET NUMBER WITH THE PAX INFO.WE DO THIS
            // * BECAUSE WHEN WE HAVE MULTIPLE PAX ON THE PNR, WE NEED TO
            //*ASSOCIATE THE CORRECT TICKET WITH EACH PAX.
            _xmlReportHelper.AddElement(traveler, "TicketNumber", "Air", row.Ticket);
            _xmlReportHelper.AddElement(traveler, "Break1Value", "trv", row.Break1);
            _xmlReportHelper.AddElement(traveler, "Break2Value", "trv", row.Break2);
            _xmlReportHelper.AddElement(traveler, "Break3Value", "trv", row.Break3);

            currentTag = _xmlReportHelper.GetTag("ReportBreaks", "trv");
            if (currentTag != null)
            {
                var reportBreaks = new XElement(_xmlns + currentTag.ActiveName);

                var reportBreak1 = new XElement(_xmlns + "ReportBreak", new XAttribute("BreakNumber", 1));
                var breakKeyTag = _xmlReportHelper.GetTag("BreakKey", "trv");
                var breakValueTag = _xmlReportHelper.GetTag("BreakValue", "trv");
                if (breakKeyTag != null)
                {
                    reportBreak1.Add(_elementBuilder.GetSimpleXElement(breakKeyTag.ActiveName, ValueConverter.ConvertValue(_globals.User.Break1Name, currentTag.Mask)));
                }
                if (breakValueTag != null)
                {
                    reportBreak1.Add(_elementBuilder.GetSimpleXElement(breakValueTag.ActiveName, ValueConverter.ConvertValue(row.Break1, currentTag.Mask)));
                }
                reportBreaks.Add(reportBreak1);

                var reportBreak2 = new XElement(_xmlns + "ReportBreak", new XAttribute("BreakNumber", 2));
                if (breakKeyTag != null)
                {
                    reportBreak2.Add(_elementBuilder.GetSimpleXElement(breakKeyTag.ActiveName, ValueConverter.ConvertValue(_globals.User.Break2Name, currentTag.Mask)));
                }
                if (breakValueTag != null)
                {
                    reportBreak2.Add(_elementBuilder.GetSimpleXElement(breakValueTag.ActiveName, ValueConverter.ConvertValue(row.Break2, currentTag.Mask)));
                }
                reportBreaks.Add(reportBreak2);

                var reportBreak3 = new XElement(_xmlns + "ReportBreak", new XAttribute("BreakNumber", 3));
                if (breakKeyTag != null)
                {
                    reportBreak3.Add(_elementBuilder.GetSimpleXElement(breakKeyTag.ActiveName, ValueConverter.ConvertValue(_globals.User.Break3Name, currentTag.Mask)));
                }
                if (breakValueTag != null)
                {
                    reportBreak3.Add(_elementBuilder.GetSimpleXElement(breakValueTag.ActiveName, ValueConverter.ConvertValue(row.Break3, currentTag.Mask)));
                }
                reportBreaks.Add(reportBreak3);
                traveler.Add(reportBreaks);
            }

            travelerInfo.Add(traveler);
            return travelerInfo;
        }
    }
}

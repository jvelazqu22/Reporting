using Domain.Models.ReportPrograms.XmlExtractReport;
using Domain.Orm.Classes;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers;
using System.Linq;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    public class XMLReportHelper
    {
        public XNamespace xmlns;
        public XmlDataStructure _xmlDataStructure;
        public XMLReportHelper(XNamespace xmlns, XmlDataStructure xmlDataStructure)
        {
            this.xmlns = xmlns;
            _xmlDataStructure = xmlDataStructure;
        }
        public void AddElement(XElement element, string tag, string tagType, object value, bool forceMask = false)
        {
            var currentTag = GetTag(tag, tagType);
            if (currentTag != null)
            {
                var val = ValueConverter.ConvertValue(value, currentTag.Mask || forceMask, true);
                element.Add(!string.IsNullOrEmpty(val)
                    ? new XElement(xmlns + currentTag.ActiveName, val)
                    : new XElement(xmlns + currentTag.ActiveName));
            }
        }

        /// <summary>
        /// Adds a new attribute to a given element. 
        /// </summary>
        /// <param name="element">The parent element</param>
        /// <param name="tag">Name of the tag we are adding</param>
        /// <param name="tagType">Type of the tag (Air, Rail, Rent, etc)</param>
        /// <param name="value">the value to add. Can be a string, decimal, int, or DateTime.</param>
        public void AddAttribute(XElement element, string tag, string tagType, object value)
        {
            var currentTag = GetTag(tag, tagType);
            if (currentTag != null)
            {
                element.Add(new XAttribute(currentTag.ActiveName, ValueConverter.ConvertValue(value, currentTag.Mask)));
            }
        }
        public XmlTag GetTag(string tagname, string tagType)
        {
            var tag = _xmlDataStructure.Tags.FirstOrDefault(s => s.TagName.EqualsIgnoreCase(tagname) && s.TagType.EqualsIgnoreCase(tagType) && s.IsOn);
            return tag;
        }

        public void AddAirRailDetails(XElement details, RawData row, string tagType, bool prismMask = false)
        {
            AddElement(details, "AirChargeAmount", tagType, row.Airchg);
            AddElement(details, "OfferedFare", tagType, row.Offrdchg);
            AddElement(details, "StandardCharge", tagType, row.Stndchg);
            AddElement(details, "CommissionAmount", tagType, row.Acommisn, prismMask);
            AddElement(details, "AgentID", tagType, row.Agentid);
            AddElement(details, "FareTax", tagType, row.Faretax);
            AddElement(details, "LastUpdate", tagType, row.Lastupdate);
            AddElement(details, "SavingCode", tagType, row.Savingcode);
            AddElement(details, "Exchange", tagType, row.Exchange);
            AddElement(details, "SourceAbbreviation", tagType, row.Sourceabbr);
            AddElement(details, "CorporateAccount", tagType, row.Corpacct);
            AddElement(details, "BookTool", tagType, row.Bktool);
            AddElement(details, "AgentContact", tagType, row.AgContact);
            AddElement(details, "ChangedBy", tagType, row.Changedby);
            AddElement(details, "ChangeTimeStamp", tagType, row.Changstamp);
            AddElement(details, "ParseTimeStamp", tagType, row.Parsestamp);
            AddElement(details, "TripDepartureDate", tagType, row.Depdate);
            AddElement(details, "TripArrivalDate", tagType, row.Arrdate);
            AddElement(details, "TicketingAgent", tagType, row.Tkagent);
            AddElement(details, "BookingAgent", tagType, row.Bkagent);

            var invoiceTag = GetTag("Invoice", tagType);
            if (invoiceTag != null)
            {
                var airInvoice = new XElement(xmlns + invoiceTag.ActiveName);
                var currentTag = GetTag("TicketType", tagType);
                if (currentTag != null)
                {
                    if (tagType.EqualsIgnoreCase("Rail"))
                    {
                        airInvoice.Add(new XElement(xmlns + currentTag.ActiveName, ValueConverter.ConvertValue(row.Tickettype, currentTag.Mask)));
                    }
                    else
                    {
                        var temp = row.Tickettype.EqualsIgnoreCase("TK") ? "PaperTicket" : "ETicket";
                        airInvoice.Add(new XElement(xmlns + currentTag.ActiveName, ValueConverter.ConvertValue(temp, currentTag.Mask)));
                    }

                }

                AddElement(airInvoice, "TicketNumber", tagType, row.Ticket);
                AddElement(airInvoice, "ValidatingCarrier", tagType, row.Valcarr, prismMask);
                AddElement(airInvoice, "InvoiceNumber", tagType, row.Invoice);
                AddElement(airInvoice, "InvoiceGMTTimestamp", tagType, row.Invdate);
                AddElement(airInvoice, "IATANumber", tagType, row.Iatanbr);
                AddElement(airInvoice, "OrigTicketNumber", tagType, row.Origticket);
                AddElement(airInvoice, "InvoiceType", tagType, row.Trantype);
                details.Add(airInvoice);
            }
        }
    }
}

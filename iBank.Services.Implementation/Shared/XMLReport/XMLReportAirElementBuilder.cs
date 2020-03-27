using iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers;
using iBank.Repository.SQL.Interfaces;
using System.Xml.Linq;
using Domain.Models.ReportPrograms.XmlExtractReport;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    public class XMLReportAirElementBuilder
    {
        private XNamespace _xmlns;
        private XMLReportHelper _xmlReportHelper;
        private ElementBuilder _elementBuilder;
        private IMasterDataStore _masterStore;
        public XMLReportAirElementBuilder(XMLReportHelper xmlReportHelper, ElementBuilder elementBuilder, IMasterDataStore masterStore)
        {
            _xmlns = xmlReportHelper.xmlns;
            _masterStore = masterStore;
            _xmlReportHelper = xmlReportHelper;
            _elementBuilder = elementBuilder;
        }
        public XElement Build(LegRawData leg, RawData row, bool prismMask)
        {
            var seq = leg.SeqNo == 0 ? string.Empty : leg.SeqNo.ToString();
            var airComponent = new XElement(_xmlns + "AirComponent", new XAttribute("SequenceNumber", seq),
                new XAttribute("SupplierServiceName", row.Gds.Trim()), new XAttribute("Type", "Air"));

            _xmlReportHelper.AddElement(airComponent, "RecordLocator", "Air", row.Recloc, prismMask);
            _xmlReportHelper.AddElement(airComponent, "BookingPseudocity", "Air", row.Pseudocity);
            _xmlReportHelper.AddElement(airComponent, "Branch", "Air", row.Branch);
            _xmlReportHelper.AddElement(airComponent, "ConnectionFlag", "Air", leg.Connect);
            _xmlReportHelper.AddElement(airComponent, "DepartureAirport", "Air", leg.Origin);
            _xmlReportHelper.AddElement(airComponent, "DepartureCity", "Air", leg.Origin);
            _xmlReportHelper.AddElement(airComponent, "ArrivalAirport", "Air", leg.Destinat);
            _xmlReportHelper.AddElement(airComponent, "ArrivalCity", "Air", leg.Destinat);

            var mktTag = _xmlReportHelper.GetTag("MarketingAirline", "Air");
            if (mktTag != null)
            {
                var code = ValueConverter.ConvertValue(leg.Airline, mktTag.Mask);
                var legCarrId = ValueConverter.ConvertValue(LookupFunctions.LookupAline(_masterStore, code, "A"), mktTag.Mask || prismMask);

                airComponent.Add(new XElement(_xmlns + mktTag.ActiveName,
                    new XAttribute("Code", code), new XAttribute("Name", legCarrId)));
            }
            _xmlReportHelper.AddElement(airComponent, "FlightNumber", "Air", leg.Fltno);
            _xmlReportHelper.AddElement(airComponent, "DepartureDate", "Air", leg.RDepDate);
            _xmlReportHelper.AddElement(airComponent, "ArrivalDate", "Air", leg.RArrDate);
            _xmlReportHelper.AddElement(airComponent, "DepartureTime", "Air", DateTimeFormater.FormatTime(leg.Deptime) + ":00");
            _xmlReportHelper.AddElement(airComponent, "ArrivalTime", "Air", DateTimeFormater.FormatTime(leg.Arrtime) + ":00");
            _xmlReportHelper.AddElement(airComponent, "ClassCategory", "Air", leg.Classcat);
            _xmlReportHelper.AddElement(airComponent, "BookingClass", "Air", leg.Class);

            //Odd special case for seat number
            var seatTag = _xmlReportHelper.GetTag("Seat", "Air");
            if (seatTag != null)
            {
                var seat = ValueConverter.ConvertValue(leg.Seat, seatTag.Mask);
                if (!seatTag.IsRenamed)
                {
                    airComponent.Add(new XElement(_xmlns + "Seats",
                        new XElement(_xmlns + seatTag.ActiveName, new XAttribute("CustomerSeqno", 0),
                            new XAttribute("Characteristic", ""),
                            new XAttribute("Number", seat))));
                }
                else
                {
                    airComponent.Add(_elementBuilder.GetSimpleXElement(seatTag.ActiveName, seat));
                }
            }

            _xmlReportHelper.AddElement(airComponent, "SegmentStatus", "Air", leg.Segstatus);
            _xmlReportHelper.AddElement(airComponent, "FareBasis", "Air", leg.Farebase);
            _xmlReportHelper.AddElement(airComponent, "NumberOfStops", "Air", leg.Stops);
            _xmlReportHelper.AddElement(airComponent, "BaseFareAmount", "Air", leg.Actfare);
            _xmlReportHelper.AddElement(airComponent, "SurchargeAmount", "Air", leg.Miscamt);
            _xmlReportHelper.AddElement(airComponent, "Miles", "Air", leg.Miles);
            _xmlReportHelper.AddElement(airComponent, "TourCode", "Air", row.Tourcode, prismMask);
            _xmlReportHelper.AddElement(airComponent, "TicketDesignator", "Air", leg.Tktdesig, prismMask);
            _xmlReportHelper.AddElement(airComponent, "AccountNumber", "Air", row.Acct);
            _xmlReportHelper.AddElement(airComponent, "DITCode", "Air", leg.DitCode);
            return airComponent;
        }

    }
}

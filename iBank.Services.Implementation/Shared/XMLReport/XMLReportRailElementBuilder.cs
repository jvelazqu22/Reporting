using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    public class XMLReportRailElementBuilder
    {
        private XNamespace _xmlns;
        private XMLReportHelper _xmlReportHelper;
        private ElementBuilder _elementBuilder;
        private IMasterDataStore _masterStore;
        public XMLReportRailElementBuilder(XMLReportHelper xmlReportHelper, ElementBuilder elmentBuilder, IMasterDataStore masterStore)
        {
            _xmlns = xmlReportHelper.xmlns;
            _masterStore = masterStore;
            _xmlReportHelper = xmlReportHelper;
            _elementBuilder = elmentBuilder;
        }
        public XElement Build(LegRawData leg, RawData row)
        {
            var seq = leg.SeqNo == 0 ? string.Empty : leg.SeqNo.ToString();
            var railComponent = new XElement(_xmlns + "RailComponent", new XAttribute("SequenceNumber", seq),
                new XAttribute("SupplierServiceName", row.Gds.Trim()), new XAttribute("Type", "Rail"));

            _xmlReportHelper.AddElement(railComponent, "RecordLocator", "Rail", row.Recloc);
            _xmlReportHelper.AddElement(railComponent, "BookingPseudocity", "Rail", row.Pseudocity);
            _xmlReportHelper.AddElement(railComponent, "Branch", "Rail", row.Branch);
            _xmlReportHelper.AddElement(railComponent, "DepartureStation", "Rail", leg.Origin.PadRight(3, ' ').Left(3));
            _xmlReportHelper.AddElement(railComponent, "DepartureCity", "Rail", leg.Origin.PadRight(3, ' ').Left(3));
            _xmlReportHelper.AddElement(railComponent, "ArrivalStation", "Rail", leg.Destinat.PadRight(3, ' ').Left(3));
            _xmlReportHelper.AddElement(railComponent, "ArrivalCity", "Rail", leg.Destinat.PadRight(3, ' ').Left(3));

            var currentTag = _xmlReportHelper.GetTag("RailCode", "Rail");
            if (currentTag != null)
            {
                var code = ValueConverter.ConvertValue(leg.Airline, currentTag.Mask);
                var legCarrId = ValueConverter.ConvertValue(LookupFunctions.LookupAline(_masterStore, leg.Airline), currentTag.Mask);
                railComponent.Add(new XElement(_xmlns + currentTag.ActiveName, new XAttribute("Code", code), new XAttribute("Name", legCarrId)));

            }

            _xmlReportHelper.AddElement(railComponent, "RailNumber", "Rail", leg.Fltno);
            _xmlReportHelper.AddElement(railComponent, "DepartureDate", "Rail", leg.RDepDate);
            _xmlReportHelper.AddElement(railComponent, "ArrivalDate", "Rail", leg.RArrDate);
            _xmlReportHelper.AddElement(railComponent, "DepartureTime", "Rail", DateTimeFormater.FormatTime(leg.Deptime) + ":00");
            _xmlReportHelper.AddElement(railComponent, "ArrivalTime", "Rail", DateTimeFormater.FormatTime(leg.Arrtime) + ":00");
            _xmlReportHelper.AddElement(railComponent, "ConnectionFlag", "Rail", leg.Connect);
            _xmlReportHelper.AddElement(railComponent, "ClassCategory", "Rail", leg.Classcat);
            _xmlReportHelper.AddElement(railComponent, "BookingClass", "Rail", leg.Class);

            currentTag = _xmlReportHelper.GetTag("Seats", "Rail");
            if (currentTag != null)
            {
                //*THE LOGIC OF THE RAIL SEATS XML IS KIND OF ODD:
                //  *   < Seats Number = '23' >
                //   *     < Seat > 23 </ Seat >
                //   *   </ Seats >
                var seat = ValueConverter.ConvertValue(leg.Seat, currentTag.Mask);
                var seats = new XElement(_xmlns + "Seats", new XAttribute("Number", seat));
                var seatTag = _xmlReportHelper.GetTag("Seat", "Rail");
                if (seatTag != null)
                {
                    seats.Add(_elementBuilder.GetSimpleXElement(seatTag.ActiveName, seat));
                }
                railComponent.Add(seats);
            }

            _xmlReportHelper.AddElement(railComponent, "SegmentStatus", "Rail", leg.Segstatus);
            _xmlReportHelper.AddElement(railComponent, "FareBasis", "Rail", leg.Farebase);
            _xmlReportHelper.AddElement(railComponent, "NumberOfStops", "Rail", leg.Stops);
            _xmlReportHelper.AddElement(railComponent, "BaseFareAmount", "Rail", leg.Actfare);
            _xmlReportHelper.AddElement(railComponent, "SurchargeAmount", "Rail", leg.Miscamt);
            _xmlReportHelper.AddElement(railComponent, "Miles", "Rail", leg.Miles);
            _xmlReportHelper.AddElement(railComponent, "TourCode", "Rail", row.Tourcode);
            _xmlReportHelper.AddElement(railComponent, "TicketDesignator", "Rail", leg.Tktdesig);
            _xmlReportHelper.AddElement(railComponent, "AccountNumber", "Rail", row.Acct);
            _xmlReportHelper.AddElement(railComponent, "DITCode", "Rail", leg.DitCode);

            return railComponent;

        }
    }
}

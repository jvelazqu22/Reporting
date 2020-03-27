using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System;
using System.Linq;
using System.Xml.Linq;
using Domain.Orm.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined;

using System.Collections.Generic;

using iBank.Services.Implementation.Classes.Interfaces;
using Domain.Interfaces;
using Domain.Models.ReportPrograms.XmlExtractReport;

using iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers;
using com.ciswired.libraries.CISLogger;
using System.Reflection;

using Domain.Orm.iBankClientQueries;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;
using iBank.Services.Implementation.Shared.XMLReport;
using iBank.Services.Implementation.Shared.XMLReport.Elements;
using Domain;

namespace iBank.Services.Implementation.Shared.XmlReport
{
    public class XMLReportBuilder : IXmlReportBuilder
    {
        private static readonly ILogger Log = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private XNamespace _xmlns = "";
        private readonly ReportGlobals _globals;
        private readonly UserReportInformation _userReport;
        private XmlExtractParameters _xmlExtractParams;
        private readonly bool _isReservation;
        private XMLReportHelper _xmlReportHelper;
        private ElementBuilder _elementBuilder;
        private TravelerInfoBuilder _travelInfoBuilder;
        private XMLReportMarketSegElementBuilder _marketSegBuilder;
        private XMLReportServiceFeeElementBuilder _serviceFeeElementBuilder;
        private XMLReportRentDetailsElementBuilder _rentDetailsBuilder;
        private XMLReportAirElementBuilder _airElementBuilder;
        private XMLReportStoredFareElementBuilder _storedFareElementBuilder;
        private XMLReportStayDetailsElementBuilder _stayDetailsElementBuilder;
        private XMLReportUdidElementBuilder _udidElementBuilder;

        private XmlDataStructure _xmlDataStructure { get; set; }

        public ClientFunctions clientFunctions = new ClientFunctions();

        private IMasterDataStore _masterStore;

        private IClientDataStore _clientStore;

        public XMLReportBuilder(ReportGlobals globals, XmlDataStructure xmlDataStructure, XmlExtractParameters xmlExtractParams, bool isReservation, IMasterDataStore masterStore, IClientDataStore clientStore)
        {
            _globals = globals;
            _xmlDataStructure = xmlDataStructure;
            _xmlExtractParams = xmlExtractParams;
            _isReservation = isReservation;
            _masterStore = masterStore;
            _clientStore = clientStore;
        }
        public XMLReportBuilder(ReportGlobals globals, UserReportInformation userReport)
        {
            _globals = globals;
            _userReport = userReport;
        }

        public XElement BuildCriteria(XmlTag mainTag, String title, string format)
        {
            var criteraBuilder = new CriteriaBuilder(_globals, _xmlns);
            return criteraBuilder.Build(mainTag, title, format);
        }

        public XElement BuildColumeStructure(string nodeName, string childName)
        {
            var structure = new XElement(_xmlns + nodeName);

            structure.Add(new XElement
                    (_xmlns + childName + "0", 
                        new XAttribute("Hidden", true), 
                        new XAttribute("HeaderName", ""),
                        new XAttribute("Name", "RECKEY"),
                        new XAttribute("TableName", "TRIPS")
                    )
                );
            foreach (UserReportColumnInformation col in _userReport.Columns)
            {
                if (col.Order != 0)
                {
                    structure.Add(new XElement
                        (_xmlns + childName + col.Order.ToString(),
                            new XAttribute("HeaderName", (col.Header1 != "" && col.Header2 !="") ? 
                                                                col.Header1 + "/" + col.Header2: 
                                                                (col.Header2 == "") ? col.Header1 : col.Header2),
                            new XAttribute("Name", col.Name),
                            new XAttribute("TableName", col.TableName)
                        )
                    );
                }
            }

            return structure;
        }
          
        public XElement BuildResultRows(List<List<string>> reportRows, string nodeName, string rowName, string rowChildName, int reckeyIdx)
        {
            var resultRows = new XElement(_xmlns + nodeName);

            foreach (var row in reportRows)
            {
                resultRows.Add(BuildOneRow(row, rowName, rowChildName, reckeyIdx));
            }

            return resultRows;
        }

        public XElement BuildOneRow(List<string> rowData, string eleName, string subEleName, int definedColumnCount)
        {
            var row = new XElement(_xmlns + eleName);
            //first colume after last user defined column is reckey, move it to the first column
            row.Add(new XElement(_xmlns + subEleName + "0", rowData[definedColumnCount]));

            for (var idx = 1; idx <= definedColumnCount; idx++) 
            {
                row.Add(new XElement(_xmlns + subEleName + idx.ToString(), rowData[idx -1]));
            }
            return row;
        }

     
        public XElement BuildOrder(IXmlTripData row)
        {
            var order = new XElement(_xmlns + "Order");

            _xmlReportHelper.AddAttribute(order, "OrderIdentifier", "trp", row.Recloc);
            _xmlReportHelper.AddAttribute(order, "Entity", "trp", row.Agency);
            _xmlReportHelper.AddAttribute(order, "GDS", "trp", row.Gds);
            _xmlReportHelper.AddAttribute(order, "OrderIdentifierContext", "trp", row.Agency);
            _xmlReportHelper.AddAttribute(order, "CurrencyType", "trp", row.Moneytype);
            _xmlReportHelper.AddAttribute(order, "SourceAbbreviation", "trp", row.Sourceabbr);
            _xmlReportHelper.AddAttribute(order, "OrderChangeReason", "trp", row.Agency.Replace("UPD", "UPDATE").Replace("CANC", "DELETE"));

            return order;
        }


        public XDocument BuildXmlDocument()
        {
            Log.Info($"BuildXmlDocument");
          //  _elementBuilder = new ElementBuilder(_xmlns);
            _xmlReportHelper = new XMLReportHelper(_xmlns, _xmlDataStructure);
            _rentDetailsBuilder = new XMLReportRentDetailsElementBuilder(_xmlReportHelper, _globals);
            _elementBuilder = new ElementBuilder(_xmlns);
            _travelInfoBuilder = new TravelerInfoBuilder(_xmlReportHelper, _elementBuilder, _globals);
            _marketSegBuilder = new XMLReportMarketSegElementBuilder(_xmlReportHelper);
            _serviceFeeElementBuilder = new XMLReportServiceFeeElementBuilder(_xmlReportHelper);
            _airElementBuilder = new XMLReportAirElementBuilder(_xmlReportHelper, _elementBuilder, _masterStore);
            _storedFareElementBuilder = new XMLReportStoredFareElementBuilder(_xmlReportHelper, _elementBuilder, _masterStore, _clientStore, _globals, new ClientFunctions());
            _stayDetailsElementBuilder = new XMLReportStayDetailsElementBuilder(_xmlReportHelper,_globals);
            _udidElementBuilder = new XMLReportUdidElementBuilder(_xmlReportHelper);

            var multiPax = _globals.ParmValueEquals(WhereCriteria.DDPASSENGERXMLRECORD, "0");
            var xDoc = new XDocument();
            var iXml = new XElement(_xmlns + "iXML");
            xDoc.AddFirst(iXml);

            var critTag = _xmlReportHelper.GetTag("iBankReportCriteria", "crit");
            if (critTag != null)
            {
                iXml.Add(BuildCriteria(critTag, _xmlDataStructure.XmlDetails.Title, "iXML"));
            }

            foreach (var row in _xmlExtractParams.TripRawDataList)
            {
                if (!string.IsNullOrEmpty(row.PassNbr.Trim()) && multiPax) continue;

                var travelItinerary = new XElement(_xmlns + "TravelItinerary", BuildOrder(row));
                var iBankTravelitinerary = new XElement(_xmlns + "iBank_TravelItinerary",
                    new XAttribute("TimeStamp", DateTimeFormater.FormatDateTime(DateTime.Now, "-", ":")), new XAttribute("PrimaryLangID", "en"), new XAttribute("version", "1.2"), travelItinerary);
                iXml.Add(iBankTravelitinerary);

                var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency);
                travelItinerary.Add(BuildItineraryRef(getAllMasterAccountsQuery, row));

                if (_xmlDataStructure.DataSwitches.TrvSwitch)
                {
                    //Traveler info
                    var travelerInfoList = new XElement(_xmlns + "TravelerInfoList");

                    travelerInfoList.Add(_travelInfoBuilder.Build(row));
                    if (multiPax)
                    {
                        var row1 = row;

                        var otherPassengers = _isReservation
                            ? _xmlExtractParams.TripRawDataList.Where(s => !string.IsNullOrEmpty(s.PassNbr) &&
                                    s.RecLoc6.EqualsIgnoreCase(row1.RecLoc6) && s.Invdate == row1.Invdate &&
                                    s.Bookdate == row1.Bookdate && s.Acct == row1.Acct)
                            : _xmlExtractParams.TripRawDataList.Where(s => !string.IsNullOrEmpty(s.PassNbr) &&
                                     s.Recloc.EqualsIgnoreCase(row1.Recloc) && s.Invdate == row1.Invdate &&
                                     s.Bookdate == row1.Bookdate && s.Acct == row1.Acct);
                        foreach (var traveler in otherPassengers)
                        {
                            travelerInfoList.Add(_travelInfoBuilder.Build(traveler));
                        }
                    }

                    travelItinerary.Add(travelerInfoList);
                }

                //Get cursors of all data related to this trip
                var tripAirLegs = _xmlDataStructure.DataSwitches.AirSwitch
                    ? _xmlExtractParams.LegRawDataList.Where(s => s.RecKey == row.RecKey && s.Mode.EqualsIgnoreCase("A")).ToList()
                    : new List<LegRawData>();
                var tripRailLegs = _xmlDataStructure.DataSwitches.RailSwitch
                   ? _xmlExtractParams.LegRawDataList.Where(s => s.RecKey == row.RecKey && s.Mode.EqualsIgnoreCase("R")).ToList()
                   : new List<LegRawData>();
                var tripCars = _xmlDataStructure.DataSwitches.CarSwitch
                   ? _xmlExtractParams.CarRawDataList.Where(s => s.RecKey == row.RecKey).ToList()
                   : new List<CarRawData>();
                var tripHotels = _xmlDataStructure.DataSwitches.HotelSwitch
                   ? _xmlExtractParams.HotelRawDataList.Where(s => s.RecKey == row.RecKey).ToList()
                   : new List<HotelRawData>();
                var tripUdids = _xmlDataStructure.DataSwitches.UdidSwitch
                   ? _xmlExtractParams.UdidRawDataList.Where(s => s.RecKey == row.RecKey).ToList()
                   : new List<UdidRawData>();
                var tripSvcFees = _xmlDataStructure.DataSwitches.SvcFeeSwitch
                   ? _xmlExtractParams.SvcFeeRawDataList.Where(s => s.RecKey == row.RecKey).ToList()
                   : new List<SvcFeeRawData>();
                var tripMktSeg = _xmlDataStructure.DataSwitches.MktSegSwitch
                   ? _xmlExtractParams.MktSegRawDataList.Where(s => s.RecKey == row.RecKey).ToList()
                   : new List<MktSegRawData>();


                if (tripAirLegs.Any() || tripRailLegs.Any() || tripCars.Any() || tripHotels.Any())
                {
                    var itineraryInfo = new XElement(_xmlns + "ItineraryInfo");
                    travelItinerary.Add(itineraryInfo);
                    var reservationItems = new XElement(_xmlns + "ReservationItems");
                    itineraryInfo.Add(reservationItems);

                    if (tripAirLegs.Any() || tripRailLegs.Any())
                    {

                        var airRailDetails = new XElement(_xmlns + "AirRailDetails");

                        var nonAirInvData = _xmlDataStructure.Tags.Any(s => s.TagName.EqualsIgnoreCase("Invoice") && s.TagType.EqualsIgnoreCase("AIR"))
                            && (_xmlDataStructure.DataSwitches.HotelSwitch || _xmlDataStructure.DataSwitches.CarSwitch);

                        if (tripAirLegs.Any() || nonAirInvData)
                        {
                            airRailDetails.Add(BuildAirDetails(getAllMasterAccountsQuery, tripAirLegs, row));
                        }
                        if (tripRailLegs.Any())
                        {
                            airRailDetails.Add(BuildRailDetails(getAllMasterAccountsQuery, tripRailLegs, row));
                        }
                        reservationItems.Add(airRailDetails);

                    }
                    if (tripCars.Any())
                    {
                        reservationItems.Add(_rentDetailsBuilder.Build(tripCars, row));
                    }
                    if (tripHotels.Any())
                    {
                        reservationItems.Add(_stayDetailsElementBuilder.Build(tripHotels, row));
                    }
                    //before UDID List and after Rail Details
                    if (tripMktSeg.Any())
                    {
                        reservationItems.Add(_marketSegBuilder.Build(tripMktSeg));
                    }

                    if (tripUdids.Any() && _xmlDataStructure.Tags.Any(s => s.TagType.EqualsIgnoreCase("UDID")))
                    {
                        reservationItems.Add(_udidElementBuilder.Build(tripUdids));
                    }

                    if (tripSvcFees.Any())
                    {
                        reservationItems.Add(_serviceFeeElementBuilder.Build(tripSvcFees));
                    }

                    //user renamed udids
                    if (_xmlDataStructure.Tags.Any(s => s.TagType.EqualsIgnoreCase("UDX") && s.IsRenamed))
                    {
                        var userNamedUdids = new XElement(_xmlns + "UserNamedUdids");

                        foreach (var tag in _xmlDataStructure.Tags.Where(s => s.TagType.EqualsIgnoreCase("UDX") && s.IsRenamed).OrderBy(s => s.TagName))
                        {
                            var udidNo = tag.TagName.Replace("xUdid", string.Empty).TryIntParse(-1);
                            if (udidNo > 0)
                            {
                                var udid = tripUdids.FirstOrDefault(s => s.Udidno == udidNo);
                                userNamedUdids.Add(udid != null
                                    ? new XElement(_xmlns + tag.AlternateTagName, ValueConverter.ConvertValue(udid.UdidText, tag.Mask))
                                    : new XElement(_xmlns + tag.AlternateTagName));
                            }
                        }
                        travelItinerary.Add(userNamedUdids);
                    }
                }
            }
            
            return xDoc;

        }

        public XDocument BuildIsosXml()
        {
            Log.Info($"BuildIsosXml");

            _xmlns = "http://www.w3.org/2001/XMLSchema";

            _xmlReportHelper = new XMLReportHelper(_xmlns, _xmlDataStructure);
            var xDoc = new XDocument();

            var root = new XElement(_xmlns + "xs" + "PNRList");
            xDoc.AddFirst(root);

            foreach (var trip in _xmlExtractParams.TripRawDataList)
            {
                var pnr = new XElement(_xmlns + "PNR");

                pnr.Add(new XElement(_xmlns + "RecordLocator", trip.RecLoc6));
                pnr.Add(new XElement(_xmlns + "ClientIdentifier", trip.Sourceabbr.Trim()));

                var travelerList = new XElement(_xmlns + "TravelerList");              

                //FoxPro result doesn't seem to have this logic
                //var trip1 = trip;
                //foreach (var otherTraveler in _xmlExtractParams.TripRawDataList.Where(s => s.Recloc.EqualsIgnoreCase(trip1.Recloc) && s.RecKey != trip1.RecKey))
                //{
                //    travelerList.Add(TravelerNameElement.Build(_xmlns, otherTraveler));
                //}

                travelerList.Add(TravelerElement.Build(_xmlns, trip));
                pnr.Add(travelerList);

                //Get cursors of all data related to this trip
                var tripAirLegs = _xmlDataStructure.DataSwitches.AirSwitch
                    ? _xmlExtractParams.LegRawDataList.Where(s => s.RecKey == trip.RecKey && s.Mode.EqualsIgnoreCase("A")).ToList()
                    : new List<LegRawData>();
                var tripRailLegs = _xmlDataStructure.DataSwitches.RailSwitch
                   ? _xmlExtractParams.LegRawDataList.Where(s => s.RecKey == trip.RecKey && s.Mode.EqualsIgnoreCase("R")).ToList()
                   : new List<LegRawData>();
                var tripCars = _xmlDataStructure.DataSwitches.CarSwitch
                   ? _xmlExtractParams.CarRawDataList.Where(s => s.RecKey == trip.RecKey).ToList()
                   : new List<CarRawData>();
                var tripHotels = _xmlDataStructure.DataSwitches.HotelSwitch
                   ? _xmlExtractParams.HotelRawDataList.Where(s => s.RecKey == trip.RecKey).ToList()
                   : new List<HotelRawData>();
                var tripUdids = _xmlDataStructure.DataSwitches.UdidSwitch
                   ? _xmlExtractParams.UdidRawDataList.Where(s => s.RecKey == trip.RecKey).ToList()
                   : new List<UdidRawData>();


                if (tripAirLegs.Any() || tripRailLegs.Any() || tripCars.Any() || tripHotels.Any())
                {
                    var segmentList = new XElement(_xmlns + "SegmentList");

                    foreach (var airLeg in tripAirLegs)
                    {                        
                        segmentList.Add(AirElement.Build(_xmlns, airLeg));
                    }

                    foreach (var railLeg in tripRailLegs)
                    {
                        segmentList.Add(RailElement.Build(_xmlns, railLeg));
                    }

                    foreach (var car in tripCars)
                    {                     
                        segmentList.Add(CarElement.Build(_xmlns,car));
                    }

                    foreach (var hotel in tripHotels)
                    {
                        segmentList.Add(HotelElement.Build(_xmlns, hotel));
                    }

                    pnr.Add(segmentList);

                    var breakTag1 = _xmlReportHelper.GetTag("Break1Value", "isos");
                    var writeCustomData = breakTag1 != null && !string.IsNullOrEmpty(ValueConverter.ConvertValue(trip.Break1, breakTag1.Mask));
                    var breakTag2 = _xmlReportHelper.GetTag("Break2Value", "isos");
                    writeCustomData = writeCustomData || breakTag2 != null && !string.IsNullOrEmpty(ValueConverter.ConvertValue(trip.Break2, breakTag2.Mask));
                    var breakTag3 = _xmlReportHelper.GetTag("Break3Value", "isos");
                    writeCustomData = writeCustomData || breakTag3 != null && !string.IsNullOrEmpty(ValueConverter.ConvertValue(trip.Break3, breakTag3.Mask));

                    if (writeCustomData)
                    {
                        var custom = new XElement(_xmlns + "CustomData");

                        if (_xmlDataStructure.Tags.Any(s => s.TagType.EqualsIgnoreCase("UDX") && s.IsRenamed))
                        {
                            // var userNamedUdids = new XElement(_xmlns + "UserNamedUdids");

                            foreach (var tag in _xmlDataStructure.Tags.Where(s => s.TagType.EqualsIgnoreCase("UDX") && s.IsRenamed).OrderBy(s => s.TagName))
                            {
                                var udidNo = tag.TagName.Replace("xUdid", string.Empty).TryIntParse(-1);
                                if (udidNo > 0)
                                {
                                    var udid = tripUdids.FirstOrDefault(s => s.Udidno == udidNo);
                                    if (udid != null)
                                    {
                                        var customField = new XElement(_xmlns + "CustomField");

                                        customField.Add(new XElement(_xmlns + "Name", tag.AlternateTagName));
                                        customField.Add(new XElement(_xmlns + "Value", ValueConverter.ConvertValue(udid.UdidText, tag.Mask)));

                                        custom.Add(customField);
                                    }
                                }
                            }
                        }


                        if (breakTag1 != null && !string.IsNullOrEmpty(trip.Break1.Trim()))
                        {
                            var customField = new XElement(_xmlns + "CustomField");

                            customField.Add(new XElement(_xmlns + "Name", breakTag1.ActiveName));
                            customField.Add(new XElement(_xmlns + "Value", trip.Break1.Trim()));

                            custom.Add(customField);
                        }

                        if (breakTag2 != null && !string.IsNullOrEmpty(trip.Break2.Trim()))
                        {
                            var customField = new XElement(_xmlns + "CustomField");

                            customField.Add(new XElement(_xmlns + "Name", breakTag2.ActiveName));
                            customField.Add(new XElement(_xmlns + "Value", trip.Break2.Trim()));

                            custom.Add(customField);
                        }

                        if (breakTag3 != null && !string.IsNullOrEmpty(trip.Break3.Trim()))
                        {
                            var customField = new XElement(_xmlns + "CustomField");

                            customField.Add(new XElement(_xmlns + "Name", breakTag3.ActiveName));
                            customField.Add(new XElement(_xmlns + "Value", trip.Break3.Trim()));

                            custom.Add(customField);
                        }

                        pnr.Add(custom);

                    }
                }

                root.Add(pnr);
            }

            return xDoc;
        }
        

        public XElement BuildItineraryRef(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, RawData row)
        {
            XElement itineraryRef;
            if (Features.XmlReportBuilder.IsEnabled())
            {
                itineraryRef = new XElement(_xmlns + "ItineraryRef",
                    new XAttribute("CreationDate", DateTimeFormater.FormatDate(row.Bookdate)),
                    new XAttribute("LastModified", DateTimeFormater.FormatDate(row.Lastupdate, "-")),
                    new XAttribute("RecordLocator", ValueConverter.ConvertValue(row.Recloc, false)),
                    new XAttribute("BookDate", DateTimeFormater.FormatDate(row.Bookdate, "-")));
            }
            else
            {
                itineraryRef = new XElement(_xmlns + "ItineraryRef",
                    new XAttribute("CreationDate", DateTimeFormater.FormatDate(row.Bookdate)),
                    new XAttribute("LastModified", DateTimeFormater.FormatDate(row.Lastupdate, "-")),
                    new XAttribute("RecordLocator", ValueConverter.ConvertValue(row.Recloc, false)));
            }

            var clientIdentifierContext = new XElement(_xmlns + "ClientIdentifierContext");

            _xmlReportHelper.AddElement(clientIdentifierContext, "AgencyAccountNumber", "trp", row.Acct);
            _xmlReportHelper.AddElement(clientIdentifierContext, "AccountName", "trp", clientFunctions.LookupCname(getAllMasterAccountsQuery, row.Acct, _globals));
            _xmlReportHelper.AddElement(clientIdentifierContext, "ClientIdentifier", "trp", row.Agency);

            itineraryRef.Add(clientIdentifierContext);
            return itineraryRef;
        }
        
        public XElement BuildAirDetails(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, List<LegRawData> tripAirLegs, RawData row)
        {
            var spclNoMaskAirlines = _globals.GetParmValue(WhereCriteria.INSPECMASKINGAIRLINES);
            var specialAirlinesPresent = false;
            if (!string.IsNullOrEmpty(spclNoMaskAirlines) && _xmlDataStructure.XmlDetails.Type.EqualsIgnoreCase("PRISM"))
            {
                //* DETERMINE IF ANY AIRLINE IN THE trpLegs CURSOR IS IN THE USER'S LIST OF AIRLINES
                specialAirlinesPresent = tripAirLegs.Any(s => spclNoMaskAirlines.Contains(s.Airline.Trim()));
            }

            var airDetails = new XElement(_xmlns + "AirDetails");

            var airComponentList = new XElement(_xmlns + "AirComponentList");
            airDetails.Add(airComponentList);

            foreach (var leg in tripAirLegs)
            {
                airComponentList.Add(_airElementBuilder.Build(leg, row, specialAirlinesPresent));
            }

            var storedAirFareTag = _xmlReportHelper.GetTag("StoredAirFare", "Air");
            if (storedAirFareTag != null)
            {
                airDetails.Add(_storedFareElementBuilder.Build(getAllMasterAccountsQuery, storedAirFareTag, row, "Air"));
            }

            _xmlReportHelper.AddAirRailDetails(airDetails, row, "Air", specialAirlinesPresent);
            return airDetails;
        }

        
        private XElement BuildRailDetails(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, List<LegRawData> tripRailLegs, RawData row)
        {
            var railDetails = new XElement(_xmlns + "RailDetails");
            var railElementBuilder = new XMLReportRailElementBuilder(_xmlReportHelper, _elementBuilder, _masterStore);

            var railComponentList = new XElement(_xmlns + "RailComponentList");

            foreach (var leg in tripRailLegs)
            {
                railComponentList.Add(railElementBuilder.Build(leg, row));
            }
            railDetails.Add(railComponentList);

            var storedAirFareTag = _xmlReportHelper.GetTag("StoredRailFare", "Rail");
            if (storedAirFareTag != null)
            {
                railDetails.Add(_storedFareElementBuilder.Build(getAllMasterAccountsQuery, storedAirFareTag, row, "Rail"));
            }
            _xmlReportHelper.AddAirRailDetails(railDetails, row, "Rail");
            return railDetails;

        }          
    }
}

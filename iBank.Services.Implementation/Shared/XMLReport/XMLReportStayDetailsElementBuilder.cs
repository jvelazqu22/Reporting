using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    class XMLReportStayDetailsElementBuilder
    {
        private XNamespace _xmlns;
        private XMLReportHelper _xmlReportHelper;
        private ReportGlobals _globals;
        public XMLReportStayDetailsElementBuilder(XMLReportHelper xmlReportHelper, ReportGlobals globals)
        {
            _xmlns = xmlReportHelper.xmlns;
            _xmlReportHelper = xmlReportHelper;
            _globals = globals;
        }

        public XElement Build(List<HotelRawData> tripHotel, RawData row)
        {
            var stayDetails = new XElement(_xmlns + "StayDetails");
            var stayComponentList = new XElement(_xmlns + "StayComponentList");
            foreach (var hotel in tripHotel)
            {
                var stayComponent = new XElement(_xmlns + "StayComponent", new XAttribute("SequenceNumber", ""), new XAttribute("SupplierServiceName", row.Gds.Trim()), new XAttribute("Type", "HOTEL"));
                var reservation = new XElement(_xmlns + "Reservation");

                _xmlReportHelper.AddElement(reservation, "RecordLocator", "Stay", hotel.Recloc);
                _xmlReportHelper.AddElement(reservation, "BookingPseudocity", "Stay", row.Pseudocity);
                _xmlReportHelper.AddElement(reservation, "Branch", "Stay", row.Branch);
                _xmlReportHelper.AddElement(reservation, "NumberOfUnits", "Stay", hotel.Rooms == 0 ? 1 : hotel.Rooms);
                var roomTag = _xmlReportHelper.GetTag("Room", "Stay");
                if (roomTag != null)
                {
                    var room = new XElement(_xmlns + roomTag.ActiveName);
                    if (!string.IsNullOrEmpty(hotel.Roomtype.Trim()))
                        _xmlReportHelper.AddElement(room, "RoomTypeCode", "Stay", hotel.Roomtype);
                    reservation.Add(room);
                }
                var roomRatesTag = _xmlReportHelper.GetTag("RoomRates", "Stay");
                if (roomRatesTag != null)
                {
                    var roomRates = new XElement(_xmlns + roomRatesTag.ActiveName);
                    var roomRateTag = _xmlReportHelper.GetTag("RoomRate", "Stay");
                    if (roomRateTag != null)
                    {
                        var roomRate = new XElement(_xmlns + roomRateTag.ActiveName);
                        _xmlReportHelper.AddElement(roomRate, "RateAmount", "Stay", hotel.Bookrate);
                        _xmlReportHelper.AddElement(roomRate, "CurrencyCode", "Stay", hotel.Moneytype);
                        _xmlReportHelper.AddElement(roomRate, "RateTypeCode", "Stay", "DLY");//* IN IBANK, ALL HOTEL RATES ARE "DAILY"
                        roomRates.Add(roomRate);
                    }
                    reservation.Add(roomRates);

                }

                _xmlReportHelper.AddElement(reservation, "GuestCount", "Stay", hotel.Numguests == 0 ? 1 : hotel.Numguests);
                _xmlReportHelper.AddElement(reservation, "ConfirmationNumber", "Stay", hotel.Confirmno);

                var timeSpanTag = _xmlReportHelper.GetTag("TimeSpan", "Stay");
                if (timeSpanTag != null)
                {
                    var timeSpan = new XElement(_xmlns + timeSpanTag.ActiveName);

                    _xmlReportHelper.AddElement(timeSpan, "CheckInDate", "Stay", hotel.Datein);
                    var outDate = hotel.Nights < 1
                        ? hotel.Datein.GetValueOrDefault().AddDays(1)
                        : hotel.Datein.GetValueOrDefault().AddDays(hotel.Nights);
                    _xmlReportHelper.AddElement(timeSpan, "CheckOutDate", "Stay", outDate);

                    reservation.Add(timeSpan);
                }

                var propertyInfoTag = _xmlReportHelper.GetTag("PropertyInfo", "Stay");
                if (propertyInfoTag != null)
                {
                    var propertyInfo = new XElement(_xmlns + propertyInfoTag.ActiveName);

                    _xmlReportHelper.AddElement(propertyInfo, "HotelName", "Stay", hotel.Hotelnam);
                    _xmlReportHelper.AddElement(propertyInfo, "HotelCityCode", "Stay", hotel.Metro);
                    _xmlReportHelper.AddElement(propertyInfo, "HotelCity", "Stay", hotel.Hotcity);
                    _xmlReportHelper.AddElement(propertyInfo, "HotelState", "Stay", hotel.Hotstate);
                    _xmlReportHelper.AddElement(propertyInfo, "HotelAddress1", "Stay", hotel.HotelAddr1);
                    _xmlReportHelper.AddElement(propertyInfo, "HotelAddress2", "Stay", hotel.HotelAddr2);
                    _xmlReportHelper.AddElement(propertyInfo, "ChainCode", "Stay", hotel.Chaincod);
                    _xmlReportHelper.AddElement(propertyInfo, "PhoneNumber", "Stay", hotel.Hotphone);

                    reservation.Add(propertyInfo);
                }
                _xmlReportHelper.AddElement(reservation, "HotelExceptionRate", "Stay", hotel.Hexcprat);
                _xmlReportHelper.AddElement(reservation, "GuaranteedIndicator", "Stay", hotel.Guarante);
                _xmlReportHelper.AddElement(reservation, "TransactionType", "Stay", hotel.Hottrantyp);
                if (_globals.User.AllowAgencyReports)
                    _xmlReportHelper.AddElement(reservation, "HotelCommission", "Stay", hotel.Hcommissn);

                stayComponent.Add(reservation);
                stayComponentList.Add(stayComponent);
            }

            stayDetails.Add(stayComponentList);

            var invoiceTag = _xmlReportHelper.GetTag("Invoice", "Stay");
            if (invoiceTag != null)
            {
                var propertyInfo = new XElement(_xmlns + invoiceTag.ActiveName);

                _xmlReportHelper.AddElement(propertyInfo, "InvoiceNumber", "Stay", row.Invoice);
                _xmlReportHelper.AddElement(propertyInfo, "InvoiceDate", "Stay", row.Invdate);
                _xmlReportHelper.AddElement(propertyInfo, "IATANumber", "Stay", row.Iatanbr);

                stayDetails.Add(propertyInfo);
            }


            return stayDetails;
        }
    }
}

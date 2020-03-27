using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Services.Implementation.ReportPrograms.XmlExtract;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Shared.XMLReport.Elements
{
    public static class HotelElement
    {
        public static XElement Build(XNamespace _xmlns, HotelRawData hotel)
        {
            var hotelSegment = new XElement(_xmlns + "HotelSegment");

            hotelSegment.Add(new XElement(_xmlns + "CheckInDate", DateTimeFormater.FormatDate(hotel.Datein, "-")));
            var returnDate = hotel.Nights < 1 ? hotel.Datein.GetValueOrDefault().Date.AddDays(1) : hotel.Datein.GetValueOrDefault().Date.AddDays(hotel.Nights);
            hotelSegment.Add(new XElement(_xmlns + "CheckOutDate", DateTimeFormater.FormatDate(returnDate, "-")));

            var metro = ValueConverter.ConvertValue(hotel.Metro, false);
            metro = string.IsNullOrEmpty(metro) ? "XXX" : metro;
            hotelSegment.Add(new XElement(_xmlns + "IATACode", metro));

            if (hotel.Hotelnam.StringHasValue())
            {
                hotelSegment.Add(new XElement(_xmlns + "HotelName", ValueConverter.ConvertValue(hotel.Hotelnam.Trim(), false)));
            }

            if (hotel.Hotcity.StringHasValue() || hotel.Hotstate.StringHasValue())
            {
                var address = hotel.Hotcity.Trim() + ", " + hotel.Hotstate.Trim();
                hotelSegment.Add(new XElement(_xmlns + "Address", ValueConverter.ConvertValue(address, false)));
            }

            if (hotel.Hotphone.StringHasValue())
            {
                hotelSegment.Add(new XElement(_xmlns + "PhoneNumber", ValueConverter.ConvertValue(hotel.Hotphone.Trim(), false)));
            }

            if (hotel.Hotpropid.StringHasValue())
            {
                hotelSegment.Add(new XElement(_xmlns + "HotelIdentifier", ValueConverter.ConvertValue(hotel.Hotpropid.Trim(), false)));
            }

            return hotelSegment;
        }
    }
}

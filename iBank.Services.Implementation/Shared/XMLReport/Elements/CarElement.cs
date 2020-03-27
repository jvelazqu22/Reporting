using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.XmlExtract;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Shared.XMLReport.Elements
{
    public static class CarElement
    {
        public static XElement Build(XNamespace xmlns, CarRawData car)
        {
            var carSegment = new XElement(xmlns + "CarSegment");

            carSegment.Add(new XElement(xmlns + "PickupIATACode", car.Citycode.Trim().Left(3)));
            carSegment.Add(new XElement(xmlns + "PickupTime", DateTimeFormater.FormatDate(car.Rentdate, "-") + "T12:00:00"));
            carSegment.Add(new XElement(xmlns + "DropoffIATACode", car.Citycode.Trim().Left(3)));

            var returnDate = car.Days < 1 ? car.Rentdate.GetValueOrDefault().Date.AddDays(1) : car.Rentdate.GetValueOrDefault().Date.AddDays(car.Days);
            carSegment.Add(new XElement(xmlns + "DropoffTime", DateTimeFormater.FormatDate(returnDate, "-") + "T12:00:00"));

            carSegment.Add(new XElement(xmlns + "VendorName", car.Company.PadRight(50).Left(50).Trim()));
            return carSegment;
        }
    }
}

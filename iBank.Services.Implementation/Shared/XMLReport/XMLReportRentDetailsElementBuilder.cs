using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers;
using System.Collections.Generic;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    class XMLReportRentDetailsElementBuilder
    {
        private XNamespace _xmlns;
        private XMLReportHelper _xmlReportHelper;
        private ReportGlobals _globals;

        public XMLReportRentDetailsElementBuilder(XMLReportHelper xmlReportHelper, ReportGlobals globals)
        {
            _xmlns = xmlReportHelper.xmlns;
            _globals = globals;
            _xmlReportHelper = xmlReportHelper;
        }

        public XElement Build(List<CarRawData> tripCar, RawData row)
        {
            var rentDetails = new XElement(_xmlns + "RentDetails");
            var rentComponentList = new XElement(_xmlns + "RentComponentList");
            foreach (var car in tripCar)
            {
                var rentComponent = new XElement(_xmlns + "RentComponent", new XAttribute("SequenceNumber", ""), new XAttribute("SupplierServiceName", row.Gds.Trim()), new XAttribute("Type", "Car"));

                _xmlReportHelper.AddElement(rentComponent, "RecordLocator", "Rent", car.Recloc);
                _xmlReportHelper.AddElement(rentComponent, "BookingPseudocity", "Rent", row.Pseudocity);
                _xmlReportHelper.AddElement(rentComponent, "Branch", "Rent", row.Branch);
                _xmlReportHelper.AddElement(rentComponent, "ConfirmationNumber", "Rent", car.Confirmno);

                var currentTag = _xmlReportHelper.GetTag("Vendor", "Rent");
                if (currentTag != null)
                {
                    var code = ValueConverter.ConvertValue(car.Carcode, currentTag.Mask);
                    var name = ValueConverter.ConvertValue(car.Company, currentTag.Mask);
                    rentComponent.Add(new XElement(_xmlns + currentTag.ActiveName, new XAttribute("Code", code), new XAttribute("Name", name)));
                }


                currentTag = _xmlReportHelper.GetTag("PickLocation", "Rent");
                if (currentTag != null)
                {
                    var code = ValueConverter.ConvertValue(car.Citycode, currentTag.Mask);
                    var name = ValueConverter.ConvertValue(car.Autocity.Trim() + " " + car.Autostat.Trim(), currentTag.Mask);
                    rentComponent.Add(new XElement(_xmlns + currentTag.ActiveName, new XAttribute("Code", code), new XAttribute("Name", name)));
                }
                _xmlReportHelper.AddElement(rentComponent, "PickupDate", "Rent", car.Rentdate);

                currentTag = _xmlReportHelper.GetTag("ReturnDate", "Rent");
                if (currentTag != null)
                {
                    var returnDate = car.Rentdate;
                    if (car.Days > 1)
                    {
                        returnDate = returnDate.GetValueOrDefault().AddDays(car.Days);
                    }
                    rentComponent.Add(new XElement(_xmlns + currentTag.ActiveName, ValueConverter.ConvertValue(returnDate, currentTag.Mask)));
                }
                currentTag = _xmlReportHelper.GetTag("Vehicle", "Rent");
                if (currentTag != null)
                {
                    var vehicle = new XElement(_xmlns + currentTag.ActiveName);

                    _xmlReportHelper.AddElement(vehicle, "VehicleType", "Rent", car.Cartype);
                    _xmlReportHelper.AddElement(vehicle, "VehicleQuantity", "Rent", car.Numcars == 0 ? 1 : car.Numcars);
                    rentComponent.Add(vehicle);
                }

                currentTag = _xmlReportHelper.GetTag("RentalRate", "Rent");
                if (currentTag != null)
                {
                    var rentalRate = new XElement(_xmlns + currentTag.ActiveName);
                    currentTag = _xmlReportHelper.GetTag("RateDistance", "Rent");
                    if (currentTag != null)
                    {
                        var rateDistance = new XElement(_xmlns + currentTag.ActiveName, new XAttribute("UnitCost", ValueConverter.ConvertValue(car.Milecost, currentTag.Mask))
                            , new XAttribute("CurrencyCode", ValueConverter.ConvertValue(car.Moneytype, currentTag.Mask)));

                        _xmlReportHelper.AddElement(rateDistance, "RateRentalPeriodDescription", "Rent", car.Ratetype);
                        _xmlReportHelper.AddElement(rateDistance, "CurrencyCode", "Rent", car.Moneytype);
                        _xmlReportHelper.AddElement(rateDistance, "RateCode", "Rent", car.Reascoda);
                        _xmlReportHelper.AddElement(rateDistance, "RateAmount", "Rent", car.Abookrat);
                        rentalRate.Add(rateDistance);
                    }
                    rentComponent.Add(rentalRate);
                }
                _xmlReportHelper.AddElement(rentComponent, "AutoState", "Rent", car.Autostat);
                _xmlReportHelper.AddElement(rentComponent, "RentalExceptionRate", "Rent", car.Aexcprat);
                _xmlReportHelper.AddElement(rentComponent, "TransactionType", "Rent", car.Cartrantyp);
                if (_globals.User.AllowAgencyReports)
                    _xmlReportHelper.AddElement(rentComponent, "RentalCommission", "Rent", car.Ccommisn);
                rentComponentList.Add(rentComponent);
            }

            rentDetails.Add(rentComponentList);

            var rentInvoiceTag = _xmlReportHelper.GetTag("Invoice", "Rent");
            if (rentInvoiceTag != null)
            {
                var rentalInvoice = new XElement(_xmlns + rentInvoiceTag.ActiveName);

                _xmlReportHelper.AddElement(rentalInvoice, "InvoiceNumber", "Rent", row.Invoice);
                _xmlReportHelper.AddElement(rentalInvoice, "InvoiceDate", "Rent", row.Invdate);
                _xmlReportHelper.AddElement(rentalInvoice, "IATANumber", "Rent", row.Iatanbr);

                rentDetails.Add(rentalInvoice);
            }

            return rentDetails;
        }
    }
}

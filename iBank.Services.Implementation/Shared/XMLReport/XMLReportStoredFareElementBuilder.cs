using Domain.Models.ReportPrograms.XmlExtractReport;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers;
using iBankDomain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    public class XMLReportStoredFareElementBuilder
    {
        private XNamespace _xmlns;
        private XMLReportHelper _xmlReportHelper;
        private ElementBuilder _elementBuilder;
        private IMasterDataStore _masterStore;
        private IClientDataStore _clientStore;
        private ReportGlobals _globals;
        private ClientFunctions _clientFunctions;


        public XMLReportStoredFareElementBuilder(XMLReportHelper xmlReportHelper, ElementBuilder elementBuilder, IMasterDataStore masterStore, IClientDataStore clientStore, ReportGlobals globals, ClientFunctions clientFunctions)
        {
            _xmlns = xmlReportHelper.xmlns;
            _masterStore = masterStore;
            _clientStore = clientStore;
            _globals = globals;
            _xmlReportHelper = xmlReportHelper;
            _elementBuilder = elementBuilder;
            _clientFunctions = clientFunctions;
        }
        public XElement Build(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, XmlTag storedAirFareTag, RawData row, string tagType)
        {
            var storedAirFare = new XElement(_xmlns + storedAirFareTag.ActiveName);

            var travelTaxListTag = _xmlReportHelper.GetTag("TravelTaxList", tagType);
            if (travelTaxListTag != null)
            {
                var totTax = row.Tax1 + row.Tax2 + row.Tax3 + row.Tax4;
                //*02 / 07 / 2013 - fix a thorny tax issue -we might have a value in crsTrips.faretax but no values
                //*  in tax1, tax2, tax3, tax4.If so, then copy faretax to tax1

                if (row.Faretax > 0 && totTax == 0)
                {
                    totTax = row.Faretax;
                }
                var travelTaxList = new XElement(_xmlns + travelTaxListTag.ActiveName,
                    new XAttribute("TotalTaxAmount", ValueConverter.ConvertValue(totTax, travelTaxListTag.Mask)));
                travelTaxList.Add(new XElement(_xmlns + "TravelTax",
                    new XElement(_xmlns + "TravelTaxAmount", ValueConverter.ConvertValue(row.Tax1, travelTaxListTag.Mask)),
                    new XElement(_xmlns + "TaxCode")));
                travelTaxList.Add(new XElement(_xmlns + "TravelTax",
                    new XElement(_xmlns + "TravelTaxAmount", ValueConverter.ConvertValue(row.Tax2, travelTaxListTag.Mask)),
                    new XElement(_xmlns + "TaxCode")));
                travelTaxList.Add(new XElement(_xmlns + "TravelTax",
                    new XElement(_xmlns + "TravelTaxAmount", ValueConverter.ConvertValue(row.Tax3, travelTaxListTag.Mask)),
                    new XElement(_xmlns + "TaxCode")));
                travelTaxList.Add(new XElement(_xmlns + "TravelTax",
                    new XElement(_xmlns + "TravelTaxAmount", ValueConverter.ConvertValue(row.Tax4, travelTaxListTag.Mask)),
                    new XElement(_xmlns + "TaxCode")));


                storedAirFare.Add(travelTaxList);
            }

            if (tagType.EqualsIgnoreCase("Air"))
            {
                _xmlReportHelper.AddElement(storedAirFare, "StoredBaseFareAmount", tagType, row.Basefare);
                _xmlReportHelper.AddElement(storedAirFare, "StoredFareCurrencyCode", tagType, row.Moneytype);
            }

            _xmlReportHelper.AddElement(storedAirFare, "ReasonCode", tagType, row.Reascode);

            _xmlReportHelper.AddElement(storedAirFare, "ReasonCodeDesc", tagType, _clientFunctions.LookupReason(getAllMasterAccountsQuery, row.Reascode, row.Acct, _clientStore, _globals, _masterStore.MastersQueryDb));

            var currentTag = _xmlReportHelper.GetTag("PaymentInformation", tagType);
            if (currentTag != null)
            {
                var paymentInformation = new XElement(_xmlns + currentTag.ActiveName);

                currentTag = _xmlReportHelper.GetTag("FormOfPayment", tagType);
                if (currentTag != null)
                {
                    var formOfPayment = new XElement(_xmlns + currentTag.ActiveName);
                    _xmlReportHelper.AddElement(formOfPayment, "CardNumber", tagType, row.Cardnum.Right(row.Cardnum.Length - 2));
                    _xmlReportHelper.AddElement(formOfPayment, "CardType", tagType, row.Cardnum.Left(2));
                    paymentInformation.Add(formOfPayment);
                }
                currentTag = _xmlReportHelper.GetTag("PaymentAmount", tagType);
                if (currentTag != null)
                {
                    paymentInformation.Add(new XElement(_xmlns + currentTag.ActiveName, ValueConverter.ConvertValue(row.Airchg, currentTag.Mask)));
                }
                storedAirFare.Add(paymentInformation);
            }
            return storedAirFare;
        }
    }
}

using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers;
using System.Collections.Generic;
using System.Xml.Linq;


namespace iBank.Services.Implementation.Shared.XMLReport
{
    public class XMLReportServiceFeeElementBuilder
    {
        private XNamespace _xmlns;
        private XMLReportHelper _xmlReportHelper;
        public XMLReportServiceFeeElementBuilder(XMLReportHelper xmlReportHelper)
        {
            _xmlns = xmlReportHelper.xmlns;
            _xmlReportHelper = xmlReportHelper;
        }
        public XElement Build(List<SvcFeeRawData> tripSvcFees)
        {
            var serviceFee = new XElement(_xmlns + "ServiceFee");
            var serviceFeeDetailList = new XElement(_xmlns + "ServiceFeeDetailList");

            foreach (var svcFee in tripSvcFees)
            {
                var serviceFeeDetail = new XElement(_xmlns + "ServiceFeeDetail");

                _xmlReportHelper.AddElement(serviceFeeDetail, "ServiceFeeAmount", "Fee", svcFee.SvcFee);
                _xmlReportHelper.AddElement(serviceFeeDetail, "FeeDescription", "Fee", svcFee.SvcDesc);
                _xmlReportHelper.AddElement(serviceFeeDetail, "InvoiceNumber", "Fee", svcFee.Invoice);
                _xmlReportHelper.AddElement(serviceFeeDetail, "MCONumber", "Fee", svcFee.Mco);
                _xmlReportHelper.AddElement(serviceFeeDetail, "TransactionDate", "Fee", svcFee.Trandate);
                _xmlReportHelper.AddElement(serviceFeeDetail, "TransactionType", "Fee", svcFee.SfTranType);

                var paymentTag = _xmlReportHelper.GetTag("FormOfPayment", "Fee");
                if (paymentTag != null)
                {
                    var payment = new XElement(_xmlns + paymentTag.ActiveName);

                    _xmlReportHelper.AddElement(payment, "CreditCardNumber", "Fee", CreditCardNumber(svcFee.SfCardnum));
                    _xmlReportHelper.AddElement(payment, "CreditCardType", "Fee", CreditCardType(svcFee.SfCardnum));

                    serviceFeeDetail.Add(payment);
                }

                serviceFeeDetailList.Add(serviceFeeDetail);
            }

            serviceFee.Add(serviceFeeDetailList);
            return serviceFee;

        }

        private string CreditCardNumber(string sfCardnum)
        {
            if (!string.IsNullOrWhiteSpace(sfCardnum))
            {
                return sfCardnum.Trim().Right(sfCardnum.Trim().Length - 2);
            }
            return "";
        }

        private string CreditCardType(string sfCardnum)
        {
            if (!string.IsNullOrWhiteSpace(sfCardnum))
            {
                return sfCardnum.Trim().Left(2);
            }
            return "";
        }
    }
}

using Domain.Models.ReportPrograms.XmlExtractReport;
using System.Collections.Generic;
using System.Xml.Linq;


namespace iBank.Services.Implementation.Shared.XMLReport
{
    public class XMLReportUdidElementBuilder
    {
        private XNamespace _xmlns;
        public XMLReportHelper _xmlReportHelper;

        public XMLReportUdidElementBuilder(XMLReportHelper xmlReportHelper)
        {
            _xmlns = xmlReportHelper.xmlns;
            _xmlReportHelper = xmlReportHelper;
        }
        public XElement Build(List<UdidRawData> tripUdids)
        {
            var udidList = new XElement(_xmlns + "UDIDList");

            foreach (var udid in tripUdids)
            {
                var udids = new XElement(_xmlns + "UDIDS");

                _xmlReportHelper.AddElement(udids, "UDIDNumber", "Udid", udid.Udidno);
                _xmlReportHelper.AddElement(udids, "UDIDValue", "Udid", udid.UdidText);

                udidList.Add(udids);
            }

            return udidList;


        }
    }
}

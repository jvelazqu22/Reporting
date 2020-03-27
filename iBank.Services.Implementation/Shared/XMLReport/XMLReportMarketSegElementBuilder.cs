using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers;
using System.Collections.Generic;
using System.Xml.Linq;


namespace iBank.Services.Implementation.Shared.XMLReport
{
    class XMLReportMarketSegElementBuilder
    {
        private XNamespace _xmlns;
        public XMLReportHelper _xmlReportHelper;

        public XMLReportMarketSegElementBuilder(XMLReportHelper xmlReportHelper)
        {
            _xmlReportHelper = xmlReportHelper;
            _xmlns = xmlReportHelper.xmlns;
        }
        public XElement Build(List<MktSegRawData> mktSeg)
        {
            var topNode = new XElement(_xmlns + "MarketSegments");
            var marketSegList = new XElement(_xmlns + "MarketSegmentsList");

            foreach (var mseg in mktSeg)
            {
                var marketSeg = new XElement(_xmlns + "MarketSegmentDetail");

                _xmlReportHelper.AddElement(marketSeg, "SegNum", "MktSeg", mseg.SegNum);
                _xmlReportHelper.AddElement(marketSeg, "SegOrigin", "MktSeg", mseg.SegOrigin);
                _xmlReportHelper.AddElement(marketSeg, "SegDest", "MktSeg", mseg.SegDest);
                _xmlReportHelper.AddElement(marketSeg, "MktSeg", "MktSeg", mseg.MktSeg);
                _xmlReportHelper.AddElement(marketSeg, "MktSegBoth", "MktSeg", mseg.MktSegBoth);
                _xmlReportHelper.AddElement(marketSeg, "MktCtry", "MktSeg", mseg.MktCtry);
                _xmlReportHelper.AddElement(marketSeg, "MktReg", "MktSeg", mseg.MktReg);
                _xmlReportHelper.AddElement(marketSeg, "MktCtry2", "MktSeg", mseg.MktCtry2);
                _xmlReportHelper.AddElement(marketSeg, "MktReg2", "MktSeg", mseg.MktReg2);
                _xmlReportHelper.AddElement(marketSeg, "Miles", "MktSeg", mseg.Miles);
                _xmlReportHelper.AddElement(marketSeg, "Stops", "MktSeg", mseg.Stops);
                _xmlReportHelper.AddElement(marketSeg, "Mode", "MktSeg", mseg.Mode);
                _xmlReportHelper.AddElement(marketSeg, "Airline", "MktSeg", mseg.Airline);
                _xmlReportHelper.AddElement(marketSeg, "FltNo", "MktSeg", mseg.FltNo);
                _xmlReportHelper.AddElement(marketSeg, "SDepdate", "MktSeg", mseg.SDepdate);
                _xmlReportHelper.AddElement(marketSeg, "SArrdate", "MktSeg", mseg.SArrdate);
                _xmlReportHelper.AddElement(marketSeg, "SDeptime", "MktSeg", mseg.SDeptime);
                _xmlReportHelper.AddElement(marketSeg, "SArrtime", "MktSeg", mseg.SArrtime);
                _xmlReportHelper.AddElement(marketSeg, "SegFare", "MktSeg", mseg.SegFare);
                _xmlReportHelper.AddElement(marketSeg, "SegMiscAmt", "MktSeg", mseg.SegMiscAmt);

                marketSegList.Add(marketSeg);
            }

            topNode.Add(marketSegList);
            return topNode;
        }
    }
}

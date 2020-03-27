using Domain.Helper;
using Domain.Models.ReportPrograms.ClassOfServiceReport;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.ClassOfService
{
    public class ClassofSvcCalculations
    {
        public string GetCrystalReportName(bool showBreakByDomesticInternational, string groupBy)
        {
            var reportName = "ibClassofSvc";

            if (showBreakByDomesticInternational) reportName = "ibClassofSvcDIT";

            switch (groupBy)
            {
                case "1":
                    break;
                case "2":
                    reportName += "2";
                    break;
                default:
                    reportName += "3";
                    break;
            }

            return reportName;
        }

        public IEnumerable<FinalData> GetMatchingCarrierData(FinalData item, IList<FinalData> carrierData)
        {
            return carrierData.Where(carrier => carrier.Acct == item.Acct
                                                && carrier.Break1 == item.Break1
                                                && carrier.Break2 == item.Break2
                                                && carrier.Break3 == item.Break3
                                                && carrier.Carrname == item.Carrname
                                                && carrier.Homectry == item.Homectry
                                                && carrier.Class == item.Class);
        }

        public int GetTotalNumberOfSegsExcludingClass(FinalData item, IList<FinalData> carrierData)
        {
            return carrierData.Where(carrier => carrier.Acct == item.Acct
                                                && carrier.Break1 == item.Break1
                                                && carrier.Break2 == item.Break2
                                                && carrier.Break3 == item.Break3
                                                && carrier.Carrname == item.Carrname
                                                && carrier.Homectry == item.Homectry).ToList().Sum(x => x.Segs);
        }

        public decimal GetTotalSegsCostExcludingClass(FinalData item, IList<FinalData> carrierData)
        {
            return carrierData.Where(carrier => carrier.Acct == item.Acct
                                                && carrier.Break1 == item.Break1
                                                && carrier.Break2 == item.Break2
                                                && carrier.Break3 == item.Break3
                                                && carrier.Carrname == item.Carrname
                                                && carrier.Homectry == item.Homectry).ToList().Sum(x => x.Segcost);
        }

        public IList<string> GetExportFields(bool accountBreak, UserBreaks userBreaks, bool isGroupByAirline, bool isGroupByHomeCountry)
        {
            var fieldList = new List<string>();

            if (accountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }

            if (userBreaks.UserBreak1) fieldList.Add("break1");

            if (userBreaks.UserBreak2) fieldList.Add("break2");

            if (userBreaks.UserBreak3) fieldList.Add("break3");

            fieldList.Add("airline");
            fieldList.Add("class");
            fieldList.Add("segcost");
            fieldList.Add("segs");
            if (isGroupByAirline)
            {
                fieldList.Add("carrsegs");
                fieldList.Add("carrcost");
            }
            else if (isGroupByHomeCountry)
            {
                fieldList.Add("ctrysegs");
                fieldList.Add("ctrycost");
            }

            return fieldList;
        }
    }
}
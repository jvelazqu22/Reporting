using System;
using System.Linq;

using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Server.Utilities.ReportCriteriaHandlers
{
    public class ReportHandoffRecordHandler
    {
        public string ReportId { get; set; }
        public string UserLanguage { get; set; }
        public string CfBox { get; set; }
        public int UserNumber { get; set; }
        public string Agency { get; set; }
        public DateTime DateCreated { get; set; }

        public ReportHandoffRecordHandler(string reportId, string userLanguage, string cfBox, int userNumber, string agency)
        {
            ReportId = reportId;
            UserLanguage = userLanguage;
            CfBox = cfBox;
            UserNumber = userNumber;
            Agency = agency;
            DateCreated = DateTime.Now;
        }

        public ReportHandoffRecordHandler(string reportId, string userLanguage, string cfBox, int userNumber, string agency, DateTime dateCreated)
        {
            ReportId = reportId;
            UserLanguage = userLanguage;
            CfBox = cfBox;
            UserNumber = userNumber;
            Agency = agency;
            DateCreated = dateCreated;
        }

        public reporthandoff SetUpReportHandoffRecord(bool inParameter)
        {
            return new reporthandoff
            {
                reportid = ReportId,
                agency = Agency,
                parminout = inParameter ? "IN" : "OUT",
                parmname = "",
                parmvalue = "",
                langCode = UserLanguage,
                datecreated = DateCreated,
                cfbox = CfBox,
                usernumber = UserNumber
            };
        }

        public reporthandoff CreateReportHandoffRecord(bool inParameter, string parmnname, string parmvalue)
        {
            return new reporthandoff
            {
                reportid = ReportId,
                agency = Agency,
                parminout = inParameter ? "IN" : "OUT",
                parmname = parmnname,
                parmvalue = parmvalue,
                langCode = UserLanguage,
                datecreated = DateCreated,
                cfbox = CfBox,
                usernumber = UserNumber
            };
        }

        public static string GetReportHandoffValue(string parmName, string reportId, IMastersQueryable mastersQueryDb)
        {
            var getReportHandoffParmsByReportIdQuery = new GetReportHandoffParmsByReportIdQuery(mastersQueryDb, reportId);
            var handoffParms = getReportHandoffParmsByReportIdQuery.ExecuteQuery();

            var record = handoffParms.FirstOrDefault(x => x.parmname.Trim().Equals(parmName, StringComparison.OrdinalIgnoreCase));

            return record == null ? "" : record.parmvalue.Trim();
        }
    }
}

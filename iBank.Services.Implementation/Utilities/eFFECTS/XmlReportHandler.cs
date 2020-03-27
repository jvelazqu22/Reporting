using System;

using CODE.Framework.Core.Utilities.Extensions;

using Domain.Helper;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.eProfile;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Utilities.eFFECTS
{
    public class XmlReportHandler
    {
        public XmlReport GetXmlReportInfo(ReportGlobals globals, IClientQueryable clientQueryDb, IMastersQueryable masterQueryDb)
        {
            var exportType = "";
            var xmlReportCrName = "";
            //if this is an iXML export the export type of the iXML report must be associated with the eProfile -- 
            var reportKey = globals.GetParmValue(WhereCriteria.UDRKEY).TryIntParse(0);
            if (reportKey > 0)
            {
                //get the export type from xmluserrpts in client db
                var exportTypeQuery = new GetCustomXmlExportTypeQuery(clientQueryDb, reportKey);
                var exportRec = exportTypeQuery.ExecuteQuery();

                if (exportRec != null)
                {
                    exportType = exportRec.exportType.Trim();
                    xmlReportCrName = exportRec.crname.Trim();
                }
            }
            else
            {
                //standard report type will have a negative key
                reportKey = Math.Abs(reportKey);

                var exportTypeQuery = new GetStandardXmlExportTypeQuery(masterQueryDb, reportKey);
                var exportRec = exportTypeQuery.ExecuteQuery();

                if (exportRec != null)
                {
                    exportType = exportRec.exportType.Trim();
                    xmlReportCrName = exportRec.crname.Trim();
                }
            }

            return new XmlReport(xmlReportCrName, exportType);
        }

        public bool IsXmlReportAuthorized(string exportType, IMastersQueryable masterQueryDb, int eProfileNumber)
        {
            var isExportTypeAuthorizedQuery = new IsExportTypeAuthorizedForEProfileNumberQuery(masterQueryDb, exportType, eProfileNumber);
            return isExportTypeAuthorizedQuery.ExecuteQuery();
        }
    }
}

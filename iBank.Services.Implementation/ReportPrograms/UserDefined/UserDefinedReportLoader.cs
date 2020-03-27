using System;
using com.ciswired.libraries.CISLogger;
using Domain.Helper;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domain;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries.Agency;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public static class UserDefinedReportLoader
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static UserReportInformation LoadUserDefinedReport(ReportGlobals globals, int reportKey, IClientDataStore clientStore, IMasterDataStore masterStore)
        {
            using (var timer = new CustomTimer(globals.ProcessKey, globals.UserNumber, globals.Agency, globals.ReportLogKey, LOG, "LoadUserDefinedReport"))
            {
                var reportDefRetriever = new UserReportDefinitionRetriever(globals, new GetActiveColumnsQuery(masterStore.MastersQueryDb));

                var userReport = reportDefRetriever.LoadUserReportInformation(clientStore, reportKey);
                userReport.CarbonCalculator = globals.GetParmValue(WhereCriteria.CARBONCALC);
                userReport.Columns = reportDefRetriever.LoadUserReportColumnData(new GetUserRpt2Query(clientStore.ClientQueryDb, reportKey));

                LOG.Debug($"LoadUserDefinedReport - LoadUserReportInformation - Report Name:[{userReport.ReportName}] | Theme:[{userReport.Theme}] |  DefinedColumnCount:[{userReport.Columns.Count}]");

                userReport.DefinedColumnCount = userReport.Columns.Count;
                userReport.SuppressDuplicates = reportDefRetriever.SuppressDuplicates(userReport.Columns);

                if (userReport.Columns.Any(s => s.Name.EqualsIgnoreCase("IATANUM")))
                {
                    userReport.IataNum = new GetAgencyiATANumQuery(masterStore.MastersQueryDb, globals.Agency).ExecuteQuery();
                }

                userReport.HasServiceFee = userReport.Columns.Any(s => s.Name.EqualsIgnoreCase("SVCFEE"));

                var userReportValidator = new UserReportValidator(globals, reportKey);
                if (!userReportValidator.IsImplementedReport(userReport)) return userReport;
          
                var carbonManager = new UserDefinedReportCarbonManager(globals, userReport);
                carbonManager.SetReportCarbonProperties(clientStore.ClientQueryDb);
                LOG.Debug($"LoadUserDefinedReport - SetReportCarbonProperties");
            
                var columnBuilder = new UserDefinedColumnBuilder();
                var finalColumns = columnBuilder.BuildColumns(globals, userReport, carbonManager, new GetActiveColumnsQuery(masterStore.MastersQueryDb).ExecuteQuery());           
                SetGoodAndBadFieldTypes(finalColumns);

                RemoveGaps(finalColumns);

                userReport.Columns = finalColumns;

                userReport.AreAllColumnsInTripTables = AreAllColumnsInTripTables(reportDefRetriever, userReport);

                userReport.IsValidReport = true;

                return userReport;
            }
        }

        // this method is used to determine if we can remove duplicate rows in excel records as long as all the columns are in any of the
        // trip tables.
        private static bool AreAllColumnsInTripTables(UserReportDefinitionRetriever reportDefRetriever, UserReportInformation userReport)
        {
            // we only care to read the columns before the reckey column.
            var maxColumn = GetReckKeyColumnIndex(userReport);
            if (maxColumn == -1) maxColumn = userReport.Columns.Count;

            for (var index = 0; index < maxColumn; index++)
            {
                var collist2Record = reportDefRetriever.collist2.FirstOrDefault(w => w.colname.Trim().Equals(userReport.Columns[index].Name.Trim(), StringComparison.OrdinalIgnoreCase));
                if (collist2Record == null)
                {
                    // if the column does not have a corresponding record in the collist2 table, we need to handle each item at a time  i.e., udid_16_udid_fld
                    if (userReport.Columns[index].Name.Length >= 4 && userReport.Columns[index].Name.Trim().Substring(0, 4).Equals("udid", StringComparison.OrdinalIgnoreCase))
                        continue;

                    //if (col.Name.Trim().Equals("reckey", StringComparison.OrdinalIgnoreCase)) continue;
                    //if (col.Name.Trim().Equals("legcntr", StringComparison.OrdinalIgnoreCase)) continue;

                    return false;
                }

                if (collist2Record.coltable.Trim().ToUpper().Equals("TRIPS")) continue;
                if (collist2Record.coltable.Trim().ToUpper().Equals("IBTRIPS")) continue;
                if (collist2Record.coltable.Trim().ToUpper().Equals("HIBTRIPS")) continue;
                if (collist2Record.coltable.Trim().ToUpper().Equals("ACCTSPCL")) continue;
                if (collist2Record.coltable.Trim().ToUpper().Equals("TRIPTLS")) continue;
                if (collist2Record.coltable.Trim().ToUpper().Equals("ONDTRIPS")) continue;

                return false;
            }

            return true;
        }

        private static int GetReckKeyColumnIndex(UserReportInformation userReport)
        {
            for (var columnRecKeyIndex = 0; columnRecKeyIndex < userReport.Columns.Count; columnRecKeyIndex++)
            {
                if (userReport.Columns[columnRecKeyIndex].Name.Equals("reckey", StringComparison.OrdinalIgnoreCase))
                {
                    return columnRecKeyIndex;
                }
            }
            return -1;
        }

        private static void SetGoodAndBadFieldTypes(List<UserReportColumnInformation> finalColumns)
        {
            foreach (var column in finalColumns)
            {
                if (column.Name.Equals(column.GoodField))
                {
                    column.GoodFieldType = column.ColumnType.Trim();
                }
                else
                {
                    var tempCol = finalColumns.FirstOrDefault(s => s.Name.Equals(column.GoodField));
                    if (tempCol != null)
                    {
                        column.GoodFieldType = tempCol.ColumnType.Trim();
                    }
                }
                if (column.Name.Equals(column.BadField))
                {
                    column.BadFieldType = column.ColumnType.Trim();
                }
                else
                {
                    var tempCol = finalColumns.FirstOrDefault(s => s.Name.Equals(column.BadField));
                    if (tempCol != null)
                    {
                        column.BadFieldType = tempCol.ColumnType.Trim();
                    }
                }
            }
        }

        private static void RemoveGaps(List<UserReportColumnInformation> finalColumns)
        {
            //Verify that there are no gaps in the breaks. If there are more than 3, remove the extras.
            var brkCount = 0;
            foreach (var col in finalColumns.Where(s => s.GroupBreak > 0).OrderBy(s => s.GroupBreak))
            {
                if (brkCount <= 3)
                {
                    brkCount++;
                    col.GroupBreak = brkCount;
                }
                else
                {
                    col.GroupBreak = 0;
                }
            }
        }
    }
}

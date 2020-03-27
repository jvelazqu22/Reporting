using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts
{
    public class UdidSqlBuilder
    {
        private static readonly WhereClauseWithAdvanceParamsHandler whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();

        //Use buildWhere and default iBank table laison
        //T1: ibtrips or hibtrips
        //T2: iblegs or hilegs
        //T3: ibudids or hibudids
        //T4: ibcar or hibcars
        //T5: ibhotel or hibhotel etc
        public string GetSql(bool isReservation, SwitchManager switchManager, string whereClause, BuildWhere buildWhere)
        {
            var sql = new SqlScript();

            //make sure both Udidnumber and UdidNo are populated, so UdidRawData and UdidInformation will both work
            sql.FieldList = "T1.reckey, convert(int, udidno) as udidnumber, convert(int, udidno) as UdidNo, udidtext ";
            sql.FromClause = ComputeFromClause(isReservation, switchManager, buildWhere.ReportGlobals);
            sql.KeyWhereClause = ComputeKeyWhereClause(switchManager, buildWhere.ReportGlobals);

            sql.WhereClause = $"{sql.KeyWhereClause} {whereClause}";

            sql.WhereClause = whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, true);

            return SqlProcessor.ProcessSql(sql.FieldList, false, sql.FromClause, sql.WhereClause, sql.OrderBy, buildWhere.ReportGlobals);
        }

        private string ComputeFromClause(bool isReservation, SwitchManager switchManager, ReportGlobals globals)
        {
            var sqlFrom = isReservation ? "ibTrips T1, ibUdids T3" : "hibTrips T1, hibUdids T3";
            if (switchManager.HotelSwitch && globals.ProcessKey == (int)ReportTitles.HotelUserDefinedReports) sqlFrom = isReservation ? $"{sqlFrom}, ibhotel T5" : $"{sqlFrom}, hibhotel T5";
            if (switchManager.CarSwitch && globals.ProcessKey == (int)ReportTitles.CarUserDefinedReports) sqlFrom = isReservation ? $"{sqlFrom}, ibcar T4" : $"{sqlFrom}, hibcars T4";

            return sqlFrom;
        }

        private string ComputeKeyWhereClause(SwitchManager switchManager, ReportGlobals globals)
        {
            var sqlKeyWhere = $"T1.reckey = T3.reckey ";
            if (switchManager.HotelSwitch && globals.ProcessKey == (int)ReportTitles.HotelUserDefinedReports) sqlKeyWhere = $"{sqlKeyWhere} and T1.reckey = T5.reckey ";
            if (switchManager.CarSwitch && globals.ProcessKey == (int)ReportTitles.CarUserDefinedReports) sqlKeyWhere = $"{sqlKeyWhere} and T1.reckey = T4.reckey ";

            sqlKeyWhere += " and ";

            return sqlKeyWhere;
        }
    }
}

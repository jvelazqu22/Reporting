using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;
using iBank.Services.Implementation.Utilities.CurrencyConversion;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.DataSetHandler
{
    public class MiscSegmentDataSetHandler
    {
        public List<MiscSegSharedRawData> GetMiscSegmentData(string segType, string whereClause, bool tripTlsSwitch, bool udidExists, bool isReservation, BuildWhere buildWhere,
            ReportGlobals globals, string moneyType)
        {
            var miscSegData = GetData(segType, whereClause, tripTlsSwitch, udidExists, isReservation, buildWhere, globals);

            ConvertCurrency(miscSegData, moneyType);

            return miscSegData;
        }

        private List<MiscSegSharedRawData> GetData(string segType, string whereClause, bool tripTlsSwitch, bool udidExists, bool isReservation, BuildWhere buildWhere,
                                                   ReportGlobals globals)
        {
            var miscSqlScript = new MiscSegSharedSqlScript { SegType = segType };
            var sql = miscSqlScript.GetSqlScript(tripTlsSwitch, udidExists, isReservation, whereClause);
            return ClientDataRetrieval.GetRawData<MiscSegSharedRawData>(sql, isReservation, buildWhere, globals, false).ToList();
        }

        private void ConvertCurrency(List<MiscSegSharedRawData> miscSegData, string moneyType)
        {
            if (miscSegData.Select(s => s.Moneytype).Distinct().Any(s => s != moneyType && moneyType != ""))
            {
                var cc = new CurrencyConverter();
                cc.ConvertCurrency(miscSegData, moneyType);
            }
        }
    }
}

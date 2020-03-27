using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;
using iBank.Services.Implementation.Utilities.CurrencyConversion;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.DataSetHandler
{
    public class MiscDataSetHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<MiscRawData> GetMiscData(string whereClause, bool tripTlsSwitch, bool udidExists, bool isReservation, BuildWhere buildWhere,
            ReportGlobals globals, string moneyType)
        {
            var miscData = GetData(whereClause, tripTlsSwitch, udidExists, isReservation, buildWhere, globals);

            ConvertCurrency(miscData, moneyType);

            LOG.Debug($"Misc Count:{miscData.Count}");

            return miscData;
        }

        private List<MiscRawData> GetData(string whereClause, bool tripTlsSwitch, bool udidExists, bool isReservation, BuildWhere buildWhere, ReportGlobals globals)
        {
            var miscSqlScript = new MiscDataSqlScript();
            var sql = miscSqlScript.GetSqlScript(tripTlsSwitch, udidExists, isReservation, whereClause);
            return ClientDataRetrieval.GetRawData<MiscRawData>(sql, isReservation, buildWhere, globals, false).ToList();
        }

        private void ConvertCurrency(List<MiscRawData> miscData, string moneyType)
        {
            if (miscData.Select(s => s.Moneytype).Distinct().Any(s => s != moneyType && moneyType != ""))
            {
                var cc = new CurrencyConverter();
                cc.ConvertCurrency(miscData, moneyType);
            }
        }
    }
}

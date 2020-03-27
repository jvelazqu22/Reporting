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
    public class MarketSegDataSetHandler
    {
        public List<MarketSegmentRawData> GetMarketSegmentData(string whereClause, bool tripTlsSwitch, bool udidExists, bool isReservation, BuildWhere buildWhere, 
            ReportGlobals globals, string moneyType)
        {
            var marketData = GetData(whereClause, tripTlsSwitch, udidExists, isReservation, buildWhere, globals);

            ConvertCurrency(marketData, moneyType);

            return marketData;
        }

        private List<MarketSegmentRawData> GetData(string whereClause, bool tripTlsSwitch, bool udidExists, bool isReservation, BuildWhere buildWhere,
                                                   ReportGlobals globals)
        {
            var sqlScript = new MarketSegmentSqlScript();
            var sql = sqlScript.GetSqlScript(tripTlsSwitch, udidExists, isReservation, whereClause);
            return ClientDataRetrieval.GetRawData<MarketSegmentRawData>(sql, isReservation, buildWhere, globals, false).ToList();
        }

        private void ConvertCurrency(List<MarketSegmentRawData> marketData, string moneyType)
        {
            if (marketData.Select(s => s.Moneytype).Distinct().Any(s => s != moneyType && moneyType != ""))
            {
                var cc = new CurrencyConverter();
                cc.ConvertCurrency(marketData, moneyType);
            }
        }
    }
}

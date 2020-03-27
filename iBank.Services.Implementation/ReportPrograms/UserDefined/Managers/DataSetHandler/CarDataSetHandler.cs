using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.CarbonCalculations;
using iBank.Services.Implementation.Utilities.ClientData;
using iBank.Services.Implementation.Utilities.CurrencyConversion;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.DataSetHandler
{
    public class CarDataSetHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly bool _isReservationReport;

        private readonly ReportGlobals _globals;

        private readonly BuildWhere _buildWhere;

        public CarDataSetHandler(bool isReservationReport, ReportGlobals globals, BuildWhere buildWhere)
        {
            _isReservationReport = isReservationReport;
            _globals = globals;
            _buildWhere = buildWhere;
        }

        public List<CarRawData> GetCarData(string whereClause, bool tripTlsSwitch, bool udidExists, string moneyType, UserReportInformation userReport)
        {
            var carData = GetData(tripTlsSwitch, udidExists, whereClause);

            ConvertCurrency(carData, moneyType);

            SetCarbon(userReport, carData);

            LOG.Debug($"CarData Count:{carData.Count}");

            return carData;
        }

        private List<CarRawData> GetData(bool tripTlsSwitch, bool udidExists, string whereClause)
        {
            var carDataManager = new CarDataSqlScript();
            var sql = carDataManager.GetSqlScript(tripTlsSwitch, udidExists, _isReservationReport, whereClause);
            return ClientDataRetrieval.GetRawData<CarRawData>(sql, _isReservationReport, _buildWhere, _globals, false).ToList();
        }

        private void ConvertCurrency(List<CarRawData> carData, string moneyType)
        {
            if (carData.Select(s => s.Moneytype).Distinct().Any(s => s != moneyType && moneyType != ""))
            {
                var cc = new CurrencyConverter();
                cc.ConvertCurrency(carData, moneyType);
            }
        }

        private void SetCarbon(UserReportInformation userReport, List<CarRawData> carData)
        {
            if (userReport.HasCarbonFields && (userReport.Columns.Select(s => s.Name).Contains("CARCO2") || userReport.Columns.Select(s => s.Name).Contains("TRIPCO2")))
            {
                var useMetric = _globals.IsParmValueOn(WhereCriteria.METRIC);
                var carbonCalc = new CarbonCalculator();
                carbonCalc.SetCarCarbon(carData, useMetric, _isReservationReport);
            }
        }
    }
}

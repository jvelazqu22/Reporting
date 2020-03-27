using System.Collections.Generic;
using System.Linq;

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
    public class HotelDataSetHandler
    {
        public List<HotelRawData> GetHotelData(string whereClause, bool tripTlsSwitch, bool udidExists, bool isReservationReport, BuildWhere buildWhere, ReportGlobals globals,
            UserReportInformation userReport, string moneyType)
        {
            var hotelData = GetData(tripTlsSwitch, udidExists, isReservationReport, whereClause, buildWhere, globals);

            OrderHotelDataSegCtr(hotelData);

            ConvertCurrency(hotelData, moneyType);

            SetCarbon(userReport, hotelData, globals, isReservationReport);

            return hotelData;
        }

        private List<HotelRawData> GetData(bool tripTlsSwitch, bool udidExists, bool isReservationReport, string whereClause, BuildWhere buildWhere, ReportGlobals globals)
        {
            var hotelDataManager = new HotelDataSqlScript();
            var sql = hotelDataManager.GetSqlScript(tripTlsSwitch, udidExists, isReservationReport, whereClause);
            return ClientDataRetrieval.GetRawData<HotelRawData>(sql, isReservationReport, buildWhere, globals, false).ToList();
        }
        
        private void OrderHotelDataSegCtr(List<HotelRawData> hotelData)
        {
            var idx = 0;
            var reckey = 0;
            foreach (var rec in hotelData)
            {
                if (reckey != rec.RecKey) idx = 0; //each recloc has it's own order starts 0
                rec.Seqctr = idx;
                reckey = rec.RecKey;
                idx++;
            }
        }

        private void ConvertCurrency(List<HotelRawData> hotelData, string moneyType)
        {
            if (hotelData.Select(s => s.Moneytype).Distinct().Any(s => s != moneyType && moneyType != ""))
            {
                var cc = new CurrencyConverter();
                cc.ConvertCurrency(hotelData, moneyType);
            }
        }

        private void SetCarbon(UserReportInformation userReport, List<HotelRawData> hotelData, ReportGlobals globals, bool isReservationReport)
        {
            if (userReport.HasCarbonFields && (userReport.Columns.Select(s => s.Name).Contains("HOTELCO2") || userReport.Columns.Select(s => s.Name).Contains("TRIPCO2")))
            {
                var carbonCalc = new CarbonCalculator();
                carbonCalc.SetHotelCarbon(hotelData, globals.IsParmValueOn(WhereCriteria.METRIC), isReservationReport);
            }
        }
    }
}

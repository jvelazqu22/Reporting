using Domain.Helper;

using iBank.Server.Utilities.Classes;

using System.Linq;
using System.Collections.Generic;

using Domain.Orm.iBankClientQueries;

using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class UserDefinedReportCarbonManager
    {
        private readonly ReportGlobals _globals;
        private readonly UserReportInformation _userReport;
        public UserDefinedReportCarbonManager(ReportGlobals globals, UserReportInformation userReport)
        {
            _globals = globals;
            _userReport = userReport;
        }

        public bool IsCarbonColumn(string name)
        {
            if (UserReportCheckLists.TripCarbonColumns.Contains(name)) return true;
            if (UserReportCheckLists.AirCarbonColumns.Contains(name)) return true;
            if (UserReportCheckLists.CarCarbonColumns.Contains(name)) return true;
            if (UserReportCheckLists.CarCarbonColumns.Contains(name)) return true;
            if (UserReportCheckLists.AltRailCarbonColumns.Contains(name)) return true;
            if (UserReportCheckLists.AltCarbonColumns.Contains(name)) return true;
            return false;
        }

        public void SetReportCarbonProperties(IClientQueryable clientQueryDb)
        {
            if (!_globals.ParmHasValue(WhereCriteria.CARBONCALC))
            {
                SetDefaultCarbCalculator(_userReport, clientQueryDb);
                _userReport.CarbonCalculator = _globals.GetParmValue(WhereCriteria.CARBONCALC);
            }

            _userReport.HasTripCarbon = CheckCarbColumns(UserReportCheckLists.TripCarbonColumns, _userReport, _globals);
            _userReport.HasAirCarbon = CheckCarbColumns(UserReportCheckLists.AirCarbonColumns, _userReport, _globals);
            _userReport.HasCarCarbon = CheckCarbColumns(UserReportCheckLists.CarCarbonColumns, _userReport, _globals);
            _userReport.HasHotelCarbon = CheckCarbColumns(UserReportCheckLists.HotelCarbonColumns, _userReport, _globals);
            _userReport.HasAltRailCarbon = CheckCarbColumns(UserReportCheckLists.AltRailCarbonColumns, _userReport, _globals);
            _userReport.HasAltCarCarbon = CheckCarbColumns(UserReportCheckLists.AltCarbonColumns, _userReport, _globals);
        }
        
        private void SetDefaultCarbCalculator(UserReportInformation userReport, IClientQueryable clientQueryDb)
        {
            var carbCalc = new GetUserDefaultCarbCalculatorQuery(clientQueryDb, _globals.User.UserNumber).ExecuteQuery();

            if (!string.IsNullOrEmpty(carbCalc))
            {
                _globals.SetParmValue(WhereCriteria.CARBONCALC, userReport.CarbonCalculator);
            }
        }

        /// <summary>
        /// looks for carbon calc columns, and removes them if there is no carb calculator specified. 
        /// </summary>
        /// <param name="colnames">A list of carbon calculator columns</param>
        /// <param name="userReport"></param>
        /// <param name="globals"></param>
        /// <returns>True if columns are found, and there is a carbon calculator specified.</returns>
        private static bool CheckCarbColumns(List<string> colnames, UserReportInformation userReport, ReportGlobals globals)
        {
            var colsFound = userReport.Columns.Where(s => colnames.Contains(s.Name)).ToList();
            if (colsFound.Any())
            {
                if (!globals.ParmHasValue(WhereCriteria.CARBONCALC))
                {
                    foreach (var column in colsFound)
                    {
                        userReport.Columns.Remove(column);
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}

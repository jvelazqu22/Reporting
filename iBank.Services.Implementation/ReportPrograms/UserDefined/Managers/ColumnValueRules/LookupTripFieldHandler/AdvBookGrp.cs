using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using System;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class AdvBookGrp : IColumnValue
    {
        private RawData _mainRec;
        private ReportGlobals _globals;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _globals = colValRulesParams.Globals;
        }

        public string CalculateColValue()
        {
            var defaultDate = new DateTime(1900, 1, 1, 0, 0, 0);
            if (_globals.Agency.Equals("GSA") && (defaultDate == _mainRec.Bookdate || defaultDate == _mainRec.DepDate.GetValueOrDefault()))
            {
                return string.Empty;
            }
            else
            {
                //Difference between Booking Date and Ticket Departure Date Grouped Into 0-2 Days, 3-6 Days, 7-13 Days, 14-20 Days, 21+ Days
                return LookupFunctions.LookupTwoDateGroup(_mainRec.Bookdate.GetValueOrDefault(), _mainRec.DepDate.GetValueOrDefault());
            }
        }
    }
}

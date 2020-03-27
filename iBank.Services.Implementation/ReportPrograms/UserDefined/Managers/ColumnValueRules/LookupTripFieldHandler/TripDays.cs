using System;
using System.Globalization;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class TripDays : IColumnValue
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
            if (_mainRec.Arrdate == null) return "0";
            var defaultDate = new DateTime(1900, 1, 1, 0, 0, 0);
            if (_globals.Agency.Equals("GSA") && (defaultDate == _mainRec.Arrdate || defaultDate == _mainRec.DepDate.GetValueOrDefault()))
            {
                return string.Empty;
            }
            else
            {
                var duration = ((_mainRec.Arrdate.Value.AddDays(1) - _mainRec.DepDate.GetValueOrDefault()).Days * _mainRec.PlusMin);
                return duration > 1000 ? "0" : duration.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}

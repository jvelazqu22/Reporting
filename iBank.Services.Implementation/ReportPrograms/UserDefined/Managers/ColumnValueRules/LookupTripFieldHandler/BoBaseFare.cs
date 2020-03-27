using CODE.Framework.Core.Utilities;
using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class BoBaseFare : IColumnValue
    {
        private RawData _mainRec;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            //Base Fare Breakout of the Segment Before Taxes
            return _mainRec.Basefare.ToStringSafe();
        }
    }
}

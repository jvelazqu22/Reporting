using System;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    public class ExecSummaryWhereClauseTransformer
    {
        public string TransformWhereClause(string whereClause, SqlType sqlType)
        {
            switch (sqlType)
            {
                case SqlType.Air:
                case SqlType.Leg:
                    return whereClause;
                case SqlType.Car:
                    return whereClause.Replace("T1.trantype", "T4.CarTranTyp");
                case SqlType.Hotel:
                    return whereClause.Replace("T1.trantype", "T5.HotTranTyp");
                case SqlType.SvcFee:
                    return whereClause.Replace("T1.trantype", "T6A.sfTrantype");
                default:
                    throw new ArgumentOutOfRangeException(nameof(sqlType), sqlType, null);
            }
        }

        public enum SqlType
        {
            Air,
            Car,
            Hotel,
            SvcFee,
            Leg
        }
    }
}
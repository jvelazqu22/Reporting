using Domain.Models;
using System;

using iBankDomain.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Factories
{
    public class SqlScriptFactory : IFactory<SqlScript>
    {
        public DataTypes.DataType DataType { get; set; }
        public bool IsPreviewReport { get; set; }
        public string WhereClause { get; set; }
        public string DateRangeType { get; set; }
        public bool IsValidUdid { get; set; }

        public SqlScriptFactory(DataTypes.DataType dataType, bool isPreviewReport, string whereClause, string dateRangeType, bool isValidUdid)
        {
            DataType = dataType;
            IsPreviewReport = isPreviewReport;
            WhereClause = whereClause;
            DateRangeType = dateRangeType;
            IsValidUdid = isValidUdid;
        }

        public SqlScript Build()
        {
            switch (DataType)
            {
                case DataTypes.DataType.Air:
                    return IsPreviewReport
                                ? BuildAirReservationDataScript()
                                : BuildAirHistoryDataScript();
                case DataTypes.DataType.Car:
                    return IsPreviewReport
                               ? BuildCarReservationDataScript()
                               : BuildCarHistoryDataScript();
                case DataTypes.DataType.Hotel:
                    return IsPreviewReport
                               ? BuildHotelReservationDataScript()
                               : BuildHotelHistoryDataScript();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private SqlScript BuildHotelHistoryDataScript()
        {
            var replacementPhrase = "T5.HotTranTyp";
            var sqlScript = new SqlScript();
            sqlScript.FieldList = string.Format("{0} as datecomp, invdate, depdate, BookDate, convert(int,hplusmin) as hplusmin, convert(int,nights) as nights, convert(int,rooms) as rooms, bookrate, reascodh, hexcprat", DateRangeType);

            if (IsValidUdid)
            {
                sqlScript.FromClause = "hibtrips T1, hibudids T3, hibhotel T5";
                sqlScript.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T5.reckey and ";
                sqlScript.WhereClause = string.Format(sqlScript.KeyWhereClause + "{0}", ReplaceTranType(WhereClause, replacementPhrase));
            }
            else
            {
                sqlScript.FromClause = "hibtrips T1, hibhotel T5";
                sqlScript.KeyWhereClause = "T1.reckey = T5.reckey and ";
                sqlScript.WhereClause = string.Format(sqlScript.KeyWhereClause + "{0}", ReplaceTranType(WhereClause, replacementPhrase));
            }

            return sqlScript;
        }

        private SqlScript BuildHotelReservationDataScript()
        {
            var sqlScript = new SqlScript();
            sqlScript.FieldList = string.Format("{0} as datecomp, invdate, depdate, BookDate, convert(int,1) as hplusmin, nights, rooms, bookrate, reascodh, hexcprat", DateRangeType);

            if (IsValidUdid)
            {
                sqlScript.FromClause = "ibtrips T1, ibudids T3, ibhotel T5";
                sqlScript.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T5.reckey and ";
                sqlScript.WhereClause = string.Format(sqlScript.KeyWhereClause + "{0}", WhereClause);
            }
            else
            {
                sqlScript.FromClause = "ibtrips T1, ibhotel T5";
                sqlScript.KeyWhereClause = "T1.reckey = T5.reckey and ";
                sqlScript.WhereClause = string.Format(sqlScript.KeyWhereClause + "{0}", WhereClause);
            }

            return sqlScript;
        }

        private SqlScript BuildCarHistoryDataScript()
        {
            var replacementPhrase = "T4.CarTranTyp";
            var sqlScript = new SqlScript();
            sqlScript.FieldList = string.Format("{0} as datecomp, invdate, depdate, BookDate, convert(int,cplusmin) as cplusmin, days, abookrat, reascoda, aexcprat", DateRangeType);

            if (IsValidUdid)
            {
                sqlScript.FromClause = "hibtrips T1, hibudids T3, hibcars T4";
                sqlScript.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T4.reckey and ";
                sqlScript.WhereClause = string.Format(sqlScript.KeyWhereClause + "{0}", ReplaceTranType(WhereClause, replacementPhrase));
            }
            else
            {
                sqlScript.FromClause = "hibtrips T1, hibcars T4";
                sqlScript.KeyWhereClause = "T1.reckey = T4.reckey and ";
                sqlScript.WhereClause = string.Format(sqlScript.KeyWhereClause + "{0}", ReplaceTranType(WhereClause, replacementPhrase));
            }

            return sqlScript;
        }

        private static string ReplaceTranType(string whereClause, string replacement)
        {
            return whereClause.Replace("T1.trantype", replacement);
        }

        private SqlScript BuildCarReservationDataScript()
        {
            var sqlScript = new SqlScript();
            sqlScript.FieldList = string.Format("{0} as datecomp, invdate, depdate, BookDate, convert(int,1) as cplusmin, days, abookrat, reascoda, aexcprat", DateRangeType);

            if (IsValidUdid)
            {
                sqlScript.FromClause = "ibtrips T1, ibudids T3, ibcar T4";
                sqlScript.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T4.reckey and ";
                sqlScript.WhereClause = string.Format(sqlScript.KeyWhereClause + "{0}", WhereClause);
            }
            else
            {
                sqlScript.FromClause = "ibtrips T1, ibcar T4";
                sqlScript.KeyWhereClause = "T1.reckey = T4.reckey and ";
                sqlScript.WhereClause = string.Format(sqlScript.KeyWhereClause + "{0}", WhereClause);
            }

            return sqlScript;
        }

        private SqlScript BuildAirHistoryDataScript()
        {
            var sqlScript = new SqlScript();
            sqlScript.FieldList = string.Format("{0} as datecomp, invdate, depdate, BookDate, convert(int,plusmin) as plusmin, reascode, savingcode, airchg, stndchg, offrdchg ",
                    DateRangeType);

            if (IsValidUdid)
            {
                sqlScript.FromClause = "hibtrips T1, hibudids T3";
                sqlScript.KeyWhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and ";
                sqlScript.WhereClause = string.Format(sqlScript.KeyWhereClause + "{0}", WhereClause);
            }
            else
            {
                sqlScript.FromClause = "hibtrips T1";
                sqlScript.KeyWhereClause = "valcarr not in ('ZZ','$$') and ";
                sqlScript.WhereClause = string.Format(sqlScript.KeyWhereClause + "{0}", WhereClause);
            }

            return sqlScript;
        }

        private SqlScript BuildAirReservationDataScript()
        {
            var sqlScript = new SqlScript();
            sqlScript.FieldList = string.Format("{0} as datecomp, invdate, depdate, BookDate, convert(int,1) as plusmin, reascode, savingcode, airchg, stndchg, offrdchg ", DateRangeType);

            if (IsValidUdid)
            {
                sqlScript.FromClause = "ibtrips T1, ibudids T3";
                sqlScript.KeyWhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and ";
                sqlScript.WhereClause = string.Format(sqlScript.KeyWhereClause + "{0}", WhereClause);
            }
            else
            {
                sqlScript.FromClause = "ibtrips T1";
                sqlScript.KeyWhereClause = "valcarr not in ('ZZ','$$') and ";
                sqlScript.WhereClause = string.Format(sqlScript.KeyWhereClause + "{0}", WhereClause);
            }

            return sqlScript;
        }
    }
}

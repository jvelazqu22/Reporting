using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryByHomeCountry
{
    public class ExecutiveSummaryByHomeCountrySqlCreator
    {
        public SqlScript CreateAirRawDataSql(string originalWhereClause, bool isReservationReport, int udidNumber)
        {
            var airSql = new SqlScript();

            if (udidNumber > 0)
            {
                airSql.FromClause = isReservationReport ? "ibtrips T1, ibudids T3" : "hibtrips T1, hibudids T3";
                airSql.KeyWhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and ";
                airSql.WhereClause = airSql.KeyWhereClause + originalWhereClause;
            }
            else
            {
                airSql.FromClause = isReservationReport ? "ibtrips T1" : "hibtrips T1";
                airSql.KeyWhereClause = "valcarr not in ('ZZ','$$') and ";
                airSql.WhereClause = airSql.KeyWhereClause + originalWhereClause;
            }

            airSql.FieldList = isReservationReport 
                ? "T1.reckey, sourceAbbr, convert(int,1) as PlusMin, AirChg, StndChg, OffrdChg, ValCarr, DepDate, ArrDate, ' ' as valcarmode "
                : "T1.reckey, SourceAbbr,convert(int,PlusMin) as PlusMin, AirChg, StndChg, OffrdChg, ValCarr, DepDate, ArrDate, valcarmode ";

            airSql.OrderBy = "";

            return airSql;
        }

        public SqlScript CreateHotelSql(string orignalWhereClause, bool isReservationReport, int udidNumber)
        {
            var hotelSql = new SqlScript();

            if (udidNumber > 0)
            {
                hotelSql.FromClause = isReservationReport ? "ibtrips T1, ibudids T3, ibhotel T5" : "hibtrips T1, hibudids T3, hibhotel T5";
                hotelSql.KeyWhereClause = isReservationReport
                                              ? "T1.reckey = T3.reckey and T1.reckey = T5.reckey and "
                                              : "T1.reckey = T3.reckey and T1.reckey = T5.reckey and hottrantyp != 'V' and ";
                hotelSql.WhereClause = hotelSql.KeyWhereClause + orignalWhereClause;
            }
            else
            {
                hotelSql.FromClause = isReservationReport ? "ibtrips T1, ibhotel T5" : "hibtrips T1, hibhotel T5";
                hotelSql.KeyWhereClause = isReservationReport
                                              ? "T1.reckey = T5.reckey and "
                                              : "T1.reckey = T5.reckey and T5.HotTranTyp != 'V' and ";
                hotelSql.WhereClause = hotelSql.KeyWhereClause + orignalWhereClause;
            }

            hotelSql.FieldList = isReservationReport 
                ? "T1.reckey, SourceAbbr, convert(int,1) as hplusmin, convert(int,nights) as nights, convert(int,rooms) as rooms, bookrate, datein " 
                : "T1.reckey, SourceAbbr, convert(int,hplusmin) as hplusmin, convert(int,nights) as nights, convert(int,rooms) as rooms, bookrate, datein ";

            hotelSql.OrderBy = "";

            return hotelSql;
        }

        public SqlScript CreateCarSql(string orignalWhereClause, bool isReservationReport, int udidNumber)
        {
            var sql = new SqlScript();

            if (udidNumber > 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibudids T3, ibcar T4" : "hibtrips T1, hibudids T3, hibcars T4";
                sql.KeyWhereClause = isReservationReport
                                         ? "T1.reckey = T3.reckey and T1.reckey = T4.reckey and "
                                         : "T1.reckey = T3.reckey and T1.reckey = T4.reckey and cartrantyp != 'V' and ";
                sql.WhereClause = sql.KeyWhereClause + orignalWhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibcar T4" : "hibtrips T1, hibcars T4";
                sql.KeyWhereClause = isReservationReport
                                         ? "T1.reckey = T4.reckey and "
                                         : "T1.reckey = T4.reckey and  cartrantyp != 'V' and ";
                sql.WhereClause = sql.KeyWhereClause + orignalWhereClause;
            }

            sql.FieldList = isReservationReport 
                ? "T1.reckey, SourceAbbr,convert(int,Days) as Days, convert(int,1) as cplusmin, abookrat, rentdate "
                : "T1.reckey, SourceAbbr, convert(int,Days) as days, convert(int,cplusmin) as cplusmin, abookrat, rentdate ";

            sql.OrderBy = "";

            return sql;
        }

        public SqlScript CreateLegSql(string existingWhereClause, int udidNumber)
        {
            var sql = new SqlScript();

            if (udidNumber > 0)
            {
                sql.FromClause = "ibtrips T1, ibLegs T2, ibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and " +
                                     "valcarr not in ('ZZ','$$') and airline not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            else
            {
                sql.FromClause = "ibtrips T1, ibLegs T2 ";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and valcarr not in ('ZZ','$$') and airline not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            sql.FieldList = "T1.reckey, airline, mode ";

            sql.OrderBy = "";

            return sql;
        }

        public SqlScript CreateSvcFeeSql(string existingWhereClause, int udidNumber, bool includeServiceFeeNoMatch)
        {
            var sql = new SqlScript();

            sql.FieldList = "T1.reckey, T1.SourceAbbr, T6A.SvcAmt, MCO ";
            sql.FromClause = "hibtrips T1, hibServices T6A ";
            sql.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svcCode = 'TSF' and ";
            sql.WhereClause = sql.KeyWhereClause + existingWhereClause.Replace("invdate", "trandate");

            if (udidNumber > 0)
            {
                sql.FromClause = "hibtrips T1, hibudids T3, hibServices T6A ";
                sql.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T6A.reckey and T1.agency = T3.agency and " +
                                     "T1.agency = T6A.agency and T6A.svcCode = 'TSF' and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;

            }
            else if(!includeServiceFeeNoMatch)
            {
                sql.FromClause = "hibtrips T1, hibServices T6A ";
                sql.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and origValCar in ('SVCFEEONLY','ZZ:S') and " +
                                     "T6A.svcCode = 'TSF' and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            sql.OrderBy = "";

            return sql;
        }

        public string GetReplacedServiceFeeWhereClause(string originalWhereClause)
        {
            return originalWhereClause
                    .Replace("T1.trantype", "T6A.sfTranType")
                    .Replace("trantype", "sfTranType");
        }
    }
}
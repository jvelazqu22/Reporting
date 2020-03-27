using System.Data.SqlClient;
using System.Reflection;

using com.ciswired.libraries.CISLogger;
using Domain.Exceptions;
using Domain.Helper;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Logging;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public class DateWhere
    {
        private readonly ErrorLogger _errorLogger = new ErrorLogger();
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG =
            new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private static readonly WhereClauseBuilder _whereClauseBuilder = new WhereClauseBuilder();

        public void GetDateWhere(ReportGlobals globals, BuildWhere where)
        {
            //Make sure we have dates
            if (!globals.BeginDate.HasValue || !globals.EndDate.HasValue)
            {
                throw new BuildWhereException("Where clause requires both a begin and an end date.");
            }

            var beginDate = globals.BeginDate.Value;
            var endDate = globals.EndDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            var dateTypeParameter = globals.GetParmValue(WhereCriteria.DATERANGE);
            if (!int.TryParse(dateTypeParameter, out var dateType)) dateType = (int)DateType.DepartureDate;
            
            switch ((DateType)dateType)
            {

                case DateType.InvoiceDate:
                    where.WhereClauseDate = " invdate >= @t1BeginDate and invdate <= @t1EndDate";
                    break;
                case DateType.BookedDate:
                    where.WhereClauseDate = "bookdate >= @t1BeginDate and bookdate <= @t1EndDate";
                    break;
                case DateType.RoutingDepartureDate:
                    where.WhereClauseRoute = "rdepdate >= @t1BeginDate and rdepdate <= @t1EndDate";
                    beginDate = beginDate.AddDays(-30);
                    endDate = endDate.AddDays(30);
                    where.WhereClauseDate = "depdate >= @t1BeginDate and depdate <= @t1EndDate";
                    break;
                case DateType.RoutingArrivalDate:
                    where.WhereClauseDate = "arrdate >= @t1BeginDate and arrdate <= @t1EndDate";
                    break;
                case DateType.CarRentalDate:
                    where.WhereClauseCar = _whereClauseBuilder.AddToWhereClause(where.WhereClauseCar, "rentdate between @t1BeginDate and @t1EndDate");
                    where.WhereClauseDate = "TripEnd >= @t1BeginDate and TripStart <= @t1EndDate";
                    break;
                case DateType.HotelCheckInDate:
                    where.WhereClauseHotel = _whereClauseBuilder.AddToWhereClause(where.WhereClauseHotel, "datein between @t1BeginDate and @t1EndDate"); 
                    where.WhereClauseDate = "TripEnd >= @t1BeginDate and TripStart <= @t1EndDate";
                    break;
                case DateType.TransactionDate:
                    where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "trandate >= @t1BeginDate and trandate <= @t1EndDate");
                    break;
                case DateType.OnTheRoadDatesSpecial:
                    where.WhereClauseDate = "depdate <= @t1EndDate and arrdate >= @t1BeginDate";
                    break;
                case DateType.OnTheRoadDatesCarRental:
                    where.WhereClauseCar = _whereClauseBuilder.AddToWhereClause(where.WhereClauseCar, "rentdate <= @t1EndDate and rentdate+days-1 >= @t1BeginDate");
                    where.WhereClauseDate = "TripEnd >= @t1BeginDate and TripStart <= @t1EndDate";
                    break;
                case DateType.OnTheRoadDatesHotel:
                    where.WhereClauseHotel = _whereClauseBuilder.AddToWhereClause(where.WhereClauseHotel, "datein <= @t1EndDate and datein+days >= @t1BeginDate");
                    where.WhereClauseDate = "TripEnd >= @t1BeginDate and TripStart <= @t1EndDate";
                    break;
                case DateType.AuthorizationStatusDate:
                    //TODO: Uses t7? 
                    //throw new NotImplementedException();
                    where.WhereClauseDate = "T7.statustime >= @t1BeginDate and T7.statustime <= @t1EndDate";
                    break;
                case DateType.PostDate:
                    where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "postdate >= @t1BeginDate and postdate <= @t1EndDate");
                    break;
                case DateType.LastUpdate:
                    // up above we set endDate to the end of the day, but for last updated we want to use the actual value
                    // I don't like doing it like this but it is the least impactful way 
                    // ReSharper disable once PossibleInvalidOperationException
                    endDate = globals.EndDate.Value;
                    where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "lastupdate >= @t1BeginDate and lastupdate <= @t1EndDate");
                    break;
                default: //Should be departure date
                    where.WhereClauseDate = "depdate >= @t1BeginDate and depdate <= @t1EndDate";
                    break;
            }

            //Everyone uses these parameters
            where.SqlParameters.Add(new SqlParameter("t1BeginDate", beginDate));
            where.SqlParameters.Add(new SqlParameter("t1EndDate", endDate));
        }
    }
}

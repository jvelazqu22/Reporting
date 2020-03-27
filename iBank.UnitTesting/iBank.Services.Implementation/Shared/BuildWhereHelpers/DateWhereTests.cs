using System;
using System.Linq;

using Domain.Exceptions;
using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    [TestClass]
    public class DateWhereTests
    {
        [ExpectedException(typeof(BuildWhereException))]
        [TestMethod]
        public void GetDateWhere_NoBeginDate_ThrowBuildWhereException()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = null;
            globals.EndDate = new DateTime(2000, 1, 1);
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);
        }

        [ExpectedException(typeof(BuildWhereException))]
        [TestMethod]
        public void GetDateWhere_NoEndDate_ThrowBuildWhereException()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = null;
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);
        }

        [TestMethod]
        public void GetDateWhere_DepartureDate()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = new DateTime(2000, 12, 12);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.DepartureDate).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("depdate >= @t1BeginDate and depdate <= @t1EndDate", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("", where.WhereClauseCar);
            Assert.AreEqual("", where.WhereClauseHotel);
            Assert.AreEqual("", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_NoDateRange_DefaultToDepartureDateLogic()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = new DateTime(2000, 12, 12);
            globals.SetParmValue(WhereCriteria.DATERANGE, "");
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("depdate >= @t1BeginDate and depdate <= @t1EndDate", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("", where.WhereClauseCar);
            Assert.AreEqual("", where.WhereClauseHotel);
            Assert.AreEqual("", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_InvoiceDate()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = new DateTime(2000, 12, 12);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.InvoiceDate).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual(" invdate >= @t1BeginDate and invdate <= @t1EndDate", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("", where.WhereClauseCar);
            Assert.AreEqual("", where.WhereClauseHotel);
            Assert.AreEqual("", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_BookedDate()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = new DateTime(2000, 12, 12);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.BookedDate).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("bookdate >= @t1BeginDate and bookdate <= @t1EndDate", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("", where.WhereClauseCar);
            Assert.AreEqual("", where.WhereClauseHotel);
            Assert.AreEqual("", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_RoutingDepartureDate()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 31);
            globals.EndDate = new DateTime(2000, 12, 1);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.RoutingDepartureDate).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("depdate >= @t1BeginDate and depdate <= @t1EndDate", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 31, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("", where.WhereClauseCar);
            Assert.AreEqual("", where.WhereClauseHotel);
            Assert.AreEqual("", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_RoutingArrivalDate()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = new DateTime(2000, 12, 12);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.RoutingArrivalDate).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("arrdate >= @t1BeginDate and arrdate <= @t1EndDate", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("", where.WhereClauseCar);
            Assert.AreEqual("", where.WhereClauseHotel);
            Assert.AreEqual("", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_CarRentalDate()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = new DateTime(2000, 12, 12);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.CarRentalDate).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("TripEnd >= @t1BeginDate and TripStart <= @t1EndDate", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("rentdate between @t1BeginDate and @t1EndDate", where.WhereClauseCar);
            Assert.AreEqual("", where.WhereClauseHotel);
            Assert.AreEqual("", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_HotelCheckInDate()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = new DateTime(2000, 12, 12);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.HotelCheckInDate).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("TripEnd >= @t1BeginDate and TripStart <= @t1EndDate", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("", where.WhereClauseCar);
            Assert.AreEqual("datein between @t1BeginDate and @t1EndDate", where.WhereClauseHotel);
            Assert.AreEqual("", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_TransactionDate()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = new DateTime(2000, 12, 12);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.TransactionDate).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("", where.WhereClauseCar);
            Assert.AreEqual("", where.WhereClauseHotel);
            Assert.AreEqual("trandate >= @t1BeginDate and trandate <= @t1EndDate", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_OnTheRoadDatesSpecial()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = new DateTime(2000, 12, 12);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.OnTheRoadDatesSpecial).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("depdate <= @t1EndDate and arrdate >= @t1BeginDate", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("", where.WhereClauseCar);
            Assert.AreEqual("", where.WhereClauseHotel);
            Assert.AreEqual("", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_OnTheRoadDatesCarRental()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = new DateTime(2000, 12, 12);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.OnTheRoadDatesCarRental).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("TripEnd >= @t1BeginDate and TripStart <= @t1EndDate", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("rentdate <= @t1EndDate and rentdate+days-1 >= @t1BeginDate", where.WhereClauseCar);
            Assert.AreEqual("", where.WhereClauseHotel);
            Assert.AreEqual("", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_OnTheRoadDatesHotel()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = new DateTime(2000, 12, 12);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.OnTheRoadDatesHotel).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("TripEnd >= @t1BeginDate and TripStart <= @t1EndDate", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("", where.WhereClauseCar);
            Assert.AreEqual("datein <= @t1EndDate and datein+days >= @t1BeginDate", where.WhereClauseHotel);
            Assert.AreEqual("", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_PostDate()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1);
            globals.EndDate = new DateTime(2000, 12, 12);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.PostDate).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 23, 59, 59), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("", where.WhereClauseCar);
            Assert.AreEqual("", where.WhereClauseHotel);
            Assert.AreEqual("postdate >= @t1BeginDate and postdate <= @t1EndDate", where.WhereClauseTrip);
        }

        [TestMethod]
        public void GetDateWhere_LastUpdate()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.BeginDate = new DateTime(2000, 1, 1, 5, 6, 0);
            globals.EndDate = new DateTime(2000, 12, 12, 7, 8, 0);
            globals.SetParmValue(WhereCriteria.DATERANGE, ((int)DateType.LastUpdate).ToString());
            where.WhereClauseDate = "";
            where.WhereClauseCar = "";
            where.WhereClauseHotel = "";
            where.WhereClauseTrip = "";
            var sut = new DateWhere();

            sut.GetDateWhere(globals, where);

            Assert.AreEqual("", where.WhereClauseDate);
            Assert.AreEqual(new DateTime(2000, 1, 1, 5, 6, 0), where.SqlParameters.First(x => x.ParameterName.Equals("t1BeginDate")).Value);
            Assert.AreEqual(new DateTime(2000, 12, 12, 7, 8, 0), where.SqlParameters.First(x => x.ParameterName.Equals("t1EndDate")).Value);

            Assert.AreEqual("", where.WhereClauseCar);
            Assert.AreEqual("", where.WhereClauseHotel);
            Assert.AreEqual("lastupdate >= @t1BeginDate and lastupdate <= @t1EndDate", where.WhereClauseTrip);
        }
    }
}

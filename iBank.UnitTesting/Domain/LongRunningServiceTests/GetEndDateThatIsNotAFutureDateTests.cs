using System;
using System.Collections.Generic;
using Domain.Services;
using Domain.Helper;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iBank.UnitTesting.Domain.LongRunningServiceTests
{
    [TestClass]
    public class GetEndDateThatIsNotAFutureDateTests
    {
        private IMasterDataStore _store;

        [TestInitialize]
        public void Init()
        {
            var db = new Mock<IMastersQueryable>();

            var mock = new Mock<IMasterDataStore>();
            mock.Setup(x => x.MastersQueryDb).Returns(db.Object);
            _store = mock.Object;
        }


        [TestMethod]
        public void GetEndDateThatIsNotAFutureDate_EndDateAsTodayDate_ReturnMatchTodaysDate()
        {
            var sut = new LongRunningService(new CacheService(), _store);
            var now = DateTime.Now;
            var today = new DateTime(now.Year, now.Month, now.Day);

            var act = DateRangeHelper.GetEndDateThatIsNotAFutureDate(now);

            Assert.AreEqual(today, act);
        }

        [TestMethod]
        public void GetEndDateThatIsNotAFutureDate_EndDateAsTomorrowDate_ReturnMatchTodaysDate()
        {
            var now = DateTime.Now;
            var today = new DateTime(now.Year, now.Month, now.Day);

            var tomorrow = DateTime.Now.AddDays(1);
            var endDate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day);

            var act = DateRangeHelper.GetEndDateThatIsNotAFutureDate(tomorrow);

            Assert.AreEqual(today, act);
        }

        [TestMethod]
        public void GetEndDateThatIsNotAFutureDate_EndDateAsYesterdayDate_ReturnMatchesYesterdaysDateNotTodaysDate()
        {
            var now = DateTime.Now;
            var today = new DateTime(now.Year, now.Month, now.Day);

            //Could be any day, but yesterday would be enough
            var yesterday = now.AddDays(-1);
            var endDate = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day);
            
            var act = DateRangeHelper.GetEndDateThatIsNotAFutureDate(yesterday);

            //match yesterday, not today
            Assert.AreEqual(yesterday, act);
            Assert.AreNotEqual(endDate, today);
        }

    }
}

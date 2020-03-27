using System.Collections.Generic;
using System.Linq;

using Domain.Orm.iBankMastersQueries.eProfile;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.Domain.ORM.MasterQueries.eProfile
{
    [TestClass]
    public class IsExportTypeAuthorizedForEProfileNumberQueryTests
    {
        [TestMethod]
        public void ExecuteQuery_RecordExists_ReturnTrue()
        {
            var data = new eProfExpTypes
            {
                eProfileNo = 1,
                ExportType = "foo"
            };
            var mockSet = new Mock<IQueryable<eProfExpTypes>>();
            mockSet.SetupIQueryable(new List<eProfExpTypes> { data }.AsQueryable());

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.EProfExpTypes).Returns(mockSet.Object);

            var query = new IsExportTypeAuthorizedForEProfileNumberQuery(mockDb.Object, "foo", 1);
            var output = query.ExecuteQuery();

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void ExecuteQuery_EProfileNumberDoesNotExists_ReturnFalse()
        {
            var data = new eProfExpTypes
            {
                eProfileNo = 1,
                ExportType = "foo"
            };
            var mockSet = new Mock<IQueryable<eProfExpTypes>>();
            mockSet.SetupIQueryable(new List<eProfExpTypes> { data }.AsQueryable());

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.EProfExpTypes).Returns(mockSet.Object);

            var query = new IsExportTypeAuthorizedForEProfileNumberQuery(mockDb.Object, "foo", 2);
            var output = query.ExecuteQuery();

            Assert.AreEqual(false, output);
        }


        [TestMethod]
        public void ExecuteQuery_ExportTypeDoesNotMatch_ReturnFalse()
        {
            var data = new eProfExpTypes
            {
                eProfileNo = 1,
                ExportType = "foo"
            };
            var mockSet = new Mock<IQueryable<eProfExpTypes>>();
            mockSet.SetupIQueryable(new List<eProfExpTypes> { data }.AsQueryable());

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.EProfExpTypes).Returns(mockSet.Object);

            var query = new IsExportTypeAuthorizedForEProfileNumberQuery(mockDb.Object, "bar", 2);
            var output = query.ExecuteQuery();

            Assert.AreEqual(false, output);
        }
    }
}

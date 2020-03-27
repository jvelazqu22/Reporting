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
    public class IsProcessKeyAuthorizedForEProfileNumberQueryTests
    {
        [TestMethod]
        public void ExecuteQuery_RecordExists_ReturnTrue()
        {
            var data = new eProfileProcs
                           {
                               eProfileNo = 1,
                               ProcessKey = 2
                           };
            var mockSet = new Mock<IQueryable<eProfileProcs>>();
            mockSet.SetupIQueryable(new List<eProfileProcs> { data }.AsQueryable());

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.EProfileProcs).Returns(mockSet.Object);

            var query = new IsProcessKeyAuthorizedForEProfileNumberQuery(mockDb.Object, 2, 1);
            var output = query.ExecuteQuery();

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void ExecuteQuery_ProcessKeyAndEProfileNumberDoesNotMatch_ReturnFalse()
        {
            var data = new eProfileProcs
            {
                eProfileNo = 1,
                ProcessKey = 2
            };
            var mockSet = new Mock<IQueryable<eProfileProcs>>();
            mockSet.SetupIQueryable(new List<eProfileProcs> { data }.AsQueryable());

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.EProfileProcs).Returns(mockSet.Object);

            var query = new IsProcessKeyAuthorizedForEProfileNumberQuery(mockDb.Object, 8, 8);
            var output = query.ExecuteQuery();

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void ExecuteQuery_EProfileNumberDoesNotMatch_ReturnFalse()
        {
            var data = new eProfileProcs
            {
                eProfileNo = 1,
                ProcessKey = 2
            };
            var mockSet = new Mock<IQueryable<eProfileProcs>>();
            mockSet.SetupIQueryable(new List<eProfileProcs> { data }.AsQueryable());

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.EProfileProcs).Returns(mockSet.Object);

            var query = new IsProcessKeyAuthorizedForEProfileNumberQuery(mockDb.Object, 2, 2);
            var output = query.ExecuteQuery();

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void ExecuteQuery_ProcessKeyDoesNotMatch_ReturnFalse()
        {
            var data = new eProfileProcs
            {
                eProfileNo = 1,
                ProcessKey = 2
            };
            var mockSet = new Mock<IQueryable<eProfileProcs>>();
            mockSet.SetupIQueryable(new List<eProfileProcs> { data }.AsQueryable());

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.EProfileProcs).Returns(mockSet.Object);

            var query = new IsProcessKeyAuthorizedForEProfileNumberQuery(mockDb.Object, 1, 1);
            var output = query.ExecuteQuery();

            Assert.AreEqual(false, output);
        }
    }
}

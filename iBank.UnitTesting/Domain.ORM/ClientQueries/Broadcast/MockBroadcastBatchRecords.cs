using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Moq;

namespace iBank.UnitTesting.Domain.ORM.ClientQueries.Broadcast
{
    internal static class MockBroadcastBatchRecords
    {

        public static Mock<IClientQueryable> GetMockDbOfQueue(IList<ibbatch> mockData)
        {
            var mockSet = new Mock<IQueryable<ibbatch>>();
            mockSet.SetupIQueryable(mockData.AsQueryable());

            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.iBBatch).Returns(mockSet.Object);

            return mockDb;
        }
    }
}

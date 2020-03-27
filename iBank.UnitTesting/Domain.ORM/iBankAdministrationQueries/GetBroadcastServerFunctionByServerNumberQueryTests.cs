using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Orm.iBankAdministrationQueries;

using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.Domain.ORM.iBankAdministrationQueries
{
    [TestClass]
    public class GetBroadcastServerFunctionByServerNumberQueryTests
    {
        [TestMethod]
        public void ExecuteQuery_ServerFunctionIdOfOne_ReturnPrimary()
        {
            var data = new broadcast_servers
                           {
                                server_number = 1,
                                server_function_id = 1
                           };

            var mockSet = new Mock<IQueryable<broadcast_servers>>();
            mockSet.SetupIQueryable(new List<broadcast_servers> { data }.AsQueryable());

            var mockDb = new Mock<IAdministrationQueryable>();
            mockDb.Setup(x => x.BroadcastServers).Returns(mockSet.Object);

            var query = new GetBroadcastServerFunctionByServerNumberQuery(mockDb.Object, 1);
            var output = query.ExecuteQuery();

            Assert.AreEqual(BroadcastServerFunction.Primary, output);
        }

        [TestMethod]
        public void ExecuteQuery_ServerFunctionIdOfTwo_ReturnOffline()
        {
            var data = new broadcast_servers
            {
                server_number = 1,
                server_function_id = 2
            };

            var mockSet = new Mock<IQueryable<broadcast_servers>>();
            mockSet.SetupIQueryable(new List<broadcast_servers> { data }.AsQueryable());

            var mockDb = new Mock<IAdministrationQueryable>();
            mockDb.Setup(x => x.BroadcastServers).Returns(mockSet.Object);

            var query = new GetBroadcastServerFunctionByServerNumberQuery(mockDb.Object, 1);
            var output = query.ExecuteQuery();

            Assert.AreEqual(BroadcastServerFunction.Offline, output);
        }

        [TestMethod]
        public void ExecuteQuery_ServerFunctionIdOfThree_ReturnHot()
        {
            var data = new broadcast_servers
            {
                server_number = 1,
                server_function_id = 3
            };

            var mockSet = new Mock<IQueryable<broadcast_servers>>();
            mockSet.SetupIQueryable(new List<broadcast_servers> { data }.AsQueryable());

            var mockDb = new Mock<IAdministrationQueryable>();
            mockDb.Setup(x => x.BroadcastServers).Returns(mockSet.Object);

            var query = new GetBroadcastServerFunctionByServerNumberQuery(mockDb.Object, 1);
            var output = query.ExecuteQuery();

            Assert.AreEqual(BroadcastServerFunction.Hot, output);
        }
    }
}

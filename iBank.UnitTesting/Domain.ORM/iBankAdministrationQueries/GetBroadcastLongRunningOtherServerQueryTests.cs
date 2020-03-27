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
    public class GetBroadcastLongRunningOtherServerQueryTests
    {

        private Mock<IAdministrationQueryable> Initialize()
        {
            var mockDb = new Mock<IAdministrationQueryable>();

            var bcstServers = new List<broadcast_servers>
            {
                new broadcast_servers {server_function_id = 6, server_number=1},
                new broadcast_servers {server_function_id = 2, server_number=2},
                new broadcast_servers {server_function_id = 6, server_number=3}
            };
            
            mockDb.Setup(x => x.BroadcastServers).Returns(MockHelper.GetListAsQueryable(bcstServers).Object);

            return mockDb;
        }

        [TestMethod]
        public void GetBroadcastLongRunningOtherServerQuery_Pass3Return1()
        {
            //arrange
            var mockDb = Initialize();

            IList<int> exp = new List<int> { 1 };
            //act
            var query = new GetBroadcastLongRunningOtherServerQuery(mockDb.Object, 3);
            var sut = query.ExecuteQuery();

            //Assert
            for (var i = 0; i< exp.Count; i++)
            {
                Assert.AreEqual(exp[i], sut[i]);
            }
        }

        [TestMethod]
        public void GetBroadcastLongRunningOtherServerQuery_Pass1_Return3()
        {
            //arrange
            var mockDb = Initialize();

            IList<int> exp = new List<int> { 3 };
            //act
            var query = new GetBroadcastLongRunningOtherServerQuery(mockDb.Object, 1);
            var sut = query.ExecuteQuery();

            //Assert
            for (var i = 0; i < exp.Count; i++)
            {
                Assert.AreEqual(exp[i], sut[i]);
            }
        }

        [TestMethod]
        public void GetBroadcastLongRunningOtherServerQuery_PassServerNumberWithWrongBroadcastServerFunction_ReturnBlankList()
        {
            //arrange
            var mockDb = Initialize();
            
            //act
            var query = new GetBroadcastLongRunningOtherServerQuery(mockDb.Object, 2);
            var sut = query.ExecuteQuery();

            //Assert
            Assert.AreEqual(0, sut.Count);
           
        }
    }
}

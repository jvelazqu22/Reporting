using System.Collections.Generic;

using Domain.Exceptions;
using Domain.Orm.iBankAdministrationQueries;

using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace iBank.UnitTesting.Domain
{
    [TestClass]
    public class IsFeatureFlagOnQueryTests
    {
        private IAdministrationQueryable _db;

        [TestInitialize]
        public void Init()
        {
            var flags = new List<ibank_feature_flag>
                            {
                                new ibank_feature_flag { toggle_name = "ON", toggle_on = true, toggle_on_stage = true },
                                new ibank_feature_flag { toggle_name = "OFF", toggle_on = false, toggle_on_stage = false },
                                new ibank_feature_flag { toggle_name = "ON_STAGE", toggle_on = true, toggle_on_stage = true },
                                new ibank_feature_flag { toggle_name = "OFF_STAGE", toggle_on = false, toggle_on_stage = false }
                            };
            var bcstServerFunction = new List<broadcast_server_function>
                                         {
                                             new broadcast_server_function { id = 1, server_function = "AGENCYSTAGE" },
                                             new broadcast_server_function { id = 2, server_function = "DBSTAGE" },
                                             new broadcast_server_function { id = 3, server_function = "STAGE" }
                                         };
            var bcstServers = new List<broadcast_servers>
                                  {
                                      new broadcast_servers { server_number = 1, server_function_id = 1 },
                                      new broadcast_servers { server_number = 2, server_function_id = 2 },
                                      new broadcast_servers { server_number = 3, server_function_id = 3 }
                                  };
            var rptServerFunction = new List<report_server_function>
                                        {
                                            new report_server_function { id = 1, server_function = "STAGE" }
                                        };
            var rptServers = new List<report_servers>
                                 {
                                    new  report_servers { server_number = 10, server_function_id = 1 }
                                 };
            var mockDb = new Mock<IAdministrationQueryable>();
            mockDb.Setup(x => x.FeatureFlag).Returns(MockHelper.GetListAsQueryable(flags).Object);
            mockDb.Setup(x => x.BroadcastServerFunction).Returns(MockHelper.GetListAsQueryable(bcstServerFunction).Object);
            mockDb.Setup(x => x.BroadcastServers).Returns(MockHelper.GetListAsQueryable(bcstServers).Object);
            mockDb.Setup(x => x.ReportServerFunction).Returns(MockHelper.GetListAsQueryable(rptServerFunction).Object);
            mockDb.Setup(x => x.ReportServers).Returns(MockHelper.GetListAsQueryable(rptServers).Object);

            _db = mockDb.Object;
        }

        [ExpectedException(typeof(FeatureFlagDoesNotExistException))]
        [TestMethod]
        public void ExecuteQuery_FlagDoesntExist_ThrowException()
        {
            var flag = "NOT_EXIST";
            var serverNumber = 200;
            var sut = new IsFeatureFlagOnQuery(_db, flag, serverNumber);

            var ouput = sut.ExecuteQuery();
        }

        [TestMethod]
        public void ExecuteQuery_BroadcastStageServer_FlagOnStage_ReturnTrue()
        {
            var flag = "ON_STAGE";
            var serverNumber = 1;
            var sut = new IsFeatureFlagOnQuery(_db, flag, serverNumber);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void ExecuteQuery_ReportServerStage_FlagOnStage_ReturnTrue()
        {
            var flag = "ON_STAGE";
            var serverNumber = 10;
            var sut = new IsFeatureFlagOnQuery(_db, flag, serverNumber);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void ExecuteQuery_BroadcastStageServer_FlagOffStage_ReturnFalse()
        {
            var flag = "OFF_STAGE";
            var serverNumber = 1;
            var sut = new IsFeatureFlagOnQuery(_db, flag, serverNumber);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void ExecuteQuery_RegularServer_FlagOn_ReturnTrue()
        {
            var flag = "ON";
            var serverNumber = 99;
            var sut = new IsFeatureFlagOnQuery(_db, flag, serverNumber);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void ExecuteQuery_RegularServer_FlagOff_ReturnFalse()
        {
            var flag = "OFF";
            var serverNumber = 99;
            var sut = new IsFeatureFlagOnQuery(_db, flag, serverNumber);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(false, output);
        }
    }
}

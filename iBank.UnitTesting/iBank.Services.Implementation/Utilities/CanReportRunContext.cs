using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Moq;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities
{
    public class CanReportRunContext
    {
        public CanReportRunContext()
        {
            SetupDatabase();
        }

        public IMasterDataStore MasterDataStore { get; set; }

        private void SetupDatabase()
        {
            var mockIbproces = new List<ibproces>
            {
                //not enabled
                new ibproces { processkey = 1, dotnet = false, staged = false },
                //enabled, not staged
                new ibproces { processkey = 2, dotnet = true, staged = false },
                //enabled and staged
                new ibproces { processkey = 3, dotnet = true, staged = true }
            };

            var reportHandoff = new List<reporthandoff>
            {
                //report 1 ; report not enabled ; agency enabled ; agency enabled ; staged for report 3
                new reporthandoff { reportid = "1", parminout = "IN", parmname = "PROCESSID", parmvalue = "1", agency = "FOO" },
                new reporthandoff { reportid = "1", parminout = "IN", parmname = "AGENCY", parmvalue = "FOO", agency = "FOO" },

                //report 2;  report enabled, not staged ; agency enabled ; staged for report 3
                new reporthandoff { reportid = "2", parminout = "IN", parmname = "PROCESSID", parmvalue = "2", agency = "FOO" },
                new reporthandoff { reportid = "2", parminout = "IN", parmname = "AGENCY", parmvalue = "FOO", agency = "FOO" },

                //report 3 ; report enabled and staged ; agency enabled ; staged for report 3
                new reporthandoff { reportid = "3", parminout = "IN", parmname = "PROCESSID", parmvalue = "3", agency = "FOO" },
                new reporthandoff { reportid = "3", parminout = "IN", parmname = "AGENCY", parmvalue = "FOO", agency = "FOO" },

                //report 2 ; report enabled, not staged ; agency not enabled
                new reporthandoff { reportid = "4", parminout = "IN", parmname = "PROCESSID", parmvalue = "2", agency = "BAR" },
                new reporthandoff { reportid = "4", parminout = "IN", parmname = "AGENCY", parmvalue = "BAR", agency = "BAR" },

                //report 3 ; report enabled and staged ; agency enabled ; belongs to staged database
                new reporthandoff { reportid = "5", parminout = "IN", parmname = "PROCESSID", parmvalue = "3", agency = "STAGED" },
                new reporthandoff { reportid = "5", parminout = "IN", parmname = "AGENCY", parmvalue = "STAGED", agency = "STAGED" },

                //report 3 ; report enabled and staged ; agency enabled ; agency not currently turned on for stage
                new reporthandoff { reportid = "6", parminout = "IN", parmname = "PROCESSID", parmvalue = "3", agency = "FOO" },
                new reporthandoff { reportid = "6", parminout = "IN", parmname = "AGENCY", parmvalue = "FOOBAR", agency = "FOOBAR" },
            };

            var clientExtras = new List<ClientExtras>
            {
                //enabled
                new ClientExtras { ClientCode = "FOO", FieldFunction = "DOT_NET_RPTSVR", FieldData = "YES", ClientType = "A" },
                new ClientExtras { ClientCode = "STAGED", FieldFunction = "DOT_NET_RPTSVR", FieldData = "YES", ClientType = "A"  },
                new ClientExtras { ClientCode = "FOOBAR", FieldFunction = "DOT_NET_RPTSVR", FieldData = "YES", ClientType = "A" },
                //not enabled
                new ClientExtras { ClientCode = "BAR", FieldFunction = "DOT_NET_RPTSVR", FieldData = "NO", ClientType = "A"  },
            };

            var mockRolloutStage = new List<report_rollout_stage>
            {
                //staged for report 3
                new report_rollout_stage { agency = "FOO", process_key = 3, database_name = null, currently_staged = true },
                //staged but not currently turned on
                new report_rollout_stage { agency = "FOOBAR", process_key = 3, database_name = null, currently_staged = false },
                //staged agency
                new report_rollout_stage { agency = null, database_name = "STAGED_DB", process_key = 3, currently_staged = true }
            };

            var mockMstrAgcy = new List<mstragcy>
            {
                //belongs to staged database
                new mstragcy { agency = "STAGED", databasename = "STAGED_DB" },
                //belongs to stage
                new mstragcy { agency = "FOO", databasename = "DB" },
                //staged but not currently turned on
                new mstragcy {agency = "FOOBAR", databasename = "DB" },
                //not enabled 
                new mstragcy {agency = "BAR", databasename = "DB" },
            };

            var mockConversionEnabledUser = new List<conversion_enabled_users>()
            {
                new conversion_enabled_users() { id=1, user_number = 20, agency = "FOO", processkey = 1, currently_enabled = false},
                new conversion_enabled_users() { id=1, user_number = 20, agency = "BAR", processkey = 2, currently_enabled = true},
            };

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.iBProcess).Returns(MockHelper.GetListAsQueryable(mockIbproces).Object);
            mockDb.Setup(x => x.ReportRolloutStage).Returns(MockHelper.GetListAsQueryable(mockRolloutStage).Object);
            mockDb.Setup(x => x.MstrAgcy).Returns(MockHelper.GetListAsQueryable(mockMstrAgcy).Object);
            mockDb.Setup(x => x.ClientExtras).Returns(MockHelper.GetListAsQueryable(clientExtras).Object);
            mockDb.Setup(x => x.ReportHandoff).Returns(MockHelper.GetListAsQueryable(reportHandoff).Object);
            mockDb.Setup(x => x.ConversionEnabledUsers).Returns(MockHelper.GetListAsQueryable(mockConversionEnabledUser).Object);

            var mockStore = new Mock<IMasterDataStore>();
            mockStore.Setup(x => x.MastersQueryDb).Returns(mockDb.Object);
            MasterDataStore = mockStore.Object;
        }
    }
}

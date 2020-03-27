using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Orm.Classes;

using iBank.Entities.MasterEntities;
using iBank.ReportServer.Helpers;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using TechTalk.SpecFlow;

namespace iBank.UnitTesting.iBank.ReportServer.Helpers
{
    [Binding]
    public class PendingReportsRetrieverSteps
    {
        private PendingReportRetrieverContext _context;

        private List<PendingReportInformation> _output;

        public PendingReportsRetrieverSteps(PendingReportRetrieverContext context)
        {
            _context = new PendingReportRetrieverContext();
        }

        [Given(@"the report server is functioning as '(.*)'")]
        public void GivenTheReportServerIsFunctioningAs(string function)
        {
            if (function == ReportServerFunction.Primary.ToString()) _context.Function = ReportServerFunction.Primary;
            else if (function == ReportServerFunction.Stage.ToString()) _context.Function = ReportServerFunction.Stage;
            else throw new System.Exception($"Function parameter [{function}] not recognized!");
        }
        
        [Given(@"the server '(.*)' in DevMode")]
        public void GivenTheServerInDevMode(string isInDevMode)
        {
            if (isInDevMode == "is") _context.IsDevMode = true;
            else if (isInDevMode == "is not") _context.IsDevMode = false;
            else throw new System.Exception($"IsInDevMode parameter [{isInDevMode}] not recognized!");
        }
        
        [Given(@"a report for agency '(.*)' with id '(.*)' '(.*)' pending")]
        public void GivenAReportForAgencyWithIdPending(string agency, string reportId, string isPending)
        {
            var status = isPending == "is" ? "PENDING" : "RUNNING";

            _context.Reports.Add(new reporthandoff
                                     {
                                         agency = agency,
                                         reportid = reportId,
                                         parmname = "REPORTSTATUS",
                                         parmvalue = status,
                                         cfbox = "1",
                                         usernumber = 1,
                                         svrnumber = 0
                                     });
        }

        private int stageId = 1;
        [Given(@"the agency '(.*)' is on stage")]
        public void GivenTheAgencyIsOnStage(string agency)
        {
            _context.ReportServerStage.Add(new report_server_stage { agency = agency, database_name = "", id = stageId });
            stageId++;
        }
        
        [Given(@"the database '(.*)' is on stage")]
        public void GivenTheDatabaseIsOnStage(string database)
        {
            _context.ReportServerStage.Add(new report_server_stage { agency = "", database_name = database,  id = stageId });
            stageId++;
        }

        [Given(@"the agency '(.*)' belongs to database '(.*)'")]
        public void GivenTheAgencyBelongsToDatabase(string agency, string database)
        {
            _context.MstrAgcy.Add(new mstragcy { agency = agency, databasename = database });
        }

        [Given(@"the pending reports table has a report id '(.*)'")]
        public void GivenThePendingReportsTableHasAReportId(string reportId)
        {
            _context.PendingReports.Add(new PendingReports() { ReportId =  reportId, IsDotNet = true, IsRunning = false});
        }

        [When(@"I retrieve the reports")]
        public void WhenIRetrieveTheReports()
        {
            var mockDatabase = new Mock<IMastersQueryable>();
            mockDatabase.Setup(x => x.MstrAgcy).Returns(MockHelper.GetListAsQueryable(_context.MstrAgcy).Object);
            mockDatabase.Setup(x => x.ReportHandoff).Returns(MockHelper.GetListAsQueryable(_context.Reports).Object);
            mockDatabase.Setup(x => x.ReportServerStage).Returns(MockHelper.GetListAsQueryable(_context.ReportServerStage).Object);
            mockDatabase.Setup(x => x.PendingReports).Returns(MockHelper.GetListAsQueryable(_context.PendingReports).Object);

            var mockStore = new Mock<IMasterDataStore>();
            mockStore.Setup(x => x.MastersQueryDb).Returns(mockDatabase.Object);
            mockStore.Setup(x => x.MastersCommandDb);

            var retriever = new PendingReportsRetriever();
            _output = retriever.GetPendingReports(mockStore.Object, new List<int>(), 1, _context.IsDevMode, _context.Function).ToList();
        }
        
        [Then(@"I have a total of '(.*)' reports")]
        public void ThenIHaveATotalOfReports(int numberOfReports)
        {
            Assert.AreEqual(numberOfReports, _output.Count);
        }
        
        [Then(@"the reportid '(.*)' exists")]
        public void ThenTheReportIdExists(string reportId)
        {
            var report = _output.First(x => x.ReportId == reportId);

            if (report == null) Assert.IsFalse(false, $"Report id [{reportId}] not found");
            else Assert.IsTrue(true);
        }
    }
}

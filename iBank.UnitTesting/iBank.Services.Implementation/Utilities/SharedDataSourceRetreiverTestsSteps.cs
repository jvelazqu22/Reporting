using Domain.Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Utilities.ClientData;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using TechTalk.SpecFlow;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities
{
    [Binding]
    public class SharedDataSourceRetreiverTestsSteps
    {
        private SharedDataSourceRetrieverContext _context;

        private IList<CorpAccountDataSource> _results;

        public SharedDataSourceRetreiverTestsSteps(SharedDataSourceRetrieverContext context)
        {
            _context = new SharedDataSourceRetrieverContext();
        }

        [Given(@"I am searching under master corp account '(.*)'")]
        public void GivenIAmSearchingUnderMasterCorpAccount(string corpAccountName)
        {
            _context.CorpAcctName = corpAccountName;
        }

        [Given(@"'(.*)' is an agency under '(.*)' can access acct '(.*)' and sourceabbr '(.*)'")]
        public void GivenIsAnAgencyUnder(string agencyName, string corpAccountName, string acct, string sourceabbr)
        {
            _context.CorpAcctNbrs.Add(new CorpAcctNbrs { Acct = acct, SourceAbbr = sourceabbr, CorpAcct = _context.CorpAcctName, Agency = agencyName });
        }

        [Given(@"'(.*)' is located on database '(.*)'")]
        public void GivenIsLocatedOnDatabase(string agencyName, string databaseName)
        {
            _context.MasterAgency.Add(new mstragcy { agency = agencyName, databasename = databaseName });
        }

        [Given(@"'(.*)' is located on server '(.*)'")]
        public void GivenIsLocatedOnServer(string databaseName, string serverName)
        {
            _context.iBankDatabases.Add(new iBankDatabases { databasename = databaseName, server_address = serverName });
        }

        [When(@"I get all the datasources")]
        public void WhenIGetAllTheDatasources()
        {
            var mockDatabase = new Mock<IMastersQueryable>();
            mockDatabase.Setup(x => x.CorpAcctNbrs).Returns(MockHelper.GetListAsQueryable(_context.CorpAcctNbrs).Object);
            mockDatabase.Setup(x => x.MstrAgcy).Returns(MockHelper.GetListAsQueryable(_context.MasterAgency).Object);
            mockDatabase.Setup(x => x.iBankDatabases).Returns(MockHelper.GetListAsQueryable(_context.iBankDatabases).Object);

            var mockStore = new Mock<IMasterDataStore>();
            mockStore.Setup(x => x.MastersQueryDb).Returns(mockDatabase.Object);

            var retriever = new SharedDataSourceRetriever(mockStore.Object);
            _results = retriever.GetDataSourcesForAllAgencies(_context.CorpAcctName);
        }

        [Then(@"I have '(.*)' data sources")]
        public void ThenIHaveDataSources(int numberOfDataSources)
        {
            Assert.AreEqual(numberOfDataSources, _results.Count);
        }

        [Then(@"the data source with database '(.*)' is on server '(.*)' and agency '(.*)'")]
        public void TheDataSourceWithDatabaseIsOnServer(string databaseName, string serverName, string agency)
        {
            var serverAddress = _results.First(x => x.DataSource.DatabaseName == databaseName && x.Agency == agency).DataSource.ServerAddress;
            Assert.AreEqual(serverName, serverAddress);
        }
    }
}

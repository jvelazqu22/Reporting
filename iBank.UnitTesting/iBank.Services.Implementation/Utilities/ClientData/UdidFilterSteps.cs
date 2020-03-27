using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Utilities.ClientData;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using TechTalk.SpecFlow;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.ClientData
{
    [Binding]
    public sealed class UdidFilterSteps
    {
        private UdidFilterContext _context;

        public UdidFilterSteps()
        {
            _context = new UdidFilterContext();
        }

        [Given(@"I have a data record with a reckey of '(.*)'")]
        public void GivenIHaveADataRecordWithAReckeyOf(int reckey)
        {
            _context.MockData.Add(new MockDataUdidFilter(reckey));
        }

        [Given(@"I have a udid filter with a udid number of (.*) and a udid text of '(.*)' and an operator of '(.*)'")]
        public void GivenIHaveAUdidFilterWithAUdidNumberOfAndAUdidTextOfAndAnOperatorOf(int udidNumber, string udidText, string oper)
        {
            Operator op;
            if (!Enum.TryParse(oper, out op)) throw new Exception($"Unrecognized operator type of [{oper}] supplied.");
            _context.MultiUdidParameters.Parameters.Add(new AdvancedParameter { FieldName = udidNumber.ToString(), Value1 = udidText, Operator = op });
        }

        [Given(@"I have a join type of '(.*)'")]
        public void GivenIHaveAJoinTypeOf(string andOr)
        {
            switch (andOr.ToUpper())
            {
                case "AND":
                    _context.MultiUdidParameters.AndOr = AndOr.And;
                    break;
                case "OR":
                    _context.MultiUdidParameters.AndOr = AndOr.Or;
                    break;
                default:
                    throw new Exception("Unrecognized AndOr value supplied.");
            }
        }

        [Given(@"I have a udid record with a reckey '(.*)' and udid number of '(.*)' and udid text of '(.*)'")]
        public void GivenIHaveAUdidRecordWithAReckeyAndUdidNumberOfAndUdidTextOf(int reckey, int udidNumber, string udidText)
        {
            _context.Udids.Add(new UdidRecord { RecKey = reckey, UdidNumber = udidNumber, UdidText = udidText });
        }
        
        [When(@"I filter the data")]
        public void WhenIFilterTheData()
        {
            var mock = new Mock<TripUdidRetriever>();
            mock.Setup(x => x.GetUdids(It.IsAny<List<MockDataUdidFilter>>(), It.IsAny<ReportGlobals>(), It.IsAny<bool>())).Returns(_context.Udids);
            _context.Globals.MultiUdidParameters = _context.MultiUdidParameters;
            var sut = new UdidFilter();

            _context.Results = sut.GetUdidFilteredData(_context.MockData, _context.Globals, true, mock.Object);
        }

        [Then(@"the reckey '(.*)' does not exist")]
        public void ThenTheReckeyDoesNotExist(int reckey)
        {
            Assert.AreEqual(false, _context.Results.Any(x => x.RecKey == reckey));
        }

        [Then(@"the reckey '(.*)' does exist")]
        public void ThenTheReckeyDoesExist(int reckey)
        {
            Assert.AreEqual(true, _context.Results.Any(x => x.RecKey == reckey));
        }

    }
}

using System;
using System.Linq;

using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.MultiUdid;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TechTalk.SpecFlow;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.MultiUdid
{
    [Binding]
    public class MultiUdidConditionalFeatureSteps
    {
        private MultiUdidConditionalFeatureContext _context;

        public MultiUdidConditionalFeatureSteps()
        {
            _context = new MultiUdidConditionalFeatureContext();
        }

        [Given(@"I have a record with a reckey of '(.*)' and a udid number of '(.*)' and a udid text of '(.*)'")]
        public void GivenIHaveARecordWithAReckeyOfAndAUdidNumberOfAndAUdidTextOf(int reckey, int udidNumber, string udidText)
        {
            _context.UdidRecords.Add(new UdidRecord { RecKey = reckey, UdidNumber = udidNumber, UdidText = udidText });
        }
        
        [Given(@"I have a udid parameter with a udid number of '(.*)' and a udid text of '(.*)' and an operator of '(.*)'")]
        public void GivenIHaveAUdidParameterWithAUdidNumberOfAndAUdidTextOfAndAnOperatorOf(int udidNumber, string udidText, string operatorType)
        {
            Operator op;
            if (!Enum.TryParse(operatorType, out op)) throw new Exception($"Unrecognized operator type of [{operatorType}] supplied.");
            _context.MultiUdidParameters.Add(new AdvancedParameter { FieldName = udidNumber.ToString(), Value1 = udidText, Operator = op });
        }

        [Given(@"I have an AndOr of '(.*)'")]
        public void GivenIHaveAnAndOrOf(string andOr)
        {
            switch (andOr.ToUpper())
            {
                case "AND":
                    _context.AndOr = AndOr.And;
                    break;
                case "OR":
                    _context.AndOr = AndOr.Or;
                    break;
                default:
                    throw new Exception("Unrecognized AndOr value supplied.");
            }
        }

        [When(@"I get the reckeys to keep")]
        public void WhenIGetTheReckeysToKeep()
        {
            var globals = new ReportGlobals();
            globals.MultiUdidParameters.AndOr = _context.AndOr;
            globals.MultiUdidParameters.Parameters = _context.MultiUdidParameters;

            var sut = new MultiUdidCriteria();
            _context.Result = sut.GetRecKeysToKeep(globals, _context.UdidRecords);
        }

        [Then(@"I have '(.*)' distinct reckeys remaining")]
        public void ThenIHaveDistinctReckeysRemaining(int totalRecordsRemaining)
        {
            Assert.AreEqual(totalRecordsRemaining, _context.Result.Distinct().Count());
        }
        
        [Then(@"the reckey '(.*)' still exists")]
        public void ThenTheReckeyStillExists(int existingRecord)
        {
            Assert.AreEqual(true, _context.Result.Any(x => x == existingRecord));
        }
        
        [Then(@"the reckey '(.*)' no longer exists")]
        public void ThenTheReckeyNoLongerExists(int recordThatNoLongerExists)
        {
            Assert.AreEqual(false, _context.Result.Any(x => x == recordThatNoLongerExists));
        }
    }
}

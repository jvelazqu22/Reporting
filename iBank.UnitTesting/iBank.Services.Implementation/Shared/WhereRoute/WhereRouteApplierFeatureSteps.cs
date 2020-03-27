using System;
using System.Linq;

using iBank.Services.Implementation.Shared.WhereRoute;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TechTalk.SpecFlow;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.WhereRoute
{
    [Binding]
    public class WhereRouteApplierFeatureSteps
    {
        private WhereRouteApplierFeatureContext _context;

        private WhereRouteApplier<DataForContext> _applier;

        public WhereRouteApplierFeatureSteps()
        {
            _context = new WhereRouteApplierFeatureContext();
            _applier = new WhereRouteApplier<DataForContext>();
        }

        [Given(@"reckey '(.*)' leg '(.*)' has an origin of '(.*)' and a destination of '(.*)'")]
        public void GivenReckeyLegHasAnOriginOfAndADestinationOf(int reckey, int seqno, string origin, string destination)
        {
            _context.AddData(reckey, seqno, origin, destination);
        }
        
        [Given(@"I have origin criteria of '(.*)'")]
        public void GivenIHaveOriginCriteriaOf(string originCriteria)
        {
            _context.OriginCriteria.Add(originCriteria);
        }

        [Given(@"I have criteria of '(.*)'")]
        public void GivenIHaveCriteriaOf(string originCriteria)
        {
            _context.OriginCriteria.Add(originCriteria);
        }


        [Given(@"I have destination criteria of '(.*)'")]
        public void GivenIHaveDestinationCriteriaOf(string destinationCriteria)
        {
            _context.DestinationCriteria.Add(destinationCriteria);
        }

        [Given(@"I '(.*)' want to return all legs")]
        public void GivenIWantToReturnAllLegs(string desireToReturnAllLegs)
        {
            switch (desireToReturnAllLegs.ToLower())
            {
                case "do":
                    _context.ReturnAllLegs = true;
                    break;
                case "do not":
                    _context.ReturnAllLegs = false;
                    break;
                default:
                    throw new Exception($"Unrecognized argument {desireToReturnAllLegs} supplied.");
            }
            
        }

        [Given(@"I want data that '(.*)' in the criteria")]
        public void GivenIWantDataThatInTheCriteria(string isIn)
        {
            switch (isIn.ToLower())
            {
                case "is":
                    _context.NotInList = false;
                    break;
                case "is not":
                    _context.NotInList = true;
                    break;
                default:
                    throw new Exception($"Unrecognized argument {isIn} supplied.");
            }
        }
        
        [When(@"I filter the data on origin criteria")]
        public void WhenIFilterTheDataOnOriginCriteria()
        {
            _context.Data = _applier.GetDataBasedOnOriginCriteria(_context.Data, _context.OriginCriteria, _context.NotInList, _context.ReturnAllLegs);
        }

        [When(@"I filter the data on destination criteria")]
        public void WhenIFilterTheDataOnDestinationCriteria()
        {
            _context.Data = _applier.GetDataBasedOnDestinationCriteria(_context.Data, _context.DestinationCriteria, _context.NotInList, _context.ReturnAllLegs);
        }

        [When(@"I filter the data on origin and destination criteria")]
        public void WhenIFilterTheDataOnOriginAndDestinationCriteria()
        {
            _context.Data = _applier.GetDataBasedOnOriginAndDestinationCriteria(_context.Data, _context.OriginCriteria, _context.DestinationCriteria,
                _context.NotInList, _context.ReturnAllLegs);
        }

        [When(@"I filter the data on origin or destination criteria")]
        public void WhenIFilterTheDataOnOriginOrDestinationCriteria()
        {
            var list = _context.OriginCriteria.ToList();
            list.AddRange(_context.DestinationCriteria);
            _context.OriginCriteria = list;

            _context.Data = _applier.GetDataBasedOnOriginOrDestinationCriteria(_context.Data, _context.OriginCriteria, _context.NotInList,
                _context.ReturnAllLegs);
        }
        
        [Then(@"I have '(.*)' total records")]
        public void ThenIHaveTotalRecords(int totalRecords)
        {
            Assert.AreEqual(totalRecords, _context.Data.Count);
        }
        
        [Then(@"reckey '(.*)' exists")]
        public void ThenReckeyExists(int reckey)
        {
            Assert.AreEqual(true, _context.Data.Any(x => x.RecKey == reckey));
        }
        
        [Then(@"reckey '(.*)' does not exist")]
        public void ThenReckeyDoesNotExist(int reckey)
        {
            Assert.AreEqual(false, _context.Data.Any(x => x.RecKey == reckey));
        }
        
        [Then(@"a record with origin '(.*)' exists")]
        public void ThenARecordWithOriginExists(string origin)
        {
            Assert.AreEqual(true, _context.Data.Any(x => x.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase)));
        }

        [Then(@"reckey '(.*)' and leg '(.*)' exists")]
        public void ThenReckeyAndLegExists(int reckey, int leg)
        {
            Assert.AreEqual(true, _context.Data.Any(x => x.RecKey == reckey && x.SeqNo == leg));
        }

        [Then(@"reckey '(.*)' and leg '(.*)' does not exist")]
        public void ThenReckeyAndLegDoesNotExist(int reckey, int leg)
        {
            Assert.AreEqual(false, _context.Data.Any(x => x.RecKey == reckey && x.SeqNo == leg));
        }
   
    }
}

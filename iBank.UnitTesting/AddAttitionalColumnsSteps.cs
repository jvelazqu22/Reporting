using System;
using TechTalk.SpecFlow;

namespace iBank.UnitTesting
{
    [Binding]
    public class AddAttitionalColumnsSteps
    {
        // test check in
        [Given(@"Pass a collection of columns '(.*)'")]
        public void GivenPassACollectionOfColumns(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"I ask for result")]
        public void WhenIAskForResult()
        {
            ScenarioContext.Current.Pending();
        }
    }
}

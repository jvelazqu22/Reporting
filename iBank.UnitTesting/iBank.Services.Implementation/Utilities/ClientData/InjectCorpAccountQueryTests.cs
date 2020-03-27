using System;
using System.Collections.Generic;
using iBank.Services.Implementation.Utilities.ClientData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.ClientData
{
    [TestClass]
    public class InjectCorpAccountQueryTests
    {
        [TestMethod]
        public void InjectCorpAccountQuery_AgencyCorpAcct_MatchResult()
        {
            var sql = "select something from sometable where something else";
            var agencies = new List<string> { "agency", "agency2" };
            var corpAcct = "corpAcct";

            var exp = $"select something from sometable where Agency = '{corpAcct}' and something else";

            var act = ClientDataRetrieval.InjectCorpAccountQuery(sql, agencies, corpAcct);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void InjectCorpAccountQuery_AgencyCorpAcctWithT1_MatchResult()
        {
            var sql = "select something from sometable T1 where something else";
            var agencies = new List<string> { "agency", "agency2" };
            var corpAcct = "corpAcct";

            var exp = $"select something from sometable T1 where T1.Agency = '{corpAcct}' and something else";

            var act = ClientDataRetrieval.InjectCorpAccountQuery(sql, agencies, corpAcct);

            Assert.AreEqual(exp, act);
        }
                        
        [TestMethod]
        public void InjectCorpAccountQuery_WithibTripsAgencyCorpAcct_MatchResult()
        {
            var sql = "select something from ibtrips T1 where something else";
            var agencies = new List<string> { "agency", "agency2" };
            var corpAcct = "corpAcct";

            var exp = $"select something from ibtrips T1 where T1.CorpAcct = '{corpAcct}' and T1.agency in ('{string.Join("','", agencies)}') and something else";

            var act = ClientDataRetrieval.InjectCorpAccountQuery(sql, agencies, corpAcct);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void InjectCorpAccountQuery_WithibTripsOthersAgencyCorpAcct_MatchResult()
        {
            var sql = "select something from ibtrips T1, others where something else";
            var agencies = new List<string> { "agency", "agency2" };
            var corpAcct = "corpAcct";

            var exp = $"select something from ibtrips T1, others where T1.CorpAcct = '{corpAcct}' and T1.agency in ('{string.Join("','", agencies)}') and something else";

            var act = ClientDataRetrieval.InjectCorpAccountQuery(sql, agencies, corpAcct);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void InjectCorpAccountQuery_WithHIBTripsAgencyCorpAcct_MatchResult()
        {
            var sql = "select something from hibtrips T1 where something else";
            var agencies = new List<string> { "agency", "agency2" };
            var corpAcct = "corpAcct";

            var exp = $"select something from hibtrips T1 where T1.CorpAcct = '{corpAcct}' and T1.agency in ('{string.Join("','", agencies)}') and something else";

            var act = ClientDataRetrieval.InjectCorpAccountQuery(sql, agencies, corpAcct);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void InjectCorpAccountQuery_WithHIBTripsAndOthersAgencyCorpAcct_MatchResult()
        {
            var sql = "select something from hibtrips T1, others where something else";
            var agencies = new List<string> { "agency", "agency2" };
            var corpAcct = "corpAcct";

            var exp = $"select something from hibtrips T1, others where T1.CorpAcct = '{corpAcct}' and T1.agency in ('{string.Join("','", agencies)}') and something else";

            var act = ClientDataRetrieval.InjectCorpAccountQuery(sql, agencies, corpAcct);

            Assert.AreEqual(exp, act);
        }
    }
}

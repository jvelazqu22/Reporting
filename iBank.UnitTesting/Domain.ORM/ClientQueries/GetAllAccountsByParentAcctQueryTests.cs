using System.Collections.Generic;
using System.Linq;

using Domain.Exceptions;
using Domain.Orm.iBankClientQueries;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.Domain.ORM.ClientQueries
{
    [TestClass]
    public class GetAllAccountsByParentAcctQueryTests
    {
        [TestMethod]
        public void ExcecuteQuery_NoWildcard_OneParentAcct()
        {
            var parentAcct = "1234";
            var db = new Mock<IClientQueryable>();
            var data = new List<acctmast>
            {
                new acctmast { acct = "foo", parentacct = "1234" },
                new acctmast { acct = "bar", parentacct = "5678" }
            };
            db.Setup(x => x.AcctMast).Returns(MockHelper.GetListAsQueryable(data).Object);
            var sut = new GetAllAccountsByParentAcctQuery(db.Object, parentAcct);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("foo", output[0]);
        }

        [TestMethod]
        public void ExcecuteQuery_NoWildcard_MultipleParentAcct()
        {
            var parentAcct = new List<string> { "1234", "5678" };
            var db = new Mock<IClientQueryable>();
            var data = new List<acctmast>
            {
                new acctmast { acct = "foo", parentacct = "1234" },
                new acctmast { acct = "bar", parentacct = "5678" },
                new acctmast { acct = "foobar", parentacct = "1" }
            };
            db.Setup(x => x.AcctMast).Returns(MockHelper.GetListAsQueryable(data).Object);
            var sut = new GetAllAccountsByParentAcctQuery(db.Object, parentAcct);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(2, output.Count);
            Assert.AreEqual(true, output.Any(x => x.Equals("foo")));
            Assert.AreEqual(true, output.Any(x => x.Equals("bar")));
        }

        [TestMethod]
        public void ExcecuteQuery_WithPercentWildcard_AtStart()
        {
            var parentAcct = "%34";
            var db = new Mock<IClientQueryable>();
            var data = new List<acctmast>
            {
                new acctmast { acct = "foo", parentacct = "1234" },
                new acctmast { acct = "bar", parentacct = "5678" }
            };
            db.Setup(x => x.AcctMast).Returns(MockHelper.GetListAsQueryable(data).Object);
            var sut = new GetAllAccountsByParentAcctQuery(db.Object, parentAcct, true);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("foo", output[0]);
        }

        [TestMethod]
        public void ExcecuteQuery_WithPercentWildcard_AtEnd()
        {
            var parentAcct = "12%";
            var db = new Mock<IClientQueryable>();
            var data = new List<acctmast>
            {
                new acctmast { acct = "foo", parentacct = "1234" },
                new acctmast { acct = "bar", parentacct = "5678" }
            };
            db.Setup(x => x.AcctMast).Returns(MockHelper.GetListAsQueryable(data).Object);
            var sut = new GetAllAccountsByParentAcctQuery(db.Object, parentAcct, true);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("foo", output[0]);
        }

        [TestMethod]
        public void ExcecuteQuery_WithAsteriskWildcard_AtStart()
        {
            var parentAcct = "*34";
            var db = new Mock<IClientQueryable>();
            var data = new List<acctmast>
            {
                new acctmast { acct = "foo", parentacct = "1234" },
                new acctmast { acct = "bar", parentacct = "5678" }
            };
            db.Setup(x => x.AcctMast).Returns(MockHelper.GetListAsQueryable(data).Object);
            var sut = new GetAllAccountsByParentAcctQuery(db.Object, parentAcct, true);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("foo", output[0]);
        }

        [TestMethod]
        public void ExcecuteQuery_WithAsteriskWildcard_AtEnd()
        {
            var parentAcct = "12*";
            var db = new Mock<IClientQueryable>();
            var data = new List<acctmast>
            {
                new acctmast { acct = "foo", parentacct = "1234" },
                new acctmast { acct = "bar", parentacct = "5678" }
            };
            db.Setup(x => x.AcctMast).Returns(MockHelper.GetListAsQueryable(data).Object);
            var sut = new GetAllAccountsByParentAcctQuery(db.Object, parentAcct, true);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("foo", output[0]);
        }

        [TestMethod]
        public void ExcecuteQuery_WithUnderscoreWildcard_AtStart()
        {
            var parentAcct = "_234";
            var db = new Mock<IClientQueryable>();
            var data = new List<acctmast>
            {
                new acctmast { acct = "foo", parentacct = "1234" },
                new acctmast { acct = "bar", parentacct = "5678" }
            };
            db.Setup(x => x.AcctMast).Returns(MockHelper.GetListAsQueryable(data).Object);
            var sut = new GetAllAccountsByParentAcctQuery(db.Object, parentAcct, true);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("foo", output[0]);
        }

        [TestMethod]
        public void ExcecuteQuery_WithUnderscoreWildcard_AtEnd()
        {
            var parentAcct = "123_";
            var db = new Mock<IClientQueryable>();
            var data = new List<acctmast>
            {
                new acctmast { acct = "foo", parentacct = "1234" },
                new acctmast { acct = "bar", parentacct = "5678" }
            };
            db.Setup(x => x.AcctMast).Returns(MockHelper.GetListAsQueryable(data).Object);
            var sut = new GetAllAccountsByParentAcctQuery(db.Object, parentAcct, true);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("foo", output[0]);
        }

        [TestMethod]
        public void ExcecuteQuery_WithQuestionMarkWildcard_AtStart()
        {
            var parentAcct = "?234";
            var db = new Mock<IClientQueryable>();
            var data = new List<acctmast>
            {
                new acctmast { acct = "foo", parentacct = "1234" },
                new acctmast { acct = "bar", parentacct = "5678" }
            };
            db.Setup(x => x.AcctMast).Returns(MockHelper.GetListAsQueryable(data).Object);
            var sut = new GetAllAccountsByParentAcctQuery(db.Object, parentAcct, true);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("foo", output[0]);
        }

        [TestMethod]
        public void ExcecuteQuery_WithQuestionMarkWildcard_AtEnd()
        {
            var parentAcct = "123?";
            var db = new Mock<IClientQueryable>();
            var data = new List<acctmast>
            {
                new acctmast { acct = "foo", parentacct = "1234" },
                new acctmast { acct = "bar", parentacct = "5678" }
            };
            db.Setup(x => x.AcctMast).Returns(MockHelper.GetListAsQueryable(data).Object);
            var sut = new GetAllAccountsByParentAcctQuery(db.Object, parentAcct, true);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("foo", output[0]);
        }

        [ExpectedException(typeof(UnknownWildcardException))]
        [TestMethod]
        public void ExecuteQuery_UnknownWildcard_ThrowException()
        {
            var parentAcct = "^234";
            var db = new Mock<IClientQueryable>();
            var data = new List<acctmast>
            {
                new acctmast { acct = "foo", parentacct = "1234" },
                new acctmast { acct = "bar", parentacct = "5678" }
            };
            db.Setup(x => x.AcctMast).Returns(MockHelper.GetListAsQueryable(data).Object);
            var sut = new GetAllAccountsByParentAcctQuery(db.Object, parentAcct, true);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("foo", output[0]);
        }
    }
}

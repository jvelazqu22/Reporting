using iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    [TestClass]
    public class ExecSummaryWhereClauseTransformerTests
    {
        private string _whereClause = "where T1.trantype = T2.trantype";

        private ExecSummaryWhereClauseTransformer _transformer = new ExecSummaryWhereClauseTransformer();

        [TestMethod]
        public void TransformWhereClause_AirType()
        {
            var output = _transformer.TransformWhereClause(_whereClause, ExecSummaryWhereClauseTransformer.SqlType.Air);

            Assert.AreEqual(_whereClause, output);
        }

        [TestMethod]
        public void TransformWhereClause_LegType()
        {
            var output = _transformer.TransformWhereClause(_whereClause, ExecSummaryWhereClauseTransformer.SqlType.Leg);

            Assert.AreEqual(_whereClause, output);
        }

        [TestMethod]
        public void TransformWhereClause_CarType()
        {
            var output = _transformer.TransformWhereClause(_whereClause, ExecSummaryWhereClauseTransformer.SqlType.Car);
            var expected = "where T4.CarTranTyp = T2.trantype";

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void TransformWhereClause_HhotelType()
        {
            var output = _transformer.TransformWhereClause(_whereClause, ExecSummaryWhereClauseTransformer.SqlType.Hotel);
            var expected = "where T5.HotTranTyp = T2.trantype";

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void TransformWhereClause_FeeType()
        {
            var output = _transformer.TransformWhereClause(_whereClause, ExecSummaryWhereClauseTransformer.SqlType.SvcFee);
            var expected = "where T6A.sfTrantype = T2.trantype";

            Assert.AreEqual(expected, output);
        }
    }
}

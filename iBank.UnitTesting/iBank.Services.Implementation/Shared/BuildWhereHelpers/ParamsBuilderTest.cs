using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Domain;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class ParamsBuilderTest
    {

        //[TestMethod]
        //public void GetBaseCounterForNextFieldNameParameterToBeAdded_ParamIsASubString_ShouldReturnZero()
        //{
        //    // Arrange
        //    string clauseParam = "T1.trantype <> @t1TranType1  AND  (T1.acct = @T1acct0 OR T1.acct = @T1acct1 OR T1.acct = @T1acct2)";
        //    string fieldNameParam = "T1.acc";
        //    int expectedResult = 0;
        //    int result = -1;

        //    // Act
        //    result = new ParamsBuilder().GetBaseCounterForNextFieldNameParameterToBeAdded(clauseParam, fieldNameParam);

        //    // Assert
        //    Assert.AreEqual(result, expectedResult);
        //}

        [TestMethod]
        public void GetBaseCounterForNextFieldNameParameterToBeAdded_OneInstanceFound_ReturnTheNumberOfInstancesFound()
        {
            // Arrange
            string clauseParam = "T1.trantype <> @t1TranType1  AND  (T1.acct = @T1acct0 OR T1.acct = @T1acct1 OR T1.acct = @T1acct2)";
            string fieldNameParam = "T1.acct";
            int expectedResult = 3;
            int result = -1;

            // Act
            result = new ParamsBuilder().GetBaseCounterForNextFieldNameParameterToBeAdded(clauseParam, fieldNameParam);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetBaseCounterForNextFieldNameParameterToBeAdded_NoInstanceFound_ReturnZero()
        {
            // Arrange
            string clauseParam = "T1.trantype <> @t1TranType1  AND  (T1.acct = @T1acct0 OR T1.acct = @T1acct1 OR T1.acct = @T1acct2)";
            string fieldNameParam = "T1.acct";
            int expectedResult = 3;
            int result = -1;

            // Act
            result = new ParamsBuilder().GetBaseCounterForNextFieldNameParameterToBeAdded(clauseParam, fieldNameParam);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void RemovePeriodFromFieldName_StringWithPeriod_ReturnStringWithoutPeriod()
        {
            // Arrange
            string param = "t1.acct";
            string expectedResult = "t1acct";
            string result = string.Empty;

            // Act
            result = new ParamsBuilder().RemovePeriodFromFieldName(param);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void RemovePeriodFromFieldName_StringWithNoPeriod_ReturnSameString()
        {
            // Arrange
            string param = "t1acct";
            string expectedResult = "t1acct";
            string result = string.Empty;

            // Act
            result = new ParamsBuilder().RemovePeriodFromFieldName(param);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void AddOrListToWhereClause_OneValueInPicklist_NotWildcard()
        {
            var clause = " T1.trantype <> @t1TranType1 ";
            var picklist = new List<string> { "1188" };
            var fieldName = "T1.acct";
            var isNotIn = false;
            var sqlParameters = new List<SqlParameter>();
            var sut = new ParamsBuilder();

            var output = sut.AddOrListToWhereClause(clause, picklist, fieldName, isNotIn, sqlParameters);

            Assert.AreEqual(" T1.trantype <> @t1TranType1   AND  (T1.acct = @T1acct0)", output);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct0")));
            Assert.AreEqual("1188", sqlParameters.First(x => x.ParameterName.Equals("T1acct0")).Value);
        }
        
        [TestMethod]
        public void AddOrListToWhereClause_TwoValuesInPicklist_NoWildcards()
        {
            var clause = " T1.trantype <> @t1TranType1 ";
            var picklist = new List<string> { "1188", "1200" };
            var fieldName = "T1.acct";
            var isNotIn = false;
            var sqlParameters = new List<SqlParameter>();
            var sut = new ParamsBuilder();

            var output = sut.AddOrListToWhereClause(clause, picklist, fieldName, isNotIn, sqlParameters);

            Assert.AreEqual(" T1.trantype <> @t1TranType1   AND  (T1.acct = @T1acct0 OR T1.acct = @T1acct1)", output);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct0")));
            Assert.AreEqual("1188", sqlParameters.First(x => x.ParameterName.Equals("T1acct0")).Value);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct1")));
            Assert.AreEqual("1200", sqlParameters.First(x => x.ParameterName.Equals("T1acct1")).Value);
        }

        [TestMethod]
        public void AddOrListToWhereClause_OneValueInPicklist_NotWildcard_IsNotIn()
        {
            var clause = " T1.trantype <> @t1TranType1 ";
            var picklist = new List<string> { "1188" };
            var fieldName = "T1.acct";
            var isNotIn = true;
            var sqlParameters = new List<SqlParameter>();
            var sut = new ParamsBuilder();

            var output = sut.AddOrListToWhereClause(clause, picklist, fieldName, isNotIn, sqlParameters);

            Assert.AreEqual(" T1.trantype <> @t1TranType1   AND   NOT (T1.acct = @T1acct0)", output);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct0")));
            Assert.AreEqual("1188", sqlParameters.First(x => x.ParameterName.Equals("T1acct0")).Value);
        }

        [TestMethod]
        public void AddOrListToWhereClause_TwoValuesInPicklist_NoWildcards_IsNotIn()
        {
            var clause = " T1.trantype <> @t1TranType1 ";
            var picklist = new List<string> { "1188", "1200" };
            var fieldName = "T1.acct";
            var isNotIn = true;
            var sqlParameters = new List<SqlParameter>();
            var sut = new ParamsBuilder();

            var output = sut.AddOrListToWhereClause(clause, picklist, fieldName, isNotIn, sqlParameters);

            Assert.AreEqual(" T1.trantype <> @t1TranType1   AND   NOT (T1.acct = @T1acct0 OR T1.acct = @T1acct1)", output);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct0")));
            Assert.AreEqual("1188", sqlParameters.First(x => x.ParameterName.Equals("T1acct0")).Value);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct1")));
            Assert.AreEqual("1200", sqlParameters.First(x => x.ParameterName.Equals("T1acct1")).Value);
        }

        [TestMethod]
        public void AddOrListToWhereClause_OneValueInPicklist_ContainsWildcard()
        {
            var clause = " T1.trantype <> @t1TranType1 ";
            var picklist = new List<string> { "118%" };
            var fieldName = "T1.acct";
            var isNotIn = false;
            var sqlParameters = new List<SqlParameter>();
            var sut = new ParamsBuilder();

            var output = sut.AddOrListToWhereClause(clause, picklist, fieldName, isNotIn, sqlParameters);

            Assert.AreEqual(" T1.trantype <> @t1TranType1   AND  (T1.acct LIKE '118%')", output);
            //Assert.AreEqual(" T1.trantype <> @t1TranType1   AND  (T1.acct LIKE @T1acct0)", output);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct0")));
            Assert.AreEqual("118%", sqlParameters.First(x => x.ParameterName.Equals("T1acct0")).Value);
        }

        [TestMethod]
        public void AddOrListToWhereClause_TwoValuesInPicklist_BothContainWildcards()
        {
            var clause = " T1.trantype <> @t1TranType1 ";
            var picklist = new List<string> { "118%", "120_" };
            var fieldName = "T1.acct";
            var isNotIn = false;
            var sqlParameters = new List<SqlParameter>();
            var sut = new ParamsBuilder();

            var output = sut.AddOrListToWhereClause(clause, picklist, fieldName, isNotIn, sqlParameters);

            Assert.AreEqual(" T1.trantype <> @t1TranType1   AND  (T1.acct LIKE '118%' OR T1.acct LIKE '120_')", output);
            //Assert.AreEqual(" T1.trantype <> @t1TranType1   AND  (T1.acct LIKE @T1acct0 OR T1.acct LIKE @T1acct1)", output);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct0")));
            Assert.AreEqual("118%", sqlParameters.First(x => x.ParameterName.Equals("T1acct0")).Value);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct1")));
            Assert.AreEqual("120_", sqlParameters.First(x => x.ParameterName.Equals("T1acct1")).Value);
        }

        [TestMethod]
        public void AddOrListToWhereClause_TwoValuesInPicklist_OneContainsWildcards()
        {
            var clause = " T1.trantype <> @t1TranType1 ";
            var picklist = new List<string> { "118%", "1200" };
            var fieldName = "T1.acct";
            var isNotIn = false;
            var sqlParameters = new List<SqlParameter>();
            var sut = new ParamsBuilder();

            var output = sut.AddOrListToWhereClause(clause, picklist, fieldName, isNotIn, sqlParameters);

            //Assert.AreEqual(" T1.trantype <> @t1TranType1   AND  (T1.acct LIKE @T1acct0 OR T1.acct = @T1acct1)", output);
            Assert.AreEqual(" T1.trantype <> @t1TranType1   AND  (T1.acct LIKE '118%' OR T1.acct = @T1acct1)", output);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct0")));
            Assert.AreEqual("118%", sqlParameters.First(x => x.ParameterName.Equals("T1acct0")).Value);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct1")));
            Assert.AreEqual("1200", sqlParameters.First(x => x.ParameterName.Equals("T1acct1")).Value);
        }

        [TestMethod]
        public void AddOrListToWhereClause_OneValueInPicklist_ContainsWildcard_IsNotIn()
        {
            var clause = " T1.trantype <> @t1TranType1 ";
            var picklist = new List<string> { "118%" };
            var fieldName = "T1.acct";
            var isNotIn = true;
            var sqlParameters = new List<SqlParameter>();
            var sut = new ParamsBuilder();

            var output = sut.AddOrListToWhereClause(clause, picklist, fieldName, isNotIn, sqlParameters);

            //Assert.AreEqual(" T1.trantype <> @t1TranType1   AND   NOT (T1.acct LIKE @T1acct0)", output);
            Assert.AreEqual(" T1.trantype <> @t1TranType1   AND   NOT (T1.acct LIKE '118%')", output);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct0")));
            Assert.AreEqual("118%", sqlParameters.First(x => x.ParameterName.Equals("T1acct0")).Value);
        }

        [TestMethod]
        public void AddOrListToWhereClause_TwoValuesInPicklist_BothContainWildcards_IsNotIn()
        {
            var clause = " T1.trantype <> @t1TranType1 ";
            var picklist = new List<string> { "118%", "120_" };
            var fieldName = "T1.acct";
            var isNotIn = true;
            var sqlParameters = new List<SqlParameter>();
            var sut = new ParamsBuilder();

            var output = sut.AddOrListToWhereClause(clause, picklist, fieldName, isNotIn, sqlParameters);

            Assert.AreEqual(" T1.trantype <> @t1TranType1   AND   NOT (T1.acct LIKE '118%' OR T1.acct LIKE '120_')", output);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct0")));
            Assert.AreEqual("118%", sqlParameters.First(x => x.ParameterName.Equals("T1acct0")).Value);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct1")));
            Assert.AreEqual("120_", sqlParameters.First(x => x.ParameterName.Equals("T1acct1")).Value);
        }

        [TestMethod]
        public void AddOrListToWhereClause_TwoValuesInPicklist_OneContainsWildcards_IsNotIn()
        {
            var clause = " T1.trantype <> @t1TranType1 ";
            var picklist = new List<string> { "118%", "1200" };
            var fieldName = "T1.acct";
            var isNotIn = true;
            var sqlParameters = new List<SqlParameter>();
            var sut = new ParamsBuilder();

            var output = sut.AddOrListToWhereClause(clause, picklist, fieldName, isNotIn, sqlParameters);

            //Assert.AreEqual(" T1.trantype <> @t1TranType1   AND   NOT (T1.acct LIKE @T1acct0 OR T1.acct = @T1acct1)", output);
            Assert.AreEqual(" T1.trantype <> @t1TranType1   AND   NOT (T1.acct LIKE '118%' OR T1.acct = @T1acct1)", output);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct0")));
            Assert.AreEqual("118%", sqlParameters.First(x => x.ParameterName.Equals("T1acct0")).Value);
            Assert.AreEqual(1, sqlParameters.Count(x => x.ParameterName.Equals("T1acct1")));
            Assert.AreEqual("1200", sqlParameters.First(x => x.ParameterName.Equals("T1acct1")).Value);
        }
    }
}

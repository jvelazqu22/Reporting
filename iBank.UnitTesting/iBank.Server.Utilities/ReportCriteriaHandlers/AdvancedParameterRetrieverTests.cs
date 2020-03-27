using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Server.Utilities;
using iBank.Server.Utilities.ReportCriteriaHandlers;

using iBankDomain.RepositoryInterfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.iBank.Server.Utilities.ReportCriteriaHandlers
{
    [TestClass]
    public class AdvancedParameterRetrieverTests
    {
        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                //mutltiudid
                new savedrpt3 { colname = "MUD:FOO" },
                //mutliudid AndOr
                new savedrpt3 { colname = "MUDANDOR" },
                //advanced criteria AndOr
                new savedrpt3 { colname = "AOCANDOR" },
                //text type - equals operator - value 1 exists - value 1a exists
                new savedrpt3 { colname = "TEXT_TYPE", oper = Operator.Equal.ToFriendlyString(), value1 = "1", value1a = "1a" },
                //numeric type - greater than operator - value 1 exists - value 1a does not exist
                new savedrpt3 { colname = "NUMERIC_TYPE", oper = Operator.GreaterThan.ToFriendlyString(), value1 = "2", value1a = "" },
                //currency type - less than operator - value 1 does not exist - value 1a does not exist
                new savedrpt3 { colname = "CURRENCY_TYPE", oper = Operator.Lessthan.ToFriendlyString(), value1 = "", value1a = "" },
                //date type - between operator
                new savedrpt3 { colname = "DATE_TYPE", oper = Operator.Between.ToFriendlyString(), value1 = "3", value1a = "3a" },
                //datetime type - not between operator
                new savedrpt3 { colname = "DATETIME_TYPE", oper = Operator.NotBetween.ToFriendlyString(), value1 = "4", value1a = "4a" }
            };

            var advancedColumns = new List<collist2>
            {
                //text type - is lookup
                new collist2 { colname = "TEXT_TYPE", advcolname = "ADV_TEXT_TYPE", islookup = true, coltype = "TEXT" },
                //numeric type - is not lookup
                new collist2 { colname = "NUMERIC_TYPE", advcolname = "ADV_NUMERIC_TYPE", islookup = false, coltype = "NUMERIC" },
                //currency type 
                new collist2 { colname = "CURRENCY_TYPE", advcolname = "ADV_CURRENCY_TYPE", islookup = false, coltype = "CURRENCY" },
                //date type
                new collist2 { colname = "DATE_TYPE", advcolname = "ADV_DATE_TYPE", islookup = false, coltype = "DATE" },
                //datetime type
                new collist2 { colname = "DATETIME_TYPE", advcolname = "ADV_DATETIME_TYPE", islookup = false, coltype = "DATETIME" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual(5, output.Count);
            Assert.AreEqual(1, output.Count(x => x.FieldName.Equals("TEXT_TYPE")));
            Assert.AreEqual(1, output.Count(x => x.FieldName.Equals("NUMERIC_TYPE")));
            Assert.AreEqual(1, output.Count(x => x.FieldName.Equals("CURRENCY_TYPE")));
            Assert.AreEqual(1, output.Count(x => x.FieldName.Equals("DATE_TYPE")));
            Assert.AreEqual(1, output.Count(x => x.FieldName.Equals("DATETIME_TYPE")));

            //text type
            var textType = output.Single(x => x.FieldName.Equals("TEXT_TYPE"));
            Assert.AreEqual(Operator.Equal, textType.Operator);
            Assert.AreEqual("1", textType.Value1);
            Assert.AreEqual("1a", textType.Value2);
            Assert.AreEqual("ADV_TEXT_TYPE", textType.AdvancedFieldName);
            Assert.AreEqual("TEXT", textType.Type);
            Assert.AreEqual(true, textType.IsLookup);
            Assert.AreEqual(false, textType.IsMultiUdid);

            //numeric type
            var numericType = output.Single(x => x.FieldName.Equals("NUMERIC_TYPE"));
            Assert.AreEqual(Operator.GreaterThan, numericType.Operator);
            Assert.AreEqual("2", numericType.Value1);
            Assert.AreEqual("", numericType.Value2);
            Assert.AreEqual("ADV_NUMERIC_TYPE", numericType.AdvancedFieldName);
            Assert.AreEqual("NUMERIC", numericType.Type);
            Assert.AreEqual(false, numericType.IsLookup);
            Assert.AreEqual(false, numericType.IsMultiUdid);

            //currency type
            var currencyType = output.Single(x => x.FieldName.Equals("CURRENCY_TYPE"));
            Assert.AreEqual(Operator.Lessthan, currencyType.Operator);
            Assert.AreEqual("", currencyType.Value1);
            Assert.AreEqual("", currencyType.Value2);
            Assert.AreEqual("ADV_CURRENCY_TYPE", currencyType.AdvancedFieldName);
            Assert.AreEqual("CURRENCY", currencyType.Type);
            Assert.AreEqual(false, currencyType.IsLookup);
            Assert.AreEqual(false, currencyType.IsMultiUdid);

            //date type
            var dateType = output.Single(x => x.FieldName.Equals("DATE_TYPE"));
            Assert.AreEqual(Operator.Between, dateType.Operator);
            Assert.AreEqual("3", dateType.Value1);
            Assert.AreEqual("3a", dateType.Value2);
            Assert.AreEqual("ADV_DATE_TYPE", dateType.AdvancedFieldName);
            Assert.AreEqual("DATE", dateType.Type);
            Assert.AreEqual(false, dateType.IsLookup);
            Assert.AreEqual(false, dateType.IsMultiUdid);

            //datetime type
            var dateTimeType = output.Single(x => x.FieldName.Equals("DATETIME_TYPE"));
            Assert.AreEqual(Operator.NotBetween, dateTimeType.Operator);
            Assert.AreEqual("4", dateTimeType.Value1);
            Assert.AreEqual("4a", dateTimeType.Value2);
            Assert.AreEqual("ADV_DATETIME_TYPE", dateTimeType.AdvancedFieldName);
            Assert.AreEqual("DATETIME", dateTimeType.Type);
            Assert.AreEqual(false, dateTimeType.IsLookup);
            Assert.AreEqual(false, dateTimeType.IsMultiUdid);
        }

        [TestMethod]
        public void GetAdvancedParametersFromReportCriteria()
        {
            var reportCrit = new List<ReportCriteria>
            {
                //not an advanced criteria 
                new ReportCriteria { VarName = "NotAdvanced", VarValue = "1" },
                //AOCANDOR
                new ReportCriteria { VarName = "AOCANDOR", VarValue = "1" },
                //field - 1
                new ReportCriteria { VarName = "AOCFLD01", VarValue = "FIELD_1" },
                //operator - 1
                new ReportCriteria { VarName = "AOCOPER01", VarValue = "=" },
                //value - 1
                new ReportCriteria { VarName = "AOCVALUE01", VarValue = "1" },
                //value1a - 1
                new ReportCriteria { VarName = "AOCVALUEA01", VarValue = "1a" },
                //field - 2
                new ReportCriteria { VarName = "AOCFLD02", VarValue = "FIELD_2" },
                //operator - 2
                new ReportCriteria { VarName = "AOCOPER02", VarValue = "<>" },
                //value - 2
                new ReportCriteria { VarName = "AOCVALUE02", VarValue = "2" },
                //value1a - 2
                new ReportCriteria { VarName = "AOCVALUEA02", VarValue = "2a" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = "FIELD_1", advcolname = "ADV_FIELD_1", islookup = false, coltype = "TEXT" },
                new collist2 { colname = "FIELD_2", advcolname = "ADV_FIELD_2", islookup = true, coltype = "NUMERIC" }
            };

            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromReportCriteria(reportCrit, query.Object);

            Assert.AreEqual(2, output.Count);
            Assert.AreEqual(1, output.Count(x => x.FieldName.Equals("FIELD_1")));
            Assert.AreEqual(1, output.Count(x => x.FieldName.Equals("FIELD_2")));
            
            //FIELD_1
            var fieldOne = output.Single(x => x.FieldName.Equals("FIELD_1"));
            Assert.AreEqual(Operator.Equal, fieldOne.Operator);
            Assert.AreEqual("1", fieldOne.Value1);
            Assert.AreEqual("1a", fieldOne.Value2);
            Assert.AreEqual("ADV_FIELD_1", fieldOne.AdvancedFieldName);
            Assert.AreEqual("TEXT", fieldOne.Type);
            Assert.AreEqual(false, fieldOne.IsLookup);
            Assert.AreEqual(false, fieldOne.IsMultiUdid);

            //FIELD_2
            var fieldTwo = output.Single(x => x.FieldName.Equals("FIELD_2"));
            Assert.AreEqual(Operator.NotEqual, fieldTwo.Operator);
            Assert.AreEqual("2", fieldTwo.Value1);
            Assert.AreEqual("2a", fieldTwo.Value2);
            Assert.AreEqual("ADV_FIELD_2", fieldTwo.AdvancedFieldName);
            Assert.AreEqual("NUMERIC", fieldTwo.Type);
            Assert.AreEqual(true, fieldTwo.IsLookup);
            Assert.AreEqual(false, fieldTwo.IsMultiUdid);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Exchange_ValueIsY_UpperCase_Return1()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.EXCHANGE, value1 = "Y", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.EXCHANGE, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.EXCHANGE)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Exchange_ValueIsY_LowerCase_Return1()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.EXCHANGE, value1 = "y", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.EXCHANGE, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.EXCHANGE)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Exchange_ValueIs1_Return1()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.EXCHANGE, value1 = "1", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.EXCHANGE, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.EXCHANGE)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Exchange_ValueIsYes_UpperCase_Return1()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.EXCHANGE, value1 = "YES", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.EXCHANGE, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.EXCHANGE)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Exchange_ValueIsYes_LowerCase_Return1()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.EXCHANGE, value1 = "yes", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.EXCHANGE, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.EXCHANGE)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Exchange_ValueIsTrue_UpperCase_Return1()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.EXCHANGE, value1 = "TRUE", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.EXCHANGE, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.EXCHANGE)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Exchange_ValueIsTrue_LowerCase_Return1()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.EXCHANGE, value1 = "true", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.EXCHANGE, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.EXCHANGE)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Exchange_ValueIsT_LowerCase_Return1()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.EXCHANGE, value1 = "t", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.EXCHANGE, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.EXCHANGE)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Exchange_ValueIsT_UpperCase_Return1()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.EXCHANGE, value1 = "T", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.EXCHANGE, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.EXCHANGE)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Exchange_ValueIsNotInListOfAcceptedValues_Return0()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.EXCHANGE, value1 = "foo", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.EXCHANGE, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("0", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.EXCHANGE)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Trefundabl_TransformData()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.TREFUNDABL, value1 = "Y", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.TREFUNDABL, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.TREFUNDABL)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Hinvbyagcy_TransformData()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.HINVBYAGCY, value1 = "Y", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.HINVBYAGCY, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.HINVBYAGCY)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Ainvbyagcy_TransformData()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.AINVBYAGCY, value1 = "Y", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.AINVBYAGCY, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.AINVBYAGCY)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Connect_ValueIsY_UpperCase_TransformToX()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.CONNECT, value1 = "Y", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.CONNECT, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("X", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.CONNECT)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Connect_ValueIsY_LowerCase_TransformToX()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.CONNECT, value1 = "y", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.CONNECT, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("X", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.CONNECT)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Connect_ValueIsX_LowerCase_TransformToX()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.CONNECT, value1 = "x", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.CONNECT, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("X", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.CONNECT)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Connect_ValueIsX_UpperCase_TransformToX()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.CONNECT, value1 = "X", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.CONNECT, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("X", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.CONNECT)).Value1);
        }

        [TestMethod]
        public void GetAdvancedParametersFromSavedReport3_SpecialCases_Connect_ValueIsNotInApprovedList_TransformToO()
        {
            var savedRpt3Data = new List<savedrpt3>
            {
                new savedrpt3 { colname = AdvancedParameterSpecialCases.CONNECT, value1 = "n", oper = Operator.Equal.ToFriendlyString(), value1a = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = AdvancedParameterSpecialCases.CONNECT, advcolname = "TEXT", islookup = true, coltype = "TEXT" }
            };
            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromSavedReport3(savedRpt3Data, query.Object);

            Assert.AreEqual("O", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.CONNECT)).Value1);
        }

        //this test doesn't run through all the special case options since we have covered that in other tests
        [TestMethod]
        public void GetAdvancedParametersFromReportCriteria_SpecialCases()
        {
            var reportCrit = new List<ReportCriteria>
            {
                //field - 1
                new ReportCriteria { VarName = "AOCFLD01", VarValue = AdvancedParameterSpecialCases.EXCHANGE },
                //operator - 1
                new ReportCriteria { VarName = "AOCOPER01", VarValue = "=" },
                //value - 1
                new ReportCriteria { VarName = "AOCVALUE01", VarValue = "y" },
                //value1a - 1
                new ReportCriteria { VarName = "AOCVALUEA01", VarValue = "" }
            };

            var advancedColumns = new List<collist2>
            {
                new collist2 { colname = "FIELD_1", advcolname = "ADV_FIELD_1", islookup = false, coltype = "TEXT" }
            };

            var query = new Mock<IQuery<IList<collist2>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(advancedColumns);
            var sut = new AdvancedParameterRetriever();

            var output = sut.GetAdvancedParametersFromReportCriteria(reportCrit, query.Object);

            Assert.AreEqual("1", output.First(x => x.FieldName.Equals(AdvancedParameterSpecialCases.EXCHANGE)).Value1);
        }
    }
}

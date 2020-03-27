using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Helper;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared.AdvancedClause;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.AdvancedClause
{
    ///[TestClass] THis is old, needs to re-write
    public class AdvancedClauseBuilderTests
    {
        [TestMethod]
        public void BuildAdvancedWhereClause_PassTwoParamsInSameTableAsRoot_Reservation_ReturnMatch()
        {
            var advanceClauseBuilder = new AdvancedClauseBuilder();
            var paramData = new List<AdvancedParameterTableInfo>();

            paramData.Add(new AdvancedParameterTableInfo
            {
                TableName = "a1",
                IsFieldSchemaMissing = false,
                IsInMainTableGroup = true,               
                Param = new AdvancedParameter
                {
                    FieldName = "f1",
                    AdvancedFieldName = "f1",
                    Operator = Operator.Equal,
                    Type = "TEXT",
                    Value1 = "v1"
                }
            });
            paramData.Add(new AdvancedParameterTableInfo
            {
                TableName = "a1",
                IsFieldSchemaMissing = false,
                IsInMainTableGroup = true,
                Param = new AdvancedParameter
                {
                    FieldName = "f2",
                    AdvancedFieldName = "f2",
                    Operator = Operator.Equal,
                    Type = "TEXT",
                    Value1 = "v2"
                }
            });

            var andOr = " and ";

            var output = advanceClauseBuilder.BuildAdvancedWhereClause(paramData, andOr, true, "b1", "somefield = 'ss'", "foo");

            var expected = $"T1.reckey in (select Distinct T1.reckey from b1 T1 where somefield = 'ss' and T1.Agency = 'foo' and (f1 = 'v1'{andOr}f2 = 'v2'))";

            Assert.AreEqual(expected, output);

        }

        [TestMethod]
        public void BuildAdvancedWhereClause_PassTwoParamsInSameTableNotAsRoot_Reservation_ReturnDoNotMatch()
        {
            var advanceClauseBuilder = new AdvancedClauseBuilder();
            var paramData = new List<AdvancedParameterTableInfo>();

            paramData.Add(new AdvancedParameterTableInfo
            {
                TableName = "a1",
                Param = new AdvancedParameter
                {
                    FieldName = "f1",
                    AdvancedFieldName = "f1",
                    Operator = Operator.Equal,
                    Type = "TEXT",
                    Value1 = "v1"
                }
            });
            paramData.Add(new AdvancedParameterTableInfo
            {
                TableName = "a1",
                Param = new AdvancedParameter
                {
                    FieldName = "f2",
                    AdvancedFieldName = "f2",
                    Operator = Operator.Equal,
                    Type = "TEXT",
                    Value1 = "v2"
                }
            });

            var andOr = " and ";

            var output = advanceClauseBuilder.BuildAdvancedWhereClause(paramData, andOr, true, "b1", "somefield = 'ss'", "foo");

            var expected = $"(f1 = 'v1'{andOr}f2 = 'v2')";

            Assert.AreNotEqual(expected, output);

        }

        [TestMethod]
        public void BuildAdvancedWhereClause_PassTwoParamsInSameTableNotAsRoot_Reservation_ReturnWithReckey()
        {
            var advanceClauseBuilder = new AdvancedClauseBuilder();
            var paramData = new List<AdvancedParameterTableInfo>();

            paramData.Add(new AdvancedParameterTableInfo
            {
                TableName = "a1",
                Param = new AdvancedParameter
                {
                    FieldName = "f1",
                    AdvancedFieldName = "f1",
                    Operator = Operator.Equal,
                    Type = "TEXT",
                    Value1 = "v1"
                }
            });
            paramData.Add(new AdvancedParameterTableInfo
            {
                TableName = "a1",
                Param = new AdvancedParameter
                {
                    FieldName = "f2",
                    AdvancedFieldName = "f2",
                    Operator = Operator.Equal,
                    Type = "TEXT",
                    Value1 = "v2"
                }
            });

            var andOr = " and ";

            var output = advanceClauseBuilder.BuildAdvancedWhereClause(paramData, andOr, true, "b1", "somefield = 'ss'", "foo");

            var expected = $"(f1 = 'v1'{andOr}f2 = 'v2')";

            Assert.AreEqual(expected, output);

        }
        [TestMethod]
        public void BuildAdvancedWhereClause_PassTwoParamsInSameTableNotAsRoot_backoffice_ReturnWithReckey()
        {
            var advanceClauseBuilder = new AdvancedClauseBuilder();
            var paramData = new List<AdvancedParameterTableInfo>();

            paramData.Add(new AdvancedParameterTableInfo
            {
                TableName = "a1",
                Param = new AdvancedParameter
                {
                    FieldName = "f1",
                    AdvancedFieldName = "f1",
                    Operator = Operator.Equal,
                    Type = "TEXT",
                    Value1 = "v1"
                }
            });
            paramData.Add(new AdvancedParameterTableInfo
            {
                TableName = "a1",
                
                Param = new AdvancedParameter
                {
                    FieldName = "f2",
                    AdvancedFieldName = "f2",
                    Operator = Operator.Equal,
                    Type = "TEXT",
                    Value1 = "v2"
                }
            });

            var andOr = " and ";

            var output = advanceClauseBuilder.BuildAdvancedWhereClause(paramData, andOr, false, "a1", "somefield = 'ss'", "foo");

            var expected = $"T1.reckey in (f1 = 'v1'{andOr}f2 = 'v2')";

            Assert.AreEqual(expected, output);

        }
    }
}

using System;

using Domain.Exceptions;

using iBank.Server.Utilities.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Server.Utilities.Helpers
{
    [TestClass]
    public class ColumnComparisonTests
    {
        #region Tests For InListByColumnType

        [TestMethod]
        public void InListByColumnType_NotTextOrNumericType_ReturnTrue()
        {
            var val = "foo";
            var crit = "foo,bar";
            var colType = "BAR";

            var output = ColumnComparison.InListByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void InListByColumnType_TextType_ValIsString_ValueInCrit_ReturnTrue()
        {
            var val = "foo";
            var crit = "foo,bar";
            var colType = "TEXT";

            var output = ColumnComparison.InListByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void InListByColumnType_TextType_ValIsString_ValueNotInCrit_ReturnFalse()
        {
            var val = "notin";
            var crit = "foo,bar";
            var colType = "TEXT";

            var output = ColumnComparison.InListByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void InListByColumnType_TextType_ValIsNotString_ThrowException()
        {
            var val = 0;
            var crit = "foo,bar";
            var colType = "TEXT";

            var output = ColumnComparison.InListByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void InListByColumnType_NumericType_ValIsInt_ValueInCrit_ReturnTrue()
        {
            var val = 1;
            var crit = "1,2";
            var colType = "NUMERIC";

            var output = ColumnComparison.InListByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void InListByColumnType_NumericType_ValIsInt_ValueNotInCrit_ReturnFalse()
        {
            var val = 1;
            var crit = "2,3";
            var colType = "NUMERIC";

            var output = ColumnComparison.InListByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void InListByColumnType_NumericType_ValIsInt_ValueNotInt_ReturnFalse()
        {
            var val = 1;
            var crit = "foo";
            var colType = "NUMERIC";

            var output = ColumnComparison.InListByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void InListByColumnType_NumericType_ValIsNotInt_ThrowException()
        {
            var val = "foo";
            var crit = "1,2";
            var colType = "NUMERIC";

            var output = ColumnComparison.InListByColumnType(val, crit, colType);
        }

        #endregion

        #region Tests For EmptyByColumnType

        [TestMethod]
        public void EmptyByColumnType_NotTextType_ReturnTrue()
        {
            var val = "1";
            var colType = "NUMERIC";

            var output = ColumnComparison.EmptyByColumnType(val, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void EmptyByColumnType_TextType_ValIsString_ValIsEmpty_ReturnTrue()
        {
            var val = "";
            var colType = "TEXT";

            var output = ColumnComparison.EmptyByColumnType(val, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void EmptyByColumnType_TextType_ValIsString_ValIsNotEmpty_ReturnFalse()
        {
            var val = "1";
            var colType = "TEXT";

            var output = ColumnComparison.EmptyByColumnType(val, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void EmptyByColumnType_TextType_ValIsNotString_ThrowException()
        {
            var val = new DateTime(2016, 1, 1);
            var colType = "TEXT";

            var output = ColumnComparison.EmptyByColumnType(val, colType);
        }

        #endregion

        #region Tests For EqualByColumnType

        [TestMethod]
        public void EqualByColumnType_TextType_ValIsString_CritIsEqual_ReturnTrue()
        {
            var val = "foo";
            var crit = "foo";
            var colType = "TEXT";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void EqualByColumnType_TextType_ValIsString_CritIsEqualButDifferentCase_ReturnTrue()
        {
            var val = "foo";
            var crit = "FOO";
            var colType = "TEXT";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void EqualByColumnType_TextType_ValIsString_CritIsNotEqual_ReturnFalse()
        {
            var val = "foo";
            var crit = "bar";
            var colType = "TEXT";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void EqualByColumnType_TextType_ValIsNotString_ThrowException()
        {
            var val = 1;
            var crit = "foo";
            var colType = "TEXT";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void EqualByColumnType_CurrencyType_ValIsDecimal_CritIsEqual_ReturnTrue()
        {
            var val = 1M;
            var crit = "1";
            var colType = "CURRENCY";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void EqualByColumnType_CurrencyType_ValIsDecimal_CritIsNotEqual_ReturnFalse()
        {
            var val = 1M;
            var crit = "2";
            var colType = "CURRENCY";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void EqualByColumnType_CurrencyType_ValIsDecimal_CritIsNotNumeric_ReturnFalse()
        {
            var val = 1M;
            var crit = "bar";
            var colType = "CURRENCY";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void EqualByColumnType_CurrencyType_ValIsNotDecimal_ThrowException()
        {
            var val = "FOO";
            var crit = "1";
            var colType = "CURRENCY";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void EqualByColumnType_NumericType_ValIsInteger_CritIsEqual_ReturnTrue()
        {
            var val = 1;
            var crit = "1";
            var colType = "NUMERIC";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void EqualByColumnType_NumericType_ValIsInteger_CritIsNotEqual_ReturnFalse()
        {
            var val = 1;
            var crit = "2";
            var colType = "NUMERIC";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void EqualByColumnType_NumericType_ValIsInteger_CritIsNotNumeric_ReturnFalse()
        {
            var val = 1;
            var crit = "bar";
            var colType = "NUMERIC";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void EqualByColumnType_NumericType_ValIsNotInteger_ThrowException()
        {
            var val = "FOO";
            var crit = "1";
            var colType = "NUMERIC";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void EqualByColumnType_DateType_ValIsDate_CritIsEqual_ReturnTrue()
        {
            var val = new DateTime(2016, 1, 1, 01, 01, 01);
            var crit = new DateTime(2016, 1, 1, 01, 01, 01).ToString();
            var colType = "DATE";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void EqualByColumnType_DateType_ValIsDate_CritIsEqualDateDifferentTime_ReturnTrue()
        {
            var val = new DateTime(2016, 1, 1, 01, 01, 01);
            var crit = new DateTime(2016, 1, 1, 06, 06, 06).ToString();
            var colType = "DATE";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void EqualByColumnType_DateType_ValIsDate_CritIsNotEqual_ReturnFalse()
        {
            var val = new DateTime(2016, 1, 1, 01, 01, 01);
            var crit = new DateTime(2019, 2, 3, 06, 06, 06).ToString();
            var colType = "DATE";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void EqualByColumnType_DateType_ValIsDate_CritIsNotDate_ReturnFalse()
        {
            var val = new DateTime(2016, 1, 1, 01, 01, 01);
            var crit = "foo";
            var colType = "DATE";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void EqualByColumnType_DateType_ValIsNotDate_ThrowException()
        {
            var val = "foo";
            var crit = new DateTime(2019, 2, 3, 06, 06, 06).ToString();
            var colType = "DATE";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void EqualByColumnType_DateTimeType_ValIsDateTime_CritIsEqual_ReturnTrue()
        {
            var val = new DateTime(2016, 1, 1, 01, 01, 01);
            var crit = new DateTime(2016, 1, 1, 01, 01, 01).ToString();
            var colType = "DATETIME";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void EqualByColumnType_DateTimeType_ValIsDateTime_CritIsEqualDateDifferentTime_ReturnFalse()
        {
            var val = new DateTime(2016, 1, 1, 01, 01, 01);
            var crit = new DateTime(2016, 1, 1, 06, 06, 06).ToString();
            var colType = "DATETIME";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void EqualByColumnType_DateTimeType_ValIsDateTime_CritIsNotEqual_ReturnFalse()
        {
            var val = new DateTime(2016, 1, 1, 01, 01, 01);
            var crit = new DateTime(2019, 2, 3, 06, 06, 06).ToString();
            var colType = "DATETIME";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void EqualByColumnType_DateTimeType_ValIsDateTime_CritIsNotDate_ReturnFalse()
        {
            var val = new DateTime(2016, 1, 1, 01, 01, 01);
            var crit = "foo";
            var colType = "DATETIME";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void EqualByColumnType_DateTimeType_ValIsNotDateTime_ThrowException()
        {
            var val = "foo";
            var crit = new DateTime(2019, 2, 3, 06, 06, 06).ToString();
            var colType = "DATETIME";

            var output = ColumnComparison.EqualByColumnType(val, crit, colType);
        }

        #endregion

        #region Tests For GreaterThanByColumnType

        [TestMethod]
        public void GreaterThanByColumnType_TextType_ValIsString_CritIsEqual_OrEqualTo_ReturnTrue()
        {
            var val = "1";
            var crit = "1";
            var colType = "TEXT";
            var orEqual = true;

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType, orEqual);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_TextType_ValIsString_CritIsEqual_NotOrEqualTo_ReturnFalse()
        {
            var val = "1";
            var crit = "1";
            var colType = "TEXT";
            var orEqual = false;

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType, orEqual);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_TextType_ValIsString_ValIsGreater_ReturnFalse()
        {
            var val = "1";
            var crit = "0";
            var colType = "TEXT";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_TextType_ValIsString_ValIsLessThan_ReturnFalse()
        {
            var val = "1";
            var crit = "2";
            var colType = "TEXT";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void GreaterThanByColumnType_TextType_ValNotString_ThrowException()
        {
            var val = 0;
            var crit = "2";
            var colType = "TEXT";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void GreaterThanByColumnType_CurrencyType_ValIsDecimal_CritIsEqual_OrEqualTo_ReturnTrue()
        {
            var val = 1M;
            var crit = "1";
            var colType = "CURRENCY";
            var orEqual = true;

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType, orEqual);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_CurrencyType_ValIsDecimal_CritIsEqual_NotOrEqualTo_ReturnFalse()
        {
            var val = 1M;
            var crit = "1";
            var colType = "CURRENCY";
            var orEqual = false;

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType, orEqual);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_CurrencyType_ValIsDecimal_ValIsGreater_ReturnFalse()
        {
            var val = 1M;
            var crit = "0";
            var colType = "CURRENCY";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_CurrencyType_ValIsDecimal_ValIsLessThan_ReturnFalse()
        {
            var val = 1M;
            var crit = "2";
            var colType = "CURRENCY";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_CurrencyType_ValIsDecimal_CritNotNumeric_ReturnFalse()
        {
            var val = 1M;
            var crit = "foo";
            var colType = "CURRENCY";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void GreaterThanByColumnType_CurrencyType_ValNotDecimal_ThrowException()
        {
            var val = "0";
            var crit = "2";
            var colType = "CURRENCY";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void GreaterThanByColumnType_NumericType_ValIsInt_CritIsEqual_OrEqualTo_ReturnTrue()
        {
            var val = 1;
            var crit = "1";
            var colType = "NUMERIC";
            var orEqual = true;

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType, orEqual);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_NumericType_ValIsInt_CritIsEqual_NotOrEqualTo_ReturnFalse()
        {
            var val = 1;
            var crit = "1";
            var colType = "NUMERIC";
            var orEqual = false;

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType, orEqual);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_NumericType_ValIsInt_ValIsGreater_ReturnFalse()
        {
            var val = 1;
            var crit = "0";
            var colType = "NUMERIC";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_NumericType_ValIsInt_ValIsLessThan_ReturnFalse()
        {
            var val = 1;
            var crit = "2";
            var colType = "NUMERIC";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_NumericType_ValIsInt_CritNotNumeric_ReturnFalse()
        {
            var val = 1;
            var crit = "foo";
            var colType = "NUMERIC";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void GreaterThanByColumnType_NumericType_ValNotInt_ThrowException()
        {
            var val = "0";
            var crit = "2";
            var colType = "NUMERIC";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void GreaterThanByColumnType_DateType_ValIsDate_CritIsEqual_OrEqualTo_ReturnTrue()
        {
            var val = new DateTime(2016, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";
            var orEqual = true;

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType, orEqual);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_DateType_ValIsDate_CritIsEqualButDifferentTime_NotOrEqualTo_ReturnFalse()
        {
            var val = new DateTime(2016, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";
            var orEqual = false;

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType, orEqual);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_DateType_ValIsDate_ValIsGreater_ReturnFalse()
        {
            var val = new DateTime(2016, 1, 1, 1, 1, 1);
            var crit = new DateTime(2015, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_DateType_ValIsDate_ValIsLessThan_ReturnFalse()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_DateType_ValIsDate_CritNotNumeric_ReturnFalse()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit = "foo";
            var colType = "DATE";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void GreaterThanByColumnType_DateType_ValIsNotDate_ThrowException()
        {
            var val = "0";
            var crit = new DateTime(2015, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void GreaterThanByColumnType_DateTimeType_ValIsDateTime_CritIsEqual_OrEqualTo_ReturnTrue()
        {
            var val = new DateTime(2016, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";
            var orEqual = true;

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType, orEqual);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_DateTimeType_ValIsDateTime_CritIsEqual_NotOrEqualTo_ReturnFalse()
        {
            var val = new DateTime(2016, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";
            var orEqual = false;

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType, orEqual);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_DateTimeType_ValIsDateTime_ValIsGreater_ReturnFalse()
        {
            var val = new DateTime(2017, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_DateTimeType_ValIsDateTime_ValIsLessThan_ReturnFalse()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void GreaterThanByColumnType_DateTimeType_ValIsDateTime_CritNotNumeric_ReturnFalse()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit = "foo";
            var colType = "DATETIME";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void GreaterThanByColumnType_DateTimeType_ValIsNotDateTime_ThrowException()
        {
            var val = "foo";
            var crit = new DateTime(2015, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";

            var output = ColumnComparison.GreaterThanByColumnType(val, crit, colType);
        }

        #endregion

        #region Tests For LessThanByColumnType

        [TestMethod]
        public void LessThanByColumnType_TextType_ValIsText_CritIsEqual_OrEqualTo_ReturnTrue()
        {
            var val = "1";
            var crit = "1";
            var colType = "TEXT";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType, true);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void LessThanByColumnType_TextType_ValIsText_CritIsEqual_NotOrEqualTo_ReturnFalse()
        {
            var val = "1";
            var crit = "1";
            var colType = "TEXT";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType, false);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void LessThanByColumnType_TextType_ValIsText_ValIsLessThan_ReturnTrue()
        {
            var val = "0";
            var crit = "1";
            var colType = "TEXT";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void LessThanByColumnType_TextType_ValIsText_ValIsGreaterThan_ReturnFalse()
        {
            var val = "2";
            var crit = "1";
            var colType = "TEXT";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void LessThanByColumnType_TextType_ValIsNotText_ThrowException()
        {
            var val = 0;
            var crit = "1";
            var colType = "TEXT";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void LessThanByColumnType_CurrencyType_ValIsCurrency_CritIsEqual_OrEqualTo_ReturnTrue()
        {
            var val = 1M;
            var crit = "1";
            var colType = "CURRENCY";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType, true);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void LessThanByColumnType_CurrencyType_ValIsCurrency_CritIsEqual_NotOrEqualTo_ReturnFalse()
        {
            var val = 1M;
            var crit = "1";
            var colType = "CURRENCY";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType, false);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void LessThanByColumnType_CurrencyType_ValIsCurrency_ValIsLessThan_ReturnTrue()
        {
            var val = 0M;
            var crit = "1";
            var colType = "CURRENCY";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void LessThanByColumnType_CurrencyType_ValIsCurrency_ValIsGreaterThan_ReturnFalse()
        {
            var val = 2M;
            var crit = "1";
            var colType = "CURRENCY";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void LessThanByColumnType_CurrencyType_ValIsCurrency_CritIsNotCurrency_ReturnFalse()
        {
            var val = 2M;
            var crit = "foo";
            var colType = "CURRENCY";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void LessThanByColumnType_CurrencyType_ValIsNotCurrency_ThrowException()
        {
            var val = "FOO";
            var crit = "1";
            var colType = "CURRENCY";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void LessThanByColumnType_NumericType_ValIsInt_CritIsEqual_OrEqualTo_ReturnTrue()
        {
            var val = 1;
            var crit = "1";
            var colType = "NUMERIC";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType, true);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void LessThanByColumnType_NumericType_ValIsInt_CritIsEqual_NotOrEqualTo_ReturnFalse()
        {
            var val = 1;
            var crit = "1";
            var colType = "NUMERIC";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType, false);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void LessThanByColumnType_NumericType_ValIsInt_ValIsLessThan_ReturnTrue()
        {
            var val = 0;
            var crit = "1";
            var colType = "NUMERIC";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void LessThanByColumnType_NumericType_ValIsInt_ValIsGreaterThan_ReturnFalse()
        {
            var val = 2;
            var crit = "1";
            var colType = "NUMERIC";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void LessThanByColumnType_NumericType_ValIsInt_CritIsNotInt_ReturnFalse()
        {
            var val = 2;
            var crit = "foo";
            var colType = "NUMERIC";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void LessThanByColumnType_NumericType_ValIsNotInt_ThrowException()
        {
            var val = "FOO";
            var crit = "1";
            var colType = "NUMERIC";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void LessThanByColumnType_DateType_ValIsDate_CritIsEqual_OrEqualTo_ReturnTrue()
        {
            var val = new DateTime(2016, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType, true);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void LessThanByColumnType_DateType_ValIsDate_CritIsEqualDateButTimeIsLessThan_OrEqualTo_ReturnTrue()
        {
            var val = new DateTime(2016, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 0).ToString();
            var colType = "DATE";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType, true);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void LessThanByColumnType_DateType_ValIsDate_CritIsEqual_NotOrEqualTo_ReturnFalse()
        {
            var val = new DateTime(2016, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType, false);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void LessThanByColumnType_DateType_ValIsDate_ValIsLessThan_ReturnTrue()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void LessThanByColumnType_DateType_ValIsDate_ValIsGreaterThan_ReturnFalse()
        {
            var val = new DateTime(2017, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void LessThanByColumnType_DateType_ValIsDate_CritIsNotDate_ReturnFalse()
        {
            var val = new DateTime(2017, 1, 1, 1, 1, 1);
            var crit = "foo";
            var colType = "DATE";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void LessThanByColumnType_DateType_ValIsNotDate_ThrowException()
        {
            var val = "FOO";
            var crit = "1";
            var colType = "DATE";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);
        }

        [TestMethod]
        public void LessThanByColumnType_DateTimeType_ValIsDateTime_CritIsEqual_OrEqualTo_ReturnTrue()
        {
            var val = new DateTime(2016, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType, true);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void LessThanByColumnType_DateTimeType_ValIsDateTime_CritIsEqual_NotOrEqualTo_ReturnFalse()
        {
            var val = new DateTime(2016, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType, false);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void LessThanByColumnType_DateTimeType_ValIsDateTime_ValIsLessThan_ReturnTrue()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void LessThanByColumnType_DateTimeType_ValIsDateTime_ValIsGreaterThan_ReturnFalse()
        {
            var val = new DateTime(2017, 1, 1, 1, 1, 1);
            var crit = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void LessThanByColumnType_DateTimeType_ValIsDateTime_CritIsNotDateTime_ReturnFalse()
        {
            var val = new DateTime(2017, 1, 1, 1, 1, 1);
            var crit = "foo";
            var colType = "DATETIME";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void LessThanByColumnType_DateTimeType_ValIsNotDateTime_ThrowException()
        {
            var val = "FOO";
            var crit = "1";
            var colType = "DATETIME";

            var output = ColumnComparison.LessThanByColumnType(val, crit, colType);
        }

        #endregion

        #region Tests For BetweenByColumnType

        [TestMethod]
        public void BetweenByColumnType_TextType_ValIsText_ValIsLessThan_NotBetweenIsTrue_ReturnTrue()
        {
            var val = "0";
            var crit1 = "1";
            var crit2 = "3";
            var colType = "TEXT";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_TextType_ValIsText_ValIsGreaterThan_NotBetweenIsTrue_ReturnTrue()
        {
            var val = "4";
            var crit1 = "1";
            var crit2 = "3";
            var colType = "TEXT";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_TextType_ValIsText_ValIsBetween_NotBetweenIsTrue_ReturnFalse()
        {
            var val = "2";
            var crit1 = "1";
            var crit2 = "3";
            var colType = "TEXT";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_TextType_ValIsText_ValIsGreaterThan_NotBetweenIsFalse_ReturnFalse()
        {
            var val = "4";
            var crit1 = "1";
            var crit2 = "3";
            var colType = "TEXT";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_TextType_ValIsText_ValIsLessThan_NotBetweenIsFalse_ReturnFalse()
        {
            var val = "0";
            var crit1 = "1";
            var crit2 = "3";
            var colType = "TEXT";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_TextType_ValIsText_ValIsBetween_NotBetweenIsFalse_ReturnTrue()
        {
            var val = "2";
            var crit1 = "1";
            var crit2 = "3";
            var colType = "TEXT";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        public void BetweenByColumnType_TextType_ValIsNotString_ThrowExcpetion()
        {
            var val = 0;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "TEXT";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);
        }

        [TestMethod]
        public void BetweenByColumnType_CurrencyType_ValIsGreaterThan_NotBetweenIsTrue_ReturnTrue()
        {
            var val = 4M;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "CURRENCY";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_CurrencyType_ValIsGreaterThan_NotBetweenIsFalse_ReturnFalse()
        {
            var val = 4M;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "CURRENCY";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_CurrencyType_ValIsLessThan_NotBetweenIsTrue_ReturnTrue()
        {
            var val = 0M;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "CURRENCY";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_CurrencyType_ValIsLessThan_NotBetweenIsFalse_ReturnFalse()
        {
            var val = 0M;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "CURRENCY";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_CurrencyType_ValIsBetween_NotBetweenIsTrue_ReturnFalse()
        {
            var val = 2M;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "CURRENCY";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_CurrencyType_ValIsBetween_NotBetweenIsFalse_ReturnTrue()
        {
            var val = 2M;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "CURRENCY";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_CurrencyType_Crit1IsNotCurrency_ReturnFalse()
        {
            var val = 2M;
            var crit1 = "foo";
            var crit2 = "3";
            var colType = "CURRENCY";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_CurrencyType_Crit2IsNotCurrency_ReturnFalse()
        {
            var val = 2M;
            var crit1 = "1";
            var crit2 = "foo";
            var colType = "CURRENCY";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void BetweenByColumnType_CurrencyType_ValIsNotCurrency_ThrowException()
        {
            var val = "foo";
            var crit1 = "1";
            var crit2 = "3";
            var colType = "CURRENCY";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);
        }

        [TestMethod]
        public void BetweenByColumnType_NumericType_ValIsGreaterThan_NotBetweenIsTrue_ReturnTrue()
        {
            var val = 4;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "NUMERIC";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_NumericType_ValIsGreaterThan_NotBetweenIsFalse_ReturnFalse()
        {
            var val = 4;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "NUMERIC";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_NumericType_ValIsLessThan_NotBetweenIsTrue_ReturnTrue()
        {
            var val = 0;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "NUMERIC";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_NumericType_ValIsLessThan_NotBetweenIsFalse_ReturnFalse()
        {
            var val = 0;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "NUMERIC";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_NumericType_ValIsBetween_NotBetweenIsTrue_ReturnFalse()
        {
            var val = 2;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "NUMERIC";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_NumericType_ValIsBetween_NotBetweenIsFalse_ReturnTrue()
        {
            var val = 2;
            var crit1 = "1";
            var crit2 = "3";
            var colType = "NUMERIC";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_NumericType_Crit1IsNotNumeric_ReturnFalse()
        {
            var val = 2;
            var crit1 = "foo";
            var crit2 = "3";
            var colType = "NUMERIC";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_NumericType_Crit2IsNotNumeric_ReturnFalse()
        {
            var val = 2;
            var crit1 = "1";
            var crit2 = "foo";
            var colType = "NUMERIC";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void BetweenByColumnType_NumericType_ValIsNotNumeric_ThrowException()
        {
            var val = "foo";
            var crit1 = "1";
            var crit2 = "3";
            var colType = "NUMERIC";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);
        }

        [TestMethod]
        public void BetweenByColumnType_DateType_ValIsGreaterThan_NotBetweenIsTrue_ReturnTrue()
        {
            var val = new DateTime(2019, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateType_ValIsGreaterThan_NotBetweenIsFalse_ReturnFalse()
        {
            var val = new DateTime(2019, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateType_ValIsLessThan_NotBetweenIsTrue_ReturnTrue()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateType_ValIsLessThan_NotBetweenIsFalse_ReturnFalse()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateType_ValIsBetween_NotBetweenIsTrue_ReturnFalse()
        {
            var val = new DateTime(2017, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateType_ValIsBetween_NotBetweenIsFalse_ReturnTrue()
        {
            var val = new DateTime(2017, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateType_Crit1IsNotDate_ReturnFalse()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit1 = "foo";
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateType_Crit2IsNotDate_ReturnFalse()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = "foo";
            var colType = "DATE";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void BetweenByColumnType_DateType_ValIsNotDate_ThrowException()
        {
            var val = "foo";
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATE";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);
        }

        [TestMethod]
        public void BetweenByColumnType_DateTimeType_ValIsGreaterThan_NotBetweenIsTrue_ReturnTrue()
        {
            var val = new DateTime(2019, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateTimeType_ValIsGreaterThan_NotBetweenIsFalse_ReturnFalse()
        {
            var val = new DateTime(2019, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateTimeType_ValIsLessThan_NotBetweenIsTrue_ReturnTrue()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateTimeType_ValIsLessThan_NotBetweenIsFalse_ReturnFalse()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateTimeType_ValIsBetween_NotBetweenIsTrue_ReturnFalse()
        {
            var val = new DateTime(2017, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateTimeType_ValIsBetween_NotBetweenIsFalse_ReturnTrue()
        {
            var val = new DateTime(2017, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";
            var notBetween = false;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateTimeType_Crit1IsNotDateTime_ReturnFalse()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit1 = "foo";
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BetweenByColumnType_DateTimeType_Crit2IsNotDateTime_ReturnFalse()
        {
            var val = new DateTime(2015, 1, 1, 1, 1, 1);
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = "foo";
            var colType = "DATETIME";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);

            Assert.AreEqual(false, output);
        }

        [ExpectedException(typeof(ColumnComparisonException))]
        [TestMethod]
        public void BetweenByColumnType_DateTimeType_ValIsNotDateTime_ThrowException()
        {
            var val = "foo";
            var crit1 = new DateTime(2016, 1, 1, 1, 1, 1).ToString();
            var crit2 = new DateTime(2018, 1, 1, 1, 1, 1).ToString();
            var colType = "DATETIME";
            var notBetween = true;

            var output = ColumnComparison.BetweenByColumnType(val, crit1, crit2, colType, notBetween);
        }

        #endregion
    }
}

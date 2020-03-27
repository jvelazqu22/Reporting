using iBank.Server.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces;

namespace iBank.UnitTesting.iBank.Server.Utilities
{
    [TestClass]
    public class ExtensionMethodsTests
    {

        [TestMethod]
        public void IsStringSqlList_ListWithMultipleStrs_ReturnsTrue()
        {
            var testList = "('test1','test2', 'test3')";
            var expected = true;

            var output = testList.IsStringSqlList();

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void IsStringSqlList_ListWithOneStr_ReturnsTrue()
        {
            var testList = "('test1')";
            var expected = true;

            var output = testList.IsStringSqlList();

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void IsNumericList_ListWithMultipleInts_ReturnsTrue()
        {
            var testList = "(574576284,1233456,9876543)";
            var expected = true;

            var output = testList.IsNumericSqlList();

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void IsNumericList_ListWithOneInt_ReturnsTrue()
        {
            var testList = "(574576284)";
            var expected = true;

            var output = testList.IsNumericSqlList();

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void Truncate_TruncateToHour()
        {
            var testDate = new DateTime(2000, 1, 1, 2, 54, 43);
            var expected = new DateTime(2000, 1, 1, 2, 0, 0);

            var output = testDate.Truncate(TimeSpan.FromHours(1));

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void Truncate_TruncateToDay()
        {
            var testDate = new DateTime(2000, 1, 1, 4, 54, 43);
            var expected = new DateTime(2000, 1, 1, 0, 0, 0);

            var output = testDate.Truncate(TimeSpan.FromDays(1));

            Assert.AreEqual(expected, output);
        }
        [TestMethod]
        public void Truncate_TruncateToMinute()
        {
            var testDate = new DateTime(2000, 1, 1, 4, 54, 43);
            var expected = new DateTime(2000, 1, 1, 4, 54, 0);

            var output = testDate.Truncate(TimeSpan.FromMinutes(1));

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void ToIBankDateFormat()
        {
            var testDate = new DateTime(2000, 1, 2);

            var output = testDate.ToIBankDateFormat();

            Assert.AreEqual("DT:2000,1,2", output);
        }

        [TestMethod]
        public void IsPriorTo_BeginDateBeforeEndDate_ReturnTrue()
        {
            var beginDate = new DateTime(2016, 1, 1);
            var endDate = new DateTime(2016, 5, 1);

            var output = beginDate.IsPriorToOrSameDay(endDate);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsPriorTo_BeginDateSameAsEndDate_ReturnTrue()
        {
            var beginDate = new DateTime(2016, 1, 1);
            var endDate = new DateTime(2016, 1, 1);

            var output = beginDate.IsPriorToOrSameDay(endDate);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsPriorToOrSameDay_BeginDateAfterEndDate_ReturnFalse()
        {
            var beginDate = new DateTime(2016, 8, 1);
            var endDate = new DateTime(2016, 5, 1);

            var output = beginDate.IsPriorToOrSameDay(endDate);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void ContainsIgnoreCase_SameCaseIsASubstring_ReturnTrue()
        {
            var source = "foobarfoo";
            var comparisonString = "bar";

            var output = source.ContainsIgnoreCase(comparisonString);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void ContainsIgnoreCase_DifferentCaseIsASubstring_ReturnTrue()
        {
            var source = "foobarfoo";
            var comparisonString = "bAR";

            var output = source.ContainsIgnoreCase(comparisonString);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void ContainsIgnoreCase_NotInString_ReturnFalse()
        {
            var source = "foobarfoo";
            var comparisonString = "no";

            var output = source.ContainsIgnoreCase(comparisonString);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void FormatWholeDateWithAmPm_AmValue()
        {
            var val = new DateTime(2016, 2, 3, 9, 1, 2);

            var output = val.FormatWholeDateWithAmPm();

            Assert.AreEqual(@"02/03/2016 09:01:02 AM", output);
        }

        [TestMethod]
        public void FormatWholeDateWithAmPm_PmValue()
        {
            var val = new DateTime(2016, 2, 3, 23, 1, 2);

            var output = val.FormatWholeDateWithAmPm();

            Assert.AreEqual(@"02/03/2016 11:01:02 PM", output);
        }

        [TestMethod]
        public void ToBatch_GoesIntoBatchesEvenly()
        {
            var list = new List<int> { 1, 2, 3, 4, 5, 6 };
            var batchSize = 3;

            var output = list.ToBatch(batchSize);

            Assert.AreEqual(2, output.Count);
        }

        [TestMethod]
        public void ToBatch_DoesNotGotIntoBatchEvenly()
        {
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            var batchSize = 3;

            var output = list.ToBatch(batchSize);

            Assert.AreEqual(3, output.Count);
        }

        [TestMethod]
        public void Truncate_LessThanLimit()
        {
            var s = "HELLO";
            var maxCharacters = 10;

            var output = s.Truncate(maxCharacters);

            Assert.AreEqual(s, output);
        }

        [TestMethod]
        public void Truncate_OverTheLimit()
        {
            var s = "HELLO";
            var maxCharacters = 3;

            var output = s.Truncate(maxCharacters);

            Assert.AreEqual("HEL", output);
        }

        [TestMethod]
        public void Truncate_EqualToLimit()
        {
            var s = "HELLO";
            var maxCharacters = 5;

            var output = s.Truncate(maxCharacters);

            Assert.AreEqual(s, output);
        }

        [TestMethod]
        public void RemoveTrailingChar_CharExistsAtEnd_ReturnStringWithoutChar()
        {
            var s = "hello'";
            var charToRemove = '\'';

            var output = s.RemoveTrailingChar(charToRemove);

            Assert.AreEqual("hello", output);
        }

        [TestMethod]
        public void RemoveTrailingChar_CharExistsAtStart_ReturnOriginal()
        {
            var s = "'hello";
            var charToRemove = '\'';

            var output = s.RemoveTrailingChar(charToRemove);

            Assert.AreEqual("'hello", output);
        }

        [TestMethod]
        public void RemoveTrailingChar_CharExistsInMiddle_ReturnOriginal()
        {
            var s = "hel'lo";
            var charToRemove = '\'';

            var output = s.RemoveTrailingChar(charToRemove);

            Assert.AreEqual("hel'lo", output);
        }

        [TestMethod]
        public void RemoveTrailingChar_CharDoesNotExist_ReturnOriginal()
        {
            var s = "hello";
            var charToRemove = '\'';

            var output = s.RemoveTrailingChar(charToRemove);

            Assert.AreEqual("hello", output);
        }

        [TestMethod]
        public void RemoveTrailingChar_StringIsEmpty_ReturnEmptyString()
        {
            var s = "";
            var charToRemove = '\'';

            var output = s.RemoveTrailingChar(charToRemove);

            Assert.AreEqual("", output);
        }

        [TestMethod]
        public void FirstCharacterEquals_AreEqual_ReturnTrue()
        {
            var sut = "%foo";

            var output = sut.FirstCharacterEquals('%');

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void FirstCharacterEquals_NotEqual_ReturnFalse()
        {
            var sut = "foo";

            var output = sut.FirstCharacterEquals('%');

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void FirstCharacterEquals_EmptyString_ReturnFalse()
        {
            var sut = " ";

            var output = sut.FirstCharacterEquals('%');

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void FirstCharacterEquals_NullString_ReturnFalse()
        {
            string sut = null;

            var output = sut.FirstCharacterEquals('%');

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void FirstCharacterEquals_DifferentCaseButMatches_ReturnTrue()
        {
            var sut = "FOO";

            var output = sut.FirstCharacterEquals('f');

            Assert.AreEqual(true, output);
        }

        public void LastCharacterEquals_AreEqual_ReturnTrue()
        {
            var sut = "foo%";

            var output = sut.FirstCharacterEquals('%');

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void LastCharacterEquals_NotEqual_ReturnFalse()
        {
            var sut = "foo";

            var output = sut.FirstCharacterEquals('%');

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void LastCharacterEquals_EmptyString_ReturnFalse()
        {
            var sut = " ";

            var output = sut.FirstCharacterEquals('%');

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void LastCharacterEquals_NullString_ReturnFalse()
        {
            string sut = null;

            var output = sut.FirstCharacterEquals('%');

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void LastCharacterEquals_DifferentCaseButValueMatches_ReturnTrue()
        {
            var sut = "FOO";

            var output = sut.LastCharacterEquals('O');

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void Like_PrecedingPercentageSign_ValueMatches_ReturnTrue()
        {
            var sut = "%foo";
            var wildcardText = "this says foo";

            var output = sut.Like(wildcardText);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void Like_PrecedingPercentageSign_ValueDoesNotMatch_ReturnFalse()
        {
            var sut = "%foo";
            var wildcardText = "this says foobar";

            var output = sut.Like(wildcardText);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void Like_EndingPercentageSign_ValueMatches_ReturnTrue()
        {
            var sut = "foo%";
            var wildcardText = "foo this says";

            var output = sut.Like(wildcardText);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void Like_EndingPercentageSign_ValueDoesNotMatch_ReturnFalse()
        {
            var sut = "foo%";
            var wildcardText = "this says foobar";

            var output = sut.Like(wildcardText);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void Like_PrecedingUnderscoreSign_ValueMatches_ReturnTrue()
        {
            var sut = "_foo";
            var wildcardText = "afoo";

            var output = sut.Like(wildcardText);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void Like_PrecedingUnderscoreSign_ValueDoesNotMatch_ReturnFalse()
        {
            var sut = "_foo";
            var wildcardText = "abfoo";

            var output = sut.Like(wildcardText);

            Assert.AreEqual(false, output);
        }


        [TestMethod]
        public void GetDateFiscalQuarter_DecQuarterAs1_ReturnMatch()
        {
            var sut = new DateTime(2017, 12, 1);
            var exp = "1";

            var output = sut.GetDateFiscalQuarter();

            Assert.AreEqual(exp, output);
        }
        
        [TestMethod]
        public void GetDateFiscalQuarter_OctQuarterAs1_ReturnMatch()
        {
            var sut = new DateTime(2017, 10, 1);
            var exp = "1";

            var output = sut.GetDateFiscalQuarter();

            Assert.AreEqual(exp, output);
        }


        [TestMethod]
        public void GetDateFiscalQuarter_SepQuarterAs4_ReturnMatch()
        {
            var sut = new DateTime(2017, 9, 1);
            var exp = "4";

            var output = sut.GetDateFiscalQuarter();

            Assert.AreEqual(exp, output);
        }

        [TestMethod]
        public void GetDateFiscalQuarter_JulQuarterAs4_ReturnMatch()
        {
            var sut = new DateTime(2017, 7, 1);
            var exp = "4";

            var output = sut.GetDateFiscalQuarter();

            Assert.AreEqual(exp, output);
        }


        [TestMethod]
        public void GetDateFiscalYear_TwoDatesIn2017_ReturnMatch()
        {
            //10/1/16-9/30/17 = 2017, 10/1/17-9/30/18=2018, etc
            var sut = new DateTime(2016, 10, 1);
            var sut2 = new DateTime(2017, 9, 30);

            var output = sut.GetDateFiscalYear();
            var output2 = sut2.GetDateFiscalYear();

            var exp = "2017";

            Assert.AreEqual(exp, output);
            Assert.AreEqual(exp, output2);
        }
        

        [TestMethod]
        public void GetDateFirstOfMonth_March18ReturnMarch1st_ReturnMatch()
        {
            //First of Month - Invoice Date (3/18/17 would be 3/1/17)
            var sut = new DateTime(2017, 3, 18);
            var exp = "2017/03/01";

            var output = sut.GetDateFirstOfMonth("yyyy/MM/dd");

            Assert.AreEqual(exp, output);
        }


        [TestMethod]
        public void GetDateFirstOfMonth_March25ReturnMarch1st1DigitMonth_ReturnMatch()
        {
            //First of Month - Invoice Date (3/18/17 would be 3/1/17)
            var sut = new DateTime(2017, 3, 25);
            var exp = "3/1/17";

            var output = sut.GetDateFirstOfMonth("M/d/yy");

            Assert.AreEqual(exp, output);
        }

        [TestMethod]        
        public void NormalizeColumnHeader__PassListsMatchLists_ReturnMatch()
        {
            var sut = new List<string> { "`123456789 0-=qwertyu", "iop[]sdf ghjkl;;..'zxc", "vbnm,./ ~!@#$%^&*(", ")_+QWERTYU IOP{}|ASDF", "GHJKL:''ZXC VBNM<>?", "\\\a\b\f\n\r\t\v\\U" };
            var exp = new List<string> { "_123456789_0_qwertyu", "iopsdf_ghjkl___zxc","vbnm___no_", "_qwertyu_iopasdf", "ghjkl__zxc_vbnm", "_____u"};

            for(var i=0; i<sut.Count; i++)
            {
                Assert.AreEqual(exp[i], sut[i].NormalizeColumnHeader());
            }
        }

        [TestMethod]
        public void ContainsIgnoreCase_string_is_null_return_false()
        {
            string s = null;

            var output = s.ContainsIgnoreCase("foo");

            Assert.IsFalse(output);
        }

        [TestMethod]
        public void Left_string_is_null_return_empty_string()
        {
            string s = null;

            var output = s.Left(1);

            Assert.AreEqual("", output);
        }

        [TestMethod]
        public void Left_string_is_not_null_return_left_substring()
        {
            string s = "foo";

            var output = s.Left(1);

            Assert.AreEqual("f", output);
        }

        [TestMethod]
        public void Right_string_is_null_return_empty_string()
        {
            string s = null;

            var output = s.Right(1);

            Assert.AreEqual("", output);
        }

        [TestMethod]
        public void Right_string_is_not_null_return_right_substring()
        {
            string s = "bar";

            var output = s.Right(1);

            Assert.AreEqual("r", output);
        }

        [TestMethod]
        public void RemoveLastChar_string_is_null_return_empty_string()
        {
            string s = null;

            var output = s.RemoveLastChar();

            Assert.AreEqual("", output);
        }

        [TestMethod]
        public void RemoveLastChar_string_is_empty_return_empty_string()
        {
            string s = "  ";

            var output = s.RemoveLastChar();

            Assert.AreEqual("", output);
        }

        [TestMethod]
        public void RemoveFirstChar_string_is_null_return_empty_string()
        {
            string s = null;

            var output = s.RemoveFirstChar();

            Assert.AreEqual("", output);
        }

        [TestMethod]
        public void RemoveFirstChar_string_is_empty_return_empty_string()
        {
            string s = "  ";

            var output = s.RemoveFirstChar();

            Assert.AreEqual("", output);
        }
    }
}

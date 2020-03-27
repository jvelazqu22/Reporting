using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Services.Implementation.Classes;
using iBank.Services.Implementation.Shared.AdvancedClause;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.AdvancedClause
{
    [TestClass]
    public class AdvancedParameterFixerTests
    {
        [TestMethod]
        public void TranslateValue1DayOfWeekNumericToCharacter_NoMatch_ReturnOriginalValue1()
        {
            var fixer = new AdvancedParameterFixer();
            var parm = new AdvancedParameter { Value1 = "0" };
            var expected = "0";

            var output = fixer.TranslateValue1DayOfWeekNumericToCharacter(parm);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void TranslateValue1DayOfWeekNumericToCharacter_MatchOn1_ReturnSun()
        {
            var fixer = new AdvancedParameterFixer();
            var parm = new AdvancedParameter { Value1 = "1" };
            var expected = "SUN";

            var output = fixer.TranslateValue1DayOfWeekNumericToCharacter(parm);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void TranslateValue1DayOfWeekNumericToCharacter_NonInt_ReturnOriginalValue()
        {
            var fixer = new AdvancedParameterFixer();
            var parm = new AdvancedParameter { Value1 = "1" };
            var expected = "SUN";

            var output = fixer.TranslateValue1DayOfWeekNumericToCharacter(parm);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void TranslateValue1DayOfWeekCharacterToNumeric_NoMatch_ReturnOriginalValue1()
        {
            var fixer = new AdvancedParameterFixer();
            var parm = new AdvancedParameter { Value1 = "Foo" };
            var expected = "Foo";

            var output = fixer.TranslateValue1DayOfWeekCharacterToNumeric(parm);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void TranslateValue1DayOfWeekCharacterToNumeric_MatchOnSun_Return1()
        {
            var fixer = new AdvancedParameterFixer();
            var parm = new AdvancedParameter { Value1 = "SUN" };
            var expected = "1";

            var output = fixer.TranslateValue1DayOfWeekCharacterToNumeric(parm);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void TranslateValue1DayOfWeekCharacterToNumeric_MatchOnSunDifferentCase_Return1()
        {
            var fixer = new AdvancedParameterFixer();
            var parm = new AdvancedParameter { Value1 = "sun" };
            var expected = "1";

            var output = fixer.TranslateValue1DayOfWeekCharacterToNumeric(parm);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void CreateCalculation_PhraseToMatchIsEntireString_ReturnTxtToFind()
        {
            var fixer = new AdvancedParameterFixer();
            var arg = "footest";
            var txtToFind = "footest";
            var txtToAdd = "bar";
            var expected = "bar";

            var output = fixer.CreateCalculation(arg, txtToFind, txtToAdd);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void CreateCalculation_PhraseToMatchIsSubstring_Return()
        {
            var fixer = new AdvancedParameterFixer();
            var arg = "footest";
            var txtToFind = "test";
            var txtToAdd = "bar";
            var expected = "foobar";

            var output = fixer.CreateCalculation(arg, txtToFind, txtToAdd);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void FixParameterAdvancedFieldName_NoMatch_ReturnOriginalValue()
        {
            var fixer = new AdvancedParameterFixer();
            var parm = new AdvancedParameter { FieldName = "#foo#" };
            var expected = "#foo#";

            var output = fixer.FixParameterAdvancedFieldName(parm);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_RECLOC_ReturnPrefixedWithT1()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "RECLOC";
            var tableName = "";
            var expected = "T1.RECLOC";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_ACCT_ReturnPrefixedWithT1()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "ACCT";
            var tableName = "";
            var expected = "T1.ACCT";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_PASSLAST_ReturnPrefixedWithT1()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "PASSLAST";
            var tableName = "";
            var expected = "T1.PASSLAST";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_PASSFRST_ReturnPrefixedWithT1()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "PASSFRST";
            var tableName = "";
            var expected = "T1.PASSFRST";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_INVOICE_ReturnPrefixedWithT1()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "INVOICE";
            var tableName = "";
            var expected = "T1.INVOICE";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_SVCFEE_ReturnPrefixedWithT1()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "SVCFEE";
            var tableName = "";
            var expected = "T1.SVCFEE";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_CARDNUM_ReturnPrefixedWithT1()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "CARDNUM";
            var tableName = "";
            var expected = "T1.CARDNUM";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_TRANTYPE_ReturnPrefixedWithT1()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "TRANTYPE";
            var tableName = "";
            var expected = "T1.TRANTYPE";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_SOURCEABBR_ReturnPrefixedWithT1()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "SOURCEABBR";
            var tableName = "";
            var expected = "T1.SOURCEABBR";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_GDS_ReturnPrefixedWithT1()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "GDS";
            var tableName = "";
            var expected = "T1.GDS";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_MONEYTYPE_ReturnPrefixedWithT1()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "MONEYTYPE";
            var tableName = "";
            var expected = "T1.MONEYTYPE";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_IATANBRAndTripsTable_ReturnPrefixedWithT1()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "IATANBR";
            var tableName = "TRIPS";
            var expected = "T1.IATANBR";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_IATNBRNotTripsTable_ReturnNotPrefixed()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "IATNBR";
            var tableName = "";
            var expected = "IATNBR";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_AMONEYTYPE_ReturnPrefixedWithT4AndDropTheA()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "AMONEYTYPE";
            var tableName = "";
            var expected = "T4.MONEYTYPE";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void HandleAmbiguousFields_HMONEYTYPE_ReturnPrefixedWithT5AndDropTheH()
        {
            var fixer = new AdvancedParameterFixer();
            var original = "HMONEYTYPE";
            var tableName = "";
            var expected = "T5.MONEYTYPE";

            var output = fixer.HandleAmbiguousFields(original, tableName);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void RemoveAdvancedFieldNameAndFieldNamePrefix_InSColumn_ReturnColumnWithoutS()
        {
            var advancedparams = new List<AdvancedParameter>
            {
                new AdvancedParameter
                    {
                        AdvancedFieldName = "SRECLOC",
                        FieldName = "SRECLOC"
                    }
            };
            var fixer = new AdvancedParameterFixer();

            var expected = new List<AdvancedParameter>
            {
                new AdvancedParameter
                    {
                        AdvancedFieldName = "RECLOC",
                        FieldName = "RECLOC"
                    }
            };

            var output = fixer.RemoveAdvancedFieldNameAndFieldNamePrefix(advancedparams);

            Assert.AreEqual(expected[0].AdvancedFieldName, output[0].AdvancedFieldName);
            Assert.AreEqual(expected[0].FieldName, output[0].FieldName);
        }

        [TestMethod]
        public void RemoveAdvancedFieldNameAndFieldNamePrefix_NotInSColumn_ReturnOriginalColumn()
        {
            var advancedparams = new List<AdvancedParameter>
            {
                new AdvancedParameter
                    {
                        AdvancedFieldName = "*RECLOC",
                        FieldName = "*RECLOC"
                    }
            };
            var fixer = new AdvancedParameterFixer();

            var expected = new List<AdvancedParameter>
            {
                new AdvancedParameter
                    {
                        AdvancedFieldName = "*RECLOC",
                        FieldName = "*RECLOC"
                    }
            };

            var output = fixer.RemoveAdvancedFieldNameAndFieldNamePrefix(advancedparams);

            Assert.AreEqual(expected[0].AdvancedFieldName, output[0].AdvancedFieldName);
            Assert.AreEqual(expected[0].FieldName, output[0].FieldName);
        }

        [TestMethod]
        public void RemoveAdvancedFieldNameAndFieldNamePrefix_OneInSColumnOneNotInSColumn_ReturnBothWithoutSPrefix()
        {
            var advancedparams = new List<AdvancedParameter>
            {
                new AdvancedParameter
                    {
                        AdvancedFieldName = "SRECLOC",
                        FieldName = "SRECLOC"
                    },
                new AdvancedParameter
                    {
                        AdvancedFieldName = "*RECLOC",
                        FieldName = "*RECLOC"
                    }
            };
            var fixer = new AdvancedParameterFixer();

            var expected = new List<AdvancedParameter>
            {
                new AdvancedParameter
                    {
                        AdvancedFieldName = "RECLOC",
                        FieldName = "RECLOC"
                    },
                new AdvancedParameter
                    {
                        AdvancedFieldName = "*RECLOC",
                        FieldName = "*RECLOC"
                    }
            };

            var output = fixer.RemoveAdvancedFieldNameAndFieldNamePrefix(advancedparams);

            for (var i = 0; i < output.Count; i++)
            {
                Assert.AreEqual(expected[i].AdvancedFieldName, output[i].AdvancedFieldName);
                Assert.AreEqual(expected[i].FieldName, output[i].FieldName);
            }
            
        }
    }
}

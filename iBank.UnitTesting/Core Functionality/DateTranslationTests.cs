using Domain.Orm.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.UnitTesting.Core_Functionality
{
    [TestClass]
    public class DateTranslationTests
    {
        /*
           Date translation in French
        */

        private static BuildWhere buildWhere;
        public DateTranslationTests()
        {
        }

        [TestInitialize]
        public void Initialize()
        {
            var baseBeginDate = new DateTime(2015, 12, 1);
            var baseEndDate = new DateTime(2015, 12, 5);
            var processKey = 18; //ibFareSaving
            var outputLang = "CS";
            baseEndDate = baseEndDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            buildWhere = InstantiateBuildWhere(baseBeginDate, baseEndDate, processKey, outputLang);
        }

        [TestMethod]
        public void TranslateDDMMMTest()
        {
            string expectLabel = "01Kve";
            DateTime d = new DateTime(2015, 5, 1);
            var actualLabel = buildWhere.ReportGlobals.TranslateDDMMMDate(d);
            Assert.AreEqual(expectLabel, actualLabel, true);
        }

        private BuildWhere InstantiateBuildWhere(DateTime? beginDate, DateTime? endDate, int processKey, string langCode)
        {
            var buildWhere = new BuildWhere(new ClientFunctions())
            {
                SqlParameters = new List<SqlParameter>()
            };

            buildWhere = SetGlobals(buildWhere, beginDate, endDate, processKey, langCode);
            return buildWhere;
        }

        private BuildWhere SetGlobals(BuildWhere buildWhere, DateTime? beginDate, DateTime? endDate, int processKey, string langCode)
        {
            buildWhere.ReportGlobals.BeginDate = beginDate;
            buildWhere.ReportGlobals.EndDate = endDate;
            buildWhere.ReportGlobals.OutputLanguage = langCode;
            buildWhere.ReportGlobals.ProcessKey = processKey;
            var langVariables = new List<LanguageVariableInfo>
            {
                new LanguageVariableInfo
                    {
                         VariableName = "lt_abbrMthsofYear",
                         NumberOfLines = 1,
                         Translation = "led, úno, bre, dub, kve, cvn, cvc, srp, zár, ríj, lis, pro",
                         TagType = "Special"
                    }
            };
            
            buildWhere.ReportGlobals.LanguageVariables = langVariables;


            return buildWhere;
        }

    }
}

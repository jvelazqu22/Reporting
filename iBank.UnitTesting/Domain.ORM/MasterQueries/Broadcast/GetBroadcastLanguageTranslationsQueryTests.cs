using System.Collections.Generic;
using System.Linq;

using Domain.Orm.iBankMastersQueries.BroadcastQueries;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.Domain.ORM.MasterQueries.Broadcast
{

    [TestClass]
    public class GetBroadcastLanguageTranslationsQueryTests
    {
        private Mock<IMastersQueryable> Initialize()
        {
            var mockDb = new Mock<IMastersQueryable>();

            var funcLangTagsData = new List<ibFuncLangTags>
                                       {
                                           new ibFuncLangTags
                                               {
                                                   FunctionLink = 9801,
                                                   TagLink = 10153
                                               }
                                       };
            var funcLangTagsSet = new Mock<IQueryable<ibFuncLangTags>>();
            funcLangTagsSet.SetupIQueryable(funcLangTagsData.AsQueryable());
            mockDb.Setup(x => x.ibFuncLangTags).Returns(funcLangTagsSet.Object);

            var languageTagsData = new List<LanguageTags>
                                       {
                                           new LanguageTags
                                               {
                                                   VarName = "lt_Report",
                                                   NbrLines = 1,
                                                   TagNo = 10153
                                               }
                                       };
            var languageTagsSet = new Mock<IQueryable<LanguageTags>>();
            languageTagsSet.SetupIQueryable(languageTagsData.AsQueryable());
            mockDb.Setup(x => x.LanguageTags).Returns(languageTagsSet.Object);

            var languageTranslationsData = new List<LanguageTranslations>
                                               {
                                                   new LanguageTranslations
                                                       {
                                                           TagLink = 10153,
                                                           LangCode = "EN",
                                                           Translatn = "Report"
                                                       }
                                               };
            var languageTranslationsSet = new Mock<IQueryable<LanguageTranslations>>();
            languageTranslationsSet.SetupIQueryable(languageTranslationsData.AsQueryable());
            mockDb.Setup(x => x.LanguageTranslations).Returns(languageTranslationsSet.Object);

            return mockDb;
        }

        [TestMethod]
        public void Valid()
        {
            var mockDb = Initialize();

            var query = new GetBroadcastLanguageTranslationsQuery(mockDb.Object, "EN");
            var actual = query.ExecuteQuery();

            Assert.AreEqual(1, actual.Count);
        }

        [TestMethod]
        public void WrongLanguage()
        {
            var mockDb = Initialize();

            var query = new GetBroadcastLanguageTranslationsQuery(mockDb.Object, "ZZ");
            var actual = query.ExecuteQuery();

            Assert.AreEqual(0, actual.Count);
        }
    }
}

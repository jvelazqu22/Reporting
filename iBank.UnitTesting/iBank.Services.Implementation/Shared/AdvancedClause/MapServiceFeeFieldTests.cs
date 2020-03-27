using System;
using System.Collections.Generic;
using Domain.Helper;
using Domain.Orm.Classes;
using iBank.Services.Implementation.Shared.AdvancedClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.AdvancedClause
{
    [TestClass]
    public class MapServiceFeeFieldTests
    {

        [TestMethod]
        public void MapServiceFeeField__FieldInSelectedCases_ReturnNotMatchExpectedValues()
        {
            //Arrange
            var exp = new List<string> { "SFCARDNUM", "SVCAMT", "SVCDESC" };

            var advancedParams = new List<AdvancedColumnInformation>
            {
                new AdvancedColumnInformation{ColName = "SCARDNUM", AdvancedColName="CARDNUM", ColTable="SVCFEE"},
                new AdvancedColumnInformation{ColName = "SSVCFEE", AdvancedColName="SVCFEE", ColTable="SVCFEE"},
                new AdvancedColumnInformation{ColName = "SDESCRIPT", AdvancedColName="DESCRIPT", ColTable="SVCFEE"}
            };

            var param = new AdvancedParameter { FieldName = "XXX", AdvancedFieldName = "YYY" };

            var act = new List<string>();
            //act
            var processor = new AdvancedClauseProcessor(advancedParams);
            for (var i = 0; i < advancedParams.Count; i++) 
            {
                var field = processor.MapServiceFeeField(advancedParams[i], param);

                act.Add(field);
            }

            //assert
            for (var i = 0; i < advancedParams.Count; i++)
            {
                Assert.AreEqual(exp[i], act[i]);
            }
        }

        [TestMethod]
        public void MapServiceFeeField_FieldNotInSelectedCases_ReturnNotMatchColName()
        {
            //Arrange
            var exp = new List<string> { "SABC" };

            var advancedParams = new List<AdvancedColumnInformation>
            {
                new AdvancedColumnInformation{ColName = "SABC", AdvancedColName="ABC", ColTable="SVCFEE"}
            };

            var param = new AdvancedParameter { FieldName = "XXX", AdvancedFieldName = "YYY" };

            var act = new List<string>();
            //act
            var processor = new AdvancedClauseProcessor(advancedParams);
            for (var i = 0; i < advancedParams.Count; i++)
            {
                var field = processor.MapServiceFeeField(advancedParams[i], param);

                act.Add(field);
            }

            //assert
            for (var i = 0; i < advancedParams.Count; i++)
            {
                Assert.AreNotEqual(exp[i], act[i]);
            }
        }

    }
}

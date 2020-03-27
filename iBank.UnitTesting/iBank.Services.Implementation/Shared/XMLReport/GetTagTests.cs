using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers;
using iBank.Services.Implementation.Shared.XMLReport;
using Domain.Orm.Classes;
using iBank.Server.Utilities.Classes;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.XMLReport
{
    /// <summary>
    /// Summary description for GetTagTests
    /// </summary>
    [TestClass]
    public class GetTagTests
    {
        public GetTagTests()
        {
            Init();
        }


        private List<XmlTag> _data;

        public void Init()
        {
            _data = new List<XmlTag>
            {
                new XmlTag
                    {
                      IsOn = true,
                      TagName = "Col1",
                      TagType = "da"
                    },
                new XmlTag
                {
                    IsOn = false,
                    TagName = "somethang",
                    TagType = "pa"
                }
            };
        }


        [TestMethod]
        public void GetTag_IsNoIsTrue_ReturnTag()
        {
            //Arrange
            var dataStructure = new XmlDataStructure(0, new ReportGlobals());
            dataStructure.Tags = _data;
            var helper = new XMLReportHelper("ns", dataStructure);

            //Act
            var act = helper.GetTag(_data[0].TagName, _data[0].TagType);

            //Assert
            Assert.AreEqual(act.TagName, _data[0].TagName);            

        }

        [TestMethod]
        public void GetTag_IsNoIsOff_ReturnNoTag()
        {
            //Arrange
            var dataStructure = new XmlDataStructure(0, new ReportGlobals());
            dataStructure.Tags = _data;
            var helper = new XMLReportHelper("ns", dataStructure);

            //Act
            var act = helper.GetTag(_data[1].TagName, _data[1].TagType);

            //Assert
            Assert.IsNull(act);

        }
    }
}

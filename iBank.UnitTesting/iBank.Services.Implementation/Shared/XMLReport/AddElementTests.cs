using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers;
using iBank.Services.Implementation.Shared.XMLReport;
using Domain.Orm.Classes;
using iBank.Server.Utilities.Classes;
using System.Xml.Linq;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.XMLReport
{
    [TestClass]
    public class AddElementTests
    {
        private List<XmlTag> _data;

        public AddElementTests()
        {
            Init();
        }
        
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
        public void AddElement_IsOnIsTrue_MatchNode()
        {
            //Arrange            
            var dataStructure = new XmlDataStructure(0, new ReportGlobals());
            dataStructure.Tags = _data;
            var helper = new XMLReportHelper("ns", dataStructure);
            var expElement = new XElement("bar");
            var actElement = new XElement("bar");

            object tagValue = "Test";

            var currentTag = helper.GetTag(_data[0].TagName, _data[0].TagType);
            if (currentTag != null)
            {
                var val = ValueConverter.ConvertValue(tagValue, currentTag.Mask);
                expElement.Add(new XElement(helper.xmlns + currentTag.ActiveName, val));
            }
            var expValue = expElement.LastNode.ToString();

            //Act
            helper.AddElement(actElement, _data[0].TagName, _data[0].TagType, tagValue);
            var actValue = actElement.LastNode.ToString();

            //Assert
            Assert.AreEqual(expValue, actValue);
        }

        [TestMethod]
        public void AddElement_IsOnIsTrue_ReturnNodeIsNotEqualTheOriginalOne()
        {
            //Arrange            
            var dataStructure = new XmlDataStructure(0, new ReportGlobals());
            dataStructure.Tags = _data;
            var helper = new XMLReportHelper("ns", dataStructure);
            var expElement = new XElement("bar");
            var actElement = new XElement("bar");

            object tagValue = "Test";

            //Act
            helper.AddElement(actElement, _data[0].TagName, _data[0].TagType, tagValue);

            //Assert
            Assert.AreNotEqual(expElement, actElement);
        }


        [TestMethod]
        public void AddElement_IsOnIsTrue_IsOnIsTrue_ReturnNodeIsNotEqualTheOriginalOne()
        {
            //Arrange            
            var dataStructure = new XmlDataStructure(0, new ReportGlobals());
            dataStructure.Tags = _data;
            var helper = new XMLReportHelper("ns", dataStructure);
            var expElement = new XElement("bar");
            var actElement = new XElement("bar");

            var expValue = "<bar />";
            object tagValue = "Test";

            //Act
            helper.AddElement(actElement, _data[1].TagName, _data[1].TagType, tagValue);
            var actValue = actElement.ToString();
            //Assert
            Assert.AreEqual(expValue, actValue);
        }
    }
}

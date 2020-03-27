using Domain.Orm.Classes;
using System;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Classes.Interfaces
{
    public interface IXmlReportBuilder
    {
        XElement BuildCriteria(XmlTag mainTag, String title, string format);
    }
}

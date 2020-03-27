using Domain.Models;

namespace Domain.Interfaces
{
    public interface IXmlSqlScript
    {
        SqlScript GetSqlScript(bool udidExists, bool isPreview, string whereClause);
    }
}

using Domain.Models;

namespace Domain.Interfaces
{
    public interface IUserDefinedSqlScript
    {
        SqlScript GetSqlScript(bool isTripTls, bool udidExists, bool isPreview, string whereClause);
    }
}

using Domain.Interfaces.Database;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.Interfaces
{
    public interface ICommand
    {
        ICommandDb Db { get; set; }
        void ExecuteCommand();
    }
}

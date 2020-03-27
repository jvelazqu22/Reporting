using iBank.Entities.MasterEntities;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Interfaces.Query
{
    public interface IDatabaseInfoQuery : IQuery<iBankDatabases>
    {
        bool HasDbBeenDisposed { get; set; }
        string DatabaseName { get; set; }
    }
}

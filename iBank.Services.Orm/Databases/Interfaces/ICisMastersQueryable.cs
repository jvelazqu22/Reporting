using Domain.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;

namespace iBank.Services.Orm.Databases.Interfaces
{
    public interface ICisMastersQueryable : IDisposable, IPrototype
    {
        IQueryable<AirMileage> AirMileage { get; }
        Database Database { get; }
    }
}

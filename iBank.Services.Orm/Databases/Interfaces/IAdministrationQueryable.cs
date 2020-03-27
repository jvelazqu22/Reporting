using Domain.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;

namespace iBank.Services.Orm.Databases.Interfaces
{
    public interface IAdministrationQueryable : IDisposable, IPrototype
    {
        IQueryable<SvrStatus> SvrStatus { get; }
        IQueryable<broadcast_servers> BroadcastServers { get; }
        IQueryable<broadcast_server_function> BroadcastServerFunction { get; }
        Database Database { get; }
    }
}

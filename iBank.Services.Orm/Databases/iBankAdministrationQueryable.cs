using System.Data.Entity;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.Databases
{
    public class iBankAdministrationQueryable : IAdministrationQueryable
    {
        private readonly iBankAdministrationEntities _context;

        public iBankAdministrationQueryable()
        {
            _context = new iBankAdministrationEntities();
        }

        public IQueryable<SvrStatus> SvrStatus
        {
            get
            {
                return _context.SvrStatus;
            }
        }

        public IQueryable<broadcast_servers> BroadcastServers
        {
            get
            {
                return _context.broadcast_servers;
            }
        }
        public IQueryable<broadcast_server_function> BroadcastServerFunction
        {
            get
            {
                return _context.broadcast_server_function;
            }
        }

        public Database Database { get
        {
            return _context.Database;
        } }

        public void Dispose()
        {
            _context.Dispose();
        }

        public object Clone()
        {
            return new iBankAdministrationQueryable();
        }

    }
}

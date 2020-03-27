using System.Data.Entity;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.Databases
{
    public class CisMastersQueryable : ICisMastersQueryable
    {
        private readonly CISMasterEntities _context;

        public CisMastersQueryable()
        {
            _context = new CISMasterEntities();
        }

        public IQueryable<AirMileage> AirMileage { get
        {
            return _context.AirMileages;
        } }

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
            return new CisMastersQueryable();
        }
    }
}

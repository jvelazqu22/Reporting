using System;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Agency
{
    public class GetAgencyiATANumQuery : IQuery<string>
    {
        public string Agency { get; set; }
        private readonly IMastersQueryable _db;

        public GetAgencyiATANumQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency.Trim();
        }

        public string ExecuteQuery()
        {
            using (_db)
            {
                var iataNum = _db.MstrAgcy.FirstOrDefault(s => s.agency.Trim().Equals(Agency, StringComparison.OrdinalIgnoreCase));
                return iataNum == null ? string.Empty : iataNum.iatanum;                
            }
        }
    }
}

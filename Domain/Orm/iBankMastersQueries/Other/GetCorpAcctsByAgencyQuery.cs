using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetCorpAcctsByAgencyQuery : IQuery<MasterAgencyInformation>
    {
        public string Agency { get; set; }
        private readonly IMastersQueryable _db;

        public GetCorpAcctsByAgencyQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public MasterAgencyInformation ExecuteQuery()
        {
            using(_db)
            {
                return _db.MstrCorpAccts.Where(x => x.CorpAcct == Agency).Select(s => new MasterAgencyInformation
                {
                    DatabaseName = s.databasename.Trim(),
                    AgencyName = s.CorpName.Trim(),
                    ReasonExclude = s.reasexclude.Trim(),
                    UseServiceFees = s.servicefees,
                    iBankVersion = s.ibankversion.Trim(),
                    ClientURL = s.ClientURL.Trim(),
                    SpecialUse1 = "0"
                }).FirstOrDefault();
            }
        }
    }
}

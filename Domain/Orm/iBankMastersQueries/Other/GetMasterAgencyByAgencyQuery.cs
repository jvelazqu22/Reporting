using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetMasterAgencyByAgencyQuery : IQuery<MasterAgencyInformation>
    {
        public string Agency { get; set; }
        private readonly IMastersQueryable _db;

        public GetMasterAgencyByAgencyQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;

        }
        public MasterAgencyInformation ExecuteQuery()
        {
            using(_db)
            {
                return _db.MstrAgcy.Where(x => x.agency == Agency).Select(s => new MasterAgencyInformation
                {
                    DatabaseName = s.databasename.Trim(),
                    AgencyName = s.agencyname.Trim(),
                    ReasonExclude = s.reasexclude.Trim(),
                    UseServiceFees = s.servicefees,
                    iBankVersion = s.ibankversion.Trim(),
                    ClientURL = s.ClientURL.Trim(),
                    SpecialUse1 = s.specialUse1.Trim(),
                    Address1 = s.agcyaddr1.Trim(),
                    Address2 = s.agcyaddr2.Trim(),
                    Address3 = s.agcyaddr3.Trim(),
                    Address4 = s.agcyaddr4.Trim()
                }).FirstOrDefault();
            }
        }
    }
}

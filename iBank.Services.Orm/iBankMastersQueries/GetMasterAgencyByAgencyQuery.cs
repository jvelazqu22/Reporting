using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetMasterAgencyByAgencyQuery : BaseiBankMastersQuery<MasterAgencyInformation>
    {
        public string Agency { get; set; }

        public GetMasterAgencyByAgencyQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;

        }
        public override MasterAgencyInformation ExecuteQuery()
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

using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetCorpAcctsByAgencyQuery : BaseiBankMastersQuery<MasterAgencyInformation>
    {
        public string Agency { get; set; }

        public GetCorpAcctsByAgencyQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public override MasterAgencyInformation ExecuteQuery()
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

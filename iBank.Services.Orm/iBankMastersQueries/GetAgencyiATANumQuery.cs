using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAgencyiATANumQuery : BaseiBankMastersQuery<string>
    {
        public string Agency { get; set; }

        public GetAgencyiATANumQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency.Trim();
        }

        public override string ExecuteQuery()
        {
            using (_db)
            {
                var iataNum = _db.MstrAgcy.FirstOrDefault(s => s.agency.Trim().Equals(Agency, StringComparison.OrdinalIgnoreCase));
                return iataNum == null ? string.Empty : iataNum.iatanum;                
            }
        }
    }
}

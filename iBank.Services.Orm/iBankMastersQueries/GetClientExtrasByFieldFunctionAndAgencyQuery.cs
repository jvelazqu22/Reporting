using System;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetClientExtrasByFieldFunctionAndAgencyQuery : BaseiBankMastersQuery<string>
    {
        public string Agency { get; set; }
        public string FieldFunction { get; set; }

        public GetClientExtrasByFieldFunctionAndAgencyQuery(IMastersQueryable db, string agency, string fieldFunction)
        {
            _db = db;
            Agency = agency.Trim();
            FieldFunction = fieldFunction.Trim();
        }

        public override string ExecuteQuery()
        {
            using (_db)
            {
                var extra = _db.ClientExtras.FirstOrDefault(x => x.ClientCode.Equals(Agency, StringComparison.OrdinalIgnoreCase) &&
                                                            (x.FieldFunction.Trim().Equals(FieldFunction, StringComparison.OrdinalIgnoreCase)));

                return extra == null ? "" : extra.FieldData;
            }
        }
    }
}

using System;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetClientExtrasByFieldFunctionAndAgencyQuery : IQuery<string>
    {
        public string Agency { get; set; }
        public string FieldFunction { get; set; }
        private readonly IMastersQueryable _db;

        public GetClientExtrasByFieldFunctionAndAgencyQuery(IMastersQueryable db, string agency, string fieldFunction)
        {
            _db = db;
            Agency = agency.Trim();
            FieldFunction = fieldFunction.Trim();
        }

        public string ExecuteQuery()
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

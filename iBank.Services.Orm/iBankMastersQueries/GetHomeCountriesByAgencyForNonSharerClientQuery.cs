using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetHomeCountriesByAgencyForNonSharerClientQuery : BaseiBankMastersQuery<IList<KeyValue>>
    {
        public string Agency { get; set; }

        public GetHomeCountriesByAgencyForNonSharerClientQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public override IList<KeyValue> ExecuteQuery()
        {
            using(_db)
            {
                return _db.MstrAgcySources.Where(x => x.agency.Equals(Agency)).Select(x => new KeyValue
                                                                                               {
                                                                                                   Key = x.SourceAbbr,
                                                                                                   Value = x.HomeCtry
                                                                                               }).ToList();
            }
        }
    }
}

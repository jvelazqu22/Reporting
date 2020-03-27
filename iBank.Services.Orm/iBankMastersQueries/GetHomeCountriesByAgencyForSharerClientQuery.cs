using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetHomeCountriesByAgencyForSharerClientQuery : BaseiBankMastersQuery<IList<KeyValue>>
    {
        public string Agency { get; set; }

        public GetHomeCountriesByAgencyForSharerClientQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public override IList<KeyValue> ExecuteQuery()
        {
            using(_db)
            {
                return _db.MstrAgcySources.Join(_db.JunctionAgcyCorp
                    .Where(s => s.CorpAcct.Equals(Agency)), m => m.agency, j => j.agency, (m, j) => new KeyValue
                                                                                                        {
                                                                                                            Key = m.SourceAbbr,
                                                                                                            Value = m.HomeCtry
                                                                                                        }).ToList();
            }
        }
    }
}

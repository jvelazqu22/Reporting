using Domain.Orm.Classes;
using iBank.Entities.MasterEntities;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Extensions
{
    public static class MstragcyExtensions
    {
  
        public static List<DatabaseInformation> ToDatabaseInformation(this IQueryable<mstragcy> mstragcy, IQueryable<bcstque4> bcstque)
        {
            var query = mstragcy.Where(s => bcstque.Select(x => x.agency).Contains(s.agency));

            return query.Select(s => new DatabaseInformation
                                {
                                    DatabaseName = s.databasename.Trim().ToLower(),
                                    TimeZoneOffset = s.tzoffset ?? 0
                                })
                                .ToList();

        }

        public static List<DatabaseInformation> ToDatabaseInformation(this IQueryable<mstragcy> mstragcy, IQueryable<bcstque4> bcstque, IQueryable<JunctionAgcyCorp> junctionAgcyCorp)
        {
             //if the agency is corpacct, will get agencies the corp account assigned to
            var corpAcctAgencies = junctionAgcyCorp.Where(x => bcstque.Select(y => y.agency).Contains(x.CorpAcct))
                                    .Select(x => x.agency)
                                    .ToList();
                       
            return mstragcy.Where(s => corpAcctAgencies.Contains(s.agency))
                         .Select(s => new DatabaseInformation
                         {
                             DatabaseName = s.databasename.Trim().ToLower(),
                             TimeZoneOffset = s.tzoffset ?? 0
                         })
                        .ToList();            

        }
    }
}

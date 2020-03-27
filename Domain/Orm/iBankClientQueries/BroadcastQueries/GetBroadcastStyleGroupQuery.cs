using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetBroadcastStyleGroupQuery : IQuery<IList<StyleGroupExtras>>
    {
        public string BroadcastLanguage { get; set; }
        public int StyleGroupNumber { get; set; }

        private readonly IClientQueryable _db;

        public GetBroadcastStyleGroupQuery(IClientQueryable db, string broadcastLanguage, int styleGroupNumber)
        {
            _db = db;
            BroadcastLanguage = broadcastLanguage;
            StyleGroupNumber = styleGroupNumber;
        }
        public IList<StyleGroupExtras> ExecuteQuery()
        {
            using (_db)
            {
                return _db.StyleGroupExtra.Where(x => x.SGroupNbr == StyleGroupNumber
                                                    && x.FieldFunction.Contains("BCAST") 
                                                    && x.FieldFunction.Contains("MSGTEXT") 
                                                    && x.LangCode.Equals(BroadcastLanguage, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
    }
}

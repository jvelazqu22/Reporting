using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetBroadcastStyleGroupQuery : BaseiBankClientQueryable<IList<StyleGroupExtra>>
    {
        public string BroadcastLanguage { get; set; }
        public int StyleGroupNumber { get; set; }

        public GetBroadcastStyleGroupQuery(IClientQueryable db, string broadcastLanguage, int styleGroupNumber)
        {
            _db = db;
            BroadcastLanguage = broadcastLanguage;
            StyleGroupNumber = styleGroupNumber;
        }
        public override IList<StyleGroupExtra> ExecuteQuery()
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

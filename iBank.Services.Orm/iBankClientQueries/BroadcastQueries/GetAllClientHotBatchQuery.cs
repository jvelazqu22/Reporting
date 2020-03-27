using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetAllClientHotBatchQuery : BaseiBankClientQueryable<IList<ibbatch>>
    {
        public DateTime CycleTimeZone { get; set; }
        private IList<string> Agencies { get; set; }

        public GetAllClientHotBatchQuery(IClientQueryable db, DateTime cycleTimeZone, IList<string> agencies)
        {
            _db = db;
            CycleTimeZone = cycleTimeZone;
            Agencies = agencies;
        }

        public override IList<ibbatch> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch.Where(x => Agencies.Contains(x.agency.Trim().ToUpper())
                                                && !x.errflag
                                                && x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest)
                                                && !x.holdrun.Equals(BroadcastCriteria.HoldOnRecord, StringComparison.OrdinalIgnoreCase)
                                                && (x.runspcl || (x.nextrun < CycleTimeZone))).ToList();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using Domain.Helper;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.iBankMastersQueries.BroadcastQueries.Helpers;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetPendingBroadcastQuery : BaseiBankMastersQuery<IList<bcstque4>>
    {
        private string UnilanguageCode { get; }
        private BroadcastServerFunction Function { get; }
        public GetPendingBroadcastQuery(IMastersQueryable db, string unilanguageCode, BroadcastServerFunction function)
        {
            _db = db;
            UnilanguageCode = unilanguageCode;
            Function = function;
        }

        public override IList<bcstque4> ExecuteQuery()
        {
            List<bcstque4> batches;

            var filter = new PendingBroadcastBatchFilter();
            var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            using (_db)
            {
                switch (Function)
                {
                    case BroadcastServerFunction.Primary:
                        batches = filter.GetPrimaryServerBatches(_db.BcstQue4, _db.BroadcastStageAgencies, _db.BroadcastLongRunningAgencies);
                        break;
                    case BroadcastServerFunction.Offline:
                        batches = filter.GetOfflineServerBatches(_db.BcstQue4, _db.BroadcastStageAgencies, _db.BroadcastLongRunningAgencies);
                        break;
                    case BroadcastServerFunction.Hot:
                        batches = filter.GetHotServerBatches(_db.BcstQue4, _db.BroadcastStageAgencies, _db.BroadcastLongRunningAgencies);
                        break;
                    case BroadcastServerFunction.AgencyStage:
                        batches = filter.GetStageServerBatches(_db.BcstQue4, _db.BroadcastStageAgencies);
                        scope.Complete();
                        return batches;
                    case BroadcastServerFunction.LongRunning:
                        batches = filter.GetLongRunningServerBatches(_db.BcstQue4, _db.BroadcastLongRunningAgencies);
                        scope.Complete();
                        return batches;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Broadcast Server Function {0} not handled.", Function));
                }

                scope.Complete();
            }

            return string.IsNullOrEmpty(UnilanguageCode) 
                ? batches.Where(x => string.IsNullOrEmpty(x.unilangcode.Trim())).ToList() 
                : batches.Where(x => x.unilangcode.Trim().Equals(UnilanguageCode.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
        }

        
    }
}

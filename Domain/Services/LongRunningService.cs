using System;
using System.Collections.Generic;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Extensions;
using Domain.Helper;
using Domain.Orm.iBankMastersCommands.LongRunning;
using Domain.Orm.iBankMastersQueries.BroadcastQueries.LongRunning;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using NodaTime;
using NodaTime.Extensions;

namespace Domain.Services
{
    public class LongRunningService
    {
        private readonly ICacheService _cache;
        private readonly IMasterDataStore _store;
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public LongRunningService(ICacheService cache, IMasterDataStore store)
        {
            _cache = cache;
            _store = store;
        }

        public virtual DateTime GetNow() => DateTime.Now;
        public void LogInstance(bcstque4 bcst)
        {
            try
            {
                var rec = new BroadcastLongRunningLog
                {
                    Agency = bcst.agency.Trim(),
                    RangeStartDate = bcst.nxtdstart ?? new DateTime(1900, 1, 1),
                    RangeEndDate = bcst.nxtdend ?? new DateTime(1900, 1, 1),
                    RequestDate = GetNow(),
                    UserNumber = bcst.UserNumber ?? 0,
                    BroadcastName = bcst.batchname,
                    BroadcastNumber = bcst.batchnum
                };

                //this call can be contained in an outer scope so we need a new scope for this call
                using (var scope = TransactionScopeBuilder.BuildScope())
                {
                    try
                    {
                        var cmd = new AddLongRunningInstanceRecordCommand(_store.MastersCommandDb, rec);
                        cmd.ExecuteCommand();
                    }
                    finally
                    {
                        scope.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                //don't want this to cause an actual error, so log and move along
                var msg = $"Error occurred adding long running log instance record. Agency: [{bcst.agency}], User: [{bcst.UserNumber}], Start: [{bcst.nxtdstart}], End: [{bcst.nxtdend}]";
                LOG.Error(msg, e);
            }
        }

        /// <summary>
        /// Returns true if the number of months in date range is over or equal to the threshold set for the agency. Does not take into account
        /// if the broadcast itself is Hot, Logging, or Stage. Apply those business rules prior to calling this method if applicable.
        /// </summary>
        /// <param name="bcst"></param>
        /// <returns></returns>w
        public bool IsLongRunningBroadcast(bcstque4 bcst)
        {
            var thresholdHelper = new LongRunningThresholdsHelper(_store, _cache);
            var thresholdRange = thresholdHelper.GetAgencyDateRangeLimit(bcst.agency.Trim());

            //if runspcl is checked, we want to see if the range is qualitified for long-running broadcast
            var actualRange = 0;
            if (bcst.runspcl)
            {
                actualRange = DateRangeHelper.GetSpecialDateRangeInterval(bcst);
            }
            else
            {
                actualRange = DateRangeHelper.GetActualDateRangeInterval(bcst);
            }

            return actualRange >= thresholdRange;
        }      
               
    }
}


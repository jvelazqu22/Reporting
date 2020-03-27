using System;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Constants;
using Domain.Helper;
using Domain.Services;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBankDomain.Interfaces;

namespace iBank.BroadcastServer.QueueManager.Factories
{
    public class BroadcastQueueRecordFactory : IFactory<bcstque4>
    {
        private ibbatch Batch { get; set; }
        private int BroadcastSequenceNumber { get; set; }
        private string DatabaseName { get; set; }
        private bool _isItLongRunning = false;
        IQueryable<broadcast_stage_agencies> _broadcastStageAgencies;
        private LongRunningService _longRunningService;
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BroadcastQueueRecordFactory(ibbatch batch, int broadcastSequenceNumber, string databaseName, 
            IQueryable<broadcast_stage_agencies> broadcastStageAgencies, LongRunningService longRunningService)
        {
            Batch = batch;
            BroadcastSequenceNumber = broadcastSequenceNumber;
            DatabaseName = databaseName;
            _broadcastStageAgencies = broadcastStageAgencies;
            _longRunningService = longRunningService;
        }

        public bcstque4 Build()
        {
            var broadcast = new bcstque4
            {
                bcstseqno = BroadcastSequenceNumber,
                dbname = DatabaseName,
                svrstatus = "PENDING",
                UserNumber = Batch.UserNumber,
                agency = Batch.agency,
                batchnum = Batch.batchnum,
                batchname = Batch.batchname.Trim(),
                emailaddr = string.IsNullOrWhiteSpace(Batch.emailaddr)
                                                ? string.Empty
                                                : Batch.emailaddr.Trim(),
                emailccadr = string.IsNullOrWhiteSpace(Batch.emailccadr)
                                                ? string.Empty
                                                : Batch.emailccadr.Trim(),
                acctlist =string.IsNullOrWhiteSpace(Batch.acctlist)
                                                ? string.Empty
                                                : Batch.acctlist.Trim(),
                prevhist = Batch.prevhist,
                weekmonth = Batch.weekmonth,
                monthstart = Batch.monthstart,
                monthrun = Batch.monthrun,
                weekstart = Batch.weekstart,
                weekrun = Batch.weekrun,
                nxtdstart = Batch.nxtdstart,
                nxtdend = Batch.nxtdend,
                lastrun = Batch.lastrun,
                lastdstart = Batch.lastdstart,
                lastdend = Batch.lastdend,
                errflag = Batch.errflag,
                runspcl = Batch.runspcl,
                spclstart = Batch.spclstart,
                spclend = Batch.spclend,
                pagebrklvl = Batch.pagebrklvl,
                titleacct = string.IsNullOrWhiteSpace(Batch.titleacct)
                                                ? string.Empty
                                                : Batch.titleacct.Trim(),
                bcsenderemail = string.IsNullOrWhiteSpace(Batch.bcsenderemail)
                                                ? string.Empty
                                                : Batch.bcsenderemail.Trim(),
                bcsendername = string.IsNullOrWhiteSpace(Batch.bcsendername)
                                                ? string.Empty
                                                : Batch.bcsendername.Trim(),
                nextrun = Batch.nextrun,
                setby = Batch.setby,
                holdrun = Batch.holdrun,
                reportdays = Batch.reportdays,
                rptusernum = Batch.rptusernum,
                usespcl = Batch.usespcl,
                nodataoptn = Batch.nodataoptn,
                displayuid = Batch.displayuid,
                emailsubj = string.IsNullOrWhiteSpace(Batch.emailsubj)
                                                ? string.Empty
                                                : Batch.emailsubj.Trim(),
                mailformat = string.IsNullOrWhiteSpace(Batch.mailformat) ? "1" : Batch.mailformat,
                outputtype = string.IsNullOrWhiteSpace(Batch.outputtype) ? "3" : Batch.outputtype,
                outputdest = string.IsNullOrWhiteSpace(Batch.outputdest) ? "2" : Batch.outputdest,
                eProfileNo = Batch.eProfileNo,
                runnewdata = Batch.RunNewData,
                unilangcode = string.IsNullOrWhiteSpace(Batch.unilangcode)
                                                ? string.Empty
                                                : Batch.unilangcode,
                send_error_email = Batch.send_error_email
            };

            if (IsBroadcastLongRunning(broadcast)) broadcast.broadcasttype = BroadcastTypes.LongRunning;

            return broadcast;
        }

        private bool IsBroadcastLongRunning(bcstque4 broadcast)
        {
            try
            {
                if (broadcast.outputdest.Equals(BroadcastCriteria.EffectsOutputDest, StringComparison.OrdinalIgnoreCase)) return false;
                if (broadcast.batchnum < 1) return false;
                if (broadcast.agency.Equals(Broadcasts.StageAgency, StringComparison.OrdinalIgnoreCase)) return false;
                if (_broadcastStageAgencies.Any(y => y.agency.Equals(broadcast.agency, StringComparison.OrdinalIgnoreCase)
                                                     && y.currently_staged
                                                     && string.IsNullOrEmpty(y.staged_batchnumber))) return false;

                if (_broadcastStageAgencies.AsEnumerable().Where(y => y.currently_staged
                                                                      && y.agency.Equals(broadcast.agency, StringComparison.OrdinalIgnoreCase)
                                                                      && !string.IsNullOrEmpty(y.staged_batchnumber))
                    .SelectMany(z => z.staged_batchnumber.Split(',', '|'))
                    .Contains(broadcast.batchnum.ToString())) return false;

                return _longRunningService.IsLongRunningBroadcast(broadcast);
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                return false;
            }
        }

    }
}

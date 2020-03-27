using System;
using System.Reflection;
using AutoMapper;
using com.ciswired.libraries.CISLogger;
using Domain;
using Domain.Orm.iBankClientCommands;
using Domain.Orm.iBankClientQueries.BroadcastQueries;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.Helper
{
    public class BatchHistoryHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IClientDataStore _clientStore;

        public BatchHistoryHandler(IClientDataStore store)
        {
            _clientStore = store;
            if (!Features.AutoMapperInitializer.IsEnabled())
            {
                Mapper.Initialize(cfg => cfg.CreateMap<ibbatch, ibbatchhistory>());
            }
        }

        public bool HistoryRecordExists(int batchNumber)
        {
            var query = new BatchHistoryRecordExistsQuery(_clientStore.ClientQueryDb, batchNumber);
            return query.ExecuteQuery();
        }

        public void InsertHistoryRecord(ibbatch batchRecord)
        {
            var batchRecordValues = string.Empty;
            try
            {
                batchRecordValues = $" bcsendername: {batchRecord.bcsendername} nextrun: {batchRecord.nextrun} setby: {batchRecord.setby} holdrun: {batchRecord.holdrun} " + 
                                    $"reportdays: {batchRecord.reportdays} rptusernum: {batchRecord.rptusernum} usespcl: {batchRecord.usespcl} " + 
                                    $"nodataoptn: {batchRecord.nodataoptn} emailsubj: {batchRecord.emailsubj} bcsenderemail: {batchRecord.bcsenderemail} " + 
                                    $"mailformat: {batchRecord.mailformat} displayuid: {batchRecord.displayuid} outputdest: {batchRecord.outputdest} eProfileNo: {batchRecord.eProfileNo} " + 
                                    $"emailccadr: {batchRecord.emailccadr} LangCode: {batchRecord.LangCode} RunNewData: {batchRecord.RunNewData} fystartmo: {batchRecord.fystartmo} " + 
                                    $"timezone: {batchRecord.timezone} gmtdiff: {batchRecord.gmtdiff} outputtype: {batchRecord.outputtype} unilangcode: {batchRecord.unilangcode} " + 
                                    $"titleacct: {batchRecord.titleacct} spclend: {batchRecord.spclend} UserNumber: {batchRecord.UserNumber} agency: {batchRecord.agency} " + 
                                    $"batchnum: {batchRecord.batchnum} batchname: {batchRecord.batchname} emailaddr: {batchRecord.emailaddr} acctlist: {batchRecord.acctlist} " + 
                                    $"prevhist: {batchRecord.prevhist} weekmonth: {batchRecord.weekmonth} monthstart: {batchRecord.monthstart} pagebrklvl: {batchRecord.pagebrklvl}" + 
                                    $"monthrun: {batchRecord.monthrun} weekrun: {batchRecord.weekrun} nxtdstart: {batchRecord.nxtdstart} nxtdend: {batchRecord.nxtdend} " + 
                                    $"lastrun: {batchRecord.lastrun} lastdstart: {batchRecord.lastdstart} lastdend: {batchRecord.lastdend} errflag: {batchRecord.errflag} " + 
                                    $"runspcl: {batchRecord.runspcl} spclstart: {batchRecord.spclstart} weekstart: {batchRecord.weekstart} send_error_email: {batchRecord.send_error_email}";

                var batchHistory = Mapper.Map<ibbatchhistory>(batchRecord);
                new AddClientBroadcastBatchHistoryCommand(_clientStore.ClientCommandDb, batchHistory).ExecuteCommand();
            }
            catch (Exception e)
            {
                //log and swallow the exception
                LOG.Error(e);
                LOG.Error(batchRecordValues, e);
            }
        }

        public void InsertHistoryRecord(bcstque4 broadcast)
        {
            try
            {
                var historyRecord = MapBcstQueueRecordToBatchHistory(broadcast);
                new AddClientBroadcastBatchHistoryCommand(_clientStore.ClientCommandDb, historyRecord).ExecuteCommand();
            }
            catch (Exception e)
            {
                //log and swallow the exception
                LOG.Error(e);
            }
        }

        private ibbatchhistory MapBcstQueueRecordToBatchHistory(bcstque4 broadcast)
        {
            return new ibbatchhistory
            {
                UserNumber = broadcast.UserNumber?? 0,
                agency = broadcast.agency,
                batchnum = broadcast.batchnum ?? 0,
                batchname = broadcast.batchname,
                emailaddr = broadcast.emailaddr,
                acctlist = broadcast.acctlist,
                prevhist = broadcast.prevhist,
                weekmonth = broadcast.weekmonth,
                monthstart = broadcast.monthstart,
                monthrun = broadcast.monthrun,
                weekstart = broadcast.weekstart,
                weekrun = broadcast.weekrun,
                nxtdstart = broadcast.nxtdstart,
                nxtdend = broadcast.nxtdend,
                lastrun = broadcast.lastrun,
                lastdstart = broadcast.lastdstart,
                lastdend = broadcast.lastdend,
                errflag = broadcast.errflag,
                runspcl = broadcast.runspcl,
                spclstart = broadcast.spclstart,
                spclend = broadcast.spclend,
                pagebrklvl = broadcast.pagebrklvl,
                titleacct = broadcast.titleacct,
                bcsenderemail = broadcast.bcsenderemail,
                bcsendername = broadcast.bcsendername,
                nextrun = broadcast.nextrun,
                setby = broadcast.setby,
                holdrun = broadcast.holdrun,
                reportdays = broadcast.reportdays,
                rptusernum = broadcast.rptusernum,
                usespcl = broadcast.usespcl,
                nodataoptn = broadcast.nodataoptn,
                emailsubj = broadcast.emailsubj,
                mailformat = broadcast.mailformat,
                outputtype = broadcast.outputtype,
                displayuid = broadcast.displayuid,
                outputdest = broadcast.outputdest,
                eProfileNo = broadcast.eProfileNo,
                emailccadr = broadcast.emailccadr,
                LangCode = "",
                RunNewData = broadcast.runnewdata,
                fystartmo = 0,
                timezone = "",
                gmtdiff = 0,
                unilangcode = broadcast.unilangcode,
                send_error_email = broadcast.send_error_email
            };
        }
    }
}

using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankClientCommands;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;
using Domain.Orm.iBankMastersQueries.Other;
using Domain.Services;
using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Interfaces;
using iBank.Server.Utilities.Logging;
using iBank.Server.Utilities.ReportCriteriaHandlers;
using iBank.Server.Utilities.ReportGlobalsSetters;

namespace iBank.Server.Utilities
{
    public class ReportDelayer : IReportDelayer
    {
        private IMasterDataStore _masterStore;
        private IClientDataStore _clientStore;

        private ReportGlobals Globals { get; set; }

        private const string OneApostrophe = "'";
        private const string TwoApostrophes = "''";

        public ReportDelayer(IMasterDataStore masterStore)
        {
            _masterStore = masterStore;
        }

        public ReportDelayer(IClientDataStore clientStore, IMasterDataStore masterStore, ReportGlobals globals)
        {
            _clientStore = clientStore;
            _masterStore = masterStore;
            Globals = globals;
        }

        public void ConvertPastDueReportToBroadcast(ICacheService cache, PendingReportInformation report, int serverNumber)
        {
            Globals = CreateGlobals(cache, report);
            _clientStore = new ClientDataStore(Globals.AgencyInformation.ServerName, Globals.AgencyInformation.DatabaseName);
            var reportName = CreateBroadcast(serverNumber.ToString(), true);

            Globals.ReportInformation.ReturnCode = 2;
            Globals.ReportInformation.ErrorMessage = $"{reportName} {Globals.ReportMessages.RptMsg_BigVolume}";

            var logger = new ReportLogLogger();
            var username = Globals.User.FirstName + " " + Globals.User.LastName;
            logger.CreateLog(Globals.Agency, Globals.User.UserId, username, ReportLogLogger.ReportMode.REGULAR,
                Globals.UserNumber, Globals.iBankVersion, Globals.ProcessKey, (byte) serverNumber, ReportLogLogger.ReportStatus.SENTOFFL);
        }

        private ReportGlobals CreateGlobals(ICacheService cache, PendingReportInformation report)
        {
            var temp = ConfigurationManager.AppSettings["ServerNumber"];
            if (!int.TryParse(temp, out var svrNumber)) throw new Exception("Invalid Server Number!");

            var reportCritQuery = new GetReportInputCriteriaByReportIdQuery(_masterStore.MastersQueryDb, report.ReportId);
            var collistQuery = new GetActiveColumnsQuery(_masterStore.MastersQueryDb);
            var whereCritLookup = WhereCriteriaLookup.GetWhereCriteriaLookup(cache, _masterStore);

            var parms = new ReportGlobalsCreatorParams.Builder()
                .WithStandardCritRetriever(new StandardReportCritieraRetriever())
                .WithAdvancedParamRetriever(new AdvancedParameterRetriever())
                .WithUdidRetriever(new MultiUdidParameterRetriever())
                .WithPendingReportInformation(report)
                .WithCrystalDirectory(ConfigurationManager.AppSettings["CrystalReportsDirectory"])
                .WithIbankVersion(ConfigurationManager.AppSettings["iBankVersion"])
                .WithOfflineServerDesignation(true)
                .WithWhereCriteriaLookup(whereCritLookup)
                .WithReportCriteriaQuery(reportCritQuery)
                .WithActiveColumnsQuery(collistQuery)
                .Build();

            return ReportGlobalsCreator.CreateFromOnlineReport(parms, svrNumber);
        }

        public void PushReportOffline(string serverNumber, string offlineMessage = null)
        {
            var reportName = CreateBroadcast(serverNumber, false);

            Globals.ReportInformation.ReturnCode = 2;
            Globals.ReportInformation.ErrorMessage = string.IsNullOrEmpty(offlineMessage)
                ? $"{reportName} {Globals.ReportMessages.RptMsg_BigVolume}"
                : $"{reportName} {offlineMessage}";

            var logger = new ReportLogLogger();
            logger.UpdateLog(Globals.ReportLogKey, ReportLogLogger.ReportStatus.SENTOFFL);
        }
        
        private string CreateBroadcast(string serverNumber, bool isReportPastDue)
        {
            var rptName = "sysDR_" + serverNumber;

            var udrKey = Globals.GetParmValue(WhereCriteria.UDRKEY).TryIntParse(0);

            var savedRpt1 = AddSavedRpt1(rptName, udrKey);

            AddSavedRpt2(savedRpt1);

            AddSavedRpt3(savedRpt1);

            var ibBatch = AddClientBatch(savedRpt1.recordnum, isReportPastDue);

            if (ibBatch == null) return string.Empty;

            AddClientBatch2(ibBatch, savedRpt1, udrKey);

            return $"sysDR:[{ibBatch.batchnum}]";
        }

        private savedrpt1 AddSavedRpt1(string reportName, int udrKey)
        {
            var savedRpt1 = new savedrpt1
            {
                usernumber = Globals.UserNumber,
                agency = Globals.Agency,
                processkey = (short)Globals.ProcessKey,
                userrptnam = reportName,
                udrkey = udrKey,
                lastused = DateTime.Now,
                srlongdesc = string.Empty
            };
            
            var addSavedRpt1Cmd = new AddSavedRpt1Command(_clientStore.ClientCommandDb, savedRpt1);
            addSavedRpt1Cmd.ExecuteCommand();
            
            return savedRpt1;
        }

        private void AddSavedRpt2(savedrpt1 savedRpt1)
        {
            var savedRpt2sToAdd = Globals.ReportParameters
                                         .Select(parm => new savedrpt2
                                         {
                                             recordlink = savedRpt1.recordnum,
                                             usernumber = Globals.UserNumber,
                                             agency = Globals.Agency,
                                             VarName = parm.Value.VarName,
                                             VarValue = parm.Value.VarValue.Replace(OneApostrophe, TwoApostrophes)
                                         }).ToList();

            var addSavedRpt2Cmd = new AddSavedRpt2Command(_clientStore.ClientCommandDb, savedRpt2sToAdd);
            addSavedRpt2Cmd.ExecuteCommand();
        }

        private void AddSavedRpt3(savedrpt1 savedRpt1)
        {
            if (Globals.AdvancedParameters.Parameters.Count == 0 && Globals.MultiUdidParameters.Parameters.Count == 0) return;

            var savedRpt3sToAdd = Globals.AdvancedParameters.Parameters
                                                            .Select((parm, i) => new savedrpt3
                                                            {
                                                                recordlink = savedRpt1.recordnum,
                                                                usernumber = Globals.UserNumber,
                                                                agency = Globals.Agency,
                                                                rowsequence = i,
                                                                colname = parm.FieldName,
                                                                oper = parm.Operator.ToFriendlyString(),
                                                                value1 = parm.Value1.Replace(OneApostrophe, TwoApostrophes),
                                                                value1a = parm.Value2.Replace(OneApostrophe, TwoApostrophes)
                                                            }).ToList();

            if (Globals.AdvancedParameters.Parameters.Count > 0)
            {
                //handle the and/or
                savedRpt3sToAdd.Add(new savedrpt3
                {
                    recordlink = savedRpt1.recordnum,
                    usernumber = Globals.UserNumber,
                    agency = Globals.Agency,
                    rowsequence = Globals.AdvancedParameters.Parameters.Count,
                    colname = "AOCANDOR",
                    oper = "=",
                    value1 = ((int)Globals.AdvancedParameters.AndOr).ToString(CultureInfo.InvariantCulture)

                });


                var addSavedRpt3Cmd = new AddSavedRpt3Command(_clientStore.ClientCommandDb, savedRpt3sToAdd);
                addSavedRpt3Cmd.ExecuteCommand();
            }

            if (Globals.MultiUdidParameters.Parameters.Count > 0)
            {
                savedRpt3sToAdd = Globals.MultiUdidParameters.Parameters
                                                    .Select((parm, i) => new savedrpt3
                                                    {
                                                        recordlink = savedRpt1.recordnum,
                                                        usernumber = Globals.UserNumber,
                                                        agency = Globals.Agency,
                                                        rowsequence = i,
                                                        colname = parm.FieldName,
                                                        oper = parm.Operator.ToFriendlyString(),
                                                        value1 = parm.Value1.Replace(OneApostrophe, TwoApostrophes),
                                                        value1a = parm.Value2.Replace(OneApostrophe, TwoApostrophes)
                                                    }).ToList();


                savedRpt3sToAdd.Add(new savedrpt3
                {
                    recordlink = savedRpt1.recordnum,
                    usernumber = Globals.UserNumber,
                    agency = Globals.Agency,
                    rowsequence = Globals.MultiUdidParameters.Parameters.Count,
                    colname = "MUDANDOR",
                    //FoxPro saved MUDANDOR oper as blank
                    oper = "",
                    value1 = ((int)Globals.MultiUdidParameters.AndOr).ToString(CultureInfo.InvariantCulture)

                });

                var addSavedRpt3Cmd2 = new AddSavedRpt3Command(_clientStore.ClientCommandDb, savedRpt3sToAdd);
                addSavedRpt3Cmd2.ExecuteCommand();
            }
        }
        
        private ibbatch AddClientBatch(int savedRpt1RecordNum, bool isReportPastDue)
        {
            var user = new GetUserByUserNumberQuery(_clientStore.ClientQueryDb, Globals.UserNumber).ExecuteQuery();
            if (user == null) return null;

            var emailAddress = user.emailaddr;

            //create the batch name
            // if the report is past due, the user will not be prompted to validate offline default values. Instead it will simply
            // run the report using the default values. Therefore, the UI will not be updating the status from PENDING to RUN.
            // Hence, we will set the status to RUN. Then, the Broadcast queue manager can add it into the bsctque4 table.
            var status = isReportPastDue ? "RUN" : "PENDING";
            var batchName = $"sysDR:[{savedRpt1RecordNum}][{status}]";
            
            var mailFormat = Globals.GetParmValue(WhereCriteria.MAILFORMAT);
            var bcSenderEmail = string.Empty;
            var bcSenderName = string.Empty;
            var bcInfo = new GetBroadcastInfoByAgencyQuery(_masterStore.MastersQueryDb, Globals.Agency).ExecuteQuery();
            if (bcInfo == null)
            {
                //might be corporate acct
                var bcCorpInfo = new GetBroadcastInfoByCorpAcctQuery(_masterStore.MastersQueryDb, Globals.Agency).ExecuteQuery();
                if (bcCorpInfo != null)
                {
                    bcSenderEmail = bcCorpInfo.bcsenderemail.Replace(OneApostrophe, TwoApostrophes);
                    bcSenderName = bcCorpInfo.bcsendername.Replace(OneApostrophe, TwoApostrophes);
                }
            }
            else
            {
                bcSenderEmail = bcInfo.bcsenderemail.Replace(OneApostrophe, TwoApostrophes);
                bcSenderName = bcInfo.bcsendername.Replace(OneApostrophe, TwoApostrophes);
            }

            var uniLanguage = GetOutputLanguage();

            var userGmtDiff = GetGMTDifferenceForUser(Globals.User.TimeZone, Globals.OutputLanguage);
            
            var ibBatch = CreateNewBatchRecord(batchName, emailAddress, mailFormat, bcSenderEmail, bcSenderName, uniLanguage, userGmtDiff);
            
            var addiBBatchCmd = new AddiBBatchCommand(_clientStore.ClientCommandDb, ibBatch);
            addiBBatchCmd.ExecuteCommand();
            
            return ibBatch;
        }

        private ibbatch CreateNewBatchRecord(string batchName, string emailAddress, string mailFormat, string bcSenderEmail,
            string bcSenderName, string uniLanguage, double userGmtDiff)
        {
            var beginningOfTime = new DateTime(1900, 1, 1);
            return new ibbatch
            {
                UserNumber = Globals.UserNumber,
                rptusernum = 0,
                agency = Globals.Agency,
                batchname = batchName,
                emailaddr = emailAddress,
                lastrun = DateTime.Now,
                nxtdstart = GetBeginDate(),
                nxtdend = GetEndDate(),
                outputtype = Globals.OutputType,
                titleacct = Globals.TitleAcct,
                outputdest = Globals.OutputDestination,
                eProfileNo = Globals.EProfileNumber,
                mailformat = mailFormat,
                bcsenderemail = bcSenderEmail,
                bcsendername = bcSenderName,
                LangCode = Globals.UserLanguage,
                unilangcode = uniLanguage,
                emailccadr = string.Empty,
                timezone = Globals.User.TimeZone,
                gmtdiff = userGmtDiff,
                acctlist = "",
                prevhist = 2,
                weekmonth = 0,
                monthstart = 0,
                monthrun = 0,
                weekstart = 0,
                weekrun = 0,
                lastdstart = beginningOfTime,
                lastdend = beginningOfTime,
                errflag = false,
                runspcl = false,
                spclstart = beginningOfTime,
                spclend = beginningOfTime,
                pagebrklvl = 0,
                nextrun = beginningOfTime,
                setby = "",
                holdrun = "",
                reportdays = 0,
                usespcl = false,
                nodataoptn = false
            };
        }

        private double GetGMTDifferenceForUser(string timeZoneCode, string userLanguageCode)
        {
            if (string.IsNullOrEmpty(userLanguageCode)) userLanguageCode = "EN";
            var getTimeZoneQuery = new GetUserTimeZoneByLangCodeQuery(_masterStore.MastersQueryDb, timeZoneCode, userLanguageCode);
            var timeZoneInfo = getTimeZoneQuery.ExecuteQuery();

            return timeZoneInfo.GMTDiff;
        }

        private void AddClientBatch2(ibbatch ibBatch, savedrpt1 savedRpt1, int udrKey)
        {
            var processKey = (Globals.OutputType == "9") ? 581 : 0;

            var ibBatch2 = new ibbatch2
            {
                UserNumber = Globals.UserNumber,
                batchnum = ibBatch.batchnum,
                savedrptnum = savedRpt1.recordnum,
                udrkey = udrKey,
                processkey = processKey
            };

            var addiBBatch2Cmd = new AddiBBatch2Command(_clientStore.ClientCommandDb, ibBatch2);
            addiBBatch2Cmd.ExecuteCommand();
        }

        private DateTime GetBeginDate()
        {
            var startDate = new DateTime(1900, 1, 1);
            if (Globals.ParmHasValue(WhereCriteria.BEGDATE))
            {
                var tempDate = Globals.GetParmValue(WhereCriteria.BEGDATE).ToDateFromiBankFormattedString();
                startDate = tempDate ?? startDate;
            }

            return startDate;
        }

        private DateTime GetEndDate()
        {
            var endDate = new DateTime(1900, 1, 1);

            if (Globals.ParmHasValue(WhereCriteria.ENDDATE))
            {
                var tempDate = Globals.GetParmValue(WhereCriteria.ENDDATE).ToDateFromiBankFormattedString(false);
                endDate = tempDate ?? endDate;
            }

            return endDate;
        }

        private string GetOutputLanguage()
        {
            var uniLanguage = string.Empty;
            if (!string.IsNullOrEmpty(Globals.OutputLanguage))
            {
                var language = new GetLanguageByLangCodeQuery(_masterStore.MastersQueryDb, Globals.OutputLanguage).ExecuteQuery();
                if (language != null && language.unicode)
                {
                    uniLanguage = Globals.OutputLanguage;
                }
            }

            return uniLanguage;
        }
    }
}


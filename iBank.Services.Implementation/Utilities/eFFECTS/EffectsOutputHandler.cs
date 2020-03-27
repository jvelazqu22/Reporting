using System.IO;
using System.Reflection;

using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities;

using com.ciswired.libraries.CISLogger;

using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries.eProfile;

using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Logging;

namespace iBank.Services.Implementation.Utilities.eFFECTS
{
    public class EffectsOuputHandler
    {
        private readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IClientQueryable _clientQueryDb;

        public IClientQueryable ClientQueryDb
        {
            get
            {
                return _clientQueryDb.Clone() as IClientQueryable;
            }
            set
            {
                _clientQueryDb = value;
            }
        }

        private IMastersQueryable _masterQueryDb;

        public IMastersQueryable MasterQueryDb
        {
            get
            {
                return _masterQueryDb.Clone() as IMastersQueryable;
            }
            set
            {
                _masterQueryDb = value;
            }
        }

        public int EProfileNumber { get; }

        public EffectsOuputHandler(int eProfileNumber, IClientQueryable clientQueryDb, IMastersQueryable masterQueryDb)
        {
            EProfileNumber = eProfileNumber;
            ClientQueryDb = clientQueryDb;
            MasterQueryDb = masterQueryDb;
        }

        public ReturnMessageInformation Process(int processKey, ReportGlobals globals, int styleGroupNumber, string generatedReportFilePath)
        {
            LOG.Info("Processing for eFFECTS delivery.".FormatMessageWithReportLogKey(globals.ReportLogKey));
            XmlReport xmlReport = null;

            //if ok to process
            if (IsAuthorizedForEProfile(processKey))
            {
                if (processKey == (int)ReportTitles.iXMLUserDefinedExport)
                {
                    var xmlHandler = new XmlReportHandler();
                    xmlReport = xmlHandler.GetXmlReportInfo(globals, ClientQueryDb, MasterQueryDb);
                    if (!xmlHandler.IsXmlReportAuthorized(xmlReport.ExportType, MasterQueryDb, EProfileNumber))
                    {
                        var errorMsg = string.Format("Export type [{0}] not authorized for delivery by eProfile Number [{1}]", xmlReport.ExportType, EProfileNumber).FormatMessageWithReportLogKey(globals.ReportLogKey);
                        var errorNbr = ErrorLogger.LogError(globals.UserNumber, globals.Agency, errorMsg, MethodBase.GetCurrentMethod(), ServerType.BroadcastServer, LOG);
                        return new ReturnMessageInformation
                        {
                            ReturnCode = 2,
                            ReturnMessage = "Export type not authorized for delivery.".FormatMessageWithErrorNumber(errorNbr)
                        };
                    }
                }
                
                var eProfileInfo = GetEProfileInformation(globals.Agency);
                if (eProfileInfo.DirectDelivery && !string.IsNullOrEmpty(eProfileInfo.Outbox))
                {
                    var fileName = Path.GetFileName(generatedReportFilePath);

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var maskHandler = new EProfileFileMaskHandler();
                        var timeStrings = new TimeStrings();
                        var fileHandler = new FileHandler();

                        var effectsOutputFileName = "";
                        if (maskHandler.NeedToApplyMask(eProfileInfo.FileNameMask))
                        {
                            var getStyleGroupQuery = new GetStyleGroupByNumberAndClientCodeQuery(ClientQueryDb, styleGroupNumber, globals.Agency);
                            effectsOutputFileName = maskHandler.ApplyMask(globals, eProfileInfo, xmlReport,
                                styleGroupNumber, timeStrings, getStyleGroupQuery);
                        }
                        else
                        {
                            effectsOutputFileName = fileHandler.GetNonMaskedOutputFile(processKey, timeStrings, eProfileInfo, globals.Agency, fileName);
                        }

                        //the filename mask might have $FIXD, so just remove that -- this is from FoxPro
                        effectsOutputFileName = effectsOutputFileName.Replace("$FIXD", "");

                        //3.10.17 - some agencies are currently restricted to a 48 character file name length, including extension
                        effectsOutputFileName = FileOutputTransformer.TruncateFileName(effectsOutputFileName, 48);
                        
                        var returnInfo = eProfileInfo.ZipOutput
                                             ? fileHandler.ZipFile(generatedReportFilePath, effectsOutputFileName)
                                             : fileHandler.CopyFile(generatedReportFilePath, effectsOutputFileName);

                        //2 in this case signifies that the eFFECTS file was delivered properly
                        if (returnInfo.ReturnCode == 2)
                        {
                            //successful
                            var msgs = new ReportMessages();
                            returnInfo.ReturnMessage = string.Format(msgs.QueuedForEffectsDelivery, Path.GetFileName(effectsOutputFileName), eProfileInfo.ProfileName);
                        }

                        return returnInfo;
                    }
                }
                else
                {
                    return new ReturnMessageInformation
                               {
                                   ReturnCode = 2,
                                   ReturnMessage = "The eProfile does not specify 'direct delivery' or no output location is specified."
                               };
                }
            }
            else
            {
                var errorMsg = string.Format("Report process key [{0}] not authorized for eProfile Number [{1}]", processKey, EProfileNumber).FormatMessageWithReportLogKey(globals.ReportLogKey);
                var errorNbr = ErrorLogger.LogError(globals.UserNumber, globals.Agency, errorMsg, MethodBase.GetCurrentMethod(), ServerType.BroadcastServer, LOG);
                return new ReturnMessageInformation
                {
                    ReturnCode = 2,
                    ReturnMessage = "This report is not authorized for delivery by the eProfile.".FormatMessageWithErrorNumber(errorNbr)
                };
            }

            return new ReturnMessageInformation();
        }
        
        private bool IsAuthorizedForEProfile(int processKey)
        {
            var isAuthorizedQuery = new IsProcessKeyAuthorizedForEProfileNumberQuery(new iBankMastersQueryable(), processKey, EProfileNumber);
            return isAuthorizedQuery.ExecuteQuery();
        }

        private EffectsOutputInformation GetEProfileInformation(string agency)
        {
            var getEProfileQuery = new GetEffectsOutputInformationQuery(MasterQueryDb, EProfileNumber, agency);
            return getEProfileQuery.ExecuteQuery();
        }
        
    }
}

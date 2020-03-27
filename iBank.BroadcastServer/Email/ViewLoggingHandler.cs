using System;
using System.Globalization;
using System.IO;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using CODE.Framework.Core.Utilities.Extensions;

using Domain.Helper;

using iBank.BroadcastServer.BroadcastReport;
using iBank.Server.Utilities.Classes;

using Domain.Orm.iBankMastersCommands;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.Email
{
    public class ViewLoggingHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public bcstrptinstance BcstRptRecord { get; set; }
        public ViewLoggingHandler(bcstque4 batch, ReportRunResults results, ReportGlobals globals, IMastersQueryable masterQueryDb, bool isAuthorizationRequired)
        {
            BcstRptRecord = GetBroadcastReportRecord(batch, results, masterQueryDb, globals.Agency, globals.ProcessKey, isAuthorizationRequired);
        }

        public void CopyRecordToViewLoggingDirectory(IMastersQueryable db, string fileName, string sourceDirectory, string agency)
        {
            var sourceFile = "";
            var destinationFile = "";
            try
            {
                var query = new GetReportOutputLocationQuery(db);
                var outputLocation = query.ExecuteQuery();
                var outputDir = outputLocation.ReportOutputDirectory.AddBS() + agency.AddBS();
                destinationFile = outputLocation.ReportOutputDirectory.AddBS() + agency.AddBS() + Path.GetFileName(fileName);

                sourceFile = $"{sourceDirectory.AddBS()}{fileName}";

                if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

                LOG.Debug($"Copying file from [{sourceFile}] to [{destinationFile}] for broadcast view logging.");
                
                if(!File.Exists(destinationFile)) File.Copy(sourceFile, destinationFile);
            }
            catch (Exception e)
            {
                var msg = $"Exception encountered copying file from [{sourceFile}] to [{destinationFile}] | {e.ToString()}";
                LOG.Error(msg, e);
            }
        }

        public void AddBroadcastReportInstanceRecord(ICommandDb masterCommandDb)
        {
            var addBcstRptInstanceCommand = new AddBcstRptInstanceCommand(masterCommandDb, BcstRptRecord);
            addBcstRptInstanceCommand.ExecuteCommand();
        }

        public string GetBroadcastReportUrl(string baseUrl)
        {
            return $@"{baseUrl.Trim()}/bcr.cfm?ky={BcstRptRecord.outputtype}_{BcstRptRecord.bcstikey}_{BcstRptRecord.bcstrkey}";
        }

        private bcstrptinstance GetBroadcastReportRecord(bcstque4 batch, ReportRunResults results, IMastersQueryable masterQueryDb, string agency, int processKey, bool isAuthorizationRequired)
        {
            var key = GetBroadcastKey(masterQueryDb);
            var rptType = GetReportTypeFromExtension(results);

            var pos = results.ReportHref.LastIndexOf("/", StringComparison.Ordinal) + 1;

            return new bcstrptinstance
            {
                bcstrkey = key,
                agency = agency,
                rptfilepath = results.ReportHref.Substring(pos, results.ReportHref.Length - pos),
                rptactive = true,
                outputtype = rptType,
                viewcount = 0,
                authreqd = isAuthorizationRequired,
                rptdate = DateTime.Now,
                batchnum = batch.batchnum ?? 0,
                processkey = processKey
            };
        }

        private string GetReportTypeFromExtension(ReportRunResults results)
        {
            var pos = results.ReportHref.LastIndexOf(".", StringComparison.Ordinal) + 1;
            var extension = results.ReportHref.Substring(pos, results.ReportHref.Length - pos);
            var rptType = "P";
            switch (extension)
            {
                case Constants.CrystalReportExt:
                    rptType = "H";
                    break;
                case Constants.ExcelWorkBookExt:
                case Constants.ExcelExt:
                    rptType = "X";
                    break;
                case Constants.CharacterSeparatedValuesExt:
                    rptType = "C";
                    break;
                case Constants.RichTextExt:
                    rptType = "R";
                    break;
                case Constants.XmlExt:
                    rptType = "L";
                    break;
                case Constants.WordForWindowsExt:
                case Constants.PowerPoint:
                    rptType = "D";
                    break;
            }

            return rptType;
        }
        
        protected virtual string GetBroadcastKey(IMastersQueryable masterQueryDb)
        {
            var random = new Random();
            var intKey = (random.NextDouble() * 1000000000000);
            var strKey = Math.Abs(Math.Truncate(intKey)).ToString(CultureInfo.InvariantCulture).PadLeft(12, '0');

            var attemptCount = 0;
            var foundKey = new IsExistingBroadcastKeyQuery(masterQueryDb.Clone() as IMastersQueryable, strKey).ExecuteQuery();
            while (foundKey)
            {
                intKey++;
                strKey = Math.Abs(Math.Truncate(intKey)).ToString(CultureInfo.InvariantCulture).PadLeft(12, '0');

                foundKey = new IsExistingBroadcastKeyQuery(masterQueryDb.Clone() as IMastersQueryable, strKey).ExecuteQuery();
                if (attemptCount > 100)
                {
                    throw new Exception("Unable to find random key for bcstrptinstance table!");
                }
                attemptCount++;
            }

            return strKey;
        }
    }
}

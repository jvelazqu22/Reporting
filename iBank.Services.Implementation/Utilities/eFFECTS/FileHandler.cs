using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

using com.ciswired.libraries.CISLogger;
using iBank.Server.Utilities;

using CODE.Framework.Core.Utilities.Extensions;

using Domain.Helper;

using iBank.Server.Utilities.Logging;

namespace iBank.Services.Implementation.Utilities.eFFECTS
{
    public class FileHandler
    {
        private readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ReturnMessageInformation ZipFile(string sourceFile, string effectsOutputFileName, ServerType serverType = ServerType.BroadcastServer)
        {
            var zipFileDestination = GetZipFileName(effectsOutputFileName);
            try
            {
                EnsureDirectoryExists(zipFileDestination);
                using (ZipArchive zip = System.IO.Compression.ZipFile.Open(zipFileDestination, ZipArchiveMode.Create))
                {
                    zip.CreateEntryFromFile(sourceFile, Path.GetFileName(effectsOutputFileName));
                    LOG.Info(string.Format("Created zip file at [{0}]", zipFileDestination));
                }
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error compressing file [{0}] to eFFECTS output [{1}]", effectsOutputFileName, zipFileDestination);
                var errorNumber = ErrorLogger.LogException(null, "", ex, errorMsg, MethodBase.GetCurrentMethod(), serverType, LOG);
                return new ReturnMessageInformation
                {
                    ReturnCode = 3,
                    ReturnMessage = "Unable to compress (zip) eFFECTS output.".FormatMessageWithErrorNumber(errorNumber)
                };
            }

            return new ReturnMessageInformation { ReturnCode = 2 };
        }

        public ReturnMessageInformation CopyFile(string sourceFile, string effectsOutputFileName, ServerType serverType = ServerType.BroadcastServer)
        {
            try
            {
                EnsureDirectoryExists(effectsOutputFileName);

                File.Copy(sourceFile, effectsOutputFileName);
                LOG.Info(string.Format("Copying file [{0}] to [{1}] for eFFECTS", sourceFile, effectsOutputFileName));
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error copying file [{0}] to [{1}]", sourceFile, effectsOutputFileName);
                var errorNbr = ErrorLogger.LogException(null, "", ex, errorMsg, MethodBase.GetCurrentMethod(), serverType, LOG);
                return new ReturnMessageInformation
                {
                    ReturnCode = 3,
                    ReturnMessage = "Unable to copy eFFECTS output.".FormatMessageWithErrorNumber(errorNbr)
                };
            }

            return new ReturnMessageInformation { ReturnCode = 2 };
        }
        
        public string GetNonMaskedOutputFile(int processKey, TimeStrings timeStrings, EffectsOutputInformation eProfileInfo, string agency, string fileName)
        {
            if (processKey == (int)ReportTitles.iBankStandardExtract)
            {
                return eProfileInfo.Outbox.AddBS() + fileName;
            }
            else
            {
                return eProfileInfo.Outbox.AddBS() + "IB" + agency + eProfileInfo.TradingPartnerName + timeStrings.Year
                             + timeStrings.Month + timeStrings.Day + timeStrings.Hour + timeStrings.Min + timeStrings.Sec +
                             Path.GetExtension(fileName);
            }
        }

        private string GetZipFileName(string effectsOutputFile)
        {
            var existingExtension = Path.GetExtension(effectsOutputFile);

            if (string.IsNullOrEmpty(existingExtension))
            {
                if (effectsOutputFile.EndsWith(".")) return effectsOutputFile + "zip";
                else return effectsOutputFile + ".zip";
            }

            return effectsOutputFile.Replace(existingExtension, ".zip");
        }

        private void EnsureDirectoryExists(string effectsOutputFileName)
        {
            var dir = Path.GetDirectoryName(effectsOutputFileName);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                LOG.Warn(string.Format("Directory [{0}] did not exist. Attempting to create.", dir));
                Directory.CreateDirectory(dir);
            }
        }
    }
}

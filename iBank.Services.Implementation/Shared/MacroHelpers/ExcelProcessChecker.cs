using System;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Orm.iBankAdministrationCommands;
using Domain.Orm.iBankAdministrationQueries.ConfigurationQueries;
using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.Shared.MacroHelpers
{
    public class ExcelProcessChecker
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ICommandDb CommandDb
        {
            get
            {
                return new iBankAdministrationCommandDb();
            }
        }

        private IAdministrationQueryable QueryDb
        {
            get
            {
                return new iBankAdministrationQueryable();
            }
        }


        // only one process should be allowed to inactivate excel.exe processes every 6 hours.
        public bool IsItOkayToInactivateOldProcesses()
        {
            LOG.Info($"InactivateOldExcelProcesses - IsItOkayToInactivateOldProcesses() - start ");

            var lastTimeUpdated = new DateTime();
            ReportingConfiguration serverConfiguration;
            try
            {
                var machineName = Environment.MachineName;
                if (string.IsNullOrEmpty(machineName))
                {
                    LOG.Error($"InactivateOldExcelProcesses - Invalid machine name: {machineName}");
                    return false;
                }

                LOG.Info($"InactivateOldExcelProcesses - machine name: {machineName}");
                var configurationName = string.Format(Configurations.LAST_DATETIME_CLEANING_EXCEL_PROCESSON_THIS_IBANK_SERVER, machineName);
                LOG.Info($"InactivateOldExcelProcesses - configuration name: {configurationName}");

                /*
                 *The below code is inside the GetConfigurationForCleaningOldExcelProcessesQuery class
                 * if (config == null) throw new InvalidDatabaseConfigurationException("Missing " + _configurationName + " from the Configurations database");
                */

                serverConfiguration = new GetConfigurationForCleaningOldExcelProcessesQuery(QueryDb, configurationName).ExecuteQuery();

                lastTimeUpdated = Convert.ToDateTime(serverConfiguration.Value);
                LOG.Info($"InactivateOldExcelProcesses - last time updated: {lastTimeUpdated}");
            }
            catch (InvalidDatabaseConfigurationException ex)
            {
                LOG.Error(ex);
                return false;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                return false;
            }

            // It is not okay to kill old excel processes if 6 hours have not passed since the last time the kill old processes
            // ran. This handles cases where there are multiple services running on the same machine/server. The first one to update
            // the lastTimeUpdated wins. This is to make sure that we don't call the code the reads the .exe processes too often
            // since it is process intensive.
            if (DateTime.Now.AddHours(-6) < lastTimeUpdated) return false;

            try
            {
                serverConfiguration.Value = DateTime.Now.ToString();
                LOG.Info($"InactivateOldExcelProcesses - new last time updated: {serverConfiguration.Value}");
                new UpdateConfigurationCommand(CommandDb, serverConfiguration).ExecuteCommand();
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                return false;
            }

            return true;
        }

    }
}

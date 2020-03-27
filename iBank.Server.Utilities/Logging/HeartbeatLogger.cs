using com.ciswired.libraries.CISLogger;

using Domain.Orm.iBankAdministrationCommands;
using Domain.Orm.iBankAdministrationQueries;

using System;
using System.Reflection;

using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Server.Utilities.Logging
{
    public class HeartbeatLogger
    {
        private readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        enum HeartbeatStatus
        {
            RUNNING,
            STOPPED
        }
        
        private ICommandDb CommandDb { get
        {
            return new iBankAdministrationCommandDb();
        } }

        private IAdministrationQueryable QueryDb { get
        {
            return new iBankAdministrationQueryable();
        } }
        
        private string ServerName { get; set; }
        private ServerType ServerType { get; set; }
        private string MachineName { get; set; }
        private int ServerNumber { get; set; }
        private ApplicationInformation ApplicationInformation { get; set; }
        
        public HeartbeatLogger(ServerType serverType, int serverNumber, string machineName, ApplicationInformation applicationInformation)
        {
            var serverName = BuildServerName(serverType, serverNumber);

            ServerType = serverType;
            ServerName = serverName;
            MachineName = machineName;
            ServerNumber = serverNumber;
            ApplicationInformation = applicationInformation;
        }

        public void StartHeartbeat()
        {
            SendHearbeat(DateTime.Now);
        }
        
        public void SendHearbeat(DateTime? lastStarted = null)
        {

            var query = new GetSvrStatusRecordQuery(QueryDb, ServerName);
            var rec = query.ExecuteQuery();

            if (rec == null)
            {
                rec = CreateHeartbeat();

                var addCmd = new AddStatusHeartbeatCommand(CommandDb, rec);
                addCmd.ExecuteCommand();
            }
            else
            {
                //record exists so update it
                rec.SvrDesc = $"{ServerName}.{MachineName}";
                rec.CheckFlag = true;
                rec.FromServer = HeartbeatStatus.RUNNING.ToString();
                rec.SvrVer = ApplicationInformation.ExecutingAssemblyVersion.ToString();
                rec.SvrDateTm = ApplicationInformation.CompileDate;
                rec.currently_running = true;
                rec.machine_name = MachineName;
                rec.server_number = ServerNumber;
                rec.server_type = ServerType.ToString();
                if (lastStarted != null) rec.last_started = lastStarted.Value;

                var updateCmd = new UpdateSvrStatusRecordCommand(CommandDb, rec);
                updateCmd.ExecuteCommand();
            }
        }

        public void KillHeartbeat()
        {
            var query = new GetSvrStatusRecordQuery(QueryDb, ServerName);
            var rec = query.ExecuteQuery();

            if (rec != null)
            {
                rec.FromServer = HeartbeatStatus.STOPPED.ToString();
                rec.currently_running = false;
                
                var updateCmd = new UpdateSvrStatusRecordCommand(CommandDb, rec);
                updateCmd.ExecuteCommand();
            }
        }

        private SvrStatus CreateHeartbeat()
        {
            LOG.Info(string.Format("Creating heartbeat for [{0}]", ServerName));
            return new SvrStatus
            {
                SvrName = ServerName,
                SvrDesc = $"{ServerName}.{MachineName}",
                SvrVer = ApplicationInformation.ExecutingAssemblyVersion.ToString(),
                SvrDateTm = ApplicationInformation.CompileDate,
                LastCheck = null,
                TimeSince = null,
                LastStatus = null,
                CheckFlag = true,
                SortOrder = (byte)0,
                FromServer = HeartbeatStatus.RUNNING.ToString(),
                WroteToLog = false,
                currently_running = true,
                last_started = DateTime.Now,
                machine_name = MachineName,
                server_number = ServerNumber,
                server_type = ServerType.ToString()
            };
        }

        private string BuildServerName(ServerType serverType, int serverNumber)
        {
            return serverType.ToString() + "." + serverNumber.ToString();
        }


    }
}

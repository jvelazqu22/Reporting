using Domain.Orm.iBankAdministrationCommands;
using Domain.Orm.iBankAdministrationQueries;

using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Repository;

namespace iBank.Server.Utilities.Logging
{
    public class MaintenanceModeState
    {
        private int _serverNumber;

        private ServerType _serverType;
        public string ServerName
        {
            get
            {
                return _serverType.ToString() + "." + _serverNumber;
            }
        }

        public MaintenanceModeState(ServerType serverType, int serverNumber)
        {
            _serverType = serverType;
            _serverNumber = serverNumber;
        }

        public SvrStatus GetServerRecord()
        {
            return new GetSvrStatusRecordQuery(new iBankAdministrationQueryable(), ServerName).ExecuteQuery();
        }

        public bool IsMaintenanceModeRequested()
        {
            var rec = GetServerRecord();

            if (rec == null) return true;
            return rec.maintenance_mode_requested;
        }

        public void EnterMaintenanceMode()
        {
            var rec = GetServerRecord();

            if (rec == null) return;

            if (rec.in_maintenance_mode) return;

            rec.in_maintenance_mode = true;
            var cmd = new UpdateSvrStatusRecordCommand(new iBankAdministrationCommandDb(), rec);
            cmd.ExecuteCommand();
        }
    }
}

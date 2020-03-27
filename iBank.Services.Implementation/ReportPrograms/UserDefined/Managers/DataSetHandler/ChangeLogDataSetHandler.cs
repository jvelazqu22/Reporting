using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.DataSetHandler
{
    public class ChangeLogDataSetHandler
    {
        private readonly bool _isReservation;

        private readonly bool _udidExists;

        private readonly BuildWhere _buildWhere;

        private readonly ReportGlobals _globals;

        private ChangeLogManager _changeLogManager;

        public ChangeLogDataSetHandler(bool isReservation, bool udidExists, BuildWhere buildWhere, ReportGlobals globals, ChangeLogManager changeLogManager)
        {
            _isReservation = isReservation;
            _udidExists = udidExists;
            _buildWhere = buildWhere;
            _globals = globals;
            _changeLogManager = changeLogManager;
        }

        public List<ChangeLogData> GetChangeLogData(string whereClause)
        {
            _changeLogManager.SetSqlProperties(_udidExists, whereClause);
            var sqlScript = _changeLogManager.ChangeLogSqlScript;
            return ClientDataRetrieval.GetRawData<ChangeLogData>(sqlScript, _isReservation, _buildWhere, _globals, false).ToList();
        }

        public List<ChangeLogData> GetCancelledTripsChangeLog(string whereClause)
        {
            var sqlScript = _changeLogManager.GetCanceledTripChangeLogScript(_udidExists, whereClause);
            return ClientDataRetrieval.GetRawData<ChangeLogData>(sqlScript, _isReservation, _buildWhere, _globals, false).ToList();
        }

        public List<RawData> GetNonCancelledTripData(List<RawData> tripData, IEnumerable<int> cancelledTripReckeys)
        {
            tripData = tripData.Where(x => !cancelledTripReckeys.Contains(x.RecKey)).ToList();

            var reckeys = _changeLogManager.GetCancelledTripsReckeys(_globals, _buildWhere.Parameters);
            if (reckeys.Any())
            {
                tripData = tripData.Where(x => !reckeys.Contains(x.RecKey)).ToList();
            }

            return tripData;
        }

        public List<RawData> GetTripsWithChangeLogInfo(List<RawData> tripData, IEnumerable<int> changeLogReckeys)
        {
            return tripData.Where(x => changeLogReckeys.Contains(x.RecKey)).ToList();
        }

        public List<ChangeLogData> OrderChangeLogDataSegCtr(List<ChangeLogData> changeLogData)
        {
            var idx = 0;
            var reckey = 0;
            foreach (var changeLog in changeLogData)
            {
                if (reckey != changeLog.RecKey) idx = 0; //each recloc has it's own order starts 0
                changeLog.Segctr = idx;
                reckey = changeLog.RecKey;
                idx++;
            }

            return changeLogData;
        }
    }
}

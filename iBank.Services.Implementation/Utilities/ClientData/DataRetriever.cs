using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.ciswired.libraries.CISLogger;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.Utilities.ClientData
{
    public class DataRetriever
    {
        private static readonly ILogger LOG =
            new LogManagerWrapper().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IClientQueryable _clientQueryable;

        private IClientQueryable ClientQueryDb
        {
            get
            {
                return _clientQueryable.Clone() as IClientQueryable;
            }
            set
            {
                _clientQueryable = value;
            }
        }

        public DataRetriever(IClientQueryable clientQueryDb)
        {
            ClientQueryDb = clientQueryDb;
        }

        /// <summary>
        /// Retrieves data from database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="buildWhere"></param>
        /// <param name="addFieldsFromLegsTable"></param>
        /// <param name="noUdidData"></param>
        /// <param name="isReservationReport"></param>
        /// <param name="includeAllLegs"></param>
        /// <param name="checkForDuplicatesAndRemoveThem"></param>
        /// <param name="handleAdvanceParamsAtReportLevelOnly"></param>
        /// <returns></returns>
        /// <exception cref="PushTimedOutReportOfflineException"></exception>
        public IList<T> GetData<T>(SqlScript sql, BuildWhere buildWhere, bool addFieldsFromLegsTable, bool noUdidData = false, bool isReservationReport = true, 
            bool includeAllLegs = true, bool checkForDuplicatesAndRemoveThem = false, bool handleAdvanceParamsAtReportLevelOnly = false) where T : IRecKey
        {
            if (buildWhere.ReportGlobals.IsOfflineServer)
            {
                return GetDataWorker<T>(sql, buildWhere, addFieldsFromLegsTable, noUdidData, isReservationReport, includeAllLegs, checkForDuplicatesAndRemoveThem, handleAdvanceParamsAtReportLevelOnly);
            }

#if DEBUG
            return GetDataWorker<T>(sql, buildWhere, addFieldsFromLegsTable, noUdidData, isReservationReport, includeAllLegs, checkForDuplicatesAndRemoveThem, handleAdvanceParamsAtReportLevelOnly);
#endif
            //if online report set a timer to wait before kicking offline
            try
            {
                var cancelToken = CancellationTokenCreator.Create();
                var task = Task.Factory.StartNew(() => GetDataWorker<T>(sql, buildWhere, addFieldsFromLegsTable,
                    noUdidData, isReservationReport, includeAllLegs, checkForDuplicatesAndRemoveThem,
                    handleAdvanceParamsAtReportLevelOnly), cancelToken);
                return task.Result;
            }
            catch (OperationCanceledException)
            {
                var msg = $"Report [{buildWhere.ReportGlobals.ReportId}] timed out while querying database and has been pushed offline.";
                throw new PushTimedOutReportOfflineException(msg);
            }

        }

        private IList<T> GetDataWorker<T>(SqlScript sql, BuildWhere buildWhere, bool addFieldsFromLegsTable, bool noUdidData = false, bool isReservationReport = true,
            bool includeAllLegs = true, bool checkForDuplicatesAndRemoveThem = false, bool handleAdvanceParamsAtReportLevelOnly = false) where T : IRecKey
        {
            return noUdidData
                ? Get<T>(sql, buildWhere, addFieldsFromLegsTable, includeAllLegs, checkForDuplicatesAndRemoveThem, handleAdvanceParamsAtReportLevelOnly)
                : Get<T>(sql, buildWhere, addFieldsFromLegsTable, includeAllLegs, checkForDuplicatesAndRemoveThem, handleAdvanceParamsAtReportLevelOnly, isReservationReport);
        }

        private IList<T> Get<T>(SqlScript sql, BuildWhere buildWhere, bool addFieldsFromLegsTable, bool includeAllLegs, bool checkForDuplicatesAndRemoveThem,
            bool handleAdvanceParamsAtReportLevelOnly)
            where T : IRecKey
        {
            IList<T> data;
            if (buildWhere.ReportGlobals.AdvancedParameters.Parameters.Count == 0)
            {
                data = ClientDataRetrieval.GetOpenQueryData<T>(sql, buildWhere.ReportGlobals, buildWhere.Parameters, addFieldsFromLegsTable);
            }
            else
            {
                var advWhere = new AdvancedCriteriaHandler<T>();
                data = advWhere.GetData(sql, buildWhere, addFieldsFromLegsTable, includeAllLegs, checkForDuplicatesAndRemoveThem, handleAdvanceParamsAtReportLevelOnly);
            }

            return data;
        }

        private IList<T> Get<T>(SqlScript sql, BuildWhere buildWhere, bool addFieldsFromLegsTable, bool includeAllLegs, 
            bool checkForDuplicatesAndRemoveThem, bool handleAdvanceParamsAtReportLevelOnly, bool isReservationReport)
            where T : IRecKey
        {
            if (buildWhere.ReportGlobals.AdvancedParameters.Parameters.Count == 0)
            {
                var processedSql = SqlProcessor.ProcessSql(sql.FieldList, addFieldsFromLegsTable, sql.FromClause, sql.WhereClause, sql.OrderBy, buildWhere.ReportGlobals);
                if (!string.IsNullOrEmpty(sql.GroupBy)) processedSql += $" {sql.GroupBy}";

                return ClientDataRetrieval.GetUdidFilteredOpenQueryData<T>(processedSql, buildWhere.ReportGlobals, buildWhere.Parameters, isReservationReport);
            }
            else
            {
                var advWhere = new AdvancedCriteriaHandler<T>();
                var data = advWhere.GetData(sql, buildWhere, addFieldsFromLegsTable, includeAllLegs, checkForDuplicatesAndRemoveThem, handleAdvanceParamsAtReportLevelOnly);

                var filter = new UdidFilter();
                return filter.GetUdidFilteredData(data, buildWhere.ReportGlobals, isReservationReport, new TripUdidRetriever());
            }
        }
    }
}

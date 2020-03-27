using Domain.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

using System.Collections.Generic;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Utilities.ClientData;

using iBankDomain.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Factories
{
    public class RawDataFactory<T> : IFactory<IList<T>> where T: IRecKey
    {
        public string DateRangeType { get; set; }
        public bool IsValidUdid { get; set; }
        public bool IsReservationReport { get; set; }
        public ReportGlobals Globals { get; set; }

        public BuildWhere BuildWhere { get; set; }

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

        public DataTypes.DataType DataType { get; set; }

        public RawDataFactory(DataTypes.DataType dataType, string dateRangeType, string possibleUdidNumber, bool isReservationReport, BuildWhere buildWhere,
                              ReportGlobals globals, IClientQueryable clientQueryDb)
        {
            DataType = dataType;
            DateRangeType = dateRangeType;
            IsValidUdid = IsValidUdidNumber(possibleUdidNumber);
            IsReservationReport = isReservationReport;
            BuildWhere = buildWhere;
            Globals = globals;
            ClientQueryDb = clientQueryDb;
        }

        public IList<T> Build()
        {
            var scriptFactory = new SqlScriptFactory(DataType, IsReservationReport, BuildWhere.WhereClauseFull, DateRangeType, IsValidUdid);
            var sqlScript = scriptFactory.Build();

            const bool ALL_LEGS = false;

            var retriever = new DataRetriever(ClientQueryDb);
            return retriever.GetData<T>(sqlScript, BuildWhere, ALL_LEGS, false, IsReservationReport);
        }

        private static bool IsValidUdidNumber(string possibleUdidNumber)
        {
            var udid = 0;
            var goodUdid = int.TryParse(possibleUdidNumber, out udid);

            return goodUdid && udid > 0;
        }
    }
}

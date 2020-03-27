using System.Collections.Generic;
using Domain.Orm.Classes;
using iBank.Entities.MasterEntities;
using iBank.Server.Utilities.Interfaces;
using iBank.Server.Utilities.ReportCriteriaHandlers;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Server.Utilities.ReportGlobalsSetters
{
    public class ReportGlobalsCreatorParams : IReportGlobalsCreatorParams
    {
        public StandardReportCritieraRetriever CriteriaRetriever { get; private set; }

        public AdvancedParameterRetriever AdvParamRetriever { get; private set; }

        public MultiUdidParameterRetriever UdidRetriever { get; private set; }

        public PendingReportInformation ReportInformation { get; private set; }

        public string CrystalDirectory { get; private set; }

        public string iBankVersion { get; private set; }

        public bool IsOfflineServer { get; private set; }

        public Dictionary<int, string> WhereCriteriaLookup { get; private set; }

        public IQuery<IList<reporthandoff>> GetReportCriteriaQuery { get; private set; }

        public IQuery<IList<collist2>> GetActiveColumnsQuery { get; private set; }

        public class Builder
        {
            private StandardReportCritieraRetriever _critRetriever;
            private AdvancedParameterRetriever _advRetriever;
            private MultiUdidParameterRetriever _udidRetriever;
            private PendingReportInformation _reportInfo;
            private string _crystalDir;
            private string _iBankVersion;
            private bool _isOfflineServer;
            private Dictionary<int, string> _whereCritLookup;
            private IQuery<IList<reporthandoff>> _reportCritQuery;
            private IQuery<IList<collist2>> _activeColsQuery;

            public Builder WithStandardCritRetriever(StandardReportCritieraRetriever critRetriever)
            {
                _critRetriever = critRetriever;
                return this;
            }

            public Builder WithAdvancedParamRetriever(AdvancedParameterRetriever advRetriever)
            {
                _advRetriever = advRetriever;
                return this;
            }

            public Builder WithUdidRetriever(MultiUdidParameterRetriever udidRetriever)
            {
                _udidRetriever = udidRetriever;
                return this;
            }

            public Builder WithPendingReportInformation(PendingReportInformation info)
            {
                _reportInfo = info;
                return this;
            }

            public Builder WithCrystalDirectory(string crystalDirectory)
            {
                _crystalDir = crystalDirectory;
                return this;
            }

            public Builder WithIbankVersion(string iBankVersion)
            {
                _iBankVersion = iBankVersion;
                return this;
            }

            public Builder WithOfflineServerDesignation(bool isOfflineServer)
            {
                _isOfflineServer = isOfflineServer;
                return this;
            }

            public Builder WithWhereCriteriaLookup(Dictionary<int, string> lookup)
            {
                _whereCritLookup = lookup;
                return this;
            }

            public Builder WithReportCriteriaQuery(IQuery<IList<reporthandoff>> reportCritQuery)
            {
                _reportCritQuery = reportCritQuery;
                return this;
            }

            public Builder WithActiveColumnsQuery(IQuery<IList<collist2>> activeColsQuery)
            {
                _activeColsQuery = activeColsQuery;
                return this;
            }

            public ReportGlobalsCreatorParams Build()
            {
                return new ReportGlobalsCreatorParams
                {
                    CriteriaRetriever = _critRetriever,
                    AdvParamRetriever = _advRetriever,
                    UdidRetriever = _udidRetriever,
                    CrystalDirectory = _crystalDir,
                    iBankVersion = _iBankVersion,
                    IsOfflineServer = _isOfflineServer,
                    ReportInformation = _reportInfo,
                    WhereCriteriaLookup = _whereCritLookup,
                    GetReportCriteriaQuery = _reportCritQuery,
                    GetActiveColumnsQuery = _activeColsQuery
                };
            }
        }
    }
}
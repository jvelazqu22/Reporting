using System.Collections.Generic;
using Domain.Orm.Classes;
using iBank.Entities.MasterEntities;
using iBank.Server.Utilities.ReportCriteriaHandlers;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Server.Utilities.Interfaces
{
    public interface IReportGlobalsCreatorParams
    {
        StandardReportCritieraRetriever CriteriaRetriever { get; }
        AdvancedParameterRetriever AdvParamRetriever { get; }
        MultiUdidParameterRetriever UdidRetriever { get; }
        PendingReportInformation ReportInformation { get; }
        string CrystalDirectory { get; }
        string iBankVersion { get; }
        bool IsOfflineServer { get; }
        Dictionary<int, string> WhereCriteriaLookup { get; }
        IQuery<IList<reporthandoff>> GetReportCriteriaQuery { get; }
        IQuery<IList<collist2>> GetActiveColumnsQuery { get; }
    }
}
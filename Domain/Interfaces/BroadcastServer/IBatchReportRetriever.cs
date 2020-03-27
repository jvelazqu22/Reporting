using System.Collections.Generic;

using Domain.Helper;

namespace Domain.Interfaces.BroadcastServer
{
    public interface IBatchReportRetriever
    {
        IList<BroadcastReportInformation> GetAllReportsInBatch(IList<ProcessCaptionInformation> processCaptions, string agency, bool isDemo, int userNumber);
    }
}

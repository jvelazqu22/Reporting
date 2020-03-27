using System;
using System.Collections.Generic;

using Domain.Orm.iBankClientQueries;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.Utilities
{
    public class SavedReportRetriever
    {
        public savedrpt1 GetSavedReport1(IClientQueryable clientQueryDb, int savedReportNumber, int? batchnumber)
        {
            var savedRpt1Query = new GetSavedReport1ByKeyQuery(clientQueryDb, savedReportNumber);
            var savedRpt1 = savedRpt1Query.ExecuteQuery();

            if (savedRpt1 == null)
            {
                throw new Exception("Missing saved report parameters for batch " + batchnumber); //should never happen
            }

            return savedRpt1;
        }

        public IList<savedrpt3> GetSavedReport3(IClientQueryable clientQueryDb, int savedReportNumber)
        {
            var savedRpt3Query = new GetAllSavedReport3ByRecordLinkQuery(clientQueryDb, savedReportNumber);
            return savedRpt3Query.ExecuteQuery();
        }
    }
}

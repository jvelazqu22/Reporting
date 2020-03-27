using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Models.ReportPrograms.AirActivityReport;

using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.AirActivity
{
    public class ServiceFeeHandler
    {
        public IClientQueryable ClientQueryDb { get; set; }

        public ServiceFeeHandler(IClientQueryable clientQueryDb)
        {
            ClientQueryDb = clientQueryDb;
        }

        public IList<ServiceFeeInformation> RetrieveServiceFeeData(ReportGlobals globals, BuildWhere buildWhere, int udid, bool isReservation)
        {
            var tranWhere = string.Empty;
            if (globals.IsParmValueOn(WhereCriteria.CBTRANDATEWITHINRANGE))
            {
                var beginDate = globals.BeginDate.Value;
                var endDate = globals.EndDate.Value;
                tranWhere += "and trandate between '" + beginDate.ToShortDateString() + "' and '" + endDate.ToShortDateString() + " 11:59:59 PM'";
            }

            tranWhere = tranWhere + buildWhere.WhereClauseServices;
            string tranFrom;

            if (udid != 0)
            {
                tranFrom = "hibtrips T1, hibServices T6A, hibudids T3";
                tranWhere = "T1.reckey = T6A.reckey and " + buildWhere.WhereClauseFull + tranWhere +
                            " and T1.agency = T6A.agency and T1.reckey = T3.reckey and T6A.svcCode = 'TSF'";

            }
            else
            {
                tranFrom = "hibtrips T1, hibServices T6A";
                tranWhere = "T1.reckey = T6A.reckey and T6A.svcCode = 'TSF' and " + buildWhere.WhereClauseFull + tranWhere +
                            " and T1.agency = T6A.agency";
            }
            
            var tranFields = "T1.reckey, T6A.svcamt as svcFee, T6A.moneytype as AirCurrTyp, invdate, bookdate ";
            
            var tranFull = SqlProcessor.ProcessSql(tranFields, false, tranFrom, tranWhere, string.Empty, globals);
            var svcFees = ClientDataRetrieval.GetUdidFilteredOpenQueryData<ServiceFeeInformation>(tranFull, globals, buildWhere.Parameters, isReservation).ToList();
            
            return svcFees;
        }

        public IList<RawData> CombineServiceFeesWithRawData(IList<ServiceFeeInformation> svcFees, IList<RawData> rawData)
        {
            svcFees = svcFees.GroupBy(s => s.RecKey).Select(s => new ServiceFeeInformation
            {
                RecKey = s.Key,
                SvcFee = s.Sum(f => f.SvcFee)
            }).ToList();

            foreach (var svcFee in svcFees)
            {
                var fee = svcFee;
                foreach (var row in rawData.Where(s => s.RecKey == fee.RecKey))
                {
                    row.Svcfee = svcFee.SvcFee;
                }
            }

            return rawData;
        }
    }
}

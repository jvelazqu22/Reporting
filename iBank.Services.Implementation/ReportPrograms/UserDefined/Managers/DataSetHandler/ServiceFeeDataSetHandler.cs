using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;
using iBank.Services.Implementation.Utilities.CurrencyConversion;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.DataSetHandler
{
    public class ServiceFeeDataSetHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<ServiceFeeData> GetServiceFeeData(string whereClause, bool tripTlsSwitch, bool udidExists, bool isReservation, BuildWhere buildWhere,
                                                      ReportGlobals globals, string moneyType)
        {
            if (globals.IsParmValueOn(WhereCriteria.CBTRANDATEWITHINRANGE))
            {
                buildWhere.WhereClauseServices = $"{buildWhere.WhereClauseServices} and trandate between '{globals.BeginDate}' and '{globals.EndDate} 11:59:59 PM'";
            }
            
            whereClause = $"{whereClause} {buildWhere.WhereClauseServices}";
            
            var svcFeeData = GetData(whereClause, tripTlsSwitch, udidExists, isReservation, buildWhere, globals);

            ConvertCurrency(svcFeeData, moneyType);

            OrderServiceFeeDataSegCtr(svcFeeData);

            LOG.Debug($"ServiceFee Count:{svcFeeData.Count}");

            return svcFeeData;
        }

        private List<ServiceFeeData> GetData(string whereClause, bool tripTlsSwitch, bool udidExists, bool isReservation, BuildWhere buildWhere,
                                             ReportGlobals globals)
        {
            var sqlScript = new ServiceFeeSqlScript();
            var sql = sqlScript.GetSqlScript(tripTlsSwitch, udidExists, isReservation, whereClause);
            return ClientDataRetrieval.GetRawData<ServiceFeeData>(sql, isReservation, buildWhere, globals, false).ToList();
        }

        private void ConvertCurrency(List<ServiceFeeData> svcFeeData, string moneyType)
        {
            if (svcFeeData.Select(s => s.Moneytype).Distinct().Any(s => s != moneyType && moneyType != ""))
            {
                var cc = new CurrencyConverter();
                cc.ConvertCurrency(svcFeeData, moneyType);
            }
        }

        private void OrderServiceFeeDataSegCtr(List<ServiceFeeData> svcFeeData)
        {
            var idx = 0;
            var reckey = 0;
            foreach (var rec in svcFeeData)
            {
                if (reckey != rec.RecKey) idx = 0; //each reckey has it's own order starts 0
                rec.Seqctr = idx;
                reckey = rec.RecKey;
                idx++;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.Client;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupServiceFeeFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly IMasterDataStore _masterStore;
        private readonly IClientDataStore _clientStore;
        private readonly ReportGlobals _globals;
        private readonly ClientFunctions _clientFunctions = new ClientFunctions();
        public List<Tuple<string, string>> TripSummaryLevel { get; set; }
        private readonly BuildWhere _buildWhere;

        public SegmentOrLeg _segmentOrLeg;

        public LookupServiceFeeFieldHandler(UserDefinedParameters userDefinedParams, IMasterDataStore masterStore, IClientDataStore clientStore, 
            ReportGlobals globals, SegmentOrLeg segmentOrLeg)
        {
            _userDefinedParams = userDefinedParams;
            _masterStore = masterStore;
            _clientStore = clientStore;
            _globals = globals;
            _segmentOrLeg = segmentOrLeg;
            TripSummaryLevel = new List<Tuple<string, string>>();
        }

        public string HandleLookupFieldServiceFee(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            var rec = _userDefinedParams.ServiceFeeLookup[mainRec.RecKey].FirstOrDefault(x => x.Seqctr == seqNo);

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "SACCOUNT":
                    //luCName(t1.sacct)
                    return _clientFunctions.LookupCname(new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency), mainRec.Acct, _globals);
                case "SCURSYMBOL":
                    //luCursymbol(moneytype)
                    return LookupFunctions.LookupCurrencySymbol(rec.Moneytype, _masterStore);
                case "SPAXNAME":
                    //alltrim(spasslast)+"/"+alltrim(spassfrst)
                    return string.Format("{0}/{1}", mainRec.Passlast.Trim(), mainRec.Passfrst.Trim());
                case "SSVCFCNT":
                    //iif(ssvcfee>0,1,0)
                    return rec.Svcfee > 0 ? "1" : "0";
                case "SSVCFEE": //column name has extra S at the front
                    return rec.Svcfee.ToString("0.00");
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }
    }
}

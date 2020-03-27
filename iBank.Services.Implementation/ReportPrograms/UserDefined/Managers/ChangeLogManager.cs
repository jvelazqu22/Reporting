using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

using System.Collections.Generic;
using System.Linq;

using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class ChangeLogManager
    {
        private readonly ReportGlobals _globals;
        
        public bool ChangeLogCriteriaPresent { get; set; }
        public string WhereChangeLogText { get; set; }
        public SqlScript ChangeLogSqlScript { get; set; }
        public string TripCancelYn { get; set; }
        public string ChangeCodes { get; set; }
        private string ChangeStampFrom { get; set; }
        private string ChangeStampTo { get; set; }
        private string CancelTimeFrom { get; set; }
        private string CancelTimeTo { get; set; }
        private string FromClause { get; set; }
        public string WhereClause { get; set; }

        public string OrderBy { get; set; }

        public string FieldList { get; set; }

        public ChangeLogManager(ReportGlobals globals)
        {
            _globals = globals;
            SetUpChangeLog();
        }

        private void SetUpChangeLog()
        {
            ChangeCodes = _globals.GetParmValue(WhereCriteria.INCHANGECODE);
            TripCancelYn = _globals.GetParmValue(WhereCriteria.CANCELCODE);
            ChangeStampFrom = _globals.GetParmValue(WhereCriteria.CHANGESTAMP);
            ChangeStampTo = _globals.GetParmValue(WhereCriteria.CHANGESTAMP2);
            CancelTimeFrom = _globals.GetParmValue(WhereCriteria.CANCELTIME);
            CancelTimeTo = _globals.GetParmValue(WhereCriteria.CANCELTIME2);
            WhereChangeLogText = "";

            ChangeLogCriteriaPresent = !string.IsNullOrEmpty(ChangeCodes) || (TripCancelYn.EqualsIgnoreCase("Y") || TripCancelYn.EqualsIgnoreCase("N")) || (!string.IsNullOrEmpty(ChangeStampFrom) && !string.IsNullOrEmpty(ChangeStampTo)) || (!string.IsNullOrEmpty(CancelTimeFrom) && !string.IsNullOrEmpty(CancelTimeTo));
        }

        private string GetChangeLogFramClause(bool udidExists)
        {
            if (udidExists)
            {
                return "ibtrips T1, changelog TCL, ibudid T3";
            }
            else
            {
                return "ibtrips T1, changelog TCL";
            }
        }

        private string GetChangeLogWhereClause(bool udidExists, string whereClause)
        {
            if (udidExists)
            {
                return "T1.reckey = TCL.reckey and T1.reckey = T3.reckey and T1.agency = T3.agency and " + whereClause;
            }
            else
            {
                return "T1.reckey = TCL.reckey and " + whereClause;
            }
        }

        public SqlScript GetCanceledTripChangeLogScript(bool udidExists, string whereClause)
        {
            var fromClause = GetChangeLogFramClause(udidExists);
            whereClause = GetChangeLogWhereClause(udidExists, whereClause);

            return new SqlScript { 
                FieldList =  FieldList,
                FromClause = fromClause.Replace("ibtrips", "ibcanctrips").Replace("ibudids", "ibcancudids"),
                WhereClause = whereClause,
                KeyWhereClause = whereClause + " and ",
                OrderBy = OrderBy,
                GroupBy = ""
            };
        }

        public void SetSqlProperties(bool udidExists, string whereClause)
        {
            FieldList = " T1.reckey, T1.agency, T1.recloc, T1.invoice, T1.invdate, pseudocity, agentid,  TCL.segnum, TCL.changecode, TCL.changstamp, TCL.parsestamp, cast(TCL.changefrom as nvarchar(max)) changefrom, cast(TCL.changeto as nvarchar(max)) changeto, TCL.changedby, cast(TCL.prioritin as nvarchar(max)) prioritin, TCL.pnrcrdtgmt";
            OrderBy = "order by T1.reckey, TCL.changstamp";

            FromClause = GetChangeLogFramClause(udidExists);
            WhereClause = GetChangeLogWhereClause(udidExists, whereClause);
            if (TripCancelYn.EqualsIgnoreCase("Y"))
            {
                if (string.IsNullOrEmpty(ChangeCodes))
                {
                    ChangeCodes = "101";
                }
                else
                {
                    ChangeCodes += ",101";
                }
            }

            if (!string.IsNullOrEmpty(ChangeCodes))
            {
                WhereClause += " and TCL.changecode in (" + ChangeCodes + ")";
                WhereChangeLogText += " Change Code: " + ChangeCodes;
            }

            if (!string.IsNullOrEmpty(ChangeStampFrom))
            {
                var fromStamp = ChangeStampFrom.ToDateFromiBankFormattedString();
                if (string.IsNullOrEmpty(ChangeStampTo))
                {
                    WhereClause += " and TCL.changstamp >= '" + fromStamp + "'";
                    WhereChangeLogText += " Change Stamp From " + fromStamp;
                }
                else
                {
                    var toStamp = ChangeStampTo.ToDateFromiBankFormattedString(true);
                    WhereClause += " and TCL.changstamp between '" + fromStamp + "' and '" + toStamp + "'";
                    WhereChangeLogText += " Change Stamp From " + fromStamp + " to " + toStamp;
                }
            }

            if (!string.IsNullOrEmpty(CancelTimeFrom) && !string.IsNullOrEmpty(CancelTimeTo))
            {
                var fromStamp = CancelTimeFrom.ToDateFromiBankFormattedString();
                if (string.IsNullOrEmpty(CancelTimeTo))
                {
                    WhereClause += " and TCL.changecode = 101 and TCL.changstamp >= '" + fromStamp + "'";
                    WhereChangeLogText += " Cancel Time From " + fromStamp;
                }
                else
                {
                    var toStamp = ChangeStampTo.ToDateFromiBankFormattedString(true);
                    WhereClause += " and TCL.changecode = 101 and TCL.changstamp between '" + fromStamp + "' and '" + toStamp + "'";
                    WhereChangeLogText += " Cancel Time From " + fromStamp + " to " + toStamp;
                }
            }

            ChangeLogSqlScript = new SqlScript
            {
                FieldList = FieldList,
                FromClause = FromClause,
                WhereClause = WhereClause,
                KeyWhereClause = WhereClause + " and ",
                OrderBy = OrderBy,
                GroupBy = ""
            };      
        }

        public List<int> GetCancelledTripsReckeys(ReportGlobals globals, object[] parameters)
        {
            var cancelWhereClause = WhereClause + " and TCL.changecode = 101 ";

            var sql = SqlProcessor.ProcessSql("T1.reckey", false, FromClause, cancelWhereClause, string.Empty, globals);
            return ClientDataRetrieval.GetOpenQueryData<int>(sql, globals, parameters).ToList();
        }
    }

}

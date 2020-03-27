using Domain.Helper;
using Domain.Models;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummarywithGraphs
{
    public class SqlBuilder
    {
        private readonly GetAllMasterAccountsQuery _getAllMasterAccountsQuery;
        private readonly BuildWhere _buildWhere;
        private readonly WhereClauseBuilder _whereClauseBuilder;
        private readonly bool _includeVoids;

        public SqlBuilder(GetAllMasterAccountsQuery getAllMasterAccountsQuery, BuildWhere buildWhere)
        {
            _getAllMasterAccountsQuery = getAllMasterAccountsQuery;
            _buildWhere = buildWhere;
            _whereClauseBuilder = new WhereClauseBuilder();
            _includeVoids = buildWhere.ReportGlobals.IsParmValueOn(WhereCriteria.CBINCLVOIDS);
        }

        public SqlScript BuildTripQuery(string useDate,bool hasUdid)
        {
            var script = new SqlScript();
            var isPreview = RunBuildWhere(WhereCriteria.PREPOSTAIR, true);

            if (hasUdid)
            {
                script.FromClause = !isPreview 
                    ? "hibtrips T1, hibudids T3"
                    : "ibtrips T1, ibudids T3";

                script.FieldList = !isPreview
                    ? "T1.reckey," + useDate + " as UseDate, convert (int,plusmin) as plusmin, valcarr, reascode, savingcode, airchg, stndchg, mktfare, offrdchg, basefare, svcfee, valcarMode, valCarMode as Mode"
                    : "T1.reckey," + useDate + " as UseDate, convert (int,1) as plusmin, valcarr, reascode, savingcode, airchg, stndchg, mktfare, offrdchg, basefare, 0.00 as svcfee, 'A' as valcarMode, T1.recloc, T1.reckey, invoice, passlast, passfrst, acct";

                script.WhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and " + _buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and ";
            }
            else
            {
                script.FromClause = !isPreview
                   ? "hibtrips T1"
                   : "ibtrips T1, iblegs T2";

                script.FieldList = !isPreview
                    ? "reckey," + useDate + " as UseDate, convert (int,plusmin) as plusmin, valcarr, reascode, savingcode, airchg, stndchg, mktfare, offrdchg, basefare, svcfee, valcarMode, valCarMode as Mode"
                    : "t1.reckey," + useDate + " as UseDate, convert (int,1) as plusmin, valcarr, reascode, savingcode, airchg, stndchg, mktfare, offrdchg, basefare, 0.00 as svcfee, 'A' as valcarMode, t2.Mode, t2.seqno, t1.recloc, invoice, passlast, passfrst, acct";

                script.WhereClause = !isPreview
                     ? "valcarr not in ('ZZ','$$') and " + _buildWhere.WhereClauseFull
                    : "t1.reckey = t2.reckey and t2.seqno = 1 and valcarr not in ('ZZ','$$') and " + _buildWhere.WhereClauseFull;

                script.KeyWhereClause = !isPreview
                     ? "valcarr not in ('ZZ','$$') and "
                    : "t1.reckey = t2.reckey and t2.seqno = 1 and valcarr not in ('ZZ','$$') and ";
            }

            return script;
        }

        public SqlScript BuildCarQuery(string useDate, bool hasUdid)
        {
            var script = new SqlScript();
            var isPreview = RunBuildWhere(WhereCriteria.PREPOSTCAR);

            var replacedWhereClause = _buildWhere.WhereClauseFull.Replace("T1.trantype", "T4.CarTranTyp");
            if (!_includeVoids && !HasTranType(replacedWhereClause, "T4.CarTranTyp"))
            {
                replacedWhereClause = _whereClauseBuilder.AddToWhereClause(replacedWhereClause, "T4.CarTranTyp <> 'V'");
            }

            if (hasUdid)
            {
                script.FromClause = !isPreview
                    ? "hibtrips T1, hibudids T3, hibcars T4"
                   : "ibtrips T1, ibudids T3, ibcar T4";

                script.FieldList = !isPreview
                   ? useDate + " as UseDate,T1.reckey,company, convert (int,days) as days, convert (int,cplusmin) as cplusmin, abookrat, reascoda, aexcprat, autocity, autostat"
                    : useDate + " as UseDate,T1.reckey,company, convert (int,days) as days, convert (int,1) as  cplusmin, abookrat, reascoda, aexcprat, autocity, autostat";

                script.WhereClause = $"T1.reckey = T3.reckey and T1.reckey = T4.reckey and {replacedWhereClause}";
                script.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T4.reckey and ";
            }
            else
            {
                script.FromClause = !isPreview
                   ? "hibtrips T1, hibcars T4"
                   : "ibtrips T1, ibcar T4";

                script.FieldList = !isPreview
                    ? useDate + " as UseDate,company, convert (int,days) as days, convert (int,cplusmin) as cplusmin, abookrat, reascoda, aexcprat, autocity, autostat"
                    : useDate + " as UseDate,company, convert (int,days) as days, convert (int,1) as  cplusmin, abookrat, reascoda, aexcprat, autocity, autostat";

                script.WhereClause = $"T1.reckey = T4.reckey and {replacedWhereClause}";
                script.KeyWhereClause = "T1.reckey = T4.reckey and ";
            }

            return script;
        }

        public SqlScript BuildHotelQuery(string useDate, bool hasUdid)
        {
            var script = new SqlScript();
            var isPreview = RunBuildWhere(WhereCriteria.PREPOSTHOT);

            var replacedWhereClause = _buildWhere.WhereClauseFull.Replace("T1.trantype", "T5.HotTranTyp");
            if (!_includeVoids && !HasTranType(replacedWhereClause, "T5.HotTranTyp"))
            {
                replacedWhereClause = _whereClauseBuilder.AddToWhereClause(replacedWhereClause, "T5.HotTranTyp <> 'V'");
            }

            if (hasUdid)
            {
                script.FromClause = !isPreview
                  ? "hibtrips T1, hibudids T3, hibhotel T5"
                   : "ibtrips T1, ibudids T3, ibhotel T5";

                script.FieldList = !isPreview
                    ? useDate + " as UseDate, datein, invdate, bookdate,T1.reckey,chaincod, convert (int,hplusmin) as hplusmin,convert (int,nights) as  nights, convert (int,rooms) as rooms, bookrate, reascodh, hexcprat, hotcity, hotstate"
                    : useDate + " as UseDate, datein, invdate, bookdate,T1.reckey,chaincod, convert (int,1) as hplusmin, convert (int,nights) as nights, convert (int,rooms) as rooms, bookrate, reascodh, hexcprat, hotcity, hotstate";

                script.WhereClause = $"T1.reckey = T3.reckey and T1.reckey = T5.reckey and {replacedWhereClause}";
                script.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T5.reckey and ";
            }
            else
            {
                script.FromClause = !isPreview
                  ? "hibtrips T1, hibhotel T5"
                   : "ibtrips T1, ibhotel T5";

                script.FieldList = !isPreview
                    ? useDate + " as UseDate, datein, invdate, bookdate,chaincod, convert (int,hplusmin) as hplusmin,convert (int,nights) as  nights, convert (int,rooms) as rooms, bookrate, reascodh, hexcprat, hotcity, hotstate"
                    : useDate + " as UseDate, datein, invdate, bookdate,chaincod, convert (int,1) as hplusmin, convert (int,nights) as nights, convert (int,rooms) as rooms, bookrate, reascodh, hexcprat, hotcity, hotstate";

                script.WhereClause = $"T1.reckey = T5.reckey and {replacedWhereClause}";
                script.KeyWhereClause = "T1.reckey = T5.reckey and ";
            }
                
            return script;
        }

        public SqlScript BuildCityPairQuery(string useDate, bool hasUdid)
        {
            var script = new SqlScript();
            var isPreview = RunBuildWhere(WhereCriteria.PREPOSTAIR);

            if (hasUdid)
            {
                script.FromClause = !isPreview
                    ? "hibtrips T1, hibmktsegs T2, hibudids T3"
                   : "ibtrips T1, ibmktsegs T2, ibudids T3";

                script.FieldList = !isPreview
                   ? "convert(varchar(8), T1.reckey) as recloc, T1.reckey, segOrigin as origin, segDest as destinat, airline, segFare as actfare, basefare,  class, fltno, sdeptime, sarrtime, mode,convert (int,plusmin * miles) as miles, segMiscAmt as miscamt, convert (int,plusmin) as plusmin,DITCode, exchange, classCat"
                    : "convert(varchar(8),T1.reckey) as recloc, T1.reckey,segOrigin as origin, segDest as destinat, airline,segFare as actfare, basefare, class, fltno, sdeptime, sarrtime, mode,convert (int,miles) as miles, segMiscAmt as miscamt,  convert (int,1) as plusmin, DITCode, exchange, classCat";

                script.WhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline not in ('$$','ZZ') and airline is not null and  " + _buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline not in ('$$','ZZ') and airline is not null and  ";
            }
            else
            {
                script.FromClause = !isPreview
                   ? "hibtrips T1, hibmktsegs T2"
                   : "ibtrips T1, ibmktsegs T2";

                script.FieldList = !isPreview
                    ? "convert(varchar(8), T1.reckey) as recloc, T1.reckey, segOrigin as origin, segDest as destinat, airline, segFare as actfare, basefare,  class, fltno, sdeptime, sarrtime, mode,convert (int,plusmin * miles) as miles, segMiscAmt as miscamt, convert (int,plusmin) as plusmin,DITCode, exchange, classCat"
                    : "convert(varchar(8),T1.reckey) as recloc, T1.reckey,segOrigin as origin, segDest as destinat, airline,segFare as actfare, basefare, class, fltno, sdeptime, sarrtime, mode,convert (int,miles) as miles, segMiscAmt as miscamt,  convert (int,1) as plusmin, DITCode, exchange, classCat";

                script.WhereClause = "T1.reckey = T2.reckey and airline not in ('$$','ZZ') and airline is not null and " + _buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T2.reckey and airline not in ('$$','ZZ') and airline is not null and ";
            }
            script.OrderBy = "order by T1.reckey";

            return script;
        }

        public SqlScript BuildSvcFeeQuery(string useDate, bool hasUdid, bool includeOrphanServiceFees)
        {
            var script = new SqlScript();
            var isPreview = RunBuildWhere(WhereCriteria.PREPOSTAIR);

            var replacedWhereClause = _buildWhere.WhereClauseFull.Replace("T1.trantype", "T6A.sfTrantype");
            if (!_includeVoids && !HasTranType(replacedWhereClause, "T6A.sfTrantype"))
            {
                replacedWhereClause = _whereClauseBuilder.AddToWhereClause(replacedWhereClause, "T6A.sfTrantype <> 'V'");
            }

            if (hasUdid)
            {
                script.FromClause = "hibtrips T1, hibServices T6A, hibudids T3";

                script.FieldList = includeOrphanServiceFees || !isPreview
                    ? "sum(svcAmt) as svcAmt"
                    : "T1.reckey, T1.recloc, T1.invoice, T1.acct, T1.passlast, T1.passfrst, T6A.svcAmt";

                script.WhereClause = $"T1.reckey = T6A.reckey and T1.agency = T6A.agency and T1.reckey = T3.reckey and T1.agency = T3.agency and T6A.svcCode = 'TSF' and {replacedWhereClause}";
                script.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T1.reckey = T3.reckey and T1.agency = T3.agency and T6A.svcCode = 'TSF' and ";

                script.GroupBy = "group by BookDate, InvDate, T6A.MoneyType, T1.MoneyType";
            }
            else
            {
                script.FromClause = "hibtrips T1, hibServices T6A";

                script.FieldList = includeOrphanServiceFees || !isPreview
                    ? "svcAmt"
                    : "T1.reckey, T1.recloc, T1.invoice, T1.acct, T1.passlast, T1.passfrst, T6A.svcAmt";

                if (includeOrphanServiceFees)
                {
                    script.WhereClause = $"T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svcCode = 'TSF' and {replacedWhereClause}";
                    script.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svcCode = 'TSF' and ";
                }
                else
                {
                    script.WhereClause = $"T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svcCode = 'TSF' and T1.origValCar not in ('SVCFEEONLY','ZZ:S') and {replacedWhereClause}";
                    script.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svcCode = 'TSF' and T1.origValCar not in ('SVCFEEONLY','ZZ:S') and ";
                }
            }

            return script;
        }

        //TODO: This code can be simplified if the AddAdvancedClauses function is modified to not change the parameter values from their original form. 
        //For example, currently, if the function is run twice, the clause will read "SavingCode = ''ND''" rather than "SavingCode = 'ND'", resulting in an error.
        private bool RunBuildWhere(WhereCriteria prePost, bool firstTime = false)
        {
            var airbor = _buildWhere.ReportGlobals.GetParmValue(prePost);
            
            _buildWhere.ReportGlobals.SetParmValue(WhereCriteria.PREPOST, airbor);

            //save the advanced where for later, since calling the function twice causes errors
            var advWhere = _buildWhere.WhereClauseAdvanced;

            _buildWhere.BuildAll(_getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false);

            if (firstTime)
            {
                _buildWhere.AddAdvancedClauses();
            }
            else
            {
                _buildWhere.WhereClauseAdvanced = advWhere;
            }

            _buildWhere.AddSecurityChecks();

            return airbor.Equals("1");
        }
 
        private bool HasTranType(string where, string type)
        {
            return where.IndexOf(type, System.StringComparison.InvariantCultureIgnoreCase) > 0; 
        }
    }
}

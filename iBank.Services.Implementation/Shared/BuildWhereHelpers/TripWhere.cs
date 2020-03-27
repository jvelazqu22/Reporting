using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankClientQueries;

using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public class TripWhere : AbstractWhere
    {
        //if any of these parameters are used, we cannot include Orphaned Service Fees. 
        private readonly List<WhereCriteria> _notAllowed = new List<WhereCriteria>
                {
                    WhereCriteria.BREAK1,
                    WhereCriteria.BREAK2,
                    WhereCriteria.BREAK3,
                    WhereCriteria.INBREAK1,
                    WhereCriteria.INBREAK2,
                    WhereCriteria.INBREAK3,
                    WhereCriteria.VALCARR,
                    WhereCriteria.INVALCARR,
                    WhereCriteria.BRANCH,
                    WhereCriteria.AGENTID,
                    WhereCriteria.TICKET,
                    WhereCriteria.REASCODE,
                    WhereCriteria.PSEUDOCITY,
                    WhereCriteria.INREASCODE
                };

        private IClientDataStore ClientDataStore { get; set; }
        private IMasterDataStore MasterDataStore { get; set; }

        public TripWhere(IClientDataStore clientStore, IMasterDataStore masterStore) : base()
        {
            ClientDataStore = clientStore;
            MasterDataStore = masterStore;
        }

        public void HandleBackOfficeReportSpecificProcessing(ReportGlobals globals, BuildWhere where, bool includeVoids)
        {
            var invcred = globals.GetParmValue(WhereCriteria.INVCRED).ToUpper();
            
            if (invcred.Left(4) == "INVO")
            {
                HandleInvoice(globals, where, includeVoids);
            }
            else if (invcred.Left(4) == "CRED")
            {
                HandleCredit(globals, where, includeVoids);
            }
            else if (invcred.Left(7) == "NEITHER")
            {
                HandleNeitherInvoiceOrCredit(globals, where);
            }
            else
            {
                if (includeVoids)
                {
                    globals.WhereText += globals.ReportMessages.IncludingVoids.Replace(':', ';');
                }
                else
                {
                    if (where.BuildTripWhere && where.BuildCarWhere && where.BuildHotelWhere)
                    {
                        where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "T1.trantype <> @t1TranType1");
                        where.SqlParameters.Add(new SqlParameter("t1TranType1", "V"));
                    }
                    else if (where.BuildCarWhere)
                    {
                        where.WhereClauseCar = _whereClauseBuilder.AddToWhereClause(where.WhereClauseCar, " T4.CarTranTyp <> 'V'");
                    }
                    else if (where.BuildHotelWhere)
                    {
                        where.WhereClauseHotel = _whereClauseBuilder.AddToWhereClause(where.WhereClauseHotel, " T5.HotTranTyp <> 'V'");
                    }
                    else
                    {
                        where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "T1.trantype <> @t1TranType1");
                        where.SqlParameters.Add(new SqlParameter("t1TranType1", "V"));
                    }
                    where.WhereClauseSvcFee += " and T6.trantype <> 'V'";
                    where.WhereClauseServices += " and T6A.sfTranType <> 'V'";
                }
            }
        }

        private void HandleInvoice(ReportGlobals globals, BuildWhere where, bool includeVoids)
        {
            if (includeVoids)
            {
                if (where.BuildCarWhere)
                {
                    where.WhereClauseCar = _whereClauseBuilder.AddToWhereClause(where.WhereClauseCar, " T4.CarTranTyp in ('I','V')");
                }
                else if (where.BuildHotelWhere)
                {
                    where.WhereClauseHotel = _whereClauseBuilder.AddToWhereClause(where.WhereClauseHotel, " T5.HotTranTyp in ('I','V')");
                }
                else
                {
                    where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "(T1.trantype = @t1TranType1 OR T1.trantype = @t1TranType2)");
                    where.SqlParameters.Add(new SqlParameter("t1TranType1", "I"));
                    where.SqlParameters.Add(new SqlParameter("t1TranType2", "V"));
                }

                where.WhereClauseSvcFee += " and T6.trantype in ('I','V')";
                where.WhereClauseServices += " and T6A.sfTranType in ('I','V')";
                globals.WhereText += globals.ReportMessages.InvoicesAndVoids + ";";
            }
            else
            {
                if (where.BuildCarWhere)
                {
                    where.WhereClauseCar = _whereClauseBuilder.AddToWhereClause(where.WhereClauseCar, " T4.CarTranTyp = 'I'");
                }
                else if (where.BuildHotelWhere)
                {
                    where.WhereClauseHotel = _whereClauseBuilder.AddToWhereClause(where.WhereClauseHotel, " T5.HotTranTyp = 'I'");
                }
                else
                {
                    where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "T1.trantype = @t1TranType1");
                    where.SqlParameters.Add(new SqlParameter("t1TranType1", "I"));
                }

                where.WhereClauseSvcFee += " and T6.trantype = 'I'";
                where.WhereClauseServices += " and T6A.sfTranType = 'I'";
                globals.WhereText += globals.ReportMessages.InvoicesOnly + ";";
            }
        }

        private void HandleCredit(ReportGlobals globals, BuildWhere where, bool includeVoids)
        {
            if (includeVoids)
            {
                if (where.BuildCarWhere)
                {
                    where.WhereClauseCar = _whereClauseBuilder.AddToWhereClause(where.WhereClauseCar, " T4.CarTranTyp in ('C','V')");

                }
                else if (where.BuildHotelWhere)
                {
                    where.WhereClauseHotel = _whereClauseBuilder.AddToWhereClause(where.WhereClauseHotel, " T5.HotTranTyp in ('C','V')");
                }
                else
                {
                    where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "(T1.trantype = @t1TranType1 OR T1.trantype = @t1TranType2)");
                    where.SqlParameters.Add(new SqlParameter("t1TranType1", "C"));
                    where.SqlParameters.Add(new SqlParameter("t1TranType2", "V"));
                }
                where.WhereClauseSvcFee += " and T6.trantype in ('C','V')";
                where.WhereClauseServices += " and T6A.sfTranType in ('C','V')";
                globals.WhereText += globals.ReportMessages.CreditsAndVoids + ";";
            }
            else
            {
                if (where.BuildCarWhere)
                {
                    where.WhereClauseCar = _whereClauseBuilder.AddToWhereClause(where.WhereClauseCar, " T4.CarTranTyp = 'C'");

                }
                else if (where.BuildHotelWhere)
                {
                    where.WhereClauseHotel = _whereClauseBuilder.AddToWhereClause(where.WhereClauseHotel, " T5.HotTranTyp = 'C'");
                }
                else
                {
                    where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "T1.trantype = @t1TranType1");
                    where.SqlParameters.Add(new SqlParameter("t1TranType1", "C"));

                }

                where.WhereClauseSvcFee += " and T6.trantype = 'C'";
                where.WhereClauseServices += " and T6A.sfTranType = 'C'";
                globals.WhereText += globals.ReportMessages.CreditsOnly + ";";
            }
        }

        private void HandleNeitherInvoiceOrCredit(ReportGlobals globals, BuildWhere where)
        {
            if (where.BuildCarWhere)
            {
                where.WhereClauseCar = _whereClauseBuilder.AddToWhereClause(where.WhereClauseCar, " T4.CarTranTyp = 'V'");

            }
            else if (where.BuildHotelWhere)
            {
                where.WhereClauseHotel = _whereClauseBuilder.AddToWhereClause(where.WhereClauseHotel, " T5.HotTranTyp = 'V'");
            }
            else
            {
                where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "T1.trantype = @t1TranType1");
                where.SqlParameters.Add(new SqlParameter("t1TranType1", "V"));
            }

            where.WhereClauseSvcFee += " and T6.trantype = 'V'";
            where.WhereClauseServices += " and T6A.sfTranType = 'V'";
            globals.WhereText += globals.ReportMessages.VoidsOnly + ";";
        }
        
        public void HandleAccountCriteriaUseOutputTranslation(ReportGlobals globals, BuildWhere where, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery,
            bool isNotIn, string pickNameAccountTitle, ClientFunctions clientFunctions, string inText, string notInText)
        {
            var pickList = globals.GetParmValue(WhereCriteria.INACCT);
            if (string.IsNullOrEmpty(pickList)) pickList = globals.GetParmValue(WhereCriteria.ACCT);

            var pickName = string.Empty;

            var pl = new PickListParms(globals);
            const string pickType = "ACCTS";
            pl.ProcessList(pickList, pickName, pickType);

            //need set Globals.PickListName so CACCTNAME would be reflected
            globals.PickListName = string.Join(",", pl.Names);

            //only do it if need to print
            if (!globals.IsListBreakoutEnabled) pickName = globals.PickListName;


            if (!string.IsNullOrEmpty(pl.PickNameAcctTitle))
            {
                globals.AccountName = pickNameAccountTitle;
            }
            else
            {
                if (pl.PickList.Any())
                {
                    globals.ReplaceCAcctNameAndAccountInParamsWithPickListName = true;
                    var outputLanguage = (!globals.OutputLanguage.IsNullOrWhiteSpace())
                         ? globals.OutputLanguage
                         : globals.UserLanguage;
                    var displayName = LookupFunctions.LookupColumnDisplayName("ACCOUNT", DEFAULT_WHERE_TEXT_DISPLAY_NAME, outputLanguage, MasterDataStore);

                    where.WhereClauseTrip = _paramsBuilder.AddOrListToWhereClause(where.WhereClauseTrip, pl.PickList, "T1.acct", isNotIn, where.SqlParameters);

                    //if we aren't doing a "not in", get the account name.
                    if (!isNotIn)
                    {
                        if (pl.PickList.Count == 1 && pl.PickList.First().HasWildCards())
                        {
                            globals.AccountName = string.Format("{0} {1}", displayName, pl.PickList.First());
                        }
                        else
                        {
                            globals.AccountName = clientFunctions.LookupCname(getAllMasterAccountsQuery, pl.PickList.First(), globals);
                        }

                    }

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pickName, displayName,
                        isNotIn, pl.PickList, inText, notInText);
                }
            }
        }

        public void HandleParentAccount(ReportGlobals globals, BuildWhere where, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, string pickNameAccountTitle,
            ClientFunctions clientFunctions)
        {
            var pickList = globals.GetParmValue(WhereCriteria.INPARENTACCT);
            if (string.IsNullOrEmpty(pickList)) pickList = globals.GetParmValue(WhereCriteria.PARENTACCT);

            var pickName = string.Empty;
            var isNotIn = globals.IsParmValueOn(WhereCriteria.NOTINPARENTACCT);

            var pl = new PickListParms(globals);
            pl.ProcessList(pickList, pickName, "PARACCTS");

            if (!pl.PickList.Any()) return;

            var displayName = "";
            if (!string.IsNullOrEmpty(pickList)) displayName = LookupFunctions.LookupColumnDisplayName("PARENTACCTS", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);

            var sameParent = pl.PickList.Count == 1
                                 ? GetAllAccountsUnderParent(pl.PickList[0])
                                 : GetAllAccountsUnderParent(pl.PickList);

            if (pl.PickList.Count == 1)
            {
                globals.WhereText += isNotIn
                    ? string.Format(FIELD_NOT_EQUALS_VALUE, displayName, string.IsNullOrEmpty(pickName) ? pl.PickList[0] : pickName)
                    : string.Format(FIELD_EQUALS_VALUE, displayName, string.IsNullOrEmpty(pickName) ? pl.PickList[0] : pickName);

            }
            else
            {
                globals.WhereText += isNotIn
                    ? string.Format(FIELD_NOT_EQUALS_VALUE, displayName, string.IsNullOrEmpty(pickName) ? pickList : pickName)
                    : string.Format(FIELD_EQUALS_VALUE, displayName, string.IsNullOrEmpty(pickName) ? pickList : pickName);
            }

            if (sameParent.Count > 0) where.WhereClauseTrip = _paramsBuilder.AddOrListToWhereClause(where.WhereClauseTrip, sameParent.ToList(), "T1.acct", isNotIn, where.SqlParameters);

            var parentAcctDescriptions = GetParentAccountNameDescription(pl.PickList, globals.Agency);
            //Get the Parent account name
            if (!string.IsNullOrEmpty(pl.PickNameAcctTitle))
            {
                globals.AccountName = pickNameAccountTitle;
            }
            else if (!string.IsNullOrEmpty(parentAcctDescriptions))
            {
                globals.AccountName = parentAcctDescriptions;
            }
            else
            {
                if (isNotIn) return;

                displayName = LookupFunctions.LookupColumnDisplayName("ACCOUNT", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
                
                if (pl.PickList.Count == 1 && pl.PickList.First().HasWildCards())
                {
                    globals.AccountName = string.Format("{0} {1}", displayName, pl.PickList.First());
                }
                else
                {
                    globals.AccountName = clientFunctions.LookupCname(getAllMasterAccountsQuery, pl.PickList.First(), globals);
                }
            }
        }

        private string GetParentAccountNameDescription(List<string> pickList, string agency)
        {
            var result = string.Empty;
            var query = new GetAllParentAccountsQuery(ClientDataStore.ClientQueryDb)
                .ExecuteQuery()
                .Where(x => x.Agency == agency).ToList();
            //get exact match of accountId
            foreach (var item in pickList)
            {
                var temp = query.Where(x => x.AccountId == item).ToList();
                if (temp.Any())
                {
                    if (!string.IsNullOrEmpty(result)) result += ",";
                    result += temp.FirstOrDefault().AccountName;
                }
            }
            return result;
        }
        private IList<string> GetAllAccountsUnderParent(string parentAcct)
        {
            var getAcctsQuery = new GetAllAccountsByParentAcctQuery(ClientDataStore.ClientQueryDb, parentAcct, parentAcct.HasWildCards());
            return getAcctsQuery.ExecuteQuery().ToList();
        }

        private IList<string> GetAllAccountsUnderParent(IList<string> parentAccts)
        {
            var getAcctsQuery = new GetAllAccountsByParentAcctQuery(ClientDataStore.ClientQueryDb, parentAccts);
            return getAcctsQuery.ExecuteQuery().ToList();
        }
        
        public void HandleBreaks(ReportGlobals globals, BuildWhere where)
        {
            HandleBreak(WhereCriteria.INBREAK1, WhereCriteria.BREAK1, WhereCriteria.NOTINBRK1, "BRK1", "BREAK1", "Break1", "T1.break1", globals, where);
            HandleBreak(WhereCriteria.INBREAK2, WhereCriteria.BREAK2, WhereCriteria.NOTINBRK2, "BRK2", "BREAK2", "Break2", "T1.break2", globals, where);
            HandleBreak(WhereCriteria.INBREAK3, WhereCriteria.BREAK3, WhereCriteria.NOTINBRK3, "BRK3", "BREAK3", "Break3", "T1.break3", globals, where);
        }

        private void HandleBreak(WhereCriteria listCrit, WhereCriteria oneCrit, WhereCriteria notCrit, string pickField, string colName, string defaultColName, string tableColName, ReportGlobals globals,
            BuildWhere where)
        {
            var pickList = globals.GetParmValue(listCrit);
            if (string.IsNullOrEmpty(pickList)) pickList = globals.GetParmValue(oneCrit);
            var pickName = string.Empty;
            var isNotIn = globals.IsParmValueOn(notCrit);

            var displayName = string.Empty;

            var pl = new PickListParms(globals);
            pl.ProcessList(pickList, pickName, pickField);
            //No fields, just return. 
            if (!pl.PickList.Any()) return;

            if (!string.IsNullOrEmpty(pickList)) displayName = LookupFunctions.LookupColumnDisplayName(colName, defaultColName, globals.UserLanguage, MasterDataStore);
            
            where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, SharedProcedures.OrList(pl.PickList, tableColName, isNotIn));

            globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName, isNotIn, string.Join(",", pl.PickList));
        }
        
        public void HandleStandardWhereClauses(ReportGlobals globals, BuildWhere where, string notInText)
        {
            //Agency/Source Abbreviation
            var displayName = LookupFunctions.LookupColumnDisplayName("DATASOURCE", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
            where.WhereClauseTrip = AddListWhere(globals, where.WhereClauseTrip, WhereCriteria.SOURCEABBR, WhereCriteria.INSOURCEABBR, WhereCriteria.NOTINSOURCEABBR, "SOURCEABBR", "T1.SourceAbbr", 
                displayName, notInText);

            // Validating Carrier
            displayName = LookupFunctions.LookupColumnDisplayName("VALCARRIER", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
            where.WhereClauseTrip = AddListWhere(globals, where.WhereClauseTrip, WhereCriteria.VALCARR, WhereCriteria.INVALCARR, WhereCriteria.NOTINVALCARR, "VALCARR", "T1.valcarr",
                displayName, notInText);

            // Exception / Out Of Policy Reason
            displayName = LookupFunctions.LookupColumnDisplayName("REASONCODE", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
            where.WhereClauseTrip = AddListWhere(globals, where.WhereClauseTrip, WhereCriteria.REASCODE, WhereCriteria.INREASCODE, WhereCriteria.NOTINREAS, "IBREASCD", "T1.reascode",
                displayName, notInText);

            // Travel Reason codes
            displayName = LookupFunctions.LookupColumnDisplayName("TRAVELREASONCODE", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
            where.WhereClauseTrip = AddListWhere(globals, where.WhereClauseTrip, WhereCriteria.TRAVREAS, WhereCriteria.INTRAVREAS, WhereCriteria.NOTINTRAVREAS, "RTVLCODE", "T1.rtvlcode", 
                displayName, notInText);
        }
        
        public void HandleDomesticIntl(string domIntl, ReportGlobals globals, BuildWhere where)
        {
            if (domIntl.IsBetween(2, 7))
            {
                if (!globals.LegDIT)
                {
                    var domIntlDesc = LookupFunctions.LookupDomesticInternational(domIntl, globals.UserLanguage, MasterDataStore);
                    if (!string.IsNullOrEmpty(domIntlDesc))
                    {
                        globals.WhereText += domIntlDesc + "; ";
                    }
                }
            }

            int intDomIntl;
            if (!globals.LegDIT && int.TryParse(domIntl, out intDomIntl))
            {
                switch (intDomIntl)
                {
                    case 2:
                        where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "T1.domintl = 'D'");
                        break;
                    case 3:
                        where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "T1.domintl = 'I'");
                        break;
                    case 4:
                        where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "T1.domintl = 'T'");
                        break;
                    case 5:
                        where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "T1.domintl <> 'D'");
                        break;
                    case 6:
                        where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "T1.domintl <> 'I'");
                        break;
                    case 7:
                        where.WhereClauseTrip = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, "T1.domintl <> 'T'");
                        break;
                }
            }
        }
        
        public void HandleSimpleFieldsWhereClauses(ReportGlobals globals, BuildWhere where)
        {
            var displayName = LookupFunctions.LookupColumnDisplayName("FIRSTNAME", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
            where.WhereClauseTrip = AddSimpleWhere(globals, where, where.WhereClauseTrip, WhereCriteria.PASSFIRST, "T1.passfrst", displayName);

            displayName = LookupFunctions.LookupColumnDisplayName("LASTNAME", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
            where.WhereClauseTrip = AddSimpleWhere(globals, where, where.WhereClauseTrip, WhereCriteria.PASSLAST, "T1.passlast", displayName);

            displayName = LookupFunctions.LookupColumnDisplayName("INVOICENBR", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
            where.WhereClauseTrip = AddSimpleWhere(globals, where, where.WhereClauseTrip, WhereCriteria.INVOICE, "T1.invoice", displayName);

            displayName = LookupFunctions.LookupColumnDisplayName("TICKET", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
            where.WhereClauseTrip = AddSimpleWhere(globals, where, where.WhereClauseTrip, WhereCriteria.TICKET, "T1.ticket", displayName);

            displayName = LookupFunctions.LookupColumnDisplayName("RECORDLOCATOR", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
            where.WhereClauseTrip = AddSimpleWhere(globals, where, where.WhereClauseTrip, WhereCriteria.RECLOC, "T1.recloc", displayName);

            displayName = LookupFunctions.LookupColumnDisplayName("PSEUDOCITY", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
            where.WhereClauseTrip = AddSimpleWhere(globals, where, where.WhereClauseTrip, WhereCriteria.PSEUDOCITY, "T1.pseudocity", displayName);

            displayName = LookupFunctions.LookupColumnDisplayName("BRANCH", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
            where.WhereClauseTrip = AddSimpleWhere(globals, where, where.WhereClauseTrip, WhereCriteria.BRANCH, "T1.branch", displayName);

            displayName = LookupFunctions.LookupColumnDisplayName("CREDITCARDNBR", DEFAULT_WHERE_TEXT_DISPLAY_NAME, globals.UserLanguage, MasterDataStore);
            where.WhereClauseTrip = AddSimpleWhere(globals, where, where.WhereClauseTrip, WhereCriteria.CARDNUM, "T1.cardnum", displayName);
            where.WhereClauseTrip = AddSimpleWhere(globals, where, where.WhereClauseTrip, WhereCriteria.LLRECNO, "T1.llRecno");
        }

        public void HandleAgentCriteria(ReportGlobals globals, BuildWhere where, bool isNotIn)
        {
            var pickList = globals.GetParmValue(WhereCriteria.AGENTID);
            var pickName = string.Empty;

            var pl = new PickListParms(globals);
            pl.ProcessList(pickList, pickName, "AGENTID");

            where.WhereClauseTrip = _paramsBuilder.AddOrListToWhereClause(where.WhereClauseTrip, pl.PickList, "T1.agentid", isNotIn, where.SqlParameters);

            if (globals.GetParmValue(WhereCriteria.PREPOST) == "1")
            {
                var mtgGroups = globals.GetParmValue(WhereCriteria.MTGGRPNBR);
                if (!string.IsNullOrEmpty(mtgGroups))
                {
                    if (mtgGroups.Contains(",")) globals.WhereText += "Mtg Group #'s in (" + mtgGroups + ");";
                    else globals.WhereText += "Mtg Group # = " + mtgGroups + ";";
                }
            }
        }
        
        public void HandleOrphanServiceFees(ReportGlobals globals, string domIntl)
        {
            //can only apply to invoice date, and not dom/intl
            if (globals.GetParmValue(WhereCriteria.DATERANGE) != "2" && domIntl.IsBetween(2, 7)) globals.IncludeOrphanServiceFees = false;

            //Check for any parameters that conflict with Orphan Service Fees. 
            foreach (var whereCriteria in _notAllowed)
            {
                var val = globals.GetParmValue(whereCriteria);

                if (string.IsNullOrEmpty(val))
                {
                    globals.IncludeOrphanServiceFees = false;
                    //found one, exit. 
                    break;
                }
            }
        }

        public void HandleAccountType(ReportGlobals globals, BuildWhere where)
        {
            var agency = globals.Agency;
            switch (globals.GetParmValue(WhereCriteria.ACCTTYPE))
            {
                case "1": //Standard accounts
                    var accts = GetAccountsByAgencyAndAcctType(ClientDataStore.ClientQueryDb, GetAcctsByAgencyAndAcctTypeQuery.AcctType.Standard, agency).ToList();
                    where.WhereClauseTrip = _paramsBuilder.AddOrListToWhereClause(where.WhereClauseTrip, accts, "T1.acct", false, where.SqlParameters);
                    globals.WhereText += globals.ReportMessages.StandardAccountsOnly;
                    break;
                case "2": //Custom accounts
                    accts = GetAccountsByAgencyAndAcctType(ClientDataStore.ClientQueryDb, GetAcctsByAgencyAndAcctTypeQuery.AcctType.Custom, agency).ToList();
                    where.WhereClauseTrip = _paramsBuilder.AddOrListToWhereClause(where.WhereClauseTrip, accts, "T1.acct", false, where.SqlParameters);
                    globals.WhereText += globals.ReportMessages.CustomAccountsOnly;
                    break;
            }
        }

        private IList<string> GetAccountsByAgencyAndAcctType(IClientQueryable clientQueryDb, GetAcctsByAgencyAndAcctTypeQuery.AcctType accountType, string agency)
        {
            var getAcctsByAgencyAndAcctTypeQuery = new GetAcctsByAgencyAndAcctTypeQuery(clientQueryDb, accountType, agency);
            return getAcctsByAgencyAndAcctTypeQuery.ExecuteQuery();
        }
    }
}

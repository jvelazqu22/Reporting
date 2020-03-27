using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain;
using Domain.Helper;
using Domain.Models;
using Domain.Models.ReportPrograms.UserDefinedReport;
using Domain.Orm.Classes;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankClientQueries.UserDefinedQueries;
using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ReportLokkup;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class ReportLookups
    {
        private ClientFunctions _clientFunctions = new ClientFunctions();

        private List<UserReportColumnInformation> Columns { get; set; } 

        public List<UdidInformation> Udids { get; set; } = new List<UdidInformation>();

        public List<TripCo2Information> TripCo2List { get; set; } = new List<TripCo2Information>();

        public List<VendorTypeInformation> VendorTypes { get; set; } = new List<VendorTypeInformation>();

        private IList<vendors> Vendors { get; set; } = new List<vendors>();

        public List<LegSegInfo> Legs { get; set; } = new List<LegSegInfo>();

        public List<LegSegInfo> Segs { get; set; } = new List<LegSegInfo>();
        public List<hibTripsDerivedData> HibTripsDerivedData { get; set; } = new List<hibTripsDerivedData>();

        private IList<UserFieldCategory> UserFieldCategories { get; set; } = new List<UserFieldCategory>();

        private IList<TicketInfo> OriginalTickets { get; set; } = new List<TicketInfo>();

        public string WhereClause { get; set; }

        public BuildWhere BuildWhere { get; set; }

        public ReportGlobals Globals { get; set; }

        private readonly WhereClauseWithAdvanceParamsHandler whereClauseWithAdvanceParamsHandler =  new WhereClauseWithAdvanceParamsHandler();

        public ReportLookups(List<UserReportColumnInformation> columns, BuildWhere buildWhere, ReportGlobals globals)
        {
            Columns = columns;
            BuildWhere = buildWhere;
            WhereClause = BuildWhere.WhereClauseFull;
            Globals = globals;
        }
        
        public void SetLegs(bool assignTripClass, IClientDataStore clientStore, bool tripTlsSwitch)
        {
            var useMileageTable = Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE);

            if (Features.LoadLegLookupWhenIsLegLevel.IsEnabled())
            {
                Legs = GetLegOrSegmentInfo(true, clientStore, useMileageTable, assignTripClass, tripTlsSwitch);
            }
            else
            {
                if (assignTripClass || Columns.Any(x => x.LookupTable.EqualsIgnoreCase("CURLEGROUT")))
                {
                    Legs = GetLegOrSegmentInfo(true, clientStore, useMileageTable, assignTripClass, tripTlsSwitch);
                }
            }
        }
        
        public void SetSegs(bool assignTripClass, IClientDataStore clientStore, bool tripTlsSwitch)
        {
            if (assignTripClass || Columns.Any(x => UserReportCheckLists.RouteColumns.Contains(x.Name.Trim()) || x.LookupTable.EqualsIgnoreCase("CURSEGROUT")))
            {
                var useMileageTable = Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE);
                Segs = GetLegOrSegmentInfo(false, clientStore, useMileageTable, assignTripClass, tripTlsSwitch);
            }
        }

        public string LookupClassCategoryDescription(string classCategory, string agency, IClientQueryable queryDb, bool isTitleCase = false)
        {
            return _clientFunctions.LookupClassCategoryDescription(classCategory, agency, queryDb, isTitleCase);
        }

        private string BuildLegSegSql(bool tripTlsSwitch)
        {
            if (Features.BuildLegSegSqlHandler.IsEnabled())
            {
                return new BuildLegSegSqlHandler().BuildLegSegSql(tripTlsSwitch, Globals, WhereClause, BuildWhere);
            }

            var udid = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(0);
            var isReservationReport = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");

            var processkey = Globals.ProcessKey;
            var sql = new SqlScript();

            if (udid != 0)
            {
                sql.FromClause = isReservationReport
                    ? "ibtrips T1, iblegs T2, ibudids T3, ibTripsDerivedData TD"
                    : "hibtrips T1, hiblegs T2, hibudids T3, hibTripsDerivedData TD";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and T1.reckey = T3.reckey and T1.reckey=TD.reckey and valcarr != 'ZZ' and valcarr != '$$' and airline != 'ZZ' and airline != '$$' and ";
                if (processkey == (int)ReportTitles.ServiceFeeUserDefinedReports)
                {
                    sql.WhereClause = sql.KeyWhereClause + $" t1.reckey in (select reckey from hibServices where {WhereClause})";
                }
                else
                {
                    sql.WhereClause = sql.KeyWhereClause + WhereClause;
                }
            }
            else
            {
                sql.FromClause = isReservationReport
                    ? "ibtrips T1, iblegs T2, ibTripsDerivedData TD"
                    : "hibtrips T1, hiblegs T2, hibTripsDerivedData TD";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and T1.reckey=TD.reckey and valcarr != 'ZZ' and valcarr != '$$' and airline != 'ZZ' and airline != '$$'  and ";
                if (processkey == (int)ReportTitles.ServiceFeeUserDefinedReports)
                {
                    sql.WhereClause = sql.KeyWhereClause + $" t1.reckey in (select reckey from hibServices where {WhereClause})";
                }
                else
                {
                    sql.WhereClause = sql.KeyWhereClause + WhereClause;
                }
            }

            if (tripTlsSwitch)
            {
                sql.FromClause = isReservationReport? sql.FromClause.Replace("ibtrips", "vibtripstls") : sql.FromClause.Replace("hibtrips", "vhibtripstls");
            }

            sql.FieldList = "T1.reckey, connect,origin,destinat,airline,class,classcat, convert(int,miles) as miles,mode,farebase, " +
                "TD.segment_itinerary as DerivedSegRouting, TD.leg_itinerary as DerivedLegRouting, TD.trip_class as DerivedTripClass, TD.trip_class_category as DerivedTripClassCat, ";

            //Reservation table doesn't have this field refer to transid.
            sql.FieldList += isReservationReport ?
                   "'' as DerivedTransId" : "TD.tripTransactionID as DerivedTransId";                    
           
            sql.OrderBy = "order by T1.reckey, seqno";

            sql.WhereClause = whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, BuildWhere, true);

            return SqlProcessor.ProcessSql(sql.FieldList, false, sql.FromClause, sql.WhereClause, sql.OrderBy, BuildWhere.ReportGlobals);
        }

        private string BuildLegSegSqlDeprecate()
        {
            var udid = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(0);
            var isReservationReport = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");

            var processkey = Globals.ProcessKey;
            var sql = new SqlScript();

            if (udid != 0)
            {
                sql.FromClause = isReservationReport
                    ? "ibtrips T1, iblegs T2, ibudids T3"
                    : "hibtrips T1, hiblegs T2, hibudids T3";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and T1.reckey = T3.reckey and valcarr != 'ZZ' and valcarr != '$$' and airline != 'ZZ' and airline != '$$' and ";
                if (processkey == (int)ReportTitles.ServiceFeeUserDefinedReports)
                {
                    sql.WhereClause = sql.KeyWhereClause + $" t1.reckey in (select reckey from hibServices where {WhereClause})";
                }
                else
                {
                    sql.WhereClause = sql.KeyWhereClause + WhereClause;
                }
            }
            else
            {
                sql.FromClause = isReservationReport
                    ? "ibtrips T1, iblegs T2"
                    : "hibtrips T1, hiblegs T2";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and valcarr != 'ZZ' and valcarr != '$$' and airline != 'ZZ' and airline != '$$'  and ";
                if (processkey == (int)ReportTitles.ServiceFeeUserDefinedReports)
                {
                    sql.WhereClause = sql.KeyWhereClause + $" t1.reckey in (select reckey from hibServices where {WhereClause})";
                }
                else
                {
                    sql.WhereClause = sql.KeyWhereClause + WhereClause;
                }
            }

            sql.FieldList = "T1.reckey, connect,origin,destinat,airline,class,classcat, convert(int,miles) as miles,mode,farebase";
            sql.OrderBy = "order by T1.reckey, seqno";

            sql.WhereClause = whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, BuildWhere, true);

            return SqlProcessor.ProcessSql(sql.FieldList, false, sql.FromClause, sql.WhereClause, sql.OrderBy, BuildWhere.ReportGlobals);
        }

        private List<LegSegInfo> GetLegOrSegmentInfo(bool isLegInfo, IClientDataStore clientStore, bool useMileageTable, bool assignTripClass, bool tripTlsSwitch)
        {
            var legSegInfo = new List<LegSegInfo>();

            var sql = string.Empty;
            if (Features.RoutingUseTripsDerivedDataTable.IsEnabled())
            {
                sql = BuildLegSegSql(tripTlsSwitch);
            }
            else
            {
                sql = BuildLegSegSqlDeprecate();
            }

            var tempData = ClientDataRetrieval.GetOpenQueryData<LegSegTemp>(sql, Globals, BuildWhere.Parameters).ToList();

            if (useMileageTable) AirMileageCalculator<LegSegTemp>.CalculateAirMileageFromTable(tempData);

            var firstRec = tempData.FirstOrDefault();
            if (firstRec == null) return legSegInfo;

            var builder = new LegSegBuilder { PrimRecKey = firstRec.RecKey };

            if (assignTripClass)
            {
                var classCatBuf = firstRec.ClassCat ?? string.Empty;
                var hierarchy = _clientFunctions.LookupClassCategoryHierarchy(classCatBuf, clientStore.ClientQueryDb);
                builder.SetFromClassCategory(firstRec, classCatBuf, hierarchy);
            }

            foreach (var item in tempData)
            {
                item.Origin = item.Origin ?? string.Empty;
                item.Airline = item.Airline ?? string.Empty;
                item.ClassCat = item.ClassCat ?? string.Empty;
                item.ClassCode = item.ClassCode ?? string.Empty;
                item.Connect = item.Connect ?? string.Empty;
                item.Destinat = item.Destinat ?? string.Empty;
                item.Farebase = item.Farebase ?? string.Empty;
                item.Mode = item.Mode ?? string.Empty;
                item.DerivedSegRouting = item.DerivedSegRouting;
                item.DerivedLegRouting = item.DerivedLegRouting;
                item.DerivedTransId = item.DerivedTransId;
                item.DerivedTripClass = item.DerivedTripClass;
                item.DerivedTripClassCat = item.DerivedTripClassCat;

                if (builder.PrimRecKey != item.RecKey)
                {
                    var newLegSeg = builder.BuildLegSegInfo();
                    legSegInfo.Add(newLegSeg);

                    builder = new LegSegBuilder { PrimRecKey = item.RecKey };
                }

     
                if (!item.Origin.EqualsIgnoreCase(builder.PreviousDestination))
                {
                    if (Features.OriginTranslatingUseMode.IsEnabled())
                    {
                        builder.SetSegRouting(item.Origin);
                        builder.ModeThisHierarchy = item.Mode;
                    }
                    else
                    {
                        builder.SetSegRoutingDeprecate(item.Origin);
                    }
                }

                if (!IsConnection(item) || isLegInfo)
                {
                    if (Features.OriginTranslatingUseMode.IsEnabled())
                    {
                        builder.SetSegRouting(item.Destinat);
                        builder.ModeThisHierarchy = item.Mode;
                    }
                    else
                    {
                        builder.SetSegRoutingDeprecate(item.Destinat);
                    }

                    builder.Carriers += item.Airline.Trim() + " ";
                    builder.Classes += item.ClassCode.Trim() + " ";
                    builder.ClassCategories += item.ClassCat.Trim() + " ";
                    builder.FareBaseCodes += item.Farebase.Trim().Left(1) + " ";
                }

                builder.DerivedLegRouting = item.DerivedLegRouting;
                builder.DerivedSegRouting = item.DerivedSegRouting;
                builder.DerivedTransId = item.DerivedTransId;
                builder.DerivedTripClass = item.DerivedTripClass;
                builder.DerivedTripClassCat = item.DerivedTripClassCat;
                builder.PreviousDestination = item.Destinat.Trim();
                builder.TripMiles += item.Miles;

                if (assignTripClass)
                {
                    var classCatBuf = item.ClassCat ?? string.Empty;
                    var hierarchy = _clientFunctions.LookupClassCategoryHierarchy(classCatBuf, clientStore.ClientQueryDb);
                    if (hierarchy < builder.LegClassHierarchy)
                    {
                        builder.SetFromClassCategory(firstRec, classCatBuf, hierarchy);
                    }
                }
            }

            var lastLegSeg = builder.BuildLegSegInfo();
            legSegInfo.Add(lastLegSeg);

            return legSegInfo;
        }

        private bool IsConnection(LegSegTemp item)
        {
            return item.Connect.EqualsIgnoreCase("X");
        }

        public string LookupVendorDescription(string clientId, string vendorCode, IClientQueryable queryDb, string agency)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(vendorCode)) return string.Empty;

            if(!Vendors.Any()) SetVendors(queryDb, agency);

            var vendor = Vendors.FirstOrDefault(s => s.clientID.EqualsIgnoreCase(clientId) && s.vendorCode.EqualsIgnoreCase(vendorCode));

            return vendor?.vendorDesc.Trim() ?? string.Empty;
        }

        public void SetTripsDerivedData(bool transIdGsaSwitch, List<int> recKeyList, IClientDataStore clientStore)
        {
            if (transIdGsaSwitch && HibTripsDerivedData.Count == 0)
            {
                HibTripsDerivedData = new GetHibTripsDerivedDataByRecKeyListQuery(clientStore.ClientQueryDb, recKeyList).ExecuteQuery();
            }
        }

        private void SetVendors(IClientQueryable queryDb, string agency)
        {
            var query = new GetAllVendorsByAgencyQuery(queryDb, agency);
            Vendors = query.ExecuteQuery();
        }

        public void SetVendorTypes(bool isPreviewReport, bool udidCriteriaExists)
        {
            if (Columns.Any(x => x.LookupTable.EqualsIgnoreCase("CURVENDTYPES")))
            {
                VendorTypes = GetVendors(isPreviewReport, udidCriteriaExists, Globals);
            }
        }

        private List<VendorTypeInformation> GetVendors(bool isPreviewReport, bool udidCriteriaExists, ReportGlobals globals)
        {
            var sql = new SqlScript();

            if (udidCriteriaExists)
            {
                sql.FromClause = isPreviewReport
                    ? "ibtrips T1, ibudids T3"
                    : "hibtrips T1, hibudids T3";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and T1.reckey = T3.reckey and valcarr != 'ZZ' and valcarr != '$$' and airline != 'ZZ' and airline != '$$' and ";
                sql.WhereClause = sql.KeyWhereClause + WhereClause;
            }
            else
            {
                sql.FromClause = isPreviewReport
                    ? "ibtrips T1"
                    : "hibtrips T1";

                sql.KeyWhereClause = "T1.reckey = T3.reckey and ";
                sql.WhereClause = sql.KeyWhereClause + WhereClause;
            }

            sql.FieldList = isPreviewReport
                    ? "T1.reckey, '' as vendortype"
                    : "T1.reckey, T9a.vendortype";
            
            sql.WhereClause = whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, BuildWhere, true);

            string sqlToExecute = SqlProcessor.ProcessSql(sql.FieldList, false, sql.FromClause, sql.WhereClause, sql.OrderBy, BuildWhere.ReportGlobals);
            
            return ClientDataRetrieval.GetOpenQueryData<VendorTypeInformation>(sqlToExecute, globals, BuildWhere.Parameters).ToList();
        }


        public void SetTripCarbon(List<LegRawData> legRawDataList)
        {
            var co2Exists = Columns.Where(x => x.GoodField.Contains("CO2"));
            if (null != co2Exists|| Columns.Any(x => x.LookupTable.EqualsIgnoreCase("CURTRIPCARBON")))
            {
                TripCo2List = legRawDataList.GroupBy(s => s.RecKey, (key, g) =>
                {
                    var reclist = g.ToList();
                    var firstOrDefault = reclist.FirstOrDefault();
                    if (firstOrDefault == null) return new TripCo2Information();

                    return new TripCo2Information
                    {
                        AirCo2 = reclist.Sum(s => s.AirCo2),
                        TripCo2 = reclist.Sum(s => s.AirCo2),
                        AltCarCo2 = reclist.Sum(s => s.AltCarCo2),
                        AltRailCo2 = reclist.Sum(s => s.AltRailCo2),
                        RecKey = firstOrDefault.RecKey
                    };
                }).ToList();
            }
        }

        public void SetUdids(bool isPreviewReport, SwitchManager switchManager)
        {
            if (Columns.Any(x => x.LookupTable.EqualsIgnoreCase("HIBUDIDS") || x.LookupTable.EqualsIgnoreCase("IBUDIDS")))
            {
                Udids = GetUdids(isPreviewReport, switchManager, BuildWhere, BuildWhere.Parameters, Globals);                
            }
        }

        private List<UdidInformation> GetUdids(bool preview, SwitchManager switchManager, BuildWhere buildWhere, object[] buildWhereParameters, ReportGlobals globals)
        {
            string sqlToExecute = new UdidSqlBuilder().GetSql(preview, switchManager, buildWhere.WhereClauseFull, buildWhere);

            return ClientDataRetrieval.GetOpenQueryData<UdidInformation>(sqlToExecute, globals, buildWhereParameters).ToList();
        }

        public string LookupFareType(string prdfarebas)
        {
            //Order	Fare Type's Contents	Predominant Fare Basis Code Criteria
            // 1. 		First 				begins with F 
            // 2. 		Business 			begins with B 
            // 3.		DG					the letters DG as the 2nd and 3rd character 
            // 4. 		CPP Business		the letters CB as the 2nd and 3rd character 
            // 5. 		YCA					these have values of YCA 
            // 5. 		Dash CA				the letters CA as the 2nd and 3rd character 
            // 7. 		Other				everything else 

            if (prdfarebas.Left(1) == "F") return "First";
            if (prdfarebas.Left(1) == "B") return "Business";
            if (prdfarebas?.SubStr(2, 2) == "DG") return "DG";
            if (prdfarebas?.SubStr(2, 2) == "CB") return "CPP Business";
            if (prdfarebas != null && prdfarebas.Contains("YCA")) return "YCA";
            if (prdfarebas?.SubStr(2, 2) == "CA") return "Dash CA";
            return "Other";
        }

        public string LookupUserFieldCategory(string cat, IClientQueryable queryDb)
        {
            if (!UserFieldCategories.Any()) SetUserFieldCategories(queryDb);

            var catNumber = cat.Substring(2, cat.Length - 2).TryIntParse(-1);
            if (catNumber == -1) return string.Empty;

            var category = UserFieldCategories.FirstOrDefault(s => s.Key == catNumber);
            return category == null ? string.Empty : category.Description;
        }

        private void SetUserFieldCategories(IClientQueryable queryDb)
        {
            var query = new GetAllUserFieldCategories(queryDb);
            UserFieldCategories = query.ExecuteQuery();
        }

        public string LookupOriginalTicket(string ticket, string type, string whereClause)
        {
            ticket = ticket.Trim();
            type = type.Trim();

            if(!OriginalTickets.Any()) SetTickets(whereClause);

            var invoice = new string(' ', 10);
            var valCarr = new string(' ', 2);
            var airChg = 0m;

            if (!string.IsNullOrWhiteSpace(ticket))
            {
                var rec = OriginalTickets.FirstOrDefault(s => s.Ticket.Trim().Equals(ticket));
                if (rec != null)
                {
                    invoice = rec.Invoice;
                    valCarr = rec.Valcarr;
                    airChg = rec.AirChg;
                }
            }

            switch (type.ToUpper())
            {
                case "INV":
                    return invoice.Trim();
                case "VC":
                    return valCarr.Trim();
                case "AMT":
                    return airChg.ToString("0.00");
                default:
                    return string.Empty;
            }
        }

        private void SetTickets(string whereClause)
        {
            var whereAcct = string.Empty;
            if (Globals.ParmHasValue(WhereCriteria.ACCT) || Globals.ParmHasValue(WhereCriteria.INACCT))
            {
                var acctList = Globals.GetParmValue(WhereCriteria.INACCT);
                if (string.IsNullOrEmpty(acctList))
                {
                    acctList = Globals.GetParmValue(WhereCriteria.ACCT);
                }

                var pl = new PickListParms(Globals);
                pl.ProcessList(acctList, string.Empty, "ACCTS");

                for (int i = 0; i < pl.PickList.Count; i++)
                {
                    whereAcct += " acct='" + pl.PickList[i] + "' ";
                    if (i != pl.PickList.Count - 1)
                    {
                        whereAcct += " or ";
                    }
                }

                if (Globals.IsParmValueOn(WhereCriteria.NOTINACCT))
                {
                    whereAcct = " not (" + whereAcct + ")";
                }

                whereAcct = " and " + whereAcct;
            }

            var fieldList = "t1.reckey,ticket,invoice,valcarr,airchg,moneytype";
            var fromList = "hibtrips T1";
            if (Globals.ParmValueEquals(WhereCriteria.PREPOST, "1"))
            {
                fieldList = "t1.reckey,ticket,invoice,valcarr,airchg,moneytype";
                fromList = "ibtrips T1";
            }

            var sqlWhere = whereClause + " and valcarr != 'ZZ' and valcarr != '$$'" + whereAcct + " and T1.agency = '" + Globals.Agency + "'" + whereAcct;

            var fullSql = SqlProcessor.ProcessSql(fieldList, false, fromList, sqlWhere, string.Empty, Globals);
            OriginalTickets = ClientDataRetrieval.GetOpenQueryData<TicketInfo>(fullSql, Globals, BuildWhere.Parameters).ToList();
        }

        public string LookupReason(string code, IClientQueryable queryDb, string agency, string languageCode)
        {
            return _clientFunctions.LookupReason(code, queryDb, agency, languageCode);
        }
    }
}

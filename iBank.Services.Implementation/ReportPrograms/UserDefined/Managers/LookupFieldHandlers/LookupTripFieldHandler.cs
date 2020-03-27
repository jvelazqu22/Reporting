using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using CODE.Framework.Core.Utilities.Extensions;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ReportLokkup;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupTripFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly IMasterDataStore _masterStore;
        private readonly IClientDataStore _clientStore;
        private readonly bool _isTitleCaseRequired;
        private readonly ReportGlobals _globals;
        private ReportLookups _reportLookups;
        private readonly ClientFunctions _clientFunctions = new ClientFunctions();
        public List<Tuple<string, string>> TripSummaryLevel { get; set; }
        private readonly BuildWhere _buildWhere;


        public SegmentOrLeg _segmentOrLeg;

        public LookupTripFieldHandler(UserDefinedParameters userDefinedParams, IMasterDataStore masterStore, IClientDataStore clientStore, bool isTitleCaseRequired,
            ReportGlobals globals, SegmentOrLeg segmentOrLeg, ReportLookups reportLookups, BuildWhere buildWhere)
        {
            _userDefinedParams = userDefinedParams;
            _masterStore = masterStore;
            _clientStore = clientStore;
            _isTitleCaseRequired = isTitleCaseRequired;
            _globals = globals;
            _segmentOrLeg = segmentOrLeg;
            _reportLookups = reportLookups;
            TripSummaryLevel = new List<Tuple<string, string>>();
            _buildWhere = buildWhere;
        }

        public string HandleLookupFieldTrip(UserReportColumnInformation column, RawData mainRec, int tripSummaryLevel, ColumnValueRulesFactory factory)
        {
            if (!Features.HandleLookupFieldTripRefactor.IsEnabled()) return HandleLookupFieldTripLegacy(column, mainRec, tripSummaryLevel);

            try
            {
                if (column.Name.Left(4).EqualsIgnoreCase("UDID"))
                {
                    var udidNumber = column.Name.Replace("UDID", string.Empty);
                    var udidInt = udidNumber.TryIntParse(-1);
                    if (udidInt < 0) return string.Empty;

                    var udidText = _userDefinedParams.UdidLookup[mainRec.RecKey].FirstOrDefault(x => x.UdidNo == udidInt);
                    return udidText != null ? udidText.UdidText : string.Empty;
                }
                if (column.Name.Left(2).EqualsIgnoreCase("UC")) return _reportLookups.LookupUserFieldCategory(column.Name, _clientStore.ClientQueryDb);

                var colValRulesParams = GetColValRulesParams(column, mainRec, tripSummaryLevel);
                var columnValue = factory.CreateInstance(column.Name, this);
                columnValue.SetupParams(colValRulesParams);
                return columnValue.CalculateColValue();
            }
            catch (Exception e)
            {
                if (e.InnerException != null) ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                throw;
            }
        }

        private ColValRulesParams GetColValRulesParams(UserReportColumnInformation column, RawData mainRec, int tripSummaryLevel)
        {
            var colValRulesParams = new ColValRulesParams()
            {
                ClientFunctions = _clientFunctions,
                ClientDataStore = _clientStore,
                MasterStore = _masterStore,
                Globals = _globals,
                MainRec = mainRec,
                ReportLookups = _reportLookups,
                IsTitleCaseRequired = _isTitleCaseRequired,
                TripSummaryLevelInt = tripSummaryLevel,
                Column = column,
                UserDefinedParams = _userDefinedParams,
                TripSummaryLevelTuple = TripSummaryLevel,
                BuildWhere = _buildWhere,
            };
            if (_globals.ParmHasValue(WhereCriteria.RBAPPLYTOLEGORSEG))
            {
                colValRulesParams.SegmentOrLeg = _globals.ParmValueEquals(WhereCriteria.RBAPPLYTOLEGORSEG, "1")
                    ? SegmentOrLeg.Leg
                    : SegmentOrLeg.Segment;
            }
            return colValRulesParams;
        }

        public string HandleLookupFieldTripLegacy(UserReportColumnInformation column, RawData mainRec, int tripSummaryLevel)
        {
            try
            {
                if (column.Name.Left(4).EqualsIgnoreCase("UDID"))
                {
                    var udidNumber = column.Name.Replace("UDID", string.Empty);
                    var udidInt = udidNumber.TryIntParse(-1);
                    if (udidInt < 0)
                    {
                        return string.Empty;
                    }

                    var udidText = _userDefinedParams.UdidLookup[mainRec.RecKey].FirstOrDefault(x => x.UdidNo == udidInt);
                    return udidText != null ? udidText.UdidText : string.Empty;
                }
                if (column.Name.Left(2).EqualsIgnoreCase("UC"))
                {
                    return _reportLookups.LookupUserFieldCategory(column.Name, _clientStore.ClientQueryDb);
                }

                switch (column.Name)
                {
                    case "ACCOUNT": // done
                        return _clientFunctions.LookupCname(new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency), mainRec.Acct, _globals);
                    case "ADVBOOK": //done
                        return (mainRec.DepDate.GetValueOrDefault() - mainRec.Bookdate.GetValueOrDefault()).Days.ToString();
                    case "ADVPURCH": //done
                        return (mainRec.DepDate.GetValueOrDefault() - mainRec.Invdate.GetValueOrDefault()).Days.ToString();
                    case "AIRREASN": // done
                        return _reportLookups.LookupReason(mainRec.Reascode, _clientStore.ClientQueryDb, _globals.Agency, _globals.UserLanguage);
                    case "ARRDATEDOW": // done
                        return mainRec.Arrdate?.DayOfWeek.ToString().Substring(0, 3) ?? "";
                    case "BKMONTH": // done
                        return mainRec.Bookdate.GetValueOrDefault().GetDateMonthNumber();
                    case "BKMTHABBR":
                        return mainRec.Bookdate.GetValueOrDefault().GetDateMonthAbbreviation();
                    case "BKMTHNAM": // done
                        return mainRec.Bookdate.GetValueOrDefault().GetDateMonthName();
                    case "BKQTR": // done
                        return mainRec.Bookdate.GetValueOrDefault().GetDateQuarter();
                    case "BKYEAR": // done
                        return mainRec.Bookdate.GetValueOrDefault().GetDateYear();
                    case "CARRDESC": // done
                        return LookupFunctions.LookupAline(_masterStore, mainRec.Valcarr, mainRec.Valcarmode, isTitleCase: _isTitleCaseRequired);
                    case "CCCOMPANY": // done
                        return LookupFunctions.LookupCreditCardCompany(mainRec.Cardnum);
                    case "CLIENTNAME": //done
                        return _clientFunctions.LookupClientName(mainRec.Clientid, _globals.Agency, _clientStore.ClientQueryDb);
                    case "CREDCOUNT": // done
                        return mainRec.Trantype.EqualsIgnoreCase("C") ? "00000001" : "00000000";
                    case "CURSYMBOL": // done
                        return LookupFunctions.LookupCurrencySymbol(mainRec.Moneytype, _masterStore);
                    case "DEPDTDOW": // done
                        return mainRec.DepDate.GetValueOrDefault().DayOfWeek.ToString().Left(3);
                    case "EXCHANGE": // done
                        return mainRec.Exchange ? "Y" : "N";
                    case "HOMCTRYCOD": // done
                        return LookupFunctions.LookupHomeCountryCode(mainRec.Sourceabbr, _globals, _masterStore);
                    case "HOMCTRYNAM": // done
                        return LookupFunctions.LookupHomeCountryName(mainRec.Sourceabbr, _globals, _masterStore);
                    case "INVAMTNFEE": // done
                        if (tripSummaryLevel == 1)
                        {
                            if (!CalcTripSummaryField(mainRec.Recloc, column.Name)) return string.Empty;

                            var recKeys = _userDefinedParams.TripDataList.Where(s => s.Recloc.EqualsIgnoreCase(mainRec.Recloc)).Select(s => s.RecKey);
                            var invAmtSum = _userDefinedParams.TripDataList.Where(s => s.Recloc.EqualsIgnoreCase(mainRec.Recloc)).Sum(s => s.PlusMin);
                            var svcFeeRecs = _userDefinedParams.ServiceFeeDataList.Where(s => recKeys.Contains(s.RecKey)).ToList();
                            if (!svcFeeRecs.Any()) return string.Empty;

                            var svcFeeSum = svcFeeRecs.Sum(s => s.Svcfee);
                            return (invAmtSum - svcFeeSum).ToString("0.00");
                        }
                        var invAmt = _userDefinedParams.TripLookup[mainRec.RecKey].Where(x => x.Invamt.HasValue).Sum(x => x.Invamt ?? 0);
                        var svcFees = _userDefinedParams.ServiceFeeLookup[mainRec.RecKey].Sum(x => x.Svcfee);

                        return (invAmt - svcFees).ToString("0.00").PadLeft(12);

                    case "INVDTDOW": // done
                        return mainRec.Invdate.GetValueOrDefault().DayOfWeek.ToString().Substring(0, 3);
                    case "INVMO": // done
                        return LookupFunctions.LookupMonth(mainRec.Invdate.GetValueOrDefault());
                    case "INVMONTH": // done
                        return mainRec.Invdate.GetValueOrDefault().GetDateMonthNumber();
                    case "INVMTHABBR": // done
                        return mainRec.Invdate.GetValueOrDefault().GetDateMonthAbbreviation();
                    case "INVMTHNAM": // done
                        return mainRec.Invdate.GetValueOrDefault().GetDateMonthName();
                    case "INVQTR": // done
                        return mainRec.Invdate.GetValueOrDefault().GetDateQuarter();
                    case "INVYR": // done
                        return mainRec.Invdate.GetValueOrDefault().Year.ToString().PadLeft(4, '0');
                    case "INVYRMO": // done
                        return (mainRec.Invdate.GetValueOrDefault().Year * 100 + mainRec.Invdate.GetValueOrDefault().Month).ToString(CultureInfo.InvariantCulture).PadLeft(6, '0');
                    case "LEGROUTING": // done
                        return LookupFunctions.LookupLegRoute(_reportLookups.Legs, mainRec.RecKey);
                    case "ORIGTKTCAR": // done
                        return _reportLookups.LookupOriginalTicket(mainRec.Origticket, "VC", _buildWhere.WhereClauseTrip);
                    case "ORIGTKTCHG": // done
                        return _reportLookups.LookupOriginalTicket(mainRec.Origticket, "AMT", _buildWhere.WhereClauseTrip);
                    case "ORIGTKTINV": // done
                        return _reportLookups.LookupOriginalTicket(mainRec.Origticket, "INV", _buildWhere.WhereClauseTrip);
                    case "PARENTACCT": // done
                        return _clientFunctions.LookupParent(new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency), mainRec.Acct, new GetAllParentAccountsQuery(_clientStore.ClientQueryDb), _globals.Agency).AccountId.PadRight(10);
                    case "PARENTDESC": // done
                        return _clientFunctions.LookupParent(new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency), mainRec.Acct, new GetAllParentAccountsQuery(_clientStore.ClientQueryDb), _globals.Agency).AccountDescription;
                    case "PAXNAME": // done
                        return mainRec.Passlast.Trim() + "/" + mainRec.Passfrst.Trim();
                    case "PAXNAMEABR": // done
                        return mainRec.Passfrst.Trim().Left(1) + ". " + mainRec.Passlast.Trim();
                    case "ROUNDTRIP": // done
                        return LookupFunctions.LookupRoundTrip(_reportLookups.Segs, mainRec.RecKey);
                    case "SEGROUTING": // done
                        return LookupFunctions.LookupSegRoute(_reportLookups.Segs, mainRec.RecKey);
                    case "SOURCEDESC": // done
                        return LookupFunctions.LookupSourceDescription(_masterStore, mainRec.Sourceabbr, mainRec.Agency);
                    case "TKTCOUNT": // done
                        if (tripSummaryLevel == 1)
                        {
                            return CalcTripSummaryField(mainRec.Recloc, column.Name) ? _userDefinedParams.TripDataList.Count(s => s.Recloc.Equals(mainRec.Recloc) && !string.IsNullOrEmpty(s.Ticket.Trim())).ToString() : string.Empty;
                        }
                        return mainRec.Trantype.EqualsIgnoreCase("I") && !string.IsNullOrEmpty(mainRec.Ticket.Trim()) ? "00000001" : "00000000";
                    case "TRIPAIRCO2": // done
                        return LookupFunctions.LookupTrpCo2(_reportLookups.TripCo2List, mainRec.RecKey, 1).ToString(CultureInfo.InvariantCulture);
                    case "TRIPCLASS": // done
                        return LookupFunctions.LookupTripClass(_reportLookups.Segs, mainRec.RecKey);
                    case "TRIPCNTR": // done
                        //000000000000
                        return "000000000000";
                    case "TRIPCO2": // done
                        return LookupFunctions.LookupTrpCo2(_reportLookups.TripCo2List, mainRec.RecKey, 2).ToString(CultureInfo.InvariantCulture);
                    case "TRIPDAYS": // done
                        if (mainRec.Arrdate == null) return "0";
                        var duration = ((mainRec.Arrdate.Value.AddDays(1) - mainRec.DepDate.GetValueOrDefault()).Days * mainRec.PlusMin);
                        return duration > 1000 ? "0" : duration.ToString(CultureInfo.InvariantCulture);
                    case "TRIPDEST": // done
                        return AportLookup.LookupAport(_masterStore, LookupFunctions.LookupSegRouteFirstDestination(_reportLookups.Segs, mainRec.RecKey), mainRec.Valcarr, _globals.Agency);
                    case "TRIPDSTCOD": // done
                        return LookupFunctions.LookupSegRouteFirstDestination(_reportLookups.Segs, mainRec.RecKey);
                    case "TRIPMILES": // done
                        return (LookupFunctions.LookupTripMiles(_reportLookups.Segs, mainRec.RecKey) * mainRec.PlusMin).ToString();
                    case "TRIPORGCOD":
                    case "ORGCTYCOD": // done - to be updated with Baofen changes
                        if (Features.OriginTranslatingUseMode.IsEnabled())
                        {
                            var ctrycode = LookupFunctions.LookupSegRouteFirstOrigin(_reportLookups.Segs, mainRec.RecKey);
                            return ctrycode.Contains('-') ? ctrycode.Split('-')[0] : ctrycode;
                        }
                        else
                        {
                            return LookupFunctions.LookupSegRouteFirstOrigin(_reportLookups.Segs, mainRec.RecKey);
                        }
                    case "TRIPORIGIN": // done
                        return AportLookup.LookupAport(_masterStore, LookupFunctions.LookupSegRouteFirstOrigin(_reportLookups.Segs, mainRec.RecKey), mainRec.Valcarr.Trim(), _globals.Agency);
                    case "TRPALTCCO2": // done
                        return LookupFunctions.LookupTrpCo2(_reportLookups.TripCo2List, mainRec.RecKey, 4).ToString(CultureInfo.InvariantCulture);
                    case "TRPALTRCO2": // done
                        return LookupFunctions.LookupTrpCo2(_reportLookups.TripCo2List, mainRec.RecKey, 3).ToString(CultureInfo.InvariantCulture);
                    case "TRPCLASCAT": // done
                        return LookupFunctions.LookupTripClassCat(_reportLookups.Segs, mainRec.RecKey);
                    case "TRPCLASDSC": // done
                        var catclass = _reportLookups.LookupClassCategoryDescription(LookupFunctions.LookupTripClassCatCode(_reportLookups.Segs, mainRec.RecKey), _globals.Agency, _clientStore.ClientQueryDb, _isTitleCaseRequired);
                        //GSA want this to be UPPERCASE
                        return _globals.Agency == "GSA" ? catclass.Upper() : catclass;
                    case "TRPDSTCTRY": // done
                        return LookupFunctions.LookupCountry(_masterStore, LookupFunctions.LookupSegRouteFirstDestination(_reportLookups.Segs, mainRec.RecKey), _isTitleCaseRequired, mainRec.Valcarr);
                    case "TRPDSTRGN": // done
                        return LookupFunctions.LookupRegion(LookupFunctions.LookupSegRouteFirstDestination(_reportLookups.Segs, mainRec.RecKey), mainRec.Valcarr, _masterStore, _isTitleCaseRequired);
                    case "TRPORGCTRY": // done
                        return LookupFunctions.LookupCountry(_masterStore, LookupFunctions.LookupSegRouteFirstOrigin(_reportLookups.Segs, mainRec.RecKey), _isTitleCaseRequired, mainRec.Valcarr);
                    case "TRPORGRGN": // done
                        return LookupFunctions.LookupRegion(LookupFunctions.LookupSegRouteFirstOrigin(_reportLookups.Segs, mainRec.RecKey), mainRec.Valcarr, _masterStore, _isTitleCaseRequired);
                    case "VOIDCOUNT": // done
                        return mainRec.Trantype.EqualsIgnoreCase("V") ? "00000001" : "00000000";
                    case "SAVNGREASN": // done
                        return _reportLookups.LookupReason(mainRec.Savingcode, _clientStore.ClientQueryDb, _globals.Agency, _globals.UserLanguage);
                    //IBTRIPS
                    case "BOOKDTDOW": // done
                        return mainRec.Bookdate.GetValueOrDefault().DayOfWeek.ToString().Substring(0, 3);
                    case "DSAVERRMK": // done
                        return LookupFunctions.LookupUdid(_reportLookups.Udids, mainRec.RecKey, 84, 16);
                    case "FALLOC": // done
                        return LookupFunctions.LookupUdid(_reportLookups.Udids, mainRec.RecKey, 81, 50);
                    case "G6FIELD": // done
                        return LookupFunctions.LookupUdid(_reportLookups.Udids, mainRec.RecKey, 83, 5);
                    case "ORDBYRMK": // done
                        return LookupFunctions.LookupUdid(_reportLookups.Udids, mainRec.RecKey, 82, 4);
                    //ACCTSPCL table
                    case "ACCTCAT1": // done
                        return LookupFunctions.LookupAccountCategory(_clientFunctions, new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency), mainRec.Acct, 1);
                    case "ACCTCAT2": // done
                        return LookupFunctions.LookupAccountCategory(_clientFunctions, new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency), mainRec.Acct, 2);
                    case "ACCTCAT3": // done
                        return LookupFunctions.LookupAccountCategory(_clientFunctions, new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency), mainRec.Acct, 3);
                    case "ACCTCAT4": // done
                        return LookupFunctions.LookupAccountCategory(_clientFunctions, new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency), mainRec.Acct, 4);
                    case "ACCTCAT5": // done
                        return LookupFunctions.LookupAccountCategory(_clientFunctions, new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency), mainRec.Acct, 5);
                    //TRIPTLS table
                    case "CTRYNBR": // done
                        if (mainRec.Invctrycod == null) return "[null]";
                        return LookupFunctions.LookupCountryNumber(_masterStore, mainRec.Invctrycod);
                    case "VENDNAMES": // done
                        if (tripSummaryLevel == 1)
                        {
                            if (!CalcTripSummaryField(mainRec.Recloc, column.Name)) return string.Empty;

                            var vendCodes = _userDefinedParams.TripDataList.Where(s => s.Recloc.EqualsIgnoreCase(mainRec.Recloc) && !string.IsNullOrEmpty(s.Clientid.Trim()) && !string.IsNullOrEmpty(s.Trpvendcod.Trim()));
                            var vendDescs = vendCodes.Select(s => _reportLookups.LookupVendorDescription(s.Clientid, s.Trpvendcod, _clientStore.ClientQueryDb, _globals.Agency));
                            return string.Join(",", vendDescs);
                        }
                        return _reportLookups.LookupVendorDescription(mainRec.Clientid, mainRec.Trpvendcod, _clientStore.ClientQueryDb, _globals.Agency);
                    case "VENDTYPES": // done
                        if (tripSummaryLevel == 1)
                        {
                            if (CalcTripSummaryField(mainRec.Recloc, column.Name))
                            {
                                var recKeys = _userDefinedParams.TripDataList.Where(s => s.Recloc.EqualsIgnoreCase(mainRec.Recloc)).Select(s => s.RecKey);

                                var vendTypes = recKeys.Select(key => LookupFunctions.LookupVendorType(_reportLookups.VendorTypes, key));

                                return string.Join(",", vendTypes);
                            }

                            return string.Empty;
                        }
                        return LookupFunctions.LookupVendorType(_reportLookups.VendorTypes, mainRec.RecKey);
                    case "HTLCOUNT": // done
                        if (tripSummaryLevel == 1)
                        {
                            if (!CalcTripSummaryField(mainRec.Recloc, column.Name)) return string.Empty;

                            var recKeys = _userDefinedParams.HotelDataList.Where(s => s.Recloc.EqualsIgnoreCase(mainRec.Recloc)).Select(s => s.RecKey);
                            return _userDefinedParams.CarDataList.Count(s => recKeys.Contains(s.RecKey)).ToString();
                        }
                        return _userDefinedParams.HotelLookup[mainRec.RecKey].ToList().Count.ToString();
                    case "HTLAVGRATE": // done
                        if (tripSummaryLevel == 1)
                        {
                            if (!CalcTripSummaryField(mainRec.Recloc, column.Name)) return string.Empty;

                            var recKeys = _userDefinedParams.TripDataList.Where(s => s.Recloc.EqualsIgnoreCase(mainRec.Recloc)).Select(s => s.RecKey);
                            var temp = _userDefinedParams.HotelDataList.Where(s => recKeys.Contains(s.RecKey)).ToList();
                            var nights = Math.Abs(temp.Sum(s => s.Nights));
                            if (nights == 0) return string.Empty;

                            var avgRate = temp.Sum(s => Math.Abs(s.Nights) * s.Bookrate / nights);
                            return avgRate.ToString("0.00");
                        }
                        var nightCount = _userDefinedParams.HotelLookup[mainRec.RecKey].Sum(x => x.Nights);
                        if (nightCount != 0)
                        {
                            var avgCost = _userDefinedParams.HotelLookup[mainRec.RecKey].Sum(s => (Math.Abs(s.Rooms) * Math.Abs(s.Nights) * Math.Abs(s.Bookrate)) / nightCount);
                            return string.Format("{0:0.00}", avgCost).PadLeft(12);
                        }
                        return "[null]";


                    case "CARCOUNT": // done
                        if (tripSummaryLevel == 1)
                        {
                            if (!CalcTripSummaryField(mainRec.Recloc, column.Name)) return string.Empty;

                            var recKeys = _userDefinedParams.TripDataList.Where(s => s.Recloc.EqualsIgnoreCase(mainRec.Recloc)).Select(s => s.RecKey);
                            return _userDefinedParams.CarDataList.Count(s => recKeys.Contains(s.RecKey)).ToString();
                        }
                        var carCount = _userDefinedParams.CarLookup[mainRec.RecKey].Count();
                        return carCount.ToString();
                    case "CARAVGRATE": // done
                        if (tripSummaryLevel == 1)
                        {
                            if (!CalcTripSummaryField(mainRec.Recloc, column.Name)) return string.Empty;

                            var recKeys = _userDefinedParams.TripDataList.Where(s => s.Recloc.EqualsIgnoreCase(mainRec.Recloc)).Select(s => s.RecKey);
                            var carDataTemp = _userDefinedParams.CarDataList.Where(s => recKeys.Contains(s.RecKey)).ToList();
                            var days = Math.Abs(carDataTemp.Sum(s => s.Days));
                            if (days == 0) return string.Empty;

                            var avgRate = carDataTemp.Sum(s => Math.Abs(s.Days) * s.Abookrat / days);
                            return string.Format("{0:0.00}", avgRate);
                        }
                        var dayCount = _userDefinedParams.CarLookup[mainRec.RecKey].Sum(x => x.Days);
                        if (dayCount != 0)
                        {
                            var avgCost = _userDefinedParams.CarLookup[mainRec.RecKey].Where(s => s.RecKey == mainRec.RecKey).Sum(s => Math.Abs(s.Days) * Math.Abs(s.Abookrat) / dayCount);
                            return avgCost.ToString().PadLeft(12);
                        }
                        return "[null]";
                    case "FLTCOUNT": // done
                        if (tripSummaryLevel == 1)
                        {
                            if (!CalcTripSummaryField(mainRec.Recloc, column.Name)) return string.Empty;

                            var recKeys = _userDefinedParams.TripDataList.Where(s => s.Recloc.EqualsIgnoreCase(mainRec.Recloc)).Select(s => s.RecKey);
                            return _userDefinedParams.LegDataList.Count(s => recKeys.Contains(s.RecKey)).ToString();
                        }
                        var fltCount = _userDefinedParams.LegLookup[mainRec.RecKey].Count();
                        return fltCount.ToString();
                    case "LOSTAIR": // done
                        return (mainRec.Airchg - mainRec.Offrdchg).ToString();
                    case "SAVEDAIR": // done
                        return (mainRec.Stndchg - mainRec.Airchg).ToString();
                    case "SAVEDMKT": // done
                        return (mainRec.Mktfare - mainRec.Airchg).ToString();
                    case "PRDCARRNAM": // done
                        var aline = LookupFunctions.LookupAirline(_masterStore, mainRec.Predomcarr);
                        return aline.Item2;
                    case "PRDCTYNAME": // done
                        return LookupFunctions.LookupAportCity(mainRec.TrpPrdDest, _masterStore, _isTitleCaseRequired, "A");
                    case "PRDOCTYNAM": // done
                        return LookupFunctions.LookupAportCity(mainRec.TrpPrdOrig, _masterStore, _isTitleCaseRequired, "A");
                    case "POTLNITES": // done
                        if (!mainRec.Tripend.HasValue || !mainRec.Tripstart.HasValue) return string.Empty;
                        var nites = (mainRec.Tripend.Value - mainRec.Tripstart.Value);
                        return (nites.Days < 99999) ? nites.Days.ToString() : "0";
                    case "LEGAIRCODS": // done
                        return LookupFunctions.LookupSegRouteCarriers(_reportLookups.Segs, mainRec.RecKey);
                    case "LEGSVCS": // done
                        return LookupFunctions.LookupSegRouteClasses(_reportLookups.Segs, mainRec.RecKey);
                    case "LEGCLSCATS": // done
                        return LookupFunctions.LookupSegRouteClassCats(_reportLookups.Segs, mainRec.RecKey);
                    case "FAREBASE1": // done
                        return LookupFunctions.LookupSegRouteFbCodes(_reportLookups.Segs, mainRec.RecKey);
                    case "PAXCOUNT": // done
                        if (tripSummaryLevel == 1)
                        {
                            if (!CalcTripSummaryField(mainRec.Recloc, column.Name)) return string.Empty;
                            return _userDefinedParams.TripDataList.Where(s => s.Recloc.EqualsIgnoreCase(mainRec.Recloc)).Sum(s => s.PlusMin).ToString();
                        }
                        return mainRec.PlusMin.ToString();
                    case "CITYPAIRCD": //GSA - City Pair Code Domestic use Airport, Intl use Metro // done
                        if (_globals.Agency.Equals("GSA"))
                        {
                            //return LookupFunctions.LookUpCityPairCode_GSA(mainRec.Trpctypair, mainRec.Domintl, _masterStore);
                            //GSA Enhancement 00170668
                            return GetGSATripCityPairCodeUseMktSegs(mainRec.RecKey, _masterStore);
                        }
                        else
                        {
                            return LookupFunctions.LookUpCityPairCode(mainRec.Trpctypair, mainRec.Domintl, _masterStore);
                        }
                    case "CITYPRDESC": // done
                        //Uses City Pair Code.  Descriptive Name.  
                        //For domestic trips use City Name + Airport Code, for international trips use City Name only. 

                        if (_globals.Agency.Equals("GSA"))
                        {
                            //return LookupFunctions.LookupCityPairDescription_GSA(mainRec.Trpctypair, mainRec.Domintl, _masterStore, _isTitleCaseRequired);
                            //GSA Enhancement 00170668
                            return GetGSATripCityPairDescriptionMktSegs(mainRec.RecKey, _masterStore);
                        }
                        else
                        {
                            return LookupFunctions.LookupCityPairDescription(mainRec.Trpctypair, mainRec.Domintl, _masterStore, _isTitleCaseRequired);
                        }
                    case "INVFQTR": // done
                        //Oct-Dec=1, Jan-Mar=2, Apr-Jun=3, Jul-Sep=4
                        return mainRec.Invdate.GetValueOrDefault().GetDateFiscalQuarter();
                    case "INVFYR": // done
                        //10/1/16-9/30/17 = 2017, 10/1/17-9/30/18=2018, etc
                        return mainRec.Invdate.GetValueOrDefault().GetDateFiscalYear();
                    case "INVFMON": // done
                        //First of Month - Invoice Date (3/18/17 would be 3/1/17)
                        return mainRec.Invdate.GetValueOrDefault().GetDateFirstOfMonth(_globals.DateDisplay);
                    case "TRANSID": // done
                        //reckey/Transaction ID
                        if (!_globals.Agency.Equals("GSA")) return mainRec.RecKey.ToString(CultureInfo.InvariantCulture);
                        if (!Features.GsaTripTransactionIdFeatureFlag.IsEnabled()) return mainRec.RecKey.ToString(CultureInfo.InvariantCulture);
                        return TripDerivedDataLookup.GetTripTransactionId(mainRec.RecKey, _reportLookups);
                    case "FARETYPE": // done
                        if (Features.FareType.IsEnabled())
                        {
                            if (mainRec.PrdFareBas == null) return "Other";
                            return FareTypeHandler.LookupFareType(mainRec.PrdFareBas.Trim());
                        }
                        else
                        {
                            return mainRec.PrdFareBas != null
                                ? _reportLookups.LookupFareType(mainRec.PrdFareBas.Trim())
                                : "Other";
                        }

                    case "CPPTKTINDR": // done
                        //need to check each segment in this trip see if there is segment that matches the specific class
                        var helper = new CppTicketIndicatorHelper();
                        return helper.FindIndicator(mainRec.RecKey, _reportLookups, _userDefinedParams.MarketSegmentDataList);
                    case "ADVBOOKGRP": // done
                        //Difference between Booking Date and Ticket Departure Date Grouped Into 0-2 Days, 3-6 Days, 7-13 Days, 14-20 Days, 21+ Days
                        return LookupFunctions.LookupTwoDateGroup(mainRec.Bookdate.GetValueOrDefault(), mainRec.DepDate.GetValueOrDefault());
                    case "ADVTKTGRP": // done
                        //Difference between Invoice Date and Ticket Departure Date Grouped Into 0-2 Days, 3-6 Days, 7-13 Days, 14-20 Days, 21+ Days
                        return LookupFunctions.LookupTwoDateGroup(mainRec.Invdate.GetValueOrDefault(), mainRec.DepDate.GetValueOrDefault());
                    case "TKTUSED": // done
                        //# of Tickets (Refund = -1, Exchange = 0, Regular = 1) 
                        return LookupHelpers.GetNumberOfTickets(mainRec.Trantype.Trim(), mainRec.Exchange);
                    case "BOBASEFARE": //done
                        //Base Fare Breakout of the Segment Before Taxes
                        return mainRec.Basefare.ToStringSafe();
                    case "BOSFARETAX": // done
                        //Taxes of the Segment
                        return LookupHelpers.GetTotalTaxes(mainRec).ToString(CultureInfo.InvariantCulture);
                    case "IATAORG": // done
                        //not working return LookupFunctions.LookupRegionName(mainRec.Origin, _masterStore);
                        return LookupFunctions.LookupRegion(LookupFunctions.LookupSegRouteFirstOrigin(_reportLookups.Segs, mainRec.RecKey), mainRec.Valcarr, _masterStore, _isTitleCaseRequired);
                    case "IATADEST": // done
                        return LookupFunctions.LookupRegion(LookupFunctions.LookupSegRouteFirstDestination(_reportLookups.Segs, mainRec.RecKey), mainRec.Valcarr, _masterStore, _isTitleCaseRequired);
                    case "VALCARIATA": // done
                        return LookupFunctions.LookupAlineNbr(_masterStore, mainRec.Valcarr);
                    case "PDFARETF": // done
                        //Paid Fare Including Taxes and Fees
                        return LookupHelpers.GetPaidFare(mainRec);
                    case "ORGDETCNTY": // done
                        //Alphabetic:  Derived from Origin Country to Destination Country (all France to United States trips and United States to France trips would show as France-United States)
                        return LookupFunctions.LookupCityPairCountryToCountry(mainRec.Trpctypair, _globals, _masterStore);
                    case "ORGDESREG": // done
                        //Alphabetic:  Derived from Origin Continent to Destination Continent (all Europe to United States trips and United States to Europe trips would show as Europe-United States)
                        return LookupFunctions.LookupCityPairRegionToRegion(mainRec.Trpctypair, _masterStore, _isTitleCaseRequired);
                    case "LGTPAIR": // done
                        //TODO code may need to change - new lookup function for Bi Directional O&D Code (based on longest segment)
                        return FindLongestCityPair(mainRec.RecKey);
                    case "LGTPAIRDES": // done
                        //TODO code may need to change - new lookup function for Bi Directional O&D Description (based on longest segment)
                        if (_globals.Agency.Equals("GSA"))
                        {
                            return LookupFunctions.LookupCityPair_GSA(FindLongestCityPair(mainRec.RecKey), "A", "CITYDESC", _masterStore, _isTitleCaseRequired);
                        }
                        else
                        {
                            return LookupFunctions.LookupCityPair(FindLongestCityPair(mainRec.RecKey), "A", "CITYDESC", _masterStore, _isTitleCaseRequired);
                        }
                    case "REFUNDIND": // done
                        return mainRec.Trantype.Trim() == "C" ? "Y" : "N";
                    case "SEGNO": // done
                        return mainRec.PlusMin.ToString();
                    case "REFUNDABLE":
                        return mainRec.Refundable.Trim() == "0" || mainRec.Refundable.Trim() == "" ? "U" : mainRec.Refundable.Trim();
                    default: // done
                        return ReportBuilder.GetValueAsString(mainRec, column.Name);
                }
            }
            catch (Exception e)
            {
                if (e.InnerException != null) ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                throw;
            }

        }

        public string FindLongestCityPair(int reckey)
        {
            if (_userDefinedParams.MarketSegmentDataList.Count == 0) return string.Empty;

            var rec = _userDefinedParams.MarketSegmentLookup[reckey].OrderByDescending(x => x.Miles)
                .FirstOrDefault();
            return (rec != null) ? rec.Mktsegboth.Trim() : string.Empty;
        }

        private string GetGSATripCityPairCodeUseMktSegs(int reckey, IMasterDataStore dataStore)
        {
            var cityPairSeg = _userDefinedParams.MarketSegmentDataList.FirstOrDefault(x => x.RecKey == reckey && x.Segnum == 1);
            if (cityPairSeg != null)
            {
                return LookupFunctions.LookUpCityPairCode_GSA(cityPairSeg.Mktsegboth, cityPairSeg.DitCode, dataStore);
            }
            return "";
        }

        private string GetGSATripCityPairDescriptionMktSegs(int reckey, IMasterDataStore store)
        {
            var cityPairSeg = _userDefinedParams.MarketSegmentDataList.FirstOrDefault(x => x.RecKey == reckey && x.Segnum == 1);
            if (cityPairSeg == null) return "";

            var cityPair = LookupFunctions.LookUpCityPairCode_GSA(cityPairSeg.Mktsegboth, cityPairSeg.DitCode, store);

            return GetGSACityPairDescription(cityPair, cityPairSeg.DitCode, store);
        }

        private string GetGSACityPairDescription(string cityPair, string domintl, IMasterDataStore store)
        {
            var splitValues = cityPair.Trim().Split('-');
            var origin = (splitValues[0].CompareTo(splitValues[1]) < 0) ? splitValues[0] : splitValues[1];
            var destination = (splitValues[0].CompareTo(splitValues[1]) < 0) ? splitValues[1] : splitValues[0];

            var originName = LookupFunctions.LookupAportCity(origin, store, true, "A").Trim();
            var originCountryCode = LookupFunctions.LookupCountryCode(origin, store);
            var destinationCountryCode = LookupFunctions.LookupCountryCode(destination, store);
            var appendOriginDestToOriginDestName = (domintl.Trim() == "D" ||
                                                    (originCountryCode.EqualsIgnoreCase("US") &&
                                                     destinationCountryCode.EqualsIgnoreCase("US")));

            if (appendOriginDestToOriginDestName) originName += " (" + origin.Trim() + ")";

            var destinationName = LookupFunctions.LookupAportCity(destination, store, true, "A").Trim();

            if (appendOriginDestToOriginDestName) destinationName += " (" + destination.Trim() + ")";

            return string.Format($"{originName}-{destinationName}");
        }

        private bool CalcTripSummaryField(string recloc, string colName)
        {
            if (TripSummaryLevel.Any(s => s.Item1.EqualsIgnoreCase(recloc) && s.Item2.EqualsIgnoreCase(colName))) return false;

            TripSummaryLevel.Add(new Tuple<string, string>(recloc, colName));
            return true;
        }

    }
}

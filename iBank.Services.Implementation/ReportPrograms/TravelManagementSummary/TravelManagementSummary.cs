using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;

using Domain.Helper;
using Domain.Models.ReportPrograms.TravelManagementSummary;
using Domain.Orm.iBankClientQueries;

using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;
using iBank.Services.Implementation.ReportPrograms.TravelManagementSummary.DataSqlScripts;
using iBank.Services.Implementation.Shared.CarbonCalculations;
using iBank.Server.Utilities.Logging;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.TravelManagementSummary
{
    public class TravelManagementSummary : ReportRunner<RawData, FinalData>
    {

        private int _year;
        private int _fiscalMonth;
        private bool _isPreview;
        private string _subTitle;
        private MonthInfo[] _months;

        private List<LegRawData> _legRawDataList = new List<LegRawData>();
        private List<CarRawData> _carRawDataList = new List<CarRawData>();
        private List<HotelRawData> _hotelRawDataList = new List<HotelRawData>();
        private List<SvcFeeRawData> _svcFeeRawDataList = new List<SvcFeeRawData>();

        private List<AirDataGrouped> _airDataByMonth = new List<AirDataGrouped>();
        private List<RailDataGrouped> _railDataByMonth = new List<RailDataGrouped>();
        private List<LegDataGrouped> _legDataByMonth = new List<LegDataGrouped>();
        private List<LegDataGrouped> _railLegDataByMonth = new List<LegDataGrouped>();
        private List<HotelDataGrouped> _hotelDataByMonth = new List<HotelDataGrouped>();
        private List<CarDataGrouped> _carDataByMonth = new List<CarDataGrouped>();
        private List<ServiceFeeDataGrouped> _svcFeeDataByMonth = new List<ServiceFeeDataGrouped>();

        private readonly List<string> _exportFields = new List<string>
        { "Category", "RowDesc","Mth1","Mth2","Mth3","Mth4","Mth5","Mth6","Mth7","Mth8","Mth9","Mth10","Mth11","Mth12","YTD"};


        public TravelManagementSummary()
        {
            CrystalReportName = "ibTravelMgmt";
        }

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;
            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            _year = Globals.GetParmValue(WhereCriteria.STARTYEAR).TryIntParse(-1);
            _fiscalMonth = Globals.GetParmValue(WhereCriteria.TXTFYSTARTMTH).MonthNumberFromName();
            if (!_fiscalMonth.IsBetween(1, 12))
                _fiscalMonth = 1;

            if (!_year.IsBetween(1998, 2025))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.SpecifyMonthYear;
                return false;
            }

            if (!IsOnlineReport()) return false;

            //don't run online without an acct. 
            if (!HasAccount()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            var beginDate = new DateTime(_year, _fiscalMonth, 1);
            var endDate = beginDate.AddMonths(12).AddSeconds(-1);
            var testDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddSeconds(-1);
            endDate = endDate < testDate ? endDate : testDate;

            if (Globals.IsParmValueOn(WhereCriteria.CBINCLUDEDATASUBSEQ))
            {
                testDate = beginDate.AddMonths(12).AddDays(-1);
                if (testDate > endDate)
                    endDate = testDate;
            }

            Globals.BeginDate = beginDate;
            Globals.EndDate = endDate;

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false))
                return false;

            if (Features.AdvancedParameterAcctPicklistCheck.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            
            BuildWhere.AddSecurityChecks();

            var originalWhere = BuildWhere.WhereClauseAdvanced != ""
                ? BuildWhere.WhereClauseFull + " and " + BuildWhere.WhereClauseAdvanced
                : BuildWhere.WhereClauseFull;

            var dateToUse = "depdate";
            _subTitle = LookupFunctions.LookupLanguageTranslation("xBasedTripDepDate", "Based on Trip Departure Date",
                Globals.LanguageVariables);
            switch (Globals.GetParmValue(WhereCriteria.DATERANGE))
            {
                case "2":
                    dateToUse = "invdate";
                    _subTitle = LookupFunctions.LookupLanguageTranslation("xBasedInvDate", "Based on Invoice Date",
                        Globals.LanguageVariables);
                    break;
                case "3":
                    dateToUse = "bookdate";
                    _subTitle = LookupFunctions.LookupLanguageTranslation("xBasedBookDate", "Based on Booked Date",
                        Globals.LanguageVariables);
                    break;
            }

            _isPreview = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");

            var orphanSvcFees = Globals.IsParmValueOn(WhereCriteria.CBINCLSVCFEENOMATCH);

            if (!Globals.AgencyInformation.UseServiceFees)
            {
                orphanSvcFees = false;
            }
            
            var udidExists = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1) > 0;
            var airSqlScript = new AirSqlScript();
            var legSqlScript = new LegSqlScript();
            var carSqlScript = new CarSqlScript();
            var hotelSqlScript = new HotelSqlScript();
            var svcFeeSqlScript = new ServiceFeeSqlScript();

            var sql = airSqlScript.GetSqlScript(dateToUse, udidExists, _isPreview, originalWhere);
            var dataLoader = new DataLoader(_isPreview, BuildWhere, Globals);
            RawDataList = dataLoader.GetAirData(sql, false);
                       
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            PerformCurrencyConversion(RawDataList);

            sql = legSqlScript.GetSqlScript(dateToUse, udidExists, _isPreview, originalWhere);
            _legRawDataList = dataLoader.GetLegData(sql, true);
            PerformCurrencyConversion(_legRawDataList);

            sql = carSqlScript.GetSqlScript(dateToUse, udidExists, _isPreview, originalWhere);
            _carRawDataList = dataLoader.GetCarData(sql, false);
             PerformCurrencyConversion(_carRawDataList);

            sql = hotelSqlScript.GetSqlScript(dateToUse, udidExists, _isPreview, originalWhere);
            _hotelRawDataList = dataLoader.GetHotelData(sql, false);
            PerformCurrencyConversion(_hotelRawDataList);

            var excludeSvcFees = _isPreview || Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVCFEES);
            if (!_isPreview && !excludeSvcFees)
            {
                sql = svcFeeSqlScript.GetSqlScript(dateToUse, udidExists, originalWhere, orphanSvcFees);
                _svcFeeRawDataList = dataLoader.GetServiceFeeData(sql, false);
                PerformCurrencyConversion(_svcFeeRawDataList);

                _svcFeeRawDataList =
                    _svcFeeRawDataList.GroupBy(s => s.UseDate, (key, recs) =>
                    {
                        var reclist = recs as IList<SvcFeeRawData> ?? recs.ToList();
                        return new SvcFeeRawData
                        {
                            UseDate = key,
                            SvcAmt = reclist.Sum(s => s.SvcAmt),
                            SvcFeeCount = reclist.Sum(s => s.SvcAmt < 0 ? -1 : 1)
                        };
                    }).ToList();

            }

            if (!RawDataList.Any() && !_carRawDataList.Any() && !_hotelRawDataList.Any() && !_svcFeeRawDataList.Any())
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_NoData;

                //update ipRptLog record
                var logger = new ReportLogLogger();
                logger.UpdateLog(Globals.ReportLogKey, ReportLogLogger.ReportStatus.NODATA);

                return false;
            }

            return true;

        }

        public override bool ProcessData()
        {
            if (Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE))
            {
                AirMileageCalculator<LegRawData>.CalculateAirMileageFromTable(_legRawDataList); 
            }
            foreach (var leg in _legRawDataList)
            {
                leg.Miles = leg.Miles*leg.Plusmin;
            }

            var doCarbonCalcs = Globals.IsParmValueOn(WhereCriteria.CARBONEMISSIONS);
            var useMetric = Globals.IsParmValueOn(WhereCriteria.METRIC);

            if (doCarbonCalcs)
            {
                var carbCalc = Globals.GetParmValue(WhereCriteria.CARBONCALC);
                var carbonCalc = new CarbonCalculator();
                carbonCalc.SetAirCarbon(_legRawDataList, useMetric, carbCalc);

                carbonCalc.SetCarCarbon(_carRawDataList, useMetric, _isPreview);

                carbonCalc.SetHotelCarbon(_hotelRawDataList, useMetric, _isPreview);
            }

            if (useMetric) MetricImperialConverter.ConvertMilesToKilometers(_legRawDataList);

            if (Globals.IsParmValueOn(WhereCriteria.CBUSEBASEFARE))
            {
                foreach (var row in RawDataList.Where(s => s.Basefare == 0))
                {
                    row.Basefare = row.Airchg; // Apparantly sometimes the data is "bad". 
                }

                RawDataList = RawDataList.Select(s => new RawData
                {
                    Plusmin = s.Plusmin,
                    Airchg = s.Airchg,
                    Basefare = s.Basefare,
                    Domintl = s.Domintl,
                    Exchange = s.Exchange,
                    Bktool = s.Bktool,
                    Valcarr = s.Valcarr,
                    ValcarMode = s.ValcarMode,
                    UseDate = s.UseDate,
                    Stndchg =
                        (Math.Abs(s.Stndchg) < Math.Abs(s.Basefare) || s.Stndchg == 0 ||
                         (s.Stndchg < 0 && s.Basefare > 0))
                            ? s.Basefare
                            : s.Stndchg,
                    Offrdchg =
                        (s.Offrdchg > 0 && s.Basefare < 0) ? s.Offrdchg : (s.Offrdchg == 0) ? s.Basefare : s.Offrdchg,
                    Mktfare = s.Mktfare == 0 ? s.Basefare : s.Mktfare
                }).ToList();

                foreach (var row in RawDataList)
                {
                    row.LostAmt = row.Basefare - row.Offrdchg;
                    row.Savings = row.Stndchg - row.Basefare;
                }
            }
            else
            {
                RawDataList = RawDataList.Select(s => new RawData
                {
                    Plusmin = s.Plusmin,
                    Airchg = s.Airchg,
                    Basefare = s.Basefare,
                    Domintl = s.Domintl,
                    Exchange = s.Exchange,
                    Bktool = s.Bktool,
                    Valcarr = s.Valcarr,
                    ValcarMode = s.ValcarMode,
                    UseDate = s.UseDate,
                    Stndchg =
                        (Math.Abs(s.Stndchg) < Math.Abs(s.Airchg) || s.Stndchg == 0 || (s.Stndchg < 0 && s.Airchg > 0))
                            ? s.Airchg
                            : s.Stndchg,
                    Offrdchg = (s.Offrdchg > 0 && s.Airchg < 0) ? s.Offrdchg : (s.Offrdchg == 0) ? s.Airchg : s.Offrdchg,
                    Mktfare = s.Mktfare == 0 ? s.Airchg : s.Mktfare
                }).ToList();

                foreach (var row in RawDataList)
                {
                    row.LostAmt = row.Airchg - row.Offrdchg;
                    row.Savings = row.Stndchg - row.Airchg;
                }
            }

            foreach (
                var row in RawDataList.Where(s => (s.LostAmt < 0 && s.Plusmin > 0) || (s.LostAmt > 0 && s.Plusmin < 0)))
            {
                row.NegotiatedSavings = 0 - row.LostAmt;
                row.LostAmt = 0;
            }

            var tempQuery = RawDataList.AsQueryable();
            var railInfo = Globals.IsParmValueOn(WhereCriteria.CBSEPARATERAIL);
            //need the list of airlines that are railroads. 
            var raillines = LookupFunctions.GetAirlines(MasterStore, "R");
            if (railInfo)
            {

                tempQuery = !_isPreview
                    ? tempQuery.Where(s => !s.ValcarMode.EqualsIgnoreCase("R"))
                    : tempQuery.Where(s => !s.ValcarMode.EqualsIgnoreCase("R") && !raillines.Contains(s.Valcarr.Trim()));
            }

            var tempList = tempQuery.ToList();
            _airDataByMonth = tempList.OrderBy(s => s.UseDate).GroupBy(s => s.UseDate.GetValueOrDefault().Month, (month, recs) =>
            {
                var reclist = recs.ToList();
                return new AirDataGrouped
                {
                    MonthNum = month,
                    GrossAir = reclist.Sum(s => s.Plusmin == 1 ? s.Airchg : 0),
                    RefundAmt = reclist.Sum(s => s.Plusmin == -1 ? s.Airchg : 0),
                    NetAir = reclist.Sum(s => s.Airchg),
                    Invoices = reclist.Sum(s => s.Plusmin == 1 ? 1 : 0),
                    Refunds = reclist.Sum(s => s.Plusmin == -1 ? 1 : 0),
                    NetTrans = reclist.Sum(s => s.Plusmin),
                    OnlineTkts = reclist.Sum(s => s.Bktool.EqualsIgnoreCase("ONLINE") ? s.Plusmin : 0),
                    OnlineAmt = reclist.Sum(s => s.Bktool.EqualsIgnoreCase("ONLINE") ? s.Airchg : 0),
                    DomAir = reclist.Sum(s => s.Domintl.EqualsIgnoreCase("D") ? s.Airchg : 0),
                    IntlAir = reclist.Sum(s => !s.Domintl.EqualsIgnoreCase("D") ? s.Airchg : 0),
                    DomTrans = reclist.Sum(s => s.Domintl.EqualsIgnoreCase("D") ? s.Plusmin : 0),
                    IntlTrans = reclist.Sum(s => !s.Domintl.EqualsIgnoreCase("D") ? s.Plusmin : 0),
                    FullFare = reclist.Sum(s => s.Stndchg),
                    Savings = reclist.Sum(s => s.Savings),
                    LowFare = reclist.Sum(s => s.Offrdchg),
                    LostAmt = reclist.Sum(s => s.LostAmt),
                    NegoSvngs = reclist.Sum(s => s.NegotiatedSavings),
                    Exchanges = reclist.Sum(s => s.Exchange ? 1 : 0),
                    ExchngAmt = reclist.Sum(s => s.Exchange ? s.Airchg : 0)
                };
            }).ToList();

            if (railInfo)
            {
                _railDataByMonth = !_isPreview
                    ? RawDataList.Where(s => s.ValcarMode.EqualsIgnoreCase("R"))
                        .GroupBy(s => s.UseDate.GetValueOrDefault().Month, (month, recs) =>
                        {
                            var reclist = recs.ToList();
                            return new RailDataGrouped
                            {
                                MonthNum = month,
                                GrossRail = reclist.Sum(s => s.Plusmin == 1 ? s.Airchg : 0),
                                NetRail = reclist.Sum(s => s.Airchg),
                                RefundAmt = reclist.Sum(s => s.Plusmin == -1 ? s.Airchg : 0),
                                Refunds = reclist.Sum(s => s.Plusmin == -1 ? 1 : 0),
                                Invoices = reclist.Sum(s => s.Plusmin == 1 ? 1 : 0),
                                NetTrans = reclist.Sum(s => s.Plusmin),
                                FullFare = reclist.Sum(s => s.Stndchg),
                                Savings = reclist.Sum(s => s.Savings),
                                LowFare = reclist.Sum(s => s.Offrdchg),
                                LostAmt = reclist.Sum(s => s.LostAmt),
                                NegoSvngs = reclist.Sum(s => s.NegotiatedSavings),
                            };

                        }).ToList()
                    : RawDataList.Where(s => s.ValcarMode.EqualsIgnoreCase("R") || raillines.Contains(s.Valcarr.Trim()))
                        .GroupBy(s => s.UseDate.GetValueOrDefault().Month, (month, recs) =>
                        {
                            var reclist = recs.ToList();
                            return new RailDataGrouped
                            {
                                MonthNum = month,
                                GrossRail = reclist.Sum(s => s.Plusmin == 1 ? s.Airchg : 0),
                                NetRail = reclist.Sum(s => s.Airchg),
                                RefundAmt = reclist.Sum(s => s.Plusmin == -1 ? s.Airchg : 0),
                                Refunds = reclist.Sum(s => s.Plusmin == -1 ? 1 : 0),
                                Invoices = reclist.Sum(s => s.Plusmin == 1 ? 1 : 0),
                                NetTrans = reclist.Sum(s => s.Plusmin),
                                FullFare = reclist.Sum(s => s.Stndchg),
                                Savings = reclist.Sum(s => s.Savings),
                                LowFare = reclist.Sum(s => s.Offrdchg),
                                LostAmt = reclist.Sum(s => s.LostAmt),
                                NegoSvngs = reclist.Sum(s => s.NegotiatedSavings),
                            };

                        }).ToList();


                _legDataByMonth = GetLegsByMonth(doCarbonCalcs, "A");

                _railLegDataByMonth = GetLegsByMonth(doCarbonCalcs, "R");

            }
            else
            {
                _legDataByMonth = GetLegsByMonth(doCarbonCalcs);
            }

            GenerateMonths();
            var xAirlines = LookupFunctions.LookupLanguageTranslation("xAirlines", "Airlines", Globals.LanguageVariables);
            var xImpactOfExchanges = LookupFunctions.LookupLanguageTranslation("xImpactOfExchanges", "Impact of Changes",
                Globals.LanguageVariables) + " **";
            var xRail = LookupFunctions.LookupLanguageTranslation("xRail", "Rail", Globals.LanguageVariables);
            var xHotelBookings = LookupFunctions.LookupLanguageTranslation("xHotelBookings", "Hotel Bookings",
                Globals.LanguageVariables);
            var xServiceFees = LookupFunctions.LookupLanguageTranslation("xServiceFees", "Service Fees",
                Globals.LanguageVariables);
            var xCarRentalBkngs = LookupFunctions.LookupLanguageTranslation("xCarRentalBkngs", "Car Rental Bookings",
                Globals.LanguageVariables);
            var xTotalCo2Emissions = LookupFunctions.LookupLanguageTranslation("xTotalCO2Emissions",
                "Total CO2 Emissions", Globals.LanguageVariables);
            var xAvgCo2Emissions = LookupFunctions.LookupLanguageTranslation("xAvgCO2Emissions", "Avg CO2 Emissions",
                Globals.LanguageVariables);
            var totalMilesKms = useMetric
                ? LookupFunctions.LookupLanguageTranslation("xTotalKms", "Total Kilometers", Globals.LanguageVariables)
                : LookupFunctions.LookupLanguageTranslation("xTotalMiles", "Total Miles", Globals.LanguageVariables);
            var poundsKilos = useMetric
                ? "Kgs"
                : "Lbs.";
            var intlMilesKms = useMetric
                ? LookupFunctions.LookupLanguageTranslation("xIntlTotKms", "Int'l Total Kilometers",
                    Globals.LanguageVariables)
                : LookupFunctions.LookupLanguageTranslation("xIntlTotMiles", "Int'l Total Miles",
                    Globals.LanguageVariables);
            var intlCostPerMilesKms = useMetric
                ? LookupFunctions.LookupLanguageTranslation("xIntlCostPerKm", "Int'l Cost per Km",
                    Globals.LanguageVariables)
                : LookupFunctions.LookupLanguageTranslation("xIntlCostPerMile", "Int'l Cost per Mile",
                    Globals.LanguageVariables);
            var domMilesKms = useMetric
                ? LookupFunctions.LookupLanguageTranslation("xDomTotKms", "Domestic Total Kilometers",
                    Globals.LanguageVariables)
                : LookupFunctions.LookupLanguageTranslation("xDomTotMiles", "Domestic Total Miles",
                    Globals.LanguageVariables);
            var domCostPerMilesKms = useMetric
                ? LookupFunctions.LookupLanguageTranslation("xDomCostPerKm", "Domestic Cost per Km",
                    Globals.LanguageVariables)
                : LookupFunctions.LookupLanguageTranslation("xDomCostPerMile", "Domestic Cost per Mile",
                    Globals.LanguageVariables);
            var avgCostMileKm = useMetric
                ? LookupFunctions.LookupLanguageTranslation("xAvgCostPerKm", "Avg Cost per Km",
                    Globals.LanguageVariables)
                : LookupFunctions.LookupLanguageTranslation("xAvgCostPerMile", "Avg Cost Per Mile",
                    Globals.LanguageVariables);


            var excludeNegoSavings = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDENEGOT);
            var excludeSavings = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVGS);
            var excludeExceptions = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDEEXCEPTNS);
            var excludeExchange = Globals.IsParmValueOn(WhereCriteria.CBEXCLEXCHINFO);

            if (!_isPreview)
            {
                AddRow(xAirlines, "A", 10,
                    LookupFunctions.LookupLanguageTranslation("xGrossAirVolume", "Gross Air Volume",
                        Globals.LanguageVariables), "DOLL", "GrossAir", RowType.Air);
                AddRow(xAirlines, "A", 20,
                    LookupFunctions.LookupLanguageTranslation("xRefundAirVolume", "Refund Air Volume",
                        Globals.LanguageVariables), "DOLL", "RefundAmt", RowType.Air);
                AddRow(xAirlines, "A", 30,
                    LookupFunctions.LookupLanguageTranslation("xNetAirVolume", "Net Air Volume",
                        Globals.LanguageVariables), "DOLL", "NetAir", RowType.Air);
                AddRow(xAirlines, "A", 40,
                    LookupFunctions.LookupLanguageTranslation("xRefundVolumePct", "Refund Volume %",
                        Globals.LanguageVariables), "PCNT", "REFVOLPCNT", RowType.Air);
                AddRow(xAirlines, "A", 50,
                    LookupFunctions.LookupLanguageTranslation("xNbrOfInvoices", "# of Invoices",
                        Globals.LanguageVariables), "INT", "Invoices", RowType.Air);
                AddRow(xAirlines, "A", 60,
                    LookupFunctions.LookupLanguageTranslation("xNbrOfRefunds", "# of Refunds", Globals.LanguageVariables),
                    "INT", "Refunds", RowType.Air);
                AddRow(xAirlines, "A", 70,
                    LookupFunctions.LookupLanguageTranslation("xNetTrans", "Net Transactions", Globals.LanguageVariables),
                    "INT", "NetTrans", RowType.Air);
                AddRow(xAirlines, "A", 80,
                    LookupFunctions.LookupLanguageTranslation("xRefundTransPct", "Refund Trans %",
                        Globals.LanguageVariables), "PCNT", "REFTRANPCNT", RowType.Air);


                AddLegRow(xAirlines, "A", 90, totalMilesKms, "INT", "TOTMILES");
                if (doCarbonCalcs)
                {
                    AddLegRow(xAirlines, "A", 92, xTotalCo2Emissions + "(" + poundsKilos + ")", "INT", "TOTAIRCO2");
                }

                if (!Globals.IsParmValueOn(WhereCriteria.CBEXCLUDEONLINEADOPT))
                {
                    AddRow(xAirlines, "A", 110,
                        LookupFunctions.LookupLanguageTranslation("xNbrTktsBookedOnline", "# Tickets Booked Online",
                            Globals.LanguageVariables), "INT", "ONLINETKTS", RowType.Air);
                    AddRow(xAirlines, "A", 120,
                        LookupFunctions.LookupLanguageTranslation("xVolumeBookedOnline", "Volume Booked Online",
                            Globals.LanguageVariables), "DOLL", "ONLINEAMT", RowType.Air);
                    AddRow(xAirlines, "A", 130,
                        LookupFunctions.LookupLanguageTranslation("xPctTktsBookedOnline", "% Tickets Booked Online",
                            Globals.LanguageVariables), "PCNT", "ONLINEPCT", RowType.Air);
                }

                AddRow(xAirlines, "B", 150,
                    LookupFunctions.LookupLanguageTranslation("xAvgGrosssAmt", "Average Gross Amt",
                        Globals.LanguageVariables), "AVG", "AVGGROSSAIR", RowType.Air);
                AddRow(xAirlines, "B", 160,
                    LookupFunctions.LookupLanguageTranslation("xAvgRefundAmt", "Average Refund Amt",
                        Globals.LanguageVariables), "AVG", "AVGREFAIR", RowType.Air);
                AddRow(xAirlines, "B", 170,
                    LookupFunctions.LookupLanguageTranslation("xAvgNetAmt", "Average Net Amt", Globals.LanguageVariables),
                    "AVG", "AVGNETAIR", RowType.Air);
                AddRow(xAirlines, "B", 190, avgCostMileKm, "CENT", "AVGMILECOST", RowType.Air);
                if (doCarbonCalcs)
                {
                    AddRow(xAirlines, "B", 192, xAvgCo2Emissions + "(" + poundsKilos + ")", "AVG", "AVGCO2", RowType.Air);
                }
            }
            else
            {
                AddRow(xAirlines, "A", 30,
                    LookupFunctions.LookupLanguageTranslation("xAirVolBooked", "Air Volume Booked",
                        Globals.LanguageVariables), "DOLL", "NetAir", RowType.Air);
                AddRow(xAirlines, "A", 70,
                    LookupFunctions.LookupLanguageTranslation("xNbrTrans", "# of Transactions",
                        Globals.LanguageVariables), "INT", "NetTrans", RowType.Air);
                AddLegRow(xAirlines, "A", 90, totalMilesKms, "INT", "TOTMILES");
                if (doCarbonCalcs)
                {
                    AddLegRow(xAirlines, "A", 92, xTotalCo2Emissions + "(" + poundsKilos + ")", "INT", "TOTAIRCO2");
                }
                AddRow(xAirlines, "B", 170,
                    LookupFunctions.LookupLanguageTranslation("xAvgTicketPrice", "Average Ticket Price",
                        Globals.LanguageVariables), "AVG", "AVGNETAIR", RowType.Air);
                AddRow(xAirlines, "B", 190, avgCostMileKm, "CENT", "AVGMILECOST", RowType.Air);
                if (doCarbonCalcs)
                {
                    AddRow(xAirlines, "B", 192, xAvgCo2Emissions + "(" + poundsKilos + ")", "AVG", "AVGCO2", RowType.Air);
                }
            }

            AddRow(xAirlines, "C", 210,
                LookupFunctions.LookupLanguageTranslation("xIntlAirVolume", "Int'l Air Volume",
                    Globals.LanguageVariables), "DOLL", "IntlAir", RowType.Air);
            AddRow(xAirlines, "C", 220,
                LookupFunctions.LookupLanguageTranslation("xIntlAirVolPct", "Int'l Air Vol %", Globals.LanguageVariables),
                "PCNT", "INTLPCNT", RowType.Air);
            AddRow(xAirlines, "C", 230,
                LookupFunctions.LookupLanguageTranslation("xIntlAirTkts", "Int'l Air Tickets", Globals.LanguageVariables),
                "INT", "IntlTrans", RowType.Air);
            AddRow(xAirlines, "C", 250,
                LookupFunctions.LookupLanguageTranslation("xIntlAvgNetAmt", "Int'l Avg Net Amt",
                    Globals.LanguageVariables), "AVG", "AVGINTLTKT", RowType.Air);
            AddLegRow(xAirlines, "C", 270, intlMilesKms, "INT", "INTLMILES");
            AddLegRow(xAirlines, "C", 280, intlCostPerMilesKms, "CENT", "INTLCPM");

            if (doCarbonCalcs)
            {
                AddLegRow(xAirlines, "C", 1272, "Int'l CO2 Emissions (" + poundsKilos + ")", "INT", "INTLAIRCO2");
                AddRow(xAirlines, "C", 290,
                    LookupFunctions.LookupLanguageTranslation("xIntlAvgCO2", "Avg Int'l CO2", Globals.LanguageVariables) +
                    " (" + poundsKilos + ")", "AVG", "INTLAVGCO2",RowType.Air);
            }

            AddRow(xAirlines, "D", 310,
                LookupFunctions.LookupLanguageTranslation("xDomAirVolume", "Domestic Air Volume",
                    Globals.LanguageVariables), "DOLL", "DomAir", RowType.Air);
            AddRow(xAirlines, "D", 320,
                LookupFunctions.LookupLanguageTranslation("xDomAirVolPct", "Domestic Air Vol %",
                    Globals.LanguageVariables), "PCNT", "DOMPCNT", RowType.Air);
            AddRow(xAirlines, "D", 330,
                LookupFunctions.LookupLanguageTranslation("xDomAirTkts", "Domestic Air Tickets",
                    Globals.LanguageVariables), "INT", "DomTrans", RowType.Air);
            AddRow(xAirlines, "D", 350,
                LookupFunctions.LookupLanguageTranslation("xDomAvgNetAmt", "Domestic Avg Net Amt",
                    Globals.LanguageVariables), "AVG", "AVGDOMTKT", RowType.Air);

            AddLegRow(xAirlines, "D", 370, domMilesKms, "INT", "DOMMILES");
            AddLegRow(xAirlines, "D", 380, domCostPerMilesKms, "CENT", "DOMCPM");

            if (doCarbonCalcs)
            {
                AddLegRow(xAirlines, "D", 1372, "Domestic CO2 Emissions (" + poundsKilos + ")", "INT", "DOMAIRCO2");
                AddRow(xAirlines, "D", 390,
                    LookupFunctions.LookupLanguageTranslation("xDomAvgCO2", "Avg Domestic CO2",
                        Globals.LanguageVariables) + " (" + poundsKilos + ")", "AVG", "DOMAVGCO2",RowType.Air);
            }

            if (!excludeSavings)
            {
                AddRow(xAirlines, "E", 410,
                    LookupFunctions.LookupLanguageTranslation("xFullFareVol", "Full Fare Volume",
                        Globals.LanguageVariables), "DOLL", "FullFare", RowType.Air);
                AddRow(xAirlines, "E", 420,
                    LookupFunctions.LookupLanguageTranslation("xSavingsVol", "Savings Volume", Globals.LanguageVariables),
                    "DOLL", "Savings", RowType.Air);
                AddRow(xAirlines, "E", 430,
                    LookupFunctions.LookupLanguageTranslation("xSavingsPct", "Savings %", Globals.LanguageVariables),
                    "PCNT", "SVNGSPCNT", RowType.Air);
            }


            if (!excludeExceptions || !excludeNegoSavings)
            {
                AddRow(xAirlines, "E", 440,
                    LookupFunctions.LookupLanguageTranslation("xLowFareVol", "Low Fare Volume",
                        Globals.LanguageVariables), "DOLL", "LowFare", RowType.Air);
            }

            if (!excludeExceptions)
            {
                AddRow(xAirlines, "E", 450,
                    LookupFunctions.LookupLanguageTranslation("xMissedSvgsVol", "Missed Savings Volume",
                        Globals.LanguageVariables), "DOLL", "LostAmt", RowType.Air);
                AddRow(xAirlines, "E", 460,
                    LookupFunctions.LookupLanguageTranslation("xMissedSvgsPct", "Missed Savings %",
                        Globals.LanguageVariables), "PCNT", "LOSTPCNT", RowType.Air);
            }

            if (!excludeNegoSavings)
            {
                AddRow(xAirlines, "E", 470,
                    LookupFunctions.LookupLanguageTranslation("xNegoSvgsVol", "Negotiated Svgs Volume",
                        Globals.LanguageVariables), "DOLL", "NegoSvngs", RowType.Air);
                AddRow(xAirlines, "E", 480,
                    LookupFunctions.LookupLanguageTranslation("xNegoSvgsPct", "Negotiated Svgs %",
                        Globals.LanguageVariables), "PCNT", "NEGOPCNT", RowType.Air);
            }

            if (!excludeExchange)
            {
                AddRow(xImpactOfExchanges, "F", 610,
                    LookupFunctions.LookupLanguageTranslation("xNbrOfExchanges", "# of Exchanges",
                        Globals.LanguageVariables), "INT", "Exchanges", RowType.Air);
                AddRow(xImpactOfExchanges, "F", 620,
                    LookupFunctions.LookupLanguageTranslation("xTotCostToExch", "Total Cost to Exchange",
                        Globals.LanguageVariables), "DOLL", "ExchngAmt", RowType.Air);
                AddRow(xImpactOfExchanges, "F", 630,
                    LookupFunctions.LookupLanguageTranslation("xAvgCostToExch", "Avg Cost to Exchange",
                        Globals.LanguageVariables), "AVG", "EXCHANGEAVG", RowType.Air);
                AddRow(xImpactOfExchanges, "F", 640,
                    LookupFunctions.LookupLanguageTranslation("xCostImpactOfExch", "Cost Impact of Exchanges",
                        Globals.LanguageVariables), "PCNT", "EXCHIMPACT", RowType.Air);
            }

            if (railInfo)
            {
                if (!_isPreview)
                {
                    AddRailRow(xRail, "G", 505,
                        LookupFunctions.LookupLanguageTranslation("xGrossRailVolume", "Gross Rail Volume",
                            Globals.LanguageVariables), "DOLL", "GrossRail");
                    AddRailRow(xRail, "G", 510,
                        LookupFunctions.LookupLanguageTranslation("xRailRefundVolume", "Refund Rail Volume",
                            Globals.LanguageVariables), "DOLL", "RefundAmt");
                    AddRailRow(xRail, "G", 515,
                        LookupFunctions.LookupLanguageTranslation("xNetRailVolume", "Net Rail Volume",
                            Globals.LanguageVariables), "DOLL", "NetRail");
                    AddRailRow(xRail, "G", 520,
                        LookupFunctions.LookupLanguageTranslation("xRailRefundVolPct", "Rail Refund Volume %",
                            Globals.LanguageVariables), "PCNT", "REFVOLPCNT");
                    AddRailRow(xRail, "G", 525,
                        LookupFunctions.LookupLanguageTranslation("xNbrOfRailInvoices", "# of Rail Invoices",
                            Globals.LanguageVariables), "INT", "Invoices");
                    AddRailRow(xRail, "G", 530,
                        LookupFunctions.LookupLanguageTranslation("xNbrOfRailRefunds", "# of Rail Refunds",
                            Globals.LanguageVariables), "INT", "Refunds");
                    AddRailRow(xRail, "G", 535,
                        LookupFunctions.LookupLanguageTranslation("xNetRailTrans", "Net Rail Transactions",
                            Globals.LanguageVariables), "INT", "NetTrans");
                    AddRailRow(xRail, "G", 540,
                        LookupFunctions.LookupLanguageTranslation("xRailRefundTransPct", "Rail Refund Trans %",
                            Globals.LanguageVariables), "PCNT", "REFTRANPCNT");

                    AddRailLegRow(xRail, "G", 541, totalMilesKms, "INT", "TOTMILES");

                    if (doCarbonCalcs)
                    {
                        AddRailLegRow(xRail, "G", 542, xTotalCo2Emissions + "(" + poundsKilos + ")", "INT", "TOTAIRCO2");
                    }
                    AddRailRow(xRail, "H", 545,
                        LookupFunctions.LookupLanguageTranslation("xAvgGrossRailAmt", "Average Gross Rail Amt",
                            Globals.LanguageVariables), "AVG", "AVGGROSSRAIL");
                    AddRailRow(xRail, "H", 550,
                        LookupFunctions.LookupLanguageTranslation("xAvgRailRefundAmt", "Average Rail Refund Amt",
                            Globals.LanguageVariables), "AVG", "AVGGREFRAIL");
                    AddRailRow(xRail, "H", 555,
                        LookupFunctions.LookupLanguageTranslation("xAvgNetRailAmt", "Average Net Rail Amt",
                            Globals.LanguageVariables), "AVG", "AVGNETRAIL");

                }
                else
                {
                    AddRailRow(xRail, "G", 515,
                        LookupFunctions.LookupLanguageTranslation("xRailVolume", "Rail Volume",
                            Globals.LanguageVariables), "DOLL", "NetRail");
                    AddRailRow(xRail, "G", 535,
                        LookupFunctions.LookupLanguageTranslation("xRailTrans", "Rail Transactions",
                            Globals.LanguageVariables), "INT", "NetTrans");

                    AddRailLegRow(xRail, "G", 541, totalMilesKms, "INT", "TOTMILES");

                    if (doCarbonCalcs)
                    {
                        AddRailLegRow(xRail, "G", 542, xTotalCo2Emissions + "(" + poundsKilos + ")", "INT", "TOTAIRCO2");
                    }
                }

                if (!excludeSavings)
                {
                    AddRailRow(xRail, "I", 560,
                        LookupFunctions.LookupLanguageTranslation("xFullFareVol", "Full Fare Volume",
                            Globals.LanguageVariables), "DOLL", "FullFare");
                    AddRailRow(xRail, "I", 565,
                        LookupFunctions.LookupLanguageTranslation("xSavingsVol", "Savings Volume",
                            Globals.LanguageVariables), "DOLL", "Savings");
                    AddRailRow(xRail, "I", 570,
                        LookupFunctions.LookupLanguageTranslation("xSavingsPct", "Savings %", Globals.LanguageVariables),
                        "PCNT", "SVNGSPCNT");
                }

                if (!excludeExceptions || !excludeNegoSavings)
                {
                    AddRailRow(xRail, "I", 575,
                        LookupFunctions.LookupLanguageTranslation("xLowFareVol", "Low Fare Volume",
                            Globals.LanguageVariables), "DOLL", "LowFare");
                }

                if (!excludeExceptions)
                {
                    AddRailRow(xRail, "I", 580,
                        LookupFunctions.LookupLanguageTranslation("xMissedSvgsVol", "Missed Savings Volume",
                            Globals.LanguageVariables), "DOLL", "LostAmt");
                    AddRailRow(xRail, "I", 585,
                        LookupFunctions.LookupLanguageTranslation("xMissedSvgsPct", "Missed Savings %",
                            Globals.LanguageVariables), "PCNT", "LOSTPCNT");

                }

                if (!excludeNegoSavings)
                {
                    AddRailRow(xRail, "I", 590,
                        LookupFunctions.LookupLanguageTranslation("xNegoSvgsVol", "Negotiated Svgs Volume",
                            Globals.LanguageVariables), "DOLL", "NegoSvngs");
                    AddRailRow(xRail, "I", 595,
                        LookupFunctions.LookupLanguageTranslation("xNegoSvgsPct", "Negotiated Svgs %",
                            Globals.LanguageVariables), "PCNT", "NEGOPCNT");

                }
            }

            if (!_isPreview && !Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVCFEES))
            {
                _svcFeeDataByMonth = _svcFeeRawDataList.GroupBy(s => s.UseDate.GetValueOrDefault().Month, (month, recs) =>
                {
                    var reclist = recs.ToList();
                    return new ServiceFeeDataGrouped
                    {
                        MonthNum = month,
                        SvcFee = reclist.Sum(s => s.SvcAmt),
                        SvcFeeCnt = reclist.Sum(s => s.SvcFeeCount)
                    };

                }).ToList();
                AddRow(xServiceFees, "J", 660, xServiceFees, "DOLL", "SvcFee", RowType.ServiceFee);
                AddRow(xServiceFees, "J", 670, LookupFunctions.LookupLanguageTranslation("xNbrOfSvcFees", "# of Service Fees",
                            Globals.LanguageVariables), "INT", "SvcFeeCnt", RowType.ServiceFee);

            }
            _hotelDataByMonth = _hotelRawDataList.GroupBy(s => s.UseDate.Month, (month, recs) =>
            {
                var reclist = recs as IList<HotelRawData> ?? recs.ToList();
                return new HotelDataGrouped
                {
                    MonthNum = month,
                    Rooms = reclist.Sum(s => s.HPlusMin*s.Rooms),
                    Nights = reclist.Sum(s => s.HPlusMin*s.Rooms*s.Nights),
                    Stays = reclist.Sum(s => s.HPlusMin),
                    HotelCost = reclist.Sum(s => s.Rooms*s.Nights*s.Bookrate),
                    HotelCo2 = reclist.Sum(s => s.HotelCo2)
                };

            }).ToList();

            AddRow(xHotelBookings, "K", 710,
                LookupFunctions.LookupLanguageTranslation("xHotelRoomsBooked", "Hotel Rooms Booked",
                    Globals.LanguageVariables), "INT", "rooms", RowType.Hotel);
            AddRow(xHotelBookings, "K", 720,
                LookupFunctions.LookupLanguageTranslation("xNbrOfRoomNights", "# of Room Nights",
                    Globals.LanguageVariables), "INT", "Nights", RowType.Hotel);
            AddRow(xHotelBookings, "K", 730,
                LookupFunctions.LookupLanguageTranslation("xHotelCosts", "Hotel Costs", Globals.LanguageVariables),
                "DOLL", "HotelCost", RowType.Hotel);
            AddRow(xHotelBookings, "K", 740,
                LookupFunctions.LookupLanguageTranslation("xAvgNbrRoomNights", "Avg # of Hotel Room Nights",
                    Globals.LanguageVariables), "AVG2", "AVGNBRNITES", RowType.Hotel);
            AddRow(xHotelBookings, "K", 750,
                LookupFunctions.LookupLanguageTranslation("xAvgCostPernight", "Avg Cost per Night",
                    Globals.LanguageVariables), "AVG", "AVGNITECOST", RowType.Hotel);

            if (doCarbonCalcs)
            {
                AddRow(xHotelBookings, "K", 760,
                    LookupFunctions.LookupLanguageTranslation("xHotelCO2Emissions", "Hotel CO2 Emissions",
                        Globals.LanguageVariables) + "(" + poundsKilos + ")", "INT", "HOTELCO2", RowType.Hotel);
            }


            _carDataByMonth = _carRawDataList.GroupBy(s => s.UseDate.Month, (month, recs) =>
            {
                var reclist = recs as IList<CarRawData> ?? recs.ToList();
                return new CarDataGrouped
                {
                    MonthNum = month,
                    Rents = reclist.Sum(s => s.CPlusMin),
                    Days = reclist.Sum(s => s.CPlusMin*s.Days),
                    CarCost = reclist.Sum(s => s.Days*s.Abookrat),
                    CarCo2 = reclist.Sum(s => s.CarCo2)
                };

            }).ToList();


            AddRow(xCarRentalBkngs, "L", 810,
                LookupFunctions.LookupLanguageTranslation("xCarsBooked", "Cars Booked", Globals.LanguageVariables),
                "INT", "Rents", RowType.Car);
            AddRow(xCarRentalBkngs, "L", 820,
                LookupFunctions.LookupLanguageTranslation("xNbrDays2", "# of Days", Globals.LanguageVariables), "INT",
                "Days", RowType.Car);
            AddRow(xCarRentalBkngs, "L", 830,
                LookupFunctions.LookupLanguageTranslation("xCarRentalCosts", "Car Rental Costs",
                    Globals.LanguageVariables), "DOLL", "CarCost", RowType.Car);
            AddRow(xCarRentalBkngs, "L", 840,
                LookupFunctions.LookupLanguageTranslation("xAvgNbrOfDays", "Avg # of Days", Globals.LanguageVariables),
                "AVG2", "AVGNBRDAYS", RowType.Car);
            AddRow(xCarRentalBkngs, "L", 850,
                LookupFunctions.LookupLanguageTranslation("xAvgCostPerDay", "Avg Cost per Day",
                    Globals.LanguageVariables), "AVG", "AVGDAYCOST", RowType.Car);

            if (doCarbonCalcs)
            {
                AddRow(xCarRentalBkngs, "L", 860,
                    LookupFunctions.LookupLanguageTranslation("xCarCO2Emissions", "Car CO2 Emissions",
                        Globals.LanguageVariables) + "(" + poundsKilos + ")", "INT", "CARCO2", RowType.Car);
            }

            AddRow(LookupFunctions.LookupLanguageTranslation("xTotalCost", "Total Cost", Globals.LanguageVariables), "M",
                910, LookupFunctions.LookupLanguageTranslation("xTotals", "Total", Globals.LanguageVariables), "TOTAL",
                "TOTALCOST", RowType.Air);

            //rows with numbers above 1000 are strictly for calculations
            FinalDataList = FinalDataList.Where(s => s.RowNum <= 1000).ToList();
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList,_exportFields, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList,_exportFields, Globals);
                    }
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." +
                                      Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);


                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    ReportSource.SetParameterValue("cCharGen1",
                        string.Join(string.Empty, _months.Select(s => s.MonthAbbreviation)));
                    ReportSource.SetParameterValue("nYear", _year);
                    ReportSource.SetParameterValue("cSubTitle", _subTitle);
                    ReportSource.SetParameterValue("lLogGen1", false);

                    if (Globals.OutputType == "R")
                    {
                        ReportSource.SetParameterValue("BoxColor", false);
                    }
                    else
                    {
                        ReportSource.SetParameterValue("BoxColor", true);
                    }

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        private List<LegDataGrouped> GetLegsByMonth(bool doCarbonCalcs, string mode = "")
        {

            mode = mode.Trim();
            var legQuery = string.IsNullOrEmpty(mode)
                ? _legRawDataList
                : _legRawDataList.Where(s => s.Mode.EqualsIgnoreCase(mode));

            return doCarbonCalcs
                ? legQuery.GroupBy(s => s.UseDate.Month,
                    (month, recs) =>
                    {
                        var reclist = recs as IList<LegRawData> ?? recs.ToList();
                        return new LegDataGrouped
                        {
                            MonthNum = month,
                            TotMiles = reclist.Sum(s => s.Plusmin * Math.Abs(s.Miles)),
                            DomMiles =
                                reclist.Sum(s => s.Domintl.EqualsIgnoreCase("D") ? s.Plusmin * Math.Abs(s.Miles) : 0),
                            IntlMiles =
                                reclist.Sum(s => !s.Domintl.EqualsIgnoreCase("D") ? s.Plusmin * Math.Abs(s.Miles) : 0),
                            TotAirCo2 = reclist.Sum(s => s.Plusmin * Math.Abs(s.AirCo2)),
                            DomAirCo2 =
                                reclist.Sum(s => s.Domintl.EqualsIgnoreCase("D") ? s.Plusmin * Math.Abs(s.AirCo2) : 0),
                            IntlAirCo2 =
                                reclist.Sum(s => !s.Domintl.EqualsIgnoreCase("D") ? s.Plusmin * Math.Abs(s.AirCo2) : 0),
                        };
                    }).ToList()
                : legQuery.GroupBy(s => s.UseDate.Month,
                    (month, recs) =>
                    {
                        var reclist = recs as IList<LegRawData> ?? recs.ToList();
                        return new LegDataGrouped
                        {
                            MonthNum = month,
                            TotMiles = reclist.Sum(s => s.Plusmin * Math.Abs(s.Miles)),
                            DomMiles =
                                reclist.Sum(s => s.Domintl.EqualsIgnoreCase("D") ? s.Plusmin * Math.Abs(s.Miles) : 0),
                            IntlMiles =
                                reclist.Sum(s => !s.Domintl.EqualsIgnoreCase("D") ? s.Plusmin * Math.Abs(s.Miles) : 0),
                        };
                    }).ToList();
        }

        private void GenerateMonths()
        {
            //Get the translated month names
            var monthAbbreviations = LookupFunctions.LookupLanguageTranslation("lt_AbbrMthsofYear",
                "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec", Globals.LanguageVariables)
                .Split(',');

            _months = new MonthInfo[12];
            for (int i = 0; i < 12; i++)
            {
                var begMonth = Globals.BeginDate.Value.AddMonths(i);
                var endMonth = begMonth.AddMonths(1).AddDays(-1);

                _months[i] = new MonthInfo
                {
                    BeginMonth = begMonth,
                    EndMonth = endMonth,
                    MonthAbbreviation = monthAbbreviations[begMonth.Month - 1],
                    MonthNumber = begMonth.Month
                };
            }
        }

        private void AddLegRow(string category, string groupCode, int rowNum, string rowDesc, string dataType,
            string colName)
        {
            var newRow = new FinalData
            {
                Category = category,
                GrpCode = groupCode,
                RowNum = rowNum,
                RowDesc = rowDesc,
                DataType = dataType
            };

            switch (dataType.ToUpper())
            {
                case "DOLL":
                case "INT":
                    AddSumRow(newRow, colName, _legDataByMonth);
                    break;
                case "CENT":
                    switch (colName.ToUpper())
                    {
                        case "INTLCPM": //INT'L COST PER MILE.
                            AddCalculatedRow(newRow, 270, 210, dataType, true);
                            break;
                        case "DOMCPM": //DOMESTIC COST PER MILE.
                            AddCalculatedRow(newRow, 370, 310, dataType);
                            break;

                    }
                    break;

            }
        }

        private void AddRailLegRow(string category, string groupCode, int rowNum, string rowDesc, string dataType,
            string colName)
        {
            var newRow = new FinalData
            {
                Category = category,
                GrpCode = groupCode,
                RowNum = rowNum,
                RowDesc = rowDesc,
                DataType = dataType
            };

            switch (dataType.ToUpper())
            {
                case "DOLL":
                case "INT":
                    AddSumRow(newRow, colName, _railLegDataByMonth);
                    break;
                case "CENT":
                    switch (colName.ToUpper())
                    {
                        case "INTLCPM": //INT'L COST PER MILE.
                            AddCalculatedRow(newRow, 270, 210, dataType, true);
                            break;
                        case "DOMCPM": //DOMESTIC COST PER MILE.
                            AddCalculatedRow(newRow, 370, 310, dataType);
                            break;

                    }
                    break;

            }
        }

        private void AddRailRow(string category, string groupCode, int rowNum, string rowDesc, string dataType,
            string colName)
        {
            var newRow = new FinalData
            {
                Category = category,
                GrpCode = groupCode,
                RowNum = rowNum,
                RowDesc = rowDesc,
                DataType = dataType
            };

            switch (dataType.ToUpper())
            {
                case "DOLL":
                case "INT":
                    AddSumRow(newRow, colName, _railDataByMonth);
                    break;
                case "PCNT":
                case "AVG":
                case "AVG2":
                case "CENT":
                    switch (colName.ToUpper())
                    {
                        case "REFVOLPCNT": //REFUND VOLUME %.
                            AddCalculatedRow(newRow, 505, 510, dataType, true);
                            break;
                        case "REFTRANPCNT": //REFUND TRANSACTIONS (CREDITS) %.
                            AddCalculatedRow(newRow, 525, 530, dataType);
                            break;
                        case "AVGGROSSRAIL": //AVERAGE GROSS RAIL TICKET AMT.
                            AddCalculatedRow(newRow, 525, 505, dataType);
                            break;
                        case "AVGGREFRAIL": //AVERAGE REFUND RAIL TICKET AMT.
                            AddCalculatedRow(newRow, 530, 510, dataType);
                            break;
                        case "AVGNETRAIL": // AVERAGE NET RAIL TICKET AMT.
                            AddCalculatedRow(newRow, 535, 515, dataType);
                            break;
                        case "SVNGSPCNT": //SAVINGS %.
                            AddCalculatedRow(newRow, 560, 565, dataType);
                            break;
                        case "LOSTPCNT": //LOST SAVINGS %.
                            AddCalculatedRow(newRow, 575, 580, dataType);
                            break;
                        case "NEGOPCNT": //NEGOTIATED SAVINGS %.
                            AddCalculatedRow(newRow, 575, 590, dataType);
                            break;

                    }
                    break;

            }
        }

        private void AddRow(string category, string groupCode, int rowNum, string rowDesc, string dataType,
            string colName, RowType type)
        {
            var newRow = new FinalData
            {
                Category = category,
                GrpCode = groupCode,
                RowNum = rowNum,
                RowDesc = rowDesc,
                DataType = dataType
            };
            switch (dataType.ToUpper())
            {
                case "DOLL":
                case "INT":
                    switch (type)
                    {
                        case RowType.Air:
                            AddSumRow(newRow, colName, _airDataByMonth);
                            break;
                        case RowType.Car:
                            AddSumRow(newRow, colName, _carDataByMonth);
                            break;
                        case RowType.Hotel:
                            AddSumRow(newRow, colName, _hotelDataByMonth);
                            break;
                        case RowType.ServiceFee:
                            AddSumRow(newRow, colName, _svcFeeDataByMonth);
                            break;

                    }

                    break;
                case "PCNT":
                case "AVG":
                case "AVG2":
                case "CENT":
                    switch (colName.ToUpper())
                    {
                        case "REFVOLPCNT": //REFUND VOLUME %.
                            AddCalculatedRow(newRow, 10, 20, dataType, true);
                            break;
                        case "REFTRANPCNT": //REFUND TRANSACTIONS (CREDITS) %.
                            AddCalculatedRow(newRow, 50, 60, dataType);
                            break;
                        case "AVGGROSSAIR": //AVERAGE GROSS AIR TICKET AMT.
                            AddCalculatedRow(newRow, 50, 10, dataType);
                            break;
                        case "AVGREFAIR": //AVERAGE REFUND AIR TICKET AMT.
                            AddCalculatedRow(newRow, 60, 20, dataType);
                            break;
                        case "AVGNETAIR": //AVERAGE NET AIR TICKET AMT.
                            AddCalculatedRow(newRow, 70, 30, dataType);
                            break;
                        case "AVGMILECOST": //AVERAGE COST PER MILE 
                            AddCalculatedRow(newRow, 90, 30, dataType);
                            break;
                        case "AVGINTLTKT": //AVERAGE INT'L NET AIR TICKET AMT
                            AddCalculatedRow(newRow, 230, 210, dataType);
                            break;
                        case "AVGDOMTKT": //AVERAGE DOMESTIC NET AIR TICKET AMT
                            AddCalculatedRow(newRow, 330, 310, dataType);
                            break;
                        case "INTLPCNT": //INTERNATIONAL VOLUME AIR %.
                            AddCalculatedRow(newRow, 30, 210, dataType);
                            break;
                        case "DOMPCNT": //DOMESTIC VOLUME AIR %.
                            AddCalculatedRow(newRow, 30, 310, dataType);
                            break;
                        case "SVNGSPCNT": //SAVINGS %.
                            AddCalculatedRow(newRow, 410, 420, dataType);
                            break;
                        case "LOSTPCNT": //LOST SAVINGS %.
                            AddCalculatedRow(newRow, 440, 450, dataType);
                            break;
                        case "NEGOPCNT": //NEGOTIATED SAVINGS %.
                            AddCalculatedRow(newRow, 440, 470, dataType);
                            break;
                        case "AVGNBRNITES": //AVERAGE # OF HOTEL NIGHTS.
                            AddCalculatedRow(newRow, 710, 720, dataType);
                            break;
                        case "AVGNITECOST": //AVERAGE COST PER NIGHT.
                            AddCalculatedRow(newRow, 720, 730, dataType);
                            break;
                        case "AVGNBRDAYS": //AVERAGE # OF CAR RENTAL DAYS.
                            AddCalculatedRow(newRow, 810, 820, dataType);
                            break;
                        case "AVGDAYCOST": //AVERAGE COST PER DAY.
                            AddCalculatedRow(newRow, 820, 830, dataType);
                            break;
                        case "EXCHANGEAVG": //AVERAGE COST TO EXCHANGE.
                            AddCalculatedRow(newRow, 610, 620, dataType);
                            break;
                        case "EXCHIMPACT": //AVERAGE # OF CAR RENTAL DAYS.
                            AddCalculatedRow(newRow, 30, 620, dataType);
                            break;
                        case "ONLINEPCT": //AVERAGE # OF CAR RENTAL DAYS.
                            AddCalculatedRow(newRow, 70, 110, dataType);
                            break;
                        case "AVGCO2": //AVG CARBON EMISSIONS PER TRIP.
                            AddCalculatedRow(newRow, 70, 92, dataType,false, true);
                            break;
                        case "DOMAVGCO2": //DOMESTIC AVG CARBON EMISSIONS PER TRIP.
                            AddCalculatedRow(newRow, 330, 1372, dataType, false, true);
                            break;
                        case "INTLAVGCO2": //INT'L AVG CARBON EMISSIONS PER TRIP.
                            AddCalculatedRow(newRow, 230, 1272, dataType, false, true);
                            break;
                    }
                    break;
                case "TOTAL":
                    var rowsToTotal = new List<int> {30, 515, 730, 830};
                    newRow.Mth1 = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Mth1);
                    newRow.Mth2 = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Mth2);
                    newRow.Mth3 = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Mth3);
                    newRow.Mth4 = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Mth4);
                    newRow.Mth5 = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Mth5);
                    newRow.Mth6 = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Mth6);
                    newRow.Mth7 = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Mth7);
                    newRow.Mth8 = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Mth8);
                    newRow.Mth9 = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Mth9);
                    newRow.Mth10 = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Mth10);
                    newRow.Mth11 = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Mth11);
                    newRow.Mth12 = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Mth12);
                    newRow.Ytd = FinalDataList.Where(s => rowsToTotal.Contains(s.RowNum)).Sum(s => s.Ytd);
                    newRow.DataType = "DOLL";
                    FinalDataList.Add(newRow);
                    break;
            }
        }

        private void AddCalculatedRow(FinalData newRow, int index1, int index2, string dataType, bool abs = false, bool forceInt = false)
        {
            var rec1 = FinalDataList.FirstOrDefault(s => s.RowNum == index1);
            var rec2 = FinalDataList.FirstOrDefault(s => s.RowNum == index2);
            if (rec1 == null || rec2 == null) return;
            switch (dataType.ToUpper())
            {
                case "AVG2":
                case "AVG":
                case "CENT":
                    var round = 1;
                    if (dataType.ToUpper().EqualsIgnoreCase("AVG")) round = 0;
                    if (dataType.ToUpper().EqualsIgnoreCase("CENT")) round = 2;

                    newRow.DataType = dataType;
                    newRow.Mth1 = rec1.Mth1 == 0 ? 0 : MathHelper.Round(rec2.Mth1/rec1.Mth1, round);
                    newRow.Mth2 = rec1.Mth2 == 0 ? 0 : MathHelper.Round(rec2.Mth2/rec1.Mth2, round);
                    newRow.Mth3 = rec1.Mth3 == 0 ? 0 : MathHelper.Round(rec2.Mth3/rec1.Mth3, round);
                    newRow.Mth4 = rec1.Mth4 == 0 ? 0 : MathHelper.Round(rec2.Mth4/rec1.Mth4, round);
                    newRow.Mth5 = rec1.Mth5 == 0 ? 0 : MathHelper.Round(rec2.Mth5/rec1.Mth5, round);
                    newRow.Mth6 = rec1.Mth6 == 0 ? 0 : MathHelper.Round(rec2.Mth6/rec1.Mth6, round);
                    newRow.Mth7 = rec1.Mth7 == 0 ? 0 : MathHelper.Round(rec2.Mth7/rec1.Mth7, round);
                    newRow.Mth8 = rec1.Mth8 == 0 ? 0 : MathHelper.Round(rec2.Mth8/rec1.Mth8, round);
                    newRow.Mth9 = rec1.Mth9 == 0 ? 0 : MathHelper.Round(rec2.Mth9/rec1.Mth9, round);
                    newRow.Mth10 = rec1.Mth10 == 0 ? 0 : MathHelper.Round(rec2.Mth10/rec1.Mth10, round);
                    newRow.Mth11 = rec1.Mth11 == 0 ? 0 : MathHelper.Round(rec2.Mth11/rec1.Mth11, round);
                    newRow.Mth12 = rec1.Mth12 == 0 ? 0 : MathHelper.Round(rec2.Mth12/rec1.Mth12, round);
                    newRow.Ytd = rec1.Ytd == 0 ? 0 : MathHelper.Round(rec2.Ytd/rec1.Ytd, round);

                    break;

                default:
                    if (abs)
                    {
                        newRow.DataType = dataType;
                        newRow.Mth1 = rec1.Mth1 == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Mth1/rec1.Mth1*100, 1));
                        newRow.Mth2 = rec1.Mth2 == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Mth2/rec1.Mth2*100, 1));
                        newRow.Mth3 = rec1.Mth3 == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Mth3/rec1.Mth3*100, 1));
                        newRow.Mth4 = rec1.Mth4 == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Mth4/rec1.Mth4*100, 1));
                        newRow.Mth5 = rec1.Mth5 == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Mth5/rec1.Mth5*100, 1));
                        newRow.Mth6 = rec1.Mth6 == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Mth6/rec1.Mth6*100, 1));
                        newRow.Mth7 = rec1.Mth7 == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Mth7/rec1.Mth7*100, 1));
                        newRow.Mth8 = rec1.Mth8 == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Mth8/rec1.Mth8*100, 1));
                        newRow.Mth9 = rec1.Mth9 == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Mth9/rec1.Mth9*100, 1));
                        newRow.Mth10 = rec1.Mth10 == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Mth10/rec1.Mth10*100, 1));
                        newRow.Mth11 = rec1.Mth11 == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Mth11/rec1.Mth11*100, 1));
                        newRow.Mth12 = rec1.Mth12 == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Mth12/rec1.Mth12*100, 1));
                        newRow.Ytd = rec1.Ytd == 0 ? 0 : Math.Abs(MathHelper.Round(rec2.Ytd/rec1.Ytd*100, 1));
                    }
                    else
                    {
                        newRow.DataType = dataType;
                        newRow.Mth1 = rec1.Mth1 == 0 ? 0 : MathHelper.Round(rec2.Mth1/rec1.Mth1*100, 1);
                        newRow.Mth2 = rec1.Mth2 == 0 ? 0 : MathHelper.Round(rec2.Mth2/rec1.Mth2*100, 1);
                        newRow.Mth3 = rec1.Mth3 == 0 ? 0 : MathHelper.Round(rec2.Mth3/rec1.Mth3*100, 1);
                        newRow.Mth4 = rec1.Mth4 == 0 ? 0 : MathHelper.Round(rec2.Mth4/rec1.Mth4*100, 1);
                        newRow.Mth5 = rec1.Mth5 == 0 ? 0 : MathHelper.Round(rec2.Mth5/rec1.Mth5*100, 1);
                        newRow.Mth6 = rec1.Mth6 == 0 ? 0 : MathHelper.Round(rec2.Mth6/rec1.Mth6*100, 1);
                        newRow.Mth7 = rec1.Mth7 == 0 ? 0 : MathHelper.Round(rec2.Mth7/rec1.Mth7*100, 1);
                        newRow.Mth8 = rec1.Mth8 == 0 ? 0 : MathHelper.Round(rec2.Mth8/rec1.Mth8*100, 1);
                        newRow.Mth9 = rec1.Mth9 == 0 ? 0 : MathHelper.Round(rec2.Mth9/rec1.Mth9*100, 1);
                        newRow.Mth10 = rec1.Mth10 == 0 ? 0 : MathHelper.Round(rec2.Mth10/rec1.Mth10*100, 1);
                        newRow.Mth11 = rec1.Mth11 == 0 ? 0 : MathHelper.Round(rec2.Mth11/rec1.Mth11*100, 1);
                        newRow.Mth12 = rec1.Mth12 == 0 ? 0 : MathHelper.Round(rec2.Mth12/rec1.Mth12*100, 1);
                        newRow.Ytd = rec1.Ytd == 0 ? 0 : MathHelper.Round(rec2.Ytd/rec1.Ytd*100, 1);
                    }

                    break;
            }

            if (forceInt)
            {
                newRow.DataType = "INT";
            }
            FinalDataList.Add(newRow);
        }

        private void AddSumRow<T>(FinalData newRow, string colName, List<T> list) where T : IGroupedByMonth
        {
            newRow.Mth1 = list.Sum(s => s.MonthNum == _months[0].MonthNumber ? MathHelper.Round(GetFieldByName(s, colName), 0) : 0);
            newRow.Mth2 = list.Sum(s => s.MonthNum == _months[1].MonthNumber ? MathHelper.Round(GetFieldByName(s, colName), 0) : 0);
            newRow.Mth3 = list.Sum(s => s.MonthNum == _months[2].MonthNumber ? MathHelper.Round(GetFieldByName(s, colName), 0) : 0);
            newRow.Mth4 = list.Sum(s => s.MonthNum == _months[3].MonthNumber ? MathHelper.Round(GetFieldByName(s, colName), 0) : 0);
            newRow.Mth5 = list.Sum(s => s.MonthNum == _months[4].MonthNumber ? MathHelper.Round(GetFieldByName(s, colName), 0) : 0);
            newRow.Mth6 = list.Sum(s => s.MonthNum == _months[5].MonthNumber ? MathHelper.Round(GetFieldByName(s, colName), 0) : 0);
            newRow.Mth7 = list.Sum(s => s.MonthNum == _months[6].MonthNumber ? MathHelper.Round(GetFieldByName(s, colName), 0) : 0);
            newRow.Mth8 = list.Sum(s => s.MonthNum == _months[7].MonthNumber ? MathHelper.Round(GetFieldByName(s, colName), 0) : 0);
            newRow.Mth9 = list.Sum(s => s.MonthNum == _months[8].MonthNumber ? MathHelper.Round(GetFieldByName(s, colName), 0) : 0);
            newRow.Mth10 = list.Sum(s => s.MonthNum == _months[9].MonthNumber ? MathHelper.Round(GetFieldByName(s, colName), 0) : 0);
            newRow.Mth11 = list.Sum(s => s.MonthNum == _months[10].MonthNumber ? MathHelper.Round(GetFieldByName(s, colName), 0) : 0);
            newRow.Mth12 = list.Sum(s => s.MonthNum == _months[11].MonthNumber ? MathHelper.Round(GetFieldByName(s, colName), 0) : 0);
            newRow.Ytd = list.Sum(s => MathHelper.Round(GetFieldByName(s, colName),0));

            FinalDataList.Add(newRow);
        }

        private decimal GetFieldByName(object rec, string colName)
        {
            var tType = rec.GetType();
            var tProperties = new List<PropertyInfo>(tType.GetProperties());
            var prop = tProperties.FirstOrDefault(p => p.Name.EqualsIgnoreCase(colName));
            if (prop == null) return 0m;

            var val = prop.GetValue(rec, null);
            if (val is decimal) return (decimal) val;
            if (val is int) return (int) val;


            return 0;
        }
    }



   public enum RowType
    {
        Air,
        Car,
        Hotel, 
        ServiceFee
    }
}

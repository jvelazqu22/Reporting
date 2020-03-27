using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopTravelersAuditedReport;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;
using Domain.Orm.iBankMastersQueries.Other;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.TopTravelersAudited
{
    public class TravAuthTopOop : ReportRunner<RawData,FinalData>
    {
        private double _offsetHours;
        private string _timeZoneName;

        public List<CarRawData> CarRawDataList { get; set; }
        public List<HotelRawData> HotelRawDataList { get; set; }


        public TravAuthTopOop()
        {
            CrystalReportName = "ibTravAuthTopOOP";
        }
        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;
            if (!IsUdidNumberSuppliedWithUdidText()) return false;
            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            GetOffsetValues();
            Globals.SetParmValue(WhereCriteria.PREPOST, "1");
            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), Globals.Agency);

            //if we apply routing in both directions "SpecRouting" is true. 
            //BuildWhere.BuildAll(getAllMasterAccountsQuery, true, true, false, false, true, false, true, false, true, false, false);
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false,buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true,inMemory: true,isRoutingBidirectional: false,legDit: true,ignoreTravel: false);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            BuildWhere.AddSecurityChecks();
            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);

            var sql = SqlBuilder.GetSql(udidNumber > 0, BuildWhere.WhereClauseFull);
            RawDataList = RetrieveRawData<RawData>(sql, GlobalCalc.IsReservationReport(), false).ToList();

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstDestination || BuildWhere.HasFirstOrigin)
            {

                var sqlLegs = SqlBuilder.GetSqlLegs(udidNumber > 0, BuildWhere.WhereClauseFull);
                var legRawDataList = RetrieveRawData<LegRawData>(sqlLegs, GlobalCalc.IsReservationReport()).ToList();

                var segmentData = Collapser<LegRawData>.Collapse(legRawDataList, Collapser<LegRawData>.CollapseType.Both);
                segmentData = GlobalCalc.IsAppliedToLegLevelData()
                    ? BuildWhere.ApplyWhereRoute(segmentData, true)
                    : BuildWhere.ApplyWhereRoute(segmentData, false);
                legRawDataList = GetLegDataFromFilteredSegData(legRawDataList, segmentData);

                RawDataList.RemoveAll(s => !legRawDataList.Select(l => l.RecKey).Distinct().Contains(s.RecKey));

            }

            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            
            sql = SqlBuilder.GetSqlCar(udidNumber > 0, BuildWhere.WhereClauseFull);
            CarRawDataList = RetrieveRawData<CarRawData>(sql, GlobalCalc.IsReservationReport(), false).ToList();
           
            sql = SqlBuilder.GetSqlHotel(udidNumber > 0, BuildWhere.WhereClauseFull);
            HotelRawDataList = RetrieveRawData<HotelRawData>(sql, GlobalCalc.IsReservationReport(), false).ToList();
            
            return true;
        }

        public override bool ProcessData()
        {
            //Only use time offset if we are using the Auth Status date. 
            if (Globals.ParmValueEquals(WhereCriteria.DATERANGE, "12"))
            {
                RawDataList =
                    RawDataList.Where(
                        s =>
                            s.Statustime.GetValueOrDefault().AddHours(_offsetHours) >= Globals.BeginDate.Value &&
                            s.Statustime.GetValueOrDefault().AddHours(_offsetHours) <= Globals.EndDate.Value).ToList();
            }

            RawDataList = TravAuthTopOopHelpers.ProcessOutOfPolicy(RawDataList, Globals);

            var reckeys = RawDataList.Select(s => s.RecKey).Distinct().ToList();

            CarRawDataList = CarRawDataList.Where(s => reckeys.Contains(s.RecKey)).ToList();
            CarRawDataList = PerformCurrencyConversion(CarRawDataList);
            HotelRawDataList = HotelRawDataList.Where(s => reckeys.Contains(s.RecKey)).ToList();
            HotelRawDataList = PerformCurrencyConversion(HotelRawDataList);

            //Combine the data
            RawDataList.AddRange(TravAuthTopOopHelpers.ConvertHotelToRawData(HotelRawDataList, RawDataList));
            RawDataList.AddRange(TravAuthTopOopHelpers.ConvertCarToRawData(CarRawDataList,RawDataList));

            FinalDataList = RawDataList
                .GroupBy(s => new {s.RecKey,s.Passlast,s.Passfrst}, (key,recs) =>
                {
                    var reclist = recs.ToList();
                    return new
                    {
                        key.RecKey,
                        Passname = key.Passlast.Trim() + ", " + key.Passfrst.Trim(),
                        PassLast = key.Passlast.Trim(),
                        PassFrst = key.Passfrst.Trim(),
                        Bookvolume = reclist.Sum(s => s.AirChg),
                        AuthStatus = reclist.Min(s => s.AuthStatus).Trim()
                    };
                })
                .GroupBy(s => s.Passname, (key, recs) =>
                {
                    var reclist = recs.ToList();
                    return new FinalData
                    {
                        Passname = key,
                        Approved = reclist.Sum(s => s.AuthStatus.Equals("A")?1:0),
                        Notifyonly = reclist.Sum(s => s.AuthStatus.Equals("N") ? 1 : 0),
                        Declined = reclist.Sum(s => s.AuthStatus.Equals("D") ? 1 : 0),
                        Expired = reclist.Sum(s => s.AuthStatus.Equals("E") ? 1 : 0),
                        Trips = reclist.Count,
                        Bookvolume = reclist.Sum(s => s.Bookvolume)
                    };
                }).ToList();

            FinalDataList = TravAuthTopOopHelpers.SortFinalData(FinalDataList, Globals);

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:

                    var exportFieldList = TravAuthTopOopHelpers.GetExportFields();

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    else
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    break;
                default:

                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }

        private void GetOffsetValues()
        {
            var tzCode = Globals.GetParmValue(WhereCriteria.TIMEZONE);
            if (string.IsNullOrEmpty(tzCode))
                tzCode = "EST";
            var tz = new GetTimeZoneQuery(MasterStore.MastersQueryDb, tzCode, Globals.UserLanguage).ExecuteQuery();

            _offsetHours = tz.GmtDiff;
            var xDateTimeTimeZone = LookupFunctions.LookupLanguageTranslation("XDATETIMETIMEZONE", "Booked Date/Time and Status Date/Time are in ", Globals.LanguageVariables);
            _timeZoneName = tz.TimeZoneName.Replace("[xxxxx]", xDateTimeTimeZone);

            if (Globals.IsParmValueOn(WhereCriteria.OBSERVEDST))
                _offsetHours += 1;
        }
    }
}

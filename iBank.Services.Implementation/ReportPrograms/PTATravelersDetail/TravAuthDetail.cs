using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.PTATravelersDetailReport;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.PTATravelersDetail
{
    public class TravAuthDetail : ReportRunner<RawData, FinalData>
    {
        public List<TripAuthorizerRawData> TripAuthorizerRawDataList;
        public List<CarRawData> CarRawDataList;
        public List<HotelRawData> HotelRawDataList;
        public List<SummaryFinalData> SummaryDataList;

        private readonly TravAuthDetailRawDataRetriever _dataRetriever;

        private double _offsetHours;

        public TravAuthDetail()
        {
            CrystalReportName = "ibTravAuthDetail";
            _dataRetriever = new TravAuthDetailRawDataRetriever();
        }

        public override bool InitialChecks()
        {
            if (Globals.ParmHasValue(WhereCriteria.TXTPARSEDTSTART))
            {
                if (!IsStartParseDateValid()) return false;
            }

            if (Globals.ParmHasValue(WhereCriteria.TXTPARSEDTEND))
            {
                if (!IsEndParseDateValid()) return false;
            }

            if (!IsDateRangeValid()) return false;
            if (!IsUdidNumberSuppliedWithUdidText()) return false;
            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            GetOffsetValues();

            Globals.SetParmValue(WhereCriteria.PREPOST, "1");
            _dataRetriever.IsReservation = true;
            _dataRetriever.GlobalCalc = GlobalCalc;

            if (!_dataRetriever.GetRawData(BuildWhere)) return false;

            RawDataList = _dataRetriever.RawDataList;

            TripAuthorizerRawDataList = _dataRetriever.TripAuthorizerRawDataList;

            CarRawDataList = _dataRetriever.CarRawDataList;

            HotelRawDataList = _dataRetriever.HotelRawDataList;

            return true;
        }
        
        public override bool ProcessData()
        {
            var travAuth = Globals.GetParmValue(WhereCriteria.DDTRAVAUTH).TryIntParse(-1);

            if (travAuth != -1) TripAuthorizerRawDataList = TripAuthorizerRawDataList.Where(s => s.AuthrzrNbr == travAuth).ToList();

            var apprDeclComms = Globals.GetParmValue(WhereCriteria.TXTAPPRDECLCOMMS);
            if (!string.IsNullOrEmpty(apprDeclComms))
            {
                TripAuthorizerRawDataList = TripAuthorizerRawDataList.Where(s => s.ApvReason.Contains(apprDeclComms)).ToList();
                Globals.WhereText += "; Comments contain " + apprDeclComms;
            }

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstDestination || BuildWhere.HasFirstOrigin)
            {
                var reckeys = RawDataList.Select(s => s.RecKey).Distinct();
                var dateRange = Globals.GetParmValue(WhereCriteria.DATERANGE);

                var begDate = Globals.BeginDate.Value;
                var endDate = Globals.EndDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                if (dateRange.Equals("3"))
                {
                    TripAuthorizerRawDataList =
                        TripAuthorizerRawDataList.Where(s => s.Bookedgmt >= begDate && s.Bookedgmt <= endDate && reckeys.Contains(s.RecKey)).ToList();
                }
                else if (dateRange.Equals("12"))
                {
                    TripAuthorizerRawDataList =
                        TripAuthorizerRawDataList.Where(s => s.StatusTime.GetValueOrDefault().AddHours(_offsetHours) >= begDate && s.StatusTime.GetValueOrDefault().AddHours(_offsetHours) <= endDate && reckeys.Contains(s.RecKey)).ToList();
                }
            }

            SetUpFinalDataListAndSummaryDataList();
            SortFinalDataList();
            return true;
        }

        private void SetUpFinalDataListAndSummaryDataList()
        {
            var sumPageOnly = Globals.IsParmValueOn(WhereCriteria.CBSUMPAGEONLY);
            var includeNotifyOnly = Globals.IsParmValueOn(WhereCriteria.CBINCLNOTIFONLY);

            var groupedTripAuthData = sumPageOnly
                ? TripAuthorizerRawDataList.GroupBy(s => new { s.RecKey, s.TravAuthNo, s.SGroupNbr, s.OutPolCods, s.AuthStatus },
                        (key, recs) => new GroupedTripAuthData { RecKey = key.RecKey, TravAuthNo = key.TravAuthNo, SGroupNbr = key.SGroupNbr, OutPolCods = key.OutPolCods, AuthStatus = key.AuthStatus, NumRecs = recs.Count() }).ToList()
                : TripAuthorizerRawDataList.GroupBy(s => new { s.RecKey, s.TravAuthNo, s.SGroupNbr, s.AuthStatus, s.OutPolCods },
                        (key, recs) => new GroupedTripAuthData { RecKey = key.RecKey, TravAuthNo = key.TravAuthNo, SGroupNbr = key.SGroupNbr, OutPolCods = key.OutPolCods, AuthStatus = key.AuthStatus, NumRecs = recs.Count() }).ToList();

            groupedTripAuthData = TravAuthDetailHelpers.ProcessOutOfPolicy(groupedTripAuthData, Globals);

            if (sumPageOnly)
            {
                SummaryDataList = TravAuthDetailHelpers.GetSummaryOnly(groupedTripAuthData, RawDataList, CarRawDataList, HotelRawDataList, includeNotifyOnly, Globals.UserLanguage);
            }
            else
            {
                var rowBuilder = new RowBuilder
                {
                    GroupedTripAuthData = groupedTripAuthData,
                    RawDataList = RawDataList,
                    CarRawDataList = CarRawDataList,
                    HotelRawDataList = HotelRawDataList,
                    TripAuthorizerRawDataList = TripAuthorizerRawDataList,
                    Globals = Globals,
                    ClientFunctions = clientFunctions,
                    OffsetHours = _offsetHours

                };

                FinalDataList = rowBuilder.BuildRows(ClientStore, MasterStore);

                SummaryDataList = TravAuthDetailHelpers.GetSummary(FinalDataList);
            }
        }

        private void SortFinalDataList()
        {
            FinalDataList = FinalDataList.OrderBy(s => s.Acct)
                .ThenBy(s => s.Passlast)
                .ThenBy(s => s.Passfrst)
                .ToList();
        }

        public override bool GenerateReport()
        {

            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    ZeroOut<FinalData>.Process(FinalDataList, new List<string>
                    {
                        "AirChg",
                        "PenaltyAmt",
                        "AirFareBkd",
                        "TktOrgAmt",
                        "AddCollAmt",
                        "TotTripchg",
                        "AirLowFare",
                        "AirLostSvg"
                    });
                    foreach (var row in FinalDataList)
                    {
                        row.Authemail1 = row.Authemail1.SafeLeft(100);
                        row.Authemail2 = row.Authemail2.SafeLeft(100);
                        row.Authemail3 = row.Authemail3.SafeLeft(100);
                        row.Authemail4 = row.Authemail4.SafeLeft(100);
                        row.Authemail5 = row.Authemail5.SafeLeft(100);

                        row.Apvreason1 = row.Apvreason1.SafeLeft(100);
                        row.Apvreason2 = row.Apvreason2.SafeLeft(100);
                        row.Apvreason3 = row.Apvreason3.SafeLeft(100);
                        row.Apvreason4 = row.Apvreason4.SafeLeft(100);
                        row.Apvreason5 = row.Apvreason5.SafeLeft(100);
                    }
                    var exportFieldList = ExportFieldHandler.GetExportFields(Globals.IsParmValueOn(WhereCriteria.CBINCLLOWFARELOSTSVGS), Globals.IsParmValueOn(WhereCriteria.CBINCLAUTHCOMMS), SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks), Globals);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    }
                    break;
                default:

                    CreatePdf();
                    break;
            }
            return true;
        }

        private void CreatePdf()
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);
            ReportSource.SetDataSource(FinalDataList);
            ReportSource.Subreports[0].SetDataSource(SummaryDataList);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            ReportSource.SetParameterValue("lLogGen1", false);
            if (!Globals.Agency.ContainsIgnoreCase("AXI")) ReportSource.SetParameterValue("RPTTITLE", "PCM Request Activity");

            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

        private void GetOffsetValues()
        {
            var tzCode = Globals.GetParmValue(WhereCriteria.TIMEZONE);
            if (string.IsNullOrEmpty(tzCode))
                tzCode = "EST";
            var tz = new GetTimeZoneQuery(new iBankMastersQueryable(), tzCode, Globals.UserLanguage).ExecuteQuery();
           
            _offsetHours = tz.GmtDiff;

            if (Globals.IsParmValueOn(WhereCriteria.OBSERVEDST)) _offsetHours += 1;
        }


    }
    
}


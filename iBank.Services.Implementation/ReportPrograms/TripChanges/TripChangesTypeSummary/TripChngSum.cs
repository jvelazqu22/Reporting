using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.TripChangesTypeSummaryReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesTypeSummary
{
    public class TripChngSum : ReportRunner<RawData, FinalData>
    {
        public DateTime? BeginDate2;
        public DateTime? EndDate2;
        public bool GoodUdid;
        public int UdidNumber;
        public List<RawData> CancelledRawDataList;
        public bool IncludeCancelledTrips;
        public string WhereClauseOriginal;
        public string Where2;
        public string ChangeStampWhere;
        public string CancelStampWhere;
        public int TotalCount { get; set; }
        public int TotalCount2 { get; set; }
        public int TotalCount3 { get; set; }
        public List<SummaryData> SubReportData { get; set; }


        public TripChngSum()
        {
            CrystalReportName = "ibTripChngSum";
        }

        #region ReportRunner Functions

        public override bool InitialChecks()
        {
            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsDateRangeValid()) return false;

            //Date checks
            BeginDate2 = Globals.GetParmValue(WhereCriteria.BEGDATE2).ToDateFromiBankFormattedString();
            EndDate2 = Globals.GetParmValue(WhereCriteria.ENDDATE2).ToDateFromiBankFormattedString();

            if (BeginDate2.HasValue || EndDate2.HasValue)
            {
                if (BeginDate2.HasValue && EndDate2.HasValue && BeginDate2 > EndDate2)
                {
                    Globals.ReportInformation.ReturnCode = 2;
                    Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_DateRange;
                    return false;
                }
            }

            if (!Globals.BeginDate.HasValue && Globals.EndDate.HasValue)
                Globals.BeginDate = Globals.EndDate;

            if (Globals.BeginDate.HasValue && !Globals.EndDate.HasValue)
                Globals.EndDate = Globals.BeginDate;

            if (!BeginDate2.HasValue && EndDate2.HasValue)
                BeginDate2 = EndDate2;

            if (BeginDate2.HasValue && !EndDate2.HasValue)
                EndDate2 = BeginDate2;

            if (!Globals.EndDate.HasValue || !Globals.BeginDate.HasValue ||
                (Globals.BeginDate.Value > Globals.EndDate.Value))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_DateRange;
                return false;
            }

            if (BeginDate2.HasValue && EndDate2.HasValue && (BeginDate2.Value > EndDate2.Value))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.BadCompareTripChanges;
                return false;
            }

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            SetReportParameters();

            //Always history
            Globals.SetParmValue(WhereCriteria.PREPOST, "1");

            var tripCancelYn = Globals.GetParmValue(WhereCriteria.CANCELCODE);
            var changeStampFrom = Globals.GetParmValue(WhereCriteria.CHANGESTAMP).ToDateFromiBankFormattedString();
            var changeStampTo = Globals.GetParmValue(WhereCriteria.CHANGESTAMP2).ToDateFromiBankFormattedString();
            var cancelTimeFrom = Globals.GetParmValue(WhereCriteria.CANCELTIME).ToDateFromiBankFormattedString();
            var cancelTimeTo = Globals.GetParmValue(WhereCriteria.CANCELTIME2).ToDateFromiBankFormattedString();

            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db),
                Globals.Agency);

            if ( !BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: true, ignoreTravel: false)) return false;

            //Build the where changes clause
            if (!BuildWhere.AddBuildWhereChanges())
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_InvalidChangeCodes;
                return false;
            }

            IncludeCancelledTrips = string.IsNullOrEmpty(BuildWhere.WhereClauseChanges) || BuildWhere.IncludeCancelled;

            /** 07/14/2014 - IF THEY CHOSE TRIP CANCELLED = N OR Y, IT **
            **OVERRIDES WHATEVER THEY PICKED FOR TYPE OF CHANGE.     * */
            if (tripCancelYn == "Y")
                IncludeCancelledTrips = true;
            if (tripCancelYn == "N")
                IncludeCancelledTrips = false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            BuildWhere.AddSecurityChecks();

            new TripChngSumRawDataProcessor().UpdateGlobalWhereVariables(BuildWhere, ref WhereClauseOriginal, changeStampFrom, changeStampTo, ref ChangeStampWhere, 
                    cancelTimeFrom, cancelTimeTo, ref CancelStampWhere, BeginDate2, EndDate2, Globals, Where2, GoodUdid, UdidNumber);

            var whereChanges = BuildWhere.WhereClauseChanges;
            if (!string.IsNullOrEmpty(whereChanges))
                whereChanges = " and " + whereChanges;

            var sql = new TripChngSqlCreator().CreateScript(GoodUdid, UdidNumber, WhereClauseOriginal, Where2, whereChanges, ChangeStampWhere);
            var fullSql = $"select {sql.FieldList} from {sql.FromClause} where {sql.WhereClause}";

            //RawDataList = ClientDataRetrieval.GetOpenQueryData<RawData>(fullSql, Globals, BuildWhere.Parameters).ToList();
            //RawDataList = RawDataList.GroupBy(s => new { s.RecKey, s.ChangeCode },
            //                    (k, g) => new RawData { RecKey = k.RecKey, ChangeCode = k.ChangeCode, NumChngs = g.Count() })
            //                        .ToList();

            RawDataList = RetrieveRawData<RawData>(sql, GlobalCalc.IsReservationReport(), false).ToList();
            RawDataList = RawDataList.GroupBy(s => new { s.RecKey, s.ChangeCode },
                            (k, g) => new RawData { RecKey = k.RecKey, ChangeCode = k.ChangeCode, NumChngs = g.Count() })
                                .ToList();

            var changeCodes =
                LookupFunctions.GetAllTripChangeCodes(MasterStore)
                    .Where(s => s.LanguageCode.EqualsIgnoreCase(Globals.UserLanguage))
                    .ToList();

            RawDataList = (from r in RawDataList
                           join c in changeCodes on r.ChangeCode equals c.ChangeCode
                           select
                               new RawData
                               {
                                   RecKey = r.RecKey,
                                   ChangeCode = r.ChangeCode,
                                   ChangeDesc = c.CodeDescription,
                                   ChangeGrp = c.ChangeGroup,
                                   NumChngs = r.NumChngs
                               }).ToList();

            if (IncludeCancelledTrips)
            {
                var sql2 = new TripChngSqlCreator().CreateScriptForIncludeCancelTripsAndUpdateWhere2(GoodUdid, UdidNumber, WhereClauseOriginal, ref Where2, CancelStampWhere);

                fullSql = $"select {sql2.FieldList} from {sql2.FromClause} where {sql2.WhereClause}";

                //CancelledRawDataList = ClientDataRetrieval.GetOpenQueryData<RawData>(fullSql, Globals, BuildWhere.Parameters).ToList();
                CancelledRawDataList = RetrieveRawData<RawData>(sql2, GlobalCalc.IsReservationReport(), false).ToList();
                CancelledRawDataList = CancelledRawDataList
                        .GroupBy(s => s.RecKey,
                            (k, g) =>
                                new RawData
                                {
                                    RecKey = k,
                                    ChangeCode = 101,
                                    ChangeDesc = "Entire Trip Cancelled",
                                    ChangeGrp = "T",
                                    NumChngs = g.Count()
                                })
                        .Where(s => RawDataList.All(t => t.RecKey != s.RecKey))
                        .ToList();

                RawDataList.AddRange(CancelledRawDataList);
            }

            /** PUT "CANCELLED TRIPS" IN WITH "TRIP / TRAVELER INFO CHANGES". **
            ** BY THE WAY, "S" STANDS FOR "SEGMENT CHANGES" IN THIS STUFF;   **
            ** BUT WE'RE GOING TO USE IT FOR ALL "AIR" CHANGES.              **/
            RawDataList.ForEach(s =>
            {
                if (s.ChangeCode == 122 || s.ChangeCode == 124 || s.ChangeCode == 126)
                    s.ChangeGrp = "S";
                else if (!string.IsNullOrEmpty(s.ChangeGrp))
                    s.ChangeGrp = s.ChangeGrp.Trim();
                else
                    s.ChangeGrp = "";

                //s.ChangeGrp = s.ChangeCode == 122 || s.ChangeCode == 124 || s.ChangeCode == 126 
                //    ? "S" 
                //    : !string.IsNullOrEmpty(s.ChangeGrp) 
                //        ? s.ChangeGrp.Trim() 
                //        : "";
            });

            if (!DataExists(RawDataList))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_NoData;
                return false;
            }

            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList = RawDataList.GroupBy(s => new { s.ChangeCode, s.ChangeDesc, s.ChangeGrp }, 
                    (k, g) => new FinalData { Changecode = k.ChangeCode, Changedesc = k.ChangeDesc, Changegrp = k.ChangeGrp, Numchngs = g.Sum(t => t.NumChngs) })
                   .OrderByDescending(t => t.Numchngs)
                   .ThenBy(t => t.Changecode)
                   .ToList();

            /** NEED TO GET THE TOTAL NUMBER OF CHANGES. **/
            var totalChanges = FinalDataList.Sum(s => s.Numchngs);
            TotalCount2 = totalChanges == 0 ? 0 : totalChanges;

            //string fromClause, whereClause = "", fieldList;

            /** FOR THE "TOTAL TRIPS IN DATA SET" FIGURE, **
            ** WE NEED TO GO BACK TO THE DATABASE AGAIN. **/
            var sql = new TripChngSqlCreator().CreateScriptForDistinctTrip(GoodUdid, UdidNumber, WhereClauseOriginal);

            var fullSql = $"select {sql.FieldList} from {sql.FromClause} where {sql.WhereClause}";

            var distinctTrips = RetrieveRawData<RawData>(sql, GlobalCalc.IsReservationReport(), false).ToList()
                                .Select(s => s.RecKey)
                                .Distinct()
                                .ToList();

            //var distinctTrips = ClientDataRetrieval.GetOpenQueryData<RawData>(fullSql, Globals, BuildWhere.Parameters)
            //                    .Select(s => s.RecKey)
            //                    .Distinct()
            //                    .ToList();

            TotalCount = distinctTrips.Any() ? distinctTrips.Count() : 0;

            if (IncludeCancelledTrips)
            {
                /** GET CANCELLED TRIP INFORMATION FOR "TOTAL TRIPS". **/
                var totalCancelledCount = CancelledRawDataList.Count(s => distinctTrips.All(t => t != s.RecKey));

                if (totalCancelledCount > 0)
                    TotalCount += totalCancelledCount;
            }

            CreateSubReportData();

            /** NEED TO GET THE TOTAL TRIPS WITH CHANGES. **/
            TotalCount3 = RawDataList.Select(s => s.RecKey).Distinct().Count();

            return true;
        }

        public override bool GenerateReport()
        {

            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = GetExportFields();

                    if (Globals.OutputFormat == DestinationSwitch.Xls)
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    else
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);

                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    //Create the ReportDocument object and load the .RPT File. 
                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);
                    ReportSource.Subreports[0].SetDataSource(SubReportData);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    ReportSource.SetParameterValue("nTotCnt", TotalCount);
                    ReportSource.SetParameterValue("nTotCnt2", TotalCount2);
                    ReportSource.SetParameterValue("nTotCnt3", TotalCount3);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Set the common report parameters
        /// </summary>
        private void SetReportParameters()
        {
            int udid;
            GoodUdid = int.TryParse(Globals.GetParmValue(WhereCriteria.UDIDNBR), out udid);
            UdidNumber = udid;

        }

        /// <summary>
        /// Create the sub report data used by the main report
        /// </summary>
        private void CreateSubReportData()
        {
            SubReportData = new List<SummaryData>();

            var sumChangesList = FinalDataList.GroupBy(s => s.Changegrp,
                (k, g) => new { ChangeGrp = k, NumChngs = g.Sum(t => t.Numchngs) }).ToList();

            var totalRecordsList = RawDataList.GroupBy(s => new { s.RecKey, s.ChangeGrp },
                (k, g) => new { k.RecKey, k.ChangeGrp, NumRecs = g.Count() });

            var numTripsList = totalRecordsList.GroupBy(s => s.ChangeGrp,
                (k, g) => new { ChangeGrp = k, NumTrip = g.Count() }).ToList();

            var codes = new Dictionary<string, string> { { "T", "Trip / Traveler Info Changes" }, { "S", "Air Changes" }, { "C", "Car Changes" }, { "H", "Hotel Changes" } };

            //Get the totals for each of the change codes listed above
            foreach (var code in codes)
            {
                int totalChanges = 0, totalTrips = 0;
                var change = sumChangesList.FirstOrDefault(s => !string.IsNullOrEmpty(s.ChangeGrp) && s.ChangeGrp.EqualsIgnoreCase(code.Key));
                if (change != null)
                    totalChanges = change.NumChngs;

                var trip = numTripsList.FirstOrDefault(s => !string.IsNullOrEmpty(s.ChangeGrp) && s.ChangeGrp.EqualsIgnoreCase(code.Key));
                if (trip != null)
                    totalTrips = trip.NumTrip;

                SubReportData.Add(new SummaryData { GrpDesc = code.Value, Numchngs = totalChanges, Numtrips = totalTrips });
            }
        }

        /// <summary>
        /// Get a list of fields to be exported to CSV/Excel
        /// </summary>
        /// <returns></returns>
        private List<string> GetExportFields()
        {
            return new List<string> {"changecode", "changedesc", "changegrp", "numchngs"};
        }

        #endregion
    }
}

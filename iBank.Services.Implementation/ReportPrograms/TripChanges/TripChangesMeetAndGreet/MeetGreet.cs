using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.TripChangesMeetAndGreetReport;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.TripChanges.SharedClasses;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities.ClientData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesMeetAndGreet
{
    public class MeetGreet : ReportRunner<RawData, FinalData>
    {
        public DateTime? BeginDate2;
        public DateTime? EndDate2;
        public bool LogGen1;
        public bool LegLevel;
        public bool SuppressChangeDetails;
        public bool ConsolidateChanges;
        public bool UseAirportCodes;
        public bool IncludeEmailAddress;
        public int UdidNumber1;
        public int UdidNumber2;
        public string UdidLabel1;
        public string UdidLabel2;
        public List<UdidData> UdidsNumber1;
        public List<UdidData> UdidsNumber2;
        public List<RawData> CancelledRawDataList;
        public List<FinalData> CancelledFinalDataList;
        public int RowCount;
        public bool IncludeCancelledTrips;
        public string Where2;
        public string SpecWhere;
        public string WhereTrip1;
        public string WhereTrip2;
        public string ChangeStampWhere;
        public string CancelStampWhere;
        public List<string> ExportFields = new List<string>();
        private MeetGreetRawDataProcessor meetGreetRawDataProcessor;
        private readonly TripChangesCalculations _sharedCalc = new TripChangesCalculations();

        public MeetGreet()
        {
            CrystalReportName = "ibMeetGreet";
            ExportFields = new List<string>();
            meetGreetRawDataProcessor = new MeetGreetRawDataProcessor(this);
        }

    #region ReportRunner Functions

    public override bool InitialChecks()
        {
            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsDateRangeValid()) return false;

            BeginDate2 = _sharedCalc.GetBeginDate2(Globals);
            EndDate2 = _sharedCalc.GetEndDate2(Globals);

            if (!_sharedCalc.IsDateRangeValid(BeginDate2, EndDate2, Globals)) return false;

            Globals.BeginDate = _sharedCalc.ReassignDate(Globals.BeginDate, Globals.EndDate);

            Globals.EndDate = _sharedCalc.ReassignDate(Globals.EndDate, Globals.BeginDate);

            BeginDate2 = _sharedCalc.ReassignDate(BeginDate2, EndDate2);

            EndDate2 = _sharedCalc.ReassignDate(EndDate2, BeginDate2);

            if (!IsDateRangeValid()) return false;

            if (!_sharedCalc.IsDateRangeValid(BeginDate2, EndDate2, Globals)) return false;
            
            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            SetReportParameters();

            if (SuppressChangeDetails)
                CrystalReportName = "ibMeetGreet2";

            Globals.SetParmValue(WhereCriteria.PREPOST, "1");

            var tripCancelYn = Globals.GetParmValue(WhereCriteria.CANCELCODE);
            var changeStampFrom = Globals.GetParmValue(WhereCriteria.CHANGESTAMP).ToDateFromiBankFormattedString();
            var changeStampTo = Globals.GetParmValue(WhereCriteria.CHANGESTAMP2).ToDateFromiBankFormattedString();
            var cancelTimeFrom = Globals.GetParmValue(WhereCriteria.CANCELTIME).ToDateFromiBankFormattedString();
            var cancelTimeTo = Globals.GetParmValue(WhereCriteria.CANCELTIME2).ToDateFromiBankFormattedString();
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (Globals.GetParmValue(WhereCriteria.DATERANGE) == "5")
            {
                if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: false, inMemory: true, isRoutingBidirectional: false, legDit: true, ignoreTravel: false)) return false;

                /** 08/25/2006 - APPLY THE DATE RANGE TO RARRDATE, **
                **BUT ADD A FEW DAYS TO BOTH ENDS OF THE RANGE -SQL SYNTAX.  * */
                WhereTrip2 = " and DepDate <= '" + Globals.EndDate.Value.AddDays(45).ToShortDateString() +
                             " 11:59:59 PM' and DepDate >= '" + Globals.BeginDate.Value.AddDays(-45).ToShortDateString() +
                             "' ";
                WhereTrip1 = " and rArrDate <= '" + Globals.EndDate.Value.ToShortDateString() +
                             " 11:59:59 PM' and rArrDate >= '" + Globals.BeginDate.Value.ToShortDateString() +
                             "' ";
            }
            else
            {
                if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: true, ignoreTravel: false)) return false;
            }

            //Build the where changes clause
            if (!BuildWhere.AddBuildWhereChanges())
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_InvalidChangeCodes;
                return false;
            }

            IncludeCancelledTrips = !string.IsNullOrEmpty(BuildWhere.WhereClauseChanges) ? BuildWhere.IncludeCancelled : true;

            /** 07/16/2014 - IF THEY CHOSE TRIP CANCELLED = N OR Y, IT **
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

            //Using WhereTrip2 (filtering by depdate) was causing erroneous results, so removed the filter
            //var whereClauseOriginal = BuildWhere.WhereClauseFull + WhereTrip1 + WhereTrip2;
            var whereClauseOriginal = BuildWhere.WhereClauseFull + WhereTrip1;

            string whereClauseRawData = string.Empty;

            meetGreetRawDataProcessor.UpdateGlobalWhereVariables(BuildWhere, ref SpecWhere, changeStampFrom, changeStampTo,
                ref ChangeStampWhere, cancelTimeFrom, cancelTimeTo, ref CancelStampWhere);

            RawDataList = meetGreetRawDataProcessor.GetRawData(Globals, whereClauseOriginal, ref whereClauseRawData);

            if (!DataExists(RawDataList)) return false;

            // ** 11/07/2005 - ADD ABILITY TO SPECIFY TO UDID #'s **
            if (UdidNumber1 > 0)
                UdidsNumber1 = meetGreetRawDataProcessor.GetUdidsNumber(ref UdidLabel1, UdidNumber1, whereClauseOriginal, Globals, BuildWhere);

            if (UdidNumber2 > 0)
                UdidsNumber2 = meetGreetRawDataProcessor.GetUdidsNumber(ref UdidLabel2, UdidNumber2, whereClauseOriginal, Globals, BuildWhere);

            //Collapse data
            if (!LegLevel)
            {
                RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Last);
                RawDataList = RawDataList.Where(s => s.Connect != "X").ToList();
                RawDataList = RawDataList.Where(w => w.SegNum == 1).ToList();
            }

            RowCount = RawDataList.Count();

            meetGreetRawDataProcessor.UpdateGlobalAndUdids(Globals, BeginDate2, EndDate2, Where2, IncludeCancelledTrips, ref RowCount,
            BuildWhere, whereClauseRawData, CancelStampWhere, ref CancelledRawDataList, LegLevel, UdidNumber1, UdidNumber2, ref UdidsNumber1, ref UdidsNumber2, whereClauseOriginal);

            if (RowCount == 0)
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_NoData;
                return false;
            }

            return true;
        }

        public override bool ProcessData()
        {
            //Apply Routing

            if (LegLevel)
            {
                RawDataList = RawDataList.Where(s => s.Connect != "X").ToList();
                CancelledRawDataList = CancelledRawDataList.Where(s => s.Connect != "X").ToList();
            }

            RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, false);
            var meetGreetFinalDataProcessor = new MeetGreetFinalDataProcessor();

            FinalDataList = meetGreetFinalDataProcessor.GetFinalData(RawDataList, UseAirportCodes, MasterStore, Globals);
            RowCount = FinalDataList.Count;

            if (IncludeCancelledTrips)
            {
                CancelledRawDataList = BuildWhere.ApplyWhereRoute(CancelledRawDataList, false);

                CancelledFinalDataList = meetGreetFinalDataProcessor.GetCancelledFinalData(CancelledRawDataList, UseAirportCodes, MasterStore, SuppressChangeDetails, Globals);

                RowCount += CancelledFinalDataList.Count;
            }

            if (RowCount == 0)
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_NoData;
                return false;
            }

            if (!string.IsNullOrEmpty(Where2))
                Where2 = Where2.Replace("T1.changstamp", "TCL.changstamp");

            var whereChanges = BuildWhere.WhereClauseChanges;
            if (!string.IsNullOrEmpty(whereChanges))
                whereChanges = " and " + whereChanges;

            int udid;
            var goodUdid = int.TryParse(Globals.GetParmValue(WhereCriteria.UDIDNBR), out udid);
            var sql = new MeetGreetSqlCreator().CreateScriptForChangesData(Globals, udid, goodUdid, SpecWhere, whereChanges, Where2, ChangeStampWhere, WhereTrip2);

            var fullSql = SqlProcessor.ProcessSql(sql.FieldList, false, sql.FromClause, sql.WhereClause, sql.OrderBy, Globals);

            var changes = ClientDataRetrieval.GetOpenQueryData<ChangesData>(fullSql, Globals, BuildWhere.Parameters).ToList();

            var changeCodes = LookupFunctions.GetAllTripChangeCodes(MasterStore).Where(s => s.LanguageCode.EqualsIgnoreCase(Globals.UserLanguage)).ToList();

            changes = meetGreetFinalDataProcessor.GetUpdatedChanges(SuppressChangeDetails, changes, changeCodes, Globals, MasterStore);

            /** WE'LL SCREEN OUT THE CHANGES FOR SEGMENTS THAT DON'T APPLY TO THE SEGMENT **
            **BEING REPORTED.CHANGES THAT APPLY TO THE WHOLE TRIP HAVE A SEGMENT = 0, **
            **AND WE WILL INCLUDE ALL OF THEM. */
            var tripsWithChanges = meetGreetFinalDataProcessor.GetTripsWithChanges(FinalDataList, changes);

            //** NOW GET THE RECORDS FROM curRpt1A THAT HAVE NO CHANGES. **
            var datesPresent = BeginDate2.HasValue && EndDate2.HasValue;
            var blankString = new string(' ', 8);

            var tripsWithNoChanges = meetGreetFinalDataProcessor.GetTripsWithNoChanges(tripsWithChanges, FinalDataList, datesPresent, 
                BeginDate2, SuppressChangeDetails, blankString);

            tripsWithChanges.AddRange(tripsWithNoChanges);

            if (IncludeCancelledTrips)
            {
                if (SuppressChangeDetails)
                    tripsWithChanges.RemoveAll(s => CancelledFinalDataList.Any(t => t.Reckey == s.Reckey));

                tripsWithChanges.AddRange(CancelledFinalDataList);
            }

            //Set the Final data list
            FinalDataList = tripsWithChanges.ToList();

            if (!DataExists(FinalDataList)) return false;

            if (!IsUnderOfflineThreshold(FinalDataList)) return false;

            /** 12/20/2004 - NEED TO COLLAPSE ONE MORE TIME.  WE'VE BEEN HAVING A PBLM **
             * WHEN A TRAVELER CHANGES HIS RETURN FLIGHT AFTER HE'S ALREADY COMPLETED **
             * THE OUTGOING FLIGHT.  WHAT HAPPENS IS THAT THE ITINERARY CHANGES, AND  **
             * THE OUTGOING FLIGHT GETS DROPPED, WHICH CHANGES THE SEGNUM.  WE WERE   **
             * USING THE SEGNUM AS A "GROUP BY" CONDITION.     */
            var meetGreetFinalDataGroupAndSort = new MeetGreetFinalDataGroupAndSort();

            if (SuppressChangeDetails)
                FinalDataList = meetGreetFinalDataGroupAndSort.GroupFinalData(FinalDataList);

            FinalDataList = meetGreetFinalDataGroupAndSort.GetSortedFinalList(FinalDataList, Globals.GetParmValue(WhereCriteria.SORTBY));

            //Add the UDID lookups
            FinalDataList.ForEach(s =>
            {
                s.Udidnbr1 = UdidNumber1;
                s.Udidtext1 = GetUdidText(s.Reckey, 1);
                s.Udidnbr2 = UdidNumber2;
                s.Udidtext2 = GetUdidText(s.Reckey, 2);
            });

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportDataList = new ExportFieldsHelper().SetupFinalReportAndSetExportFields(FinalDataList, UdidLabel1, ConsolidateChanges,
                        UdidLabel2, UseAirportCodes, ref ExportFields, IncludeEmailAddress);

                    if (Globals.OutputFormat == DestinationSwitch.Xls)
                        ExportHelper.ListToXlsx(exportDataList, ExportFields, Globals);
                    else
                        ExportHelper.ConvertToCsv(exportDataList, ExportFields, Globals);

                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    //Create the ReportDocument object and load the .RPT File. 
                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    ReportSource.SetParameterValue("cUdidLbl1", UdidLabel1);
                    ReportSource.SetParameterValue("cUdidLbl2", UdidLabel2);
                    ReportSource.SetParameterValue("lLogGen1", LogGen1);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }

        #endregion

        /// <summary>
        /// Set the common report parameters
        /// </summary>
        private void SetReportParameters()
        {
            LogGen1 = Globals.IsParmValueOn(WhereCriteria.CBINCLSUBTOTSBYFLT);

            LegLevel = Globals.IsParmValueOn(WhereCriteria.CBUSECONNECTLEGS);

            SuppressChangeDetails = Globals.IsParmValueOn(WhereCriteria.CBSUPPDETCHANGE);

            ConsolidateChanges = Globals.IsParmValueOn(WhereCriteria.CBCONSOLIDATECHNGES);

            UseAirportCodes = Globals.IsParmValueOn(WhereCriteria.CBUSEAIRPORTCODES);

            IncludeEmailAddress = Globals.IsParmValueOn(WhereCriteria.CBINCLEMAILADDR);

            UdidLabel1 = Globals.GetParmValue(WhereCriteria.UDIDLBL1);
            UdidLabel2 = Globals.GetParmValue(WhereCriteria.UDIDLBL2);

            int udid;
            int.TryParse(Globals.GetParmValue(WhereCriteria.UDIDONRPT1), out udid);
            UdidNumber1 = udid;

            int.TryParse(Globals.GetParmValue(WhereCriteria.UDIDONRPT2), out udid);
            UdidNumber2 = udid;

        }

        /// <summary>
        /// Get the Udid Text for the given rec key
        /// </summary>
        /// <param name="recKey"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private string GetUdidText(int recKey, int number)
        {
            var description = "";
            if (number == 1)
            {
                if (UdidNumber1 > 0)
                {
                    var udidInfo = UdidsNumber1.FirstOrDefault(s => s.RecKey == recKey);
                    description = udidInfo != null ? udidInfo.UdidText.Trim() : "";
                }
            }
            if (number == 2)
            {
                if (UdidNumber2 > 0)
                {
                    var udidInfo = UdidsNumber2.FirstOrDefault(s => s.RecKey == recKey);
                    description = udidInfo != null ? udidInfo.UdidText.Trim() : "";
                }
            }

            return description.PadRight(80);
        }

    }
}

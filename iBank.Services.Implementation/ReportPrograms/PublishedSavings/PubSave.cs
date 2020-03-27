using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;

using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.PublishedSavings;
using Domain.Orm.iBankClientQueries;
using Domain;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.PublishedSavings
{
    public class PubSave : ReportRunner<RawData, FinalData>
    {
        private bool IsReservationReport { get; set; }
        public UserBreaks UserBreaks { get; set; }
        public bool IsPreview { get; set; }

        public PubSave()
        {
            CrystalReportName = "ibPubSave";
        }

        #region ReportRunner functions
        public override bool InitialChecks()
        {

            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            IsPreview = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");

            return true;
        }

        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false,legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            int udid = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(0);
            IsReservationReport = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
            var sql = new SqlCreator().CreateScript(BuildWhere.WhereClauseFull, udid, IsReservationReport);

            RawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, true).ToList();

            if (!DataExists(RawDataList)) return false;

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination)
            {
                RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, GlobalCalc.IsAppliedToLegLevelData());
            }
            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            var tempData = RawDataList.Select(s => new FinalData
            {
                Reckey = s.RecKey,
                Recloc = s.Recloc,
                Invoice = s.Invoice,
                Ticket = s.Ticket,
                Acct = Globals.User.AccountBreak ? s.Acct : Constants.NotApplicable,
                Acctdesc =
                    !Globals.User.AccountBreak ? Constants.NotApplicable : clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, Globals),
                Break1 = !UserBreaks.UserBreak1
                    ? Constants.NotApplicable
                    : string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1,
                Break2 = !UserBreaks.UserBreak2
                    ? Constants.NotApplicable
                    : string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2,
                Break3 = !UserBreaks.UserBreak3
                    ? Constants.NotApplicable
                    : string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(16) : s.Break3,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                ReasonCode = string.IsNullOrEmpty(s.Reascode) ? new string(' ', 5) : s.Reascode,
                Savingcode = string.IsNullOrEmpty(s.Savingcode) && !string.IsNullOrEmpty(s.Reascode) ? s.Reascode : s.Savingcode,
                Airchg = s.Airchg,
                Stndchg =
                    Math.Abs(s.Stndchg) < Math.Abs(s.Airchg) || s.Stndchg == 0 || (s.Stndchg > 0 && s.Airchg < 0)
                        ? s.Airchg
                        : s.Stndchg,
                Depdate = s.RDepDate.ToDateTimeSafe(),
                Origin = ReportHelper.CreateOriginDestCode(s.Origin, s.Mode, s.Airline),
                Orgdesc = AportLookup.LookupAport(MasterStore, s.Origin, s.Mode, s.Airline),
                Destinat = ReportHelper.CreateOriginDestCode(s.Destinat, s.Mode, s.Airline),
                Destdesc = AportLookup.LookupAport(MasterStore, s.Destinat, s.Mode, s.Airline),
                Connect = s.Connect,
                Airline = s.Airline,
                Rdepdate = s.RDepDate.ToDateTimeSafe(),
                Fltno = s.Fltno,
                Carrdesc = LookupFunctions.LookupAline(MasterStore, s.Airline, s.Mode),
                Class = s.Class,
                Seqno = s.SeqNo,
                OrigAcct = s.Acct
            });

            FinalDataList = tempData.Where(s => Math.Abs(s.Stndchg) > Math.Abs(s.Airchg)
            ).Select(s => new FinalData
            {
                Reckey = s.Reckey,
                Recloc = s.Recloc,
                Invoice = s.Invoice,
                Ticket = s.Ticket,
                Acct = s.Acct,
                Acctdesc = s.Acctdesc,
                Break1 = s.Break1,
                Break2 = s.Break2,
                Break3 = s.Break3,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                ReasonCode = s.ReasonCode,
                Savingcode = s.Savingcode,
                Airchg = s.Airchg,
                Stndchg = s.Stndchg,
                Savings = s.Stndchg - s.Airchg,
                //TODO: Uncomment line below to use the other overload of the lookup reason and comment out the line below it to test the LookupReason performance issue
                //Svngdesc = LookupFunctions.LookupReason(s.Savingcode, s.Acct, Globals),
                Svngdesc = clientFunctions.LookupReason(getAllMasterAccountsQuery, s.Savingcode, s.Acct, ClientStore, Globals, MasterStore.MastersQueryDb),
                Depdate = s.Depdate,
                Origin = s.Origin,
                Orgdesc = s.Orgdesc,
                Destinat = s.Destinat,
                Destdesc = s.Destdesc,
                Connect = s.Connect,
                Airline = s.Airline,
                Rdepdate = s.Rdepdate.ToDateTimeSafe(),
                Fltno = s.Fltno,
                Carrdesc = s.Carrdesc,
                Class = s.Class,
                Seqno = s.Seqno,
            }).OrderBy(x => x.Acct)
                .ThenBy(x => x.Break1)
                .ThenBy(x => x.Break2)
                .ThenBy(x => x.Break3)
                .ThenBy(x => x.Svngdesc)
                .ThenBy(x => x.Passlast)
                .ThenBy(x => x.Passfrst)
                .ThenBy(x => x.Reckey)
                .ThenBy(x => x.Seqno)
                .ToList();

            if (!DataExists(FinalDataList)) return false;

            if (!IsUnderOfflineThreshold(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            var sumPageOnly = Globals.IsParmValueOn(WhereCriteria.CBSUMPAGEONLY);

            CrystalReportName = sumPageOnly ? "ibPubSaveSum" : "ibPubSave";

            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = new TopBottomDestinationsData().GetExportFields(Globals, UserBreaks);
                    FinalDataList = ZeroOut<FinalData>.Process(FinalDataList, new List<string> { "airchg", "stndchg", "savings" });

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    }
                    break;
                default:
                    var subReportData = BuildSubReport();

                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    if (sumPageOnly)
                    {
                        ReportSource.SetDataSource(subReportData);
                    }
                    else
                    {
                        ReportSource.SetDataSource(FinalDataList);
                        ReportSource.Subreports[0].SetDataSource(subReportData);
                    }

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }

        #endregion

        #region Helper Functions

        private List<SubReportData> BuildSubReport()
        {
            var subReportList =
                FinalDataList.Select(s => new { s.Reckey, s.Svngdesc, s.Savings })
                    .Distinct()
                    .GroupBy(s => s.Svngdesc, (k, grp) => new SubReportData { Svngdesc = k, NumRecs = grp.Count(), Savings = grp.Sum(s => s.Savings) }).ToList();

            if (!subReportList.Any())
            {
                subReportList.Add(new SubReportData { Svngdesc = "No Published Savings" });
            }

            return subReportList.OrderBy(x=>x.Svngdesc).ToList();
        }

        #endregion
    }
}

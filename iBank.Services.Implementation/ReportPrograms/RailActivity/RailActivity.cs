using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;

using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.RailActivityReport;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;

using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Utilities.ClientData;
using Domain;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.RailActivity
{
    public class RailActivity : ReportRunner<RawData, FinalData>
    {
        public RailActivity()
        {
            AccountName = "";
            ColHead = "";
        }

        public bool ApplyWhereRouteToSegments { get; set; }
        public List<RawData> MktSegsRawDataList { get; set; }
        private string AccountName { get; set; }
        private string ColHead { get; set; }
        private bool PrePostPreview { get; set; }
        private UserInformation User { get; set; }
        private UserBreaks UserBreaks { get; set; }

        public override bool InitialChecks()
        {
            SetProperties();
            
            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            Globals.SetParmValue(WhereCriteria.MODE, Constants.RailMode);
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: false, ignoreTravel: true)) return false;

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

            var sqlCreator = new RailActivitySqlCreator();
            var sql = sqlCreator.CreateScript(BuildWhere.WhereClauseFull, udid, PrePostPreview);
            RawDataList = RetrieveRawData<RawData>(sql, PrePostPreview, true).ToList();

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            
            ApplyWhereRouteToSegments = !string.IsNullOrEmpty(Globals.GetParmValue(WhereCriteria.TXTFLTSEGMENTS)) || !Globals.GetParmValue(WhereCriteria.RBAPPLYTOLEGORSEG).Equals("1");
            
            if (ApplyWhereRouteToSegments)
            {
                if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstDestination || BuildWhere.HasFirstOrigin)
                {
                    var segData = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);
                    segData = BuildWhere.ApplyWhereRoute(segData, false);
                    RawDataList = GetLegDataFromFilteredSegData(RawDataList, segData);
                }
            }
            else
            {
                RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, true);
            }

            HandleMoney();
            
            return true;
        }

        public override bool ProcessData()
        {
            const string Void = "    ** VOID **    ";
            
            var dateSort = Globals.IsParmValueOn(WhereCriteria.CBINCLBREAKBYDATE);
            ColHead = "Departure Date";
            var sortBy = string.Empty;
            if (dateSort)
            {
                sortBy = Globals.GetParmValue(WhereCriteria.DATERANGE);
                switch (sortBy)
                {
                    case "2": //Invoice Date
                        ColHead = LookupFunctions.LookupLanguageTranslation("xInvoiceDate", "Invoice Date",
                            Globals.LanguageVariables);
                        break;

                    case "3": //booked date
                        ColHead = LookupFunctions.LookupLanguageTranslation("ll_BookedDate", "Booked Date",
                            Globals.LanguageVariables);
                        break;

                    default:
                        ColHead = LookupFunctions.LookupLanguageTranslation("ll_DepartureDate", "Departure Date",
                            Globals.LanguageVariables);
                        break;
                }
            }
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            var query = RawDataList.Select(s =>
                new FinalData
                {
                    RecKey = s.RecKey,
                    RecLoc = s.RecLoc,
                    InvDate = s.InvDate ?? DateTime.MinValue,
                    BookDate = s.BookDate ?? DateTime.MinValue,
                    Invoice = s.Invoice,
                    Ticket = s.Ticket,
                    SeqNo = ApplyWhereRouteToSegments ? 0 : s.SeqNo,
                    PseudoCity = s.PseudoCity,
                    Acct = !User.AccountBreak ? "^na^" : s.Acct,
                    AcctDesc = !User.AccountBreak ? "^na^" : clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, Globals),
                    Break1 =
                        !UserBreaks.UserBreak1
                            ? "^na^"
                            : (string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1.Trim()),
                    Break2 =
                        !UserBreaks.UserBreak2
                            ? "^na^"
                            : (string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2.Trim()),
                    Break3 =
                        !UserBreaks.UserBreak3
                            ? "^na^"
                            : (string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(30) : s.Break3.Trim()),
                    PassLast = s.PassLast,
                    PassFrst = s.PassFrst,
                    Origin = s.Origin,
                    OrgDesc = AportLookup.LookupAport(MasterStore, s.Origin, s.Mode, Globals.Agency),
                    Destinat = s.Destinat,
                    DestDesc = AportLookup.LookupAport(MasterStore, s.Destinat, s.Mode, Globals.Agency),
                    Connect = s.Connect,
                    TranType = s.TranType,
                    CardNum = s.TranType.EqualsIgnoreCase("V") ? Void : s.CardNum,
                    DepDate = s.DepDate ?? DateTime.MinValue,
                    RdepDate = s.RDepDate ?? DateTime.MinValue,
                    Airline = LookupFunctions.LookupAlineCode(MasterStore, s.Airline, s.Mode),
                    FltNo = s.fltno,
                    TktDesig = s.TktDesig,
                    AirChg = s.AirChg,
                    OffRdChg = s.OffRdChg,
                    SvcFee = s.SvcFee,
                    SfTranType = s.SFTranType,
                    Exchange = s.Exchange,
                    OrigTicket = s.OrigTicket,
                    PlusMin = s.PlusMin,
                    SortDate = dateSort ? (sortBy == "2" ? s.InvDate ?? DateTime.MinValue : sortBy == "3" ? s.BookDate ?? DateTime.MinValue : s.DepDate ?? DateTime.MinValue) : DateTime.MinValue,
                    Classcode = s.ClassCode
                }).OrderBy(s => s.PseudoCity)
                .ThenBy(s => s.AcctDesc)
                .ThenBy(s => s.Break1)
                .ThenBy(s => s.Break2)
                .ThenBy(s => s.Break3);

            if (dateSort) query = query.ThenBy(s => new RailActivityData().GetSortDate(s, sortBy));

            FinalDataList = query.ThenBy(s => s.PassLast)
                .ThenBy(s => s.PassFrst)
                .ThenBy(s => s.InvDate)
                .ThenBy(s => s.Invoice)
                .ThenBy(s => s.RecKey)
                .ThenBy(s => s.SeqNo)
                .ToList();

            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            //ServiceFees. Not sure why they use two variables here, but might be for readability/convenience
            var excludeServiceFees = PrePostPreview || Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVCFEES);

            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    
                    var zeroFields = excludeServiceFees ? new List<string> { "airchg" } : new List<string> { "airchg", "svcfee" };
                    FinalDataList = ZeroOut<FinalData>.Process(FinalDataList, zeroFields);

                    var exportFieldList = new RailActivityData().GetExportFields(Globals.User.AccountBreak, UserBreaks, excludeServiceFees, PrePostPreview, Globals.User);

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
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    SetCrystalReportParameters(ReportSource, excludeServiceFees);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        private void HandleMoney()
        {
            PerformCurrencyConversion(RawDataList);

            if (!PrePostPreview) HandleServiceFees();
        }

        private void HandleServiceFees()
        {
            if (PrePostPreview) return;

            var tranWhere = string.Empty;
            if (Globals.IsParmValueOn(WhereCriteria.CBTRANDATEWITHINRANGE))
            {
                var beginDate = Globals.BeginDate.Value.ToShortDateString();
                var endDate = Globals.EndDate.Value.ToShortDateString();
                tranWhere += "and trandate between '" + beginDate + "' and '" + endDate + " 11:59:59 PM'";
            }

            tranWhere = tranWhere + BuildWhere.WhereClauseServices;
            string tranFrom;

            int udid;
            var udidParm = Globals.GetParmValue(WhereCriteria.UDIDNBR);
            int.TryParse(udidParm, out udid);

            if (udid != 0)
            {
                tranFrom = "hibtrips T1, hibServices T6A, hibudids T3";
                tranWhere = "T1.reckey = T6A.reckey and " + BuildWhere.WhereClauseFull + tranWhere +
                            " and T1.agency = T6A.agency and T1.reckey = T3.reckey and T6A.svcCode = 'TSF'";
            }
            else
            {
                tranFrom = "hibtrips T1, hibServices T6A";
                tranWhere = "T1.reckey = T6A.reckey and T6A.svcCode = 'TSF' and " + BuildWhere.WhereClauseFull + tranWhere +
                            " and T1.agency = T6A.agency";
            }

            var tranFields = "T1.reckey, T6A.svcamt as svcFee, T6A.moneytype as AirCurrTyp, invdate, bookdate ";

            //for this query, we want to ensure we don't try currency conversion.
            var moneyType = Globals.GetParmValue(WhereCriteria.MONEYTYPE);
            Globals.SetParmValue(WhereCriteria.MONEYTYPE, string.Empty);
            var tranFull = SqlProcessor.ProcessSql(tranFields, false, tranFrom, tranWhere, string.Empty, Globals);

            //reinstate original value for use in international settings.
            Globals.SetParmValue(WhereCriteria.MONEYTYPE, moneyType);

            var svcFees = ClientDataRetrieval.GetUdidFilteredOpenQueryData<ServiceFeeInformation>(tranFull, Globals, BuildWhere.Parameters, PrePostPreview).ToList();

            PerformCurrencyConversion(svcFees);

            svcFees = svcFees.GroupBy(s => s.RecKey).Select(s => new ServiceFeeInformation
                                                                     {
                                                                         RecKey = s.Key,
                                                                         SvcFee = s.Sum(x => x.SvcFee)
                                                                     }).ToList();

            foreach (var svcFee in svcFees)
            {
                var fee = svcFee;
                foreach (var row in RawDataList.Where(s => s.RecKey == fee.RecKey))
                {
                    row.SvcFee = svcFee.SvcFee;
                }
            }
        }

        private void SetCrystalReportParameters(ReportDocument reportSource, bool excludeServiceFees)
        {
            reportSource.SetParameterValue("cColHead1", ColHead);

            reportSource.SetParameterValue("FtrAvgPrice", "Average Ticket Price:");
            reportSource.SetParameterValue("FtrTickets", "Net # of Tickets:");
            reportSource.SetParameterValue("FtrVoidValue", "Value of Voided Tickets:");
            reportSource.SetParameterValue("FtrPcntOfTot", "% of Total:");
            reportSource.SetParameterValue("FtrInclVoids", "Including Voids:");
            if (!PrePostPreview)
            {
                reportSource.SetParameterValue("FtrNetVoidTicks", "Net # of Voided Tickets:");
                reportSource.SetParameterValue("FtrRptTots", "Report Totals:");
                reportSource.SetParameterValue("lInclVoids", BuildWhere.IncludeVoids);
            }
            var totalAirCharge = FinalDataList.Where(v => !v.TranType.EqualsIgnoreCase("V")).GroupBy(s => new { s.RecKey, s.AirChg }).Sum(t => t.Key.AirChg).ToDecimalSafe();
            reportSource.SetParameterValue("nTotChg", totalAirCharge);
            reportSource.SetParameterValue("lExSvcFee", excludeServiceFees);
        }

        private void SetProperties()
        {
            PrePostPreview = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
            CrystalReportName = PrePostPreview ? "ibRailActivity2" : "ibRailActivity";
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
            User = Globals.User;
        }

    }
}
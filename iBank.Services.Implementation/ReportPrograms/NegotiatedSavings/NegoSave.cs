using System;
using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Interfaces;
using Domain.Models.ReportPrograms.NegotiatedSavings;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;

using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.NegotiatedSavings
{
    public class NegoSave : ReportRunner<RawData, FinalData>
    {
        public UserBreaks UserBreaks { get; set; }

        public bool IsPreview { get; set; }
        
        public NegoSave()
        {
            CrystalReportName = "ibNegoSave";
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
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var whereClause = string.IsNullOrWhiteSpace(BuildWhere.WhereClauseAdvanced)
                            ? BuildWhere.WhereClauseFull
                            : BuildWhere.WhereClauseFull + " AND " + BuildWhere.WhereClauseAdvanced;

            int udid;
            var goodUdid = int.TryParse(Globals.GetParmValue(WhereCriteria.UDIDNBR), out udid);
            string fromClause;
            string fieldList;

            if (goodUdid && udid != 0)
            {
                fromClause = IsPreview ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3";
                whereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ', '$$') and " + whereClause;
            }
            else
            {
                fromClause = IsPreview ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";
                whereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and " + whereClause;
            }

            fieldList = "T1.recloc, T1.reckey, invoice, ticket, acct, break1, break2, break3, passlast, passfrst, reascode, savingcode, offrdchg, airchg";

            var fullSql = SqlProcessor.ProcessSql(fieldList, true, fromClause, whereClause, string.Empty, Globals);
            RawDataList = ClientDataRetrieval.GetUdidFilteredOpenQueryData<RawData>(fullSql, Globals, BuildWhere.Parameters, IsPreview).ToList();

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

            FinalDataList = RawDataList.Select(s => new FinalData
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
                ReasonCode = string.IsNullOrEmpty(s.Reascode) ? new string(' ', 2) : s.Reascode,
                Savingcode = string.IsNullOrEmpty(s.Savingcode) && !string.IsNullOrEmpty(s.Reascode) ? s.Reascode : s.Savingcode,
                Negosvgs = ((s.Offrdchg > 0 && s.Airchg < 0)
                        ? 0 - s.Offrdchg
                        : s.Offrdchg == 0
                            ? s.Airchg
                            : s.Offrdchg) - s.Airchg,
                Airchg = s.Airchg,
                Offrdchg = 
                     (s.Offrdchg > 0 && s.Airchg < 0) 
                        ? 0 - s.Offrdchg
                        : s.Offrdchg == 0 
                            ? s.Airchg
                            : s.Offrdchg,
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
                Seqno = s.SeqNo
            }).Where(s => Math.Abs(s.Offrdchg) > Math.Abs(s.Airchg)).OrderBy(x => x.Acct)
                .ThenBy(x => x.Break1)
                .ThenBy(x => x.Break2)
                .ThenBy(x => x.Break3)
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
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = GetExportFields(Globals, UserBreaks);
                    FinalDataList = ZeroOut<FinalData>.Process(FinalDataList, new List<string> { "airchg", "offrdchg", "negosvgs" });

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

        #endregion

        #region Helper Functions
        private List<string> GetExportFields(ReportGlobals globals, UserBreaks userBreaks)
        {
            var fieldList = new List<string>();

            if (Globals.User.AccountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (userBreaks.UserBreak1)
            {
                fieldList.Add($"break1 as {globals.User.Break1Name}");
            }
            if (userBreaks.UserBreak2)
            {
                fieldList.Add($"break2 as {globals.User.Break2Name}");
            }
            if (userBreaks.UserBreak3)
            {
                fieldList.Add($"break3 as {globals.User.Break3Name}");
            }

            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("reckey");
            fieldList.Add("ticket");
            fieldList.Add("rdepdate");
            fieldList.Add("seqno");
            fieldList.Add("origin");
            fieldList.Add("orgdesc");
            fieldList.Add("destinat");
            fieldList.Add("destdesc");
            fieldList.Add("airline");
            fieldList.Add("carrdesc");
            fieldList.Add("class");
            fieldList.Add("connect");
            fieldList.Add("airchg");
            fieldList.Add("offrdchg");
            fieldList.Add("negosvgs");
            

            return fieldList;
        }


        #endregion

    }

}

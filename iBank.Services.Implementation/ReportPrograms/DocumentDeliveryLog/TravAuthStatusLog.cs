using System;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.DocumentDeliveryLog;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Services.Implementation.ReportPrograms.PTARequestActivity;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.DocumentDeliveryLog
{
    public class TravAuthStatusLog : ReportRunner<RawData, FinalData>
    {
        private double _offsetHours;
        private string _timeZoneAbbreviation;
       
        public TravAuthStatusLog()
        {
            CrystalReportName = "ibTravAuthStatusLog";
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
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            //if we apply routing in both directions "SpecRouting" is true. 
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: false, inMemory: true, isRoutingBidirectional: false, legDit: true, ignoreTravel: false);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();
          
            var airSql = SqlBuilder.GetSql(BuildWhere,_offsetHours, Globals);
            RawDataList = RetrieveRawData<RawData>(airSql, GlobalCalc.IsReservationReport(), false).ToList();

            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            return true;
        }

        public override bool ProcessData()
        {
            var authStatuses = new GetMiscParamListQuery(MasterStore.MastersQueryDb, "TRAVAUTHSTAT", Globals.UserLanguage).ExecuteQuery().ToList();
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            var accts = RawDataList.Select(s => s.Acct).Distinct().ToList();
            var acctLookups = accts.Select(s => new Tuple<string, string>(s, clientFunctions.LookupCname(getAllMasterAccountsQuery, s, Globals))).ToList();

            FinalDataList = RawDataList.Select(s => new FinalData
            {
                Reckey = s.RecKey,
                Acct = s.Acct,
                Acctdesc = SpeedLookup.Lookup(s.Acct, acctLookups), 
                Depdate = s.Depdate ?? DateTime.MinValue,
                Recloc = s.Recloc,
                Passfrst = s.Passfrst,
                Passlast = s.Passlast,
                Timezone = _timeZoneAbbreviation,
                Bookedtim = TravAuthStatusLogHelpers.GmtConvert(s.Bookedgmt,_offsetHours,s.Gds),
                Rtvlcode = s.Rtvlcode,
                Travauthno = s.TravAuthNo,
                Sgroupnbr = s.Sgroupnbr,
                Authstatus = s.AuthStatus,
                Statusdesc = PtaLookups.LookupAuthStatus(authStatuses, s.AuthStatus),
                Statustime = s.Statustime.GetValueOrDefault().AddHours(_offsetHours),
                Authlognbr = s.Authlognbr,
                Statusnbr = s.Statusnbr,
                Docstattim = s.DocStatTim.GetValueOrDefault().AddHours(_offsetHours),
                Docsuccess = s.DocSuccess,
                Doctype = s.DocType,
                Docrecips = s.DocRecips,
                Docsubject = s.DocSubject,
                Doctext = string.IsNullOrWhiteSpace(s.DocHtml) ? s.DocText : s.DocHtml,
                Dochtml = s.DocHtml,
                Dlvrespons = s.DlvRespons
            })
            //.Where(w => !string.IsNullOrWhiteSpace(w.Dochtml))
            //.Where(w => w.Reckey == 23058101)
            .OrderBy(s => s.Travauthno)
            .ThenBy(s => s.Statusnbr)
            .ToList();

            if (!DataExists(FinalDataList)) return false;

            foreach (var row in FinalDataList.Where(s => s.Docsubject.Contains("AUTHORIZATION - NEW PASSWORD")))
            {
                row.Dochtml = "The auto-gen user password email text is not displayed for security reasons.";
                row.Doctext = "The auto-gen user password email text is not displayed for security reasons.";
            }
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                   
                    var exportFieldList = TravAuthStatusLogHelpers.GetExportFields();

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals, "", true, $"{Globals.DateDisplay} HH:mm");
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals, true, $"{Globals.DateDisplay} HH:mm");
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

        private void GetOffsetValues()
        {
            var tzCode = Globals.GetParmValue(WhereCriteria.TIMEZONE);
            if (string.IsNullOrEmpty(tzCode))
                tzCode = "EST";
            var tz = new GetTimeZoneQuery(MasterStore.MastersQueryDb, tzCode, Globals.UserLanguage).ExecuteQuery();
            
            _offsetHours = tz.GmtDiff;
            _timeZoneAbbreviation = tz.TimeZoneCode;
            
            if (Globals.IsParmValueOn(WhereCriteria.OBSERVEDST))
            {
                _offsetHours += 1;
                _timeZoneAbbreviation = tz.DstAbbrev;
            }
            

        }
    }
}

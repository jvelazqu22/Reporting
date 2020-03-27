using System;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.PTARequestActivityReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using Domain.Orm.iBankMastersQueries.Other;

namespace iBank.Services.Implementation.ReportPrograms.PTARequestActivity
{
    public class TravAuthStatusDet : ReportRunner<RawData, FinalData>
    {
        private double _offsetHours;
        private string _timeZoneName;
        private UserBreaks _userBreaks;

        private readonly TravAuthStatusDetRawDataRetriever _dataRetriever;

        public TravAuthStatusDet()
        {
            CrystalReportName = "ibTravAuthStatusDet";
            _dataRetriever = new TravAuthStatusDetRawDataRetriever();
        }

        public override bool InitialChecks()
        {
            if (Globals.ParmHasValue(WhereCriteria.TXTPARSEDTSTART)) if (!IsStartParseDateValid()) return false;

            if (Globals.ParmHasValue(WhereCriteria.TXTPARSEDTEND)) if (!IsEndParseDateValid()) return false;

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
            _dataRetriever.OffsetHours = _offsetHours;
            _dataRetriever.TimeZoneName = _timeZoneName;

            if (!_dataRetriever.GetRawData(BuildWhere)) return false;

            RawDataList = _dataRetriever.RawDataList;
            PerformCurrencyConversion(RawDataList);
            return true;
        }

        public override bool ProcessData()
        {
            _userBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);

            FinalDataList = new TravAuthStatusDetFinalData().GetFinalData(MasterStore, Globals, ClientStore, RawDataList, clientFunctions, _userBreaks);

            FinalDataList = TravAuthStatusDetHelpers.ProcessNotifyOnly(FinalDataList, Globals.IsParmValueOn(WhereCriteria.CBINCLNOTIFONLY));

            FinalDataList = TravAuthStatusDetHelpers.ProcessNotRequired(FinalDataList);

            FinalDataList = TravAuthStatusDetHelpers.ProcessOutOfPolicy(FinalDataList,Globals);

            if (!DataExists(FinalDataList)) return false;
            FinalDataList = TravAuthStatusDetHelpers.SortData(FinalDataList, Globals.GetParmValue(WhereCriteria.SORTBY));

            return true;
        }

        public override bool GenerateReport()
        {
            var includeFareInfo = Globals.IsParmValueOn(WhereCriteria.CBINCLFAREINFO);

            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    foreach (var row in FinalDataList)
                    {
                        if (row.Auth1email.Length > 100)
                            row.Auth1email = row.Auth1email.Left(100);
                        if (row.Apvreason.Length > 100)
                            row.Apvreason = row.Apvreason.Left(100);
                    }
                    var exportFieldList = TravAuthStatusDetHelpers.GetExportFields(includeFareInfo, _userBreaks, Globals);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    }
                    else
                    {
                        //ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals, includeHeaders: true, dateTimeFormat: "MM/dd/yyyy HH:mm");
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals, includeHeaders: true, dateTimeFormat: Globals.DateDisplay + " HH:mm");
                    }
                    break;
                default:
                    CreatePdf(includeFareInfo);
                    break;
            }
            return true;
        }

        private void CreatePdf(bool includeFareInfo)
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            ReportSource.SetParameterValue("cTZoneName", TravAuthStatusDetHelpers.BuildTimeZoneCaption(_timeZoneName, Globals));
            ReportSource.SetParameterValue("lLogGen1", includeFareInfo);
            ReportSource.SetParameterValue("xLowestOffered", LookupFunctions.LookupLanguageTranslation("xLowestOffered", "Lowest Offered", Globals.LanguageVariables));
            ReportSource.SetParameterValue("xLostSvgsMsgPart2", LookupFunctions.LookupLanguageTranslation("xLostSvgsMsgPart2", "Lost Savings", Globals.LanguageVariables));
            if (!Globals.Agency.ContainsIgnoreCase("AXI"))
            {
                ReportSource.SetParameterValue("RPTTITLE", LookupFunctions.LookupLanguageTranslation("RPTTITLE", "PCM Request Activity", Globals.LanguageVariables));
            }
            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

        private void GetOffsetValues()
        {
            var tzCode = Globals.GetParmValue(WhereCriteria.TIMEZONE);
            if (string.IsNullOrEmpty(tzCode))
                tzCode = "EST";
            var tz = new GetTimeZoneQuery(MasterStore.MastersQueryDb, tzCode, Globals.UserLanguage).ExecuteQuery();
            
            _timeZoneName = tz.TimeZoneName;
            _offsetHours = tz.GmtDiff;
        }

    }
}

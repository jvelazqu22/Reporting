using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.QuickSummaryReport;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.QuickSummary
{
    public class Qview2 : ReportRunner<RawData, FinalData>
    {
        private bool _splitRail;
        private bool _hasUdid;
        private bool _breakByDomIntl;
        private bool _excludeExceptions;
        private bool _excludeSavings;
        private bool _excludeNegoSvgs;

        private List<CarRawData> _carRawData;
        private List<HotelRawData> _hotelRawData;

        private bool _useComparisonDates;
        private DateTime _beginDate2;
        private DateTime _endDate2;
        private List<RawData> _rawDataComp;
        private List<CarRawData> _carRawDataComp;
        private List<HotelRawData> _hotelRawDataComp;

        public Qview2()
        {
            CrystalReportName = "ibQview2";
        }

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            if (!IsDateRangeValid()) return false;

            SetFlags();



            var beginDate = Globals.GetParmValue(WhereCriteria.BEGDATE2).ToDateFromiBankFormattedString();
            var endDate = Globals.GetParmValue(WhereCriteria.ENDDATE2).ToDateFromiBankFormattedString(true);

            
            if (!beginDate.HasValue && endDate.HasValue)
            {
                beginDate = endDate;
            }
            if (!endDate.HasValue && beginDate.HasValue)
            {
                endDate = beginDate;
            }
            _useComparisonDates = beginDate.HasValue && endDate.HasValue && !_breakByDomIntl;

            if (_useComparisonDates)
            {
                _beginDate2 = beginDate.Value;
                _endDate2 = endDate.Value;

                if (_beginDate2 > _endDate2)
                {
                    Globals.ReportInformation.ReturnCode = 2;
                    Globals.ReportInformation.ErrorMessage = "Your comparison date range doesn't make sense";
                    return false;
                }
            }


            return true;

        }

        public override bool GetRawData()
        {
            
            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), Globals.Agency);
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var airScript = SqlBuilder.BuildTripSql(GlobalCalc.IsReservationReport(), _hasUdid, BuildWhere.WhereClauseFull);

            RawDataList = RetrieveRawData<RawData>(airScript, GlobalCalc.IsReservationReport(), false).ToList();
            
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);
            ProcessTripData(RawDataList);

            var carScript = SqlBuilder.BuildCarSql(GlobalCalc.IsReservationReport(), _hasUdid, BuildWhere.WhereClauseFull);
            
            _carRawData = RetrieveRawData<CarRawData>(carScript, GlobalCalc.IsReservationReport(), false).ToList();
            PerformCurrencyConversion(_carRawData);

           var hotelScript = SqlBuilder.BuildHotelSql(GlobalCalc.IsReservationReport(), _hasUdid, BuildWhere.WhereClauseFull);

            _hotelRawData = RetrieveRawData<HotelRawData>(hotelScript, GlobalCalc.IsReservationReport(), false).ToList();
            PerformCurrencyConversion(_hotelRawData);

            if (!DataExists(RawDataList) && !DataExists(_carRawData) && !DataExists(_hotelRawData)) return false;

            if (_useComparisonDates)
            {
                BuildWhere.SqlParameters[0] = new SqlParameter("t1BeginDate", _beginDate2);
                BuildWhere.SqlParameters[1] = new SqlParameter("t1EndDate", _endDate2);

                _rawDataComp = RetrieveRawData<RawData>(airScript, GlobalCalc.IsReservationReport(), false).ToList();
                PerformCurrencyConversion(_rawDataComp);
                ProcessTripData(_rawDataComp);

                _carRawDataComp = RetrieveRawData<CarRawData>(carScript, GlobalCalc.IsReservationReport(), false).ToList();
                PerformCurrencyConversion(_carRawDataComp);

                _hotelRawDataComp = RetrieveRawData<HotelRawData>(hotelScript, GlobalCalc.IsReservationReport(), false).ToList();
                PerformCurrencyConversion(_hotelRawDataComp);
            }


            return true;
        }

        public override bool ProcessData()
        {
            var translations = new Translations(Globals, _splitRail);
            var currencySymbol = "$";
            var currencyPosition = "L";

            var moneyType = Globals.GetParmValue(WhereCriteria.MONEYTYPE);
            if (!string.IsNullOrEmpty(moneyType))
            {
                var curSettings = new GetCurrencySettingsByMoneyTypeQuery(new iBankMastersQueryable(), moneyType).ExecuteQuery();
                currencySymbol = curSettings.csymbol;
                currencyPosition = curSettings.cleftright;
            }
            

            if (_breakByDomIntl)
            {
                CrystalReportName += "A";
                FinalDataList = RowBuilder.GetTripRowsDit(RawDataList, translations, _splitRail, _excludeExceptions, _excludeSavings, _excludeNegoSvgs, currencySymbol,currencyPosition);
                FinalDataList.AddRange(RowBuilder.GetCarRowsDit(_carRawData, translations, _excludeExceptions, currencySymbol, currencyPosition));
                FinalDataList.AddRange(RowBuilder.GetHotelRowsDit(_hotelRawData, translations, _excludeExceptions, currencySymbol, currencyPosition));
                FinalDataList.AddRange(RowBuilder.GetTotalRowsDit(FinalDataList, translations, _excludeExceptions, currencySymbol, currencyPosition));
               
            }
            else if (_useComparisonDates)
            {
                CrystalReportName += "B";
                FinalDataList = RowBuilder.GetTripRowsComp(RawDataList,_rawDataComp, translations, _splitRail, _excludeExceptions, _excludeSavings, _excludeNegoSvgs, currencySymbol, currencyPosition);
                FinalDataList.AddRange(RowBuilder.GetCarRowsComp(_carRawData,_carRawDataComp, translations, _excludeExceptions, currencySymbol, currencyPosition));
                FinalDataList.AddRange(RowBuilder.GetHotelRowsComp(_hotelRawData, _hotelRawDataComp, translations, _excludeExceptions, currencySymbol, currencyPosition));
                FinalDataList.AddRange(RowBuilder.GetTotalRowsComp(FinalDataList, translations, _excludeExceptions, currencySymbol, currencyPosition));
                
            }
            else
            {
                FinalDataList = RowBuilder.GetTripRows(RawDataList, translations, _splitRail, _excludeExceptions, _excludeSavings, _excludeNegoSvgs, currencySymbol, currencyPosition);
                FinalDataList.AddRange(RowBuilder.GetCarRows(_carRawData, translations, _excludeExceptions, currencySymbol, currencyPosition));
                FinalDataList.AddRange(RowBuilder.GetHotelRows(_hotelRawData, translations, _excludeExceptions, currencySymbol, currencyPosition));
                FinalDataList.AddRange(RowBuilder.GetTotalRows(FinalDataList, translations, _excludeExceptions, currencySymbol, currencyPosition));
            }

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = GetExportFields().ToList();
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

                    if (_useComparisonDates)
                    {
                        ReportSource.SetParameterValue("cDateDesc1", string.Format("From {0} to {1}", Globals.BeginDate.Value.ToShortDateString(), Globals.EndDate.Value.ToShortDateString()));
                        ReportSource.SetParameterValue("cDateDesc2", string.Format("From {0} to {1}", _beginDate2.ToShortDateString(), _endDate2.ToShortDateString()));
                    }

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        private IList<string> GetExportFields()
        {
            return new List<string>
            {
                "rownum",
                "grp",
                "grpsort",
                "subgrp",
                "descrip",
                "totsdecimal",
                "tots",
                "avgs",
                "svgs",
                "tots1",
                "tots1decimal",
                "avgs1",
                "svgs1",
                "tots2",
                "tots2decimal",
                "avgs2",
                "svgs2",
                "tots3",
                "tots3decimal",
                "avgs3",
                "svgs3"
            };
        }

        private void ProcessTripData(List<RawData> list)
        {
            var reasExclude = Globals.AgencyInformation.ReasonExclude.Split(',').ToList();

            foreach (var row in list)
            {
                row.Offrdchg = row.Offrdchg > 0 && row.Airchg < 0
                    ? 0 - row.Offrdchg
                    : row.Offrdchg == 0 ? row.Airchg : row.Offrdchg;
                row.Stndchg = Math.Abs(row.Stndchg) < Math.Abs(row.Airchg) || row.Stndchg == 0 || (row.Stndchg > 0 && row.Airchg < 0)
                    ? row.Airchg
                    : row.Stndchg;
                row.Savings = 0;
                row.Lostamt = 0;
                row.Negosvngs = 0;

                row.Lostamt = row.Airchg - row.Offrdchg;
                row.Savings = row.Stndchg - row.Airchg;

                if (string.IsNullOrEmpty(row.Reascode) || reasExclude.Contains(row.Reascode))
                {
                    row.Reascode = string.Empty;
                }

                if (string.IsNullOrEmpty(row.Savingcode) && !string.IsNullOrEmpty(row.Reascode) && row.Lostamt == 0 && row.Savings > 0)
                {
                    row.Savingcode = row.Reascode;
                    row.Reascode = string.Empty;
                }

                if ((row.Lostamt < 0 && row.Plusmin > 0) || (row.Lostamt > 0 && row.Plusmin < 0))
                {
                    row.Negosvngs = 0 - row.Lostamt;
                    row.Lostamt = 0;
                }
            }
        }

        private void SetFlags()
        {
            _splitRail = Globals.IsParmValueOn(WhereCriteria.CBSEPARATERAIL);
            _hasUdid = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1) > 0;
            _breakByDomIntl = Globals.IsParmValueOn(WhereCriteria.CBSHOWBRKBYDOMINTL);

            _excludeExceptions = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDEEXCEPTNS);
            _excludeSavings = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVGS);
            _excludeNegoSvgs = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDENEGOT);
        }
    }
}

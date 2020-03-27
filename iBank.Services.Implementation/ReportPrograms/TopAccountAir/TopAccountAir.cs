using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.Graphs;
using Domain.Models.ReportPrograms.TopAccountAir;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopAccountAir
{
    public class TopAccountAir : ReportRunner<RawData, FinalData>
    {
        private string _sortBy;
        private bool _isReservation;
        private bool _incLowFareLostSvgs;
        private bool _isGraphOutput;
        private bool _hasRouting;
        private List<string> graphOutputTypes = new List<string> { "4", "6", "TG", "XG" };

        public int TotCount;
        public decimal TotCharge;
        public decimal TotCommission;
        public decimal TotSvcFee;
        public decimal TotLostAmt;

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            GetReportParameters();

            if (_isReservation && _sortBy.Equals("4"))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = "You cannot sort the Preview version of this report by commission.";
                return false;
            }

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            //if we apply routing in both directions "SpecRouting" is true. 
            BuildWhere.BuildAll(getAllMasterAccountsQuery, true, false, false, false, true, true, false, Globals.IsParmValueOn(WhereCriteria.CBAPPLYBOTHDIRS), false, false);

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
            if (_hasRouting)
            {
                // Get data if there is route criteria
                var airSql = SqlBuilder.GetSqlWithLegs(udidNumber > 0, _isReservation, BuildWhere.WhereClauseFull);
                RawDataList = RetrieveRawData<RawData>(airSql, _isReservation).ToList();
                if (!IsUnderOfflineThreshold(RawDataList)) return false;

                if (!Globals.ParmValueEquals(WhereCriteria.RBAPPLYTOLEGORSEG, "1"))
                {
                    RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);
                }

                RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, false, false);

                var temp = RawDataList.GroupBy(s => new { s.RecKey, s.SourceAbbr, s.Acct }, (key, recs) => key).ToList();

                //must have AirCurrTyp,BookDate and InvDate for currency conversion
                RawDataList = RawDataList.Join(temp, r => r.RecKey, t => t.RecKey, (r, t) => new RawData
                {
                    RecKey = t.RecKey,
                    SourceAbbr = r.SourceAbbr,
                    Acct = r.Acct,
                    AirChg = r.AirChg,
                    OffrdChg = r.OffrdChg,
                    SvcFee = r.SvcFee,
                    ACommisn = r.ACommisn,
                    Plusmin = r.Plusmin,
                    AirCurrTyp = r.AirCurrTyp,
                    BookDate = r.BookDate,
                    InvDate = r.InvDate
                })
                .GroupBy(s => s.RecKey, (key, recs) =>
                {
                    var firstRec = recs.FirstOrDefault() ?? new RawData();
                    return new RawData
                    {
                        RecKey = key,
                        SourceAbbr = firstRec.SourceAbbr,
                        Acct = firstRec.Acct,
                        AirChg = firstRec.AirChg,
                        OffrdChg = firstRec.OffrdChg,
                        SvcFee = firstRec.SvcFee,
                        ACommisn = firstRec.ACommisn,
                        Plusmin = firstRec.Plusmin,
                        AirCurrTyp = firstRec.AirCurrTyp,
                        BookDate = firstRec.BookDate,
                        InvDate = firstRec.InvDate
                    };
                })
                .ToList();
            }
            else
            {
                // Get data if no route criteria
                var airSql = SqlBuilder.GetSql(udidNumber > 0, _isReservation, BuildWhere.WhereClauseFull);
                RawDataList = RetrieveRawData<RawData>(airSql, _isReservation, false).ToList();
            }

            // Get Service fee data if necessary
            var svcFeeData = _isReservation ? new List<SvcFeeData>() : GetServiceFeeData();
            if (!_isReservation)
            {
                if (_hasRouting) svcFeeData = TopAccountAirHelpers.FilterSvcFees(svcFeeData, RawDataList);
            }
            RawDataList = TopAccountAirDataProcessor.AddSvcFees(svcFeeData, RawDataList);
            //Data Checks
            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            // Set up data processing
            var getMasterAccounts = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            var getParentAccounts = new GetAllParentAccountsQuery(ClientStore.ClientQueryDb);
            var dataProcessor = new TopAccountAirDataProcessor(Globals, clientFunctions, getParentAccounts, getMasterAccounts);

            // Convert raw data to final data
            FinalDataList = dataProcessor.ConvertRawDatatoFinalData(RawDataList, _isReservation, MasterStore);

            //Check that there's still data
            if (!DataExists(FinalDataList)) return false;

            //Sort data
            FinalDataList = TopAccountAirHelpers.SortData(FinalDataList, !Globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "2") && !_sortBy.Equals("5"), _sortBy);

            // Get summary amounts
            TotCount = FinalDataList.Sum(s => s.Trips);
            TotCharge = FinalDataList.Sum(s => s.Amt);
            TotCommission = FinalDataList.Sum(s => s.Acommisn);
            TotSvcFee = FinalDataList.Sum(s => s.Svcfee);
            TotLostAmt = FinalDataList.Sum(s => s.LostAmt);

            // Trim list to number wanted
            var howMany = Globals.GetParmValue(WhereCriteria.HOWMANY).TryIntParse(0);
            if (howMany > 0) FinalDataList = FinalDataList.Take(howMany).ToList();
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = TopAccountAirHelpers.GetExportFields(Globals.GetParmValue(WhereCriteria.GROUPBY), _incLowFareLostSvgs);

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

                    if (_isGraphOutput)
                    {
                        return GenerateGraph();
                    }
                    else
                    {
                        return GeneratePDF();
                    }
            }
            return true;
        }

        // Creates PDF report
        private bool GeneratePDF()
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            ReportSource.SetParameterValue("cColHead1", TopAccountAirHelpers.GetColHead1(Globals.GetParmValue(WhereCriteria.GROUPBY)));
            ReportSource.SetParameterValue("nTotCnt", TotCount);
            ReportSource.SetParameterValue("nTotChg", TotCharge);
            ReportSource.SetParameterValue("nSvcFee", TotSvcFee);
            ReportSource.SetParameterValue("nTotComm", TotCommission);
            ReportSource.SetParameterValue("lPreview", _isReservation);
            ReportSource.SetParameterValue("cSortBy", _sortBy);
            if (CrystalReportName.Right(1).Equals("2")) ReportSource.SetParameterValue("nTotLost", TotLostAmt);

            CrystalFunctions.CreatePdf(ReportSource, Globals);
            return true;
        }


        // Creates Graph report
        private bool GenerateGraph()
        {
            string graphTitle;
            switch (_sortBy)
            {
                case "2":
                    graphTitle = "Avg Cost per Trip";
                    break;
                case "3":
                    graphTitle = "# of Trips";
                    break;
                case "4":
                    graphTitle = "Commissions";
                    break;
                default:
                    graphTitle = "Volume Booked";
                    break;
            }

            var graphData = FinalDataList.Select(s => new Graph1FinalData
            {
                CatDesc = s.Account.Left(30),
                Data1 = TopAccountAirHelpers.GetGraphData1(s, _sortBy)
            }).ToList();

            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(graphData);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("cGrDataType", _sortBy.Equals("3") ? "N" : "C");
            ReportSource.SetParameterValue("cCatDesc", TopAccountAirHelpers.GetColHead1(Globals.GetParmValue(WhereCriteria.GROUPBY)));
            ReportSource.SetParameterValue("cSubTitle", graphTitle);
            ReportSource.SetParameterValue("cGrColHdr1", Globals.BeginDate.Value.ToShortDateString() + " - " + Globals.EndDate.Value.ToShortDateString());

            CrystalFunctions.CreatePdf(ReportSource, Globals);
            return true;
        }

        // Gets global report parameters and variables
        private void GetReportParameters()
        {
            _sortBy = Globals.GetParmValue(WhereCriteria.SORTBY);
            _isReservation = GlobalCalc.IsReservationReport();
            _incLowFareLostSvgs = Globals.IsParmValueOn(WhereCriteria.CBINCLLOWFARELOSTSVGS);
            _isGraphOutput = graphOutputTypes.Contains(Globals.GetParmValue(WhereCriteria.OUTPUTTYPE));
            _hasRouting = BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstDestination || BuildWhere.HasFirstOrigin || Globals.ParmHasValue(WhereCriteria.MODE);
            CrystalReportName = GetCrystalReportName(_isGraphOutput, _incLowFareLostSvgs);
            Globals.UseHibServices = _isReservation ? false : true;
        }


        // Gets service fee data
        private List<SvcFeeData> GetServiceFeeData()
        {
            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);
            var sql = SqlBuilder.GetSqlSvcFees(udidNumber > 0, BuildWhere.WhereClauseFull);
            var svcFeeData = RetrieveRawData<SvcFeeData>(sql, _isReservation, false).ToList();

            return svcFeeData;
        }

        // Generate Crystal Report name
        private string GetCrystalReportName(bool isGraph, bool incLowFareLostSvgs)
        {
            if (isGraph) return "ibGraph1";
            return incLowFareLostSvgs ? "ibTopAcctAir2" : "ibTopAcctAir";
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.TravAuthKpi;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.TravAuthKpi
{  
    public class TravAuthKpi : ReportRunner<RawData, FinalData>
    {
        public List<KpiTravelersRawData> TbTravelersRawData { get; set; }
        public List<KpiApproversRawData> TbApproversRawData { get; set; }

        public List<KpiTravelersData> TopTravelers { get; set; }
        public List<KpiApproversData> TopApprovers { get; set; }
        public List<KpiApproversData> TopDecliners { get; set; }
        public List<KpiApproversData> TopNoAction { get; set; }

        //Reason code lists
        public List<ReasonCodeRawData> ReasonsCyMth { get; set; }
        public List<ReasonCodeRawData> ReasonsCyYtd { get; set; }
        public List<ReasonCodeRawData> ReasonsPyMth { get; set; }
        public List<ReasonCodeRawData> ReasonsPyYtd { get; set; }

        private FinalData _finalData;
        private TravAuthKPIDateValidator _dateValidator;
        private TravAuthKPIRawDataRetriever _dataRetriever;

        public override bool InitialChecks()
        {
            _dateValidator = new TravAuthKPIDateValidator();
            _dataRetriever = new TravAuthKPIRawDataRetriever();

            if (!_dateValidator.Validate(Globals)) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            Globals.SetParmValue(WhereCriteria.PREPOST, "1");
            _dataRetriever.IsReservation = true;
            
            if (!_dataRetriever.GetRawData(BuildWhere)) return false;

            RawDataList = _dataRetriever.RawDataList;
            _finalData = _dataRetriever.FinalData;
            FinalDataList = _dataRetriever.FinalDataList;
            TbTravelersRawData = _dataRetriever.TbTravelersRawData;
            TbApproversRawData = _dataRetriever.TbApproversRawData;
            ReasonsCyMth = _dataRetriever.ReasonsCyMth;
            ReasonsCyYtd = _dataRetriever.ReasonsCyYtd;
            ReasonsPyMth = _dataRetriever.ReasonsPyMth;
            ReasonsPyYtd = _dataRetriever.ReasonsPyYtd;
 
            return true;
        }

        public override bool ProcessData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
           
            ReasonsCyMth = TravAuthKpiHelpers.SplitCodes(ReasonsCyMth, Globals);
            ReasonsCyYtd = TravAuthKpiHelpers.SplitCodes(ReasonsCyYtd, Globals);
            ReasonsPyMth = TravAuthKpiHelpers.SplitCodes(ReasonsPyMth, Globals);
            ReasonsPyYtd = TravAuthKpiHelpers.SplitCodes(ReasonsPyYtd, Globals);

            //summarize
            ReasonsCyMth = TravAuthKpiHelpers.GroupReasonsAgain(ReasonsCyMth, clientFunctions, getAllMasterAccountsQuery, Globals, ClientStore, MasterStore);
            ReasonsCyYtd = TravAuthKpiHelpers.GroupReasonsAgain(ReasonsCyYtd, clientFunctions, getAllMasterAccountsQuery, Globals, ClientStore, MasterStore);
            ReasonsPyMth = TravAuthKpiHelpers.GroupReasonsAgain(ReasonsPyMth, clientFunctions, getAllMasterAccountsQuery, Globals, ClientStore, MasterStore);
            ReasonsPyYtd = TravAuthKpiHelpers.GroupReasonsAgain(ReasonsPyYtd, clientFunctions, getAllMasterAccountsQuery, Globals, ClientStore, MasterStore);

            if (ReasonsCyMth.Count >= 1) SetReasonCode1Values();

            if (ReasonsCyMth.Count >= 2) SetReasonCode2Values();

            if (ReasonsCyMth.Count >= 3) SetReasonCode3Values();

            SetTopTravelers();

            SetTopApproversTopDeclinersAndTopNoAction();

            return true;
        }

        private void SetReasonCode1Values()
        {
            _finalData.Reascode1 = ReasonsCyMth[0].OutPolCods;
            _finalData.Reascd1Cm = ReasonsCyMth[0].NumTrips;
            var reas = ReasonsCyYtd.FirstOrDefault(s => s.OutPolCods.Equals(_finalData.Reascode1));

            if (reas != null) _finalData.Reascd1Cy = reas.NumTrips;

            reas = ReasonsPyMth.FirstOrDefault(s => s.OutPolCods.Equals(_finalData.Reascode1));

            if (reas != null) _finalData.Reascd1Py = reas.NumTrips;

            reas = ReasonsPyYtd.FirstOrDefault(s => s.OutPolCods.Equals(_finalData.Reascode1));

            if (reas != null) _finalData.Reascd1Pm = reas.NumTrips;
        }

        private void SetReasonCode2Values()
        {
            _finalData.Reascode2 = ReasonsCyMth[1].OutPolCods;
            _finalData.Reascd2Cm = ReasonsCyMth[1].NumTrips;
            var reas = ReasonsCyYtd.FirstOrDefault(s => s.OutPolCods.Equals(_finalData.Reascode2));
            if (reas != null) _finalData.Reascd2Cy = reas.NumTrips;

            reas = ReasonsPyMth.FirstOrDefault(s => s.OutPolCods.Equals(_finalData.Reascode2));
            if (reas != null) _finalData.Reascd2Py = reas.NumTrips;

            reas = ReasonsPyYtd.FirstOrDefault(s => s.OutPolCods.Equals(_finalData.Reascode2));
            if (reas != null) _finalData.Reascd2Pm = reas.NumTrips;
        }

        private void SetReasonCode3Values()
        {
            _finalData.Reascode3 = ReasonsCyMth[2].OutPolCods;
            _finalData.Reascd3Cm = ReasonsCyMth[2].NumTrips;
            var reas = ReasonsCyYtd.FirstOrDefault(s => s.OutPolCods.Equals(_finalData.Reascode3));
            if (reas != null) _finalData.Reascd3Cy = reas.NumTrips;

            reas = ReasonsPyMth.FirstOrDefault(s => s.OutPolCods.Equals(_finalData.Reascode3));
            if (reas != null) _finalData.Reascd3Py = reas.NumTrips;

            reas = ReasonsPyYtd.FirstOrDefault(s => s.OutPolCods.Equals(_finalData.Reascode3));
            if (reas != null) _finalData.Reascd3Pm = reas.NumTrips;
        }

        private void SetTopTravelers()
        {
            TopTravelers = TbTravelersRawData.GroupBy(s => new { s.PassLast, s.PassFrst }, (key, recs) =>
                {
                    var reclist = recs.ToList();
                    return new KpiTravelersData
                    {
                        PassLast = key.PassLast,
                        PassFrst = key.PassFrst,
                        PassName = (key.PassFrst + " " + key.PassLast).PadRight(30),
                        Numtrips = reclist.Sum(s => s.NumRecs),
                        Statarecs = reclist.Sum(s => s.AuthStatus.Equals("A") ? s.NumRecs : 0),
                        Statprecs = reclist.Sum(s => s.AuthStatus.Equals("P") ? s.NumRecs : 0),
                        Statdrecs = reclist.Sum(s => s.AuthStatus.Equals("D") ? s.NumRecs : 0),
                        Staterecs = reclist.Sum(s => s.AuthStatus.Equals("E") ? s.NumRecs : 0),
                    };
                })
                .OrderByDescending(s => s.Numtrips)
                .ThenBy(s => s.PassLast)
                .ThenBy(s => s.PassFrst)
                .Take(5)
                .ToList();
        }

        private void SetTopApproversTopDeclinersAndTopNoAction()
        {
            var approversTemp = TbApproversRawData.GroupBy(s => TravAuthKpiHelpers.LookupAuthorizerName(s.AuthrzrNbr, s.Auth1Email, ClientStore.ClientQueryDb), (key, recs) =>
                {
                    var reclist = recs.ToList();
                    return new KpiApproversData
                    {
                        Approver = key,
                        Numtrips = reclist.Sum(s => s.NumRecs),
                        Statarecs = reclist.Sum(s => s.AuthStatus.Equals("A") ? s.NumRecs : 0),
                        Statprecs = reclist.Sum(s => s.AuthStatus.Equals("P") ? s.NumRecs : 0),
                        Statdrecs = reclist.Sum(s => s.AuthStatus.Equals("D") ? s.NumRecs : 0),
                        Staterecs = reclist.Sum(s => s.AuthStatus.Equals("E") ? s.NumRecs : 0),
                    };
                })
                .ToList();

            TopApprovers = approversTemp.OrderByDescending(s => s.Statarecs)
                .ThenBy(s => s.Numtrips)
                .ThenBy(s => s.Approver)
                .Take(5)
                .ToList();

            TopDecliners = approversTemp.OrderByDescending(s => s.Statdrecs)
                .ThenBy(s => s.Numtrips)
                .ThenBy(s => s.Approver)
                .Take(5)
                .ToList();

            TopNoAction = approversTemp.OrderByDescending(s => s.Staterecs)
                .ThenBy(s => s.Numtrips)
                .ThenBy(s => s.Approver)
                .Take(5)
                .ToList();
        }

        public override bool GenerateReport()
        {
            CrystalReportName = Globals.IsParmValueOn(WhereCriteria.CBINCLNOTIFONLY)
                ? "ibTravAuthKpi"
                : "ibTravAuthKpiNotNotify";
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);
            ReportSource.Subreports["kpiTopTravelers"].SetDataSource(TopTravelers);
            ReportSource.Subreports["kbiTopNoAction"].SetDataSource(TopNoAction);
            ReportSource.Subreports["kpiTopApprovers"].SetDataSource(TopApprovers);
            ReportSource.Subreports["kpiBottomApprovers"].SetDataSource(TopDecliners);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            ReportSource.SetParameterValue("nYear", _dateValidator.Year);
            ReportSource.SetParameterValue("cMthName", _dateValidator.MonthName);
           
            CrystalFunctions.CreatePdf(ReportSource, Globals);

            return true;
        }
     
    }
}

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TravAuthKpi;
using iBank.Services.Implementation.Utilities;
using iBank.Services.Implementation.Utilities.ClientData;
using Domain.Orm.iBankClientQueries;
using iBankDomain.RepositoryInterfaces;
using iBank.Repository.SQL.Repository;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.TravAuthKpi
{
    public class TravAuthKPIRawDataRetriever
    {
        private readonly ReportRunConditionals _conditionals = new ReportRunConditionals();

        private BuildWhere BuildWhere { get; set; }
        private ReportGlobals Globals { get; set; }
        public List<RawData> RawDataList { get; set; } = new List<RawData>();
        public WhereClauses Clauses { get; set; }
        public string DefaultWhereClause { get; set; }
        public bool IsReservation { get; set; }

        public List<KpiTravelersRawData> TbTravelersRawData { get; set; } = new List<KpiTravelersRawData>();
        public List<KpiApproversRawData> TbApproversRawData { get; set; } = new List<KpiApproversRawData>();
                
        public List<FinalData> FinalDataList { get; set; } = new List<FinalData>();
        public List<ReasonCodeRawData> ReasonsCyMth { get; set; } = new List<ReasonCodeRawData>();
        public List<ReasonCodeRawData> ReasonsCyYtd { get; set; } = new List<ReasonCodeRawData>();
        public List<ReasonCodeRawData> ReasonsPyMth { get; set; } = new List<ReasonCodeRawData>();
        public List<ReasonCodeRawData> ReasonsPyYtd { get; set; } = new List<ReasonCodeRawData>();
        public FinalData FinalData { get; set; }

        private IClientQueryable ClientQueryableDb
        {
            get
            {
                return new iBankClientQueryable(Globals.AgencyInformation.ServerName, Globals.AgencyInformation.DatabaseName);
            }
        }

        private iBankMastersQueryable MastersQueryDb
        {
            get
            {
                return new iBankMastersQueryable();
            }
        }

        private ICommandDb ClientCommandDb
        {
            get
            {
                return new iBankClientCommandDb(Globals.AgencyInformation.ServerName, Globals.AgencyInformation.DatabaseName);
            }
        }

        public bool GetRawData(BuildWhere buildWhere)
        {
            BuildWhere = buildWhere;
            Globals = BuildWhere.ReportGlobals;

            DefaultWhereClause = TravAuthKpiHelpers.GetDefaultWhereClause(BuildWhere);
            Clauses = TravAuthKpiHelpers.GetWhereClauses(Globals);

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientQueryableDb, Globals.Agency);

            //if we apply routing in both directions "SpecRouting" is true. 
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true,isRoutingBidirectional: false,legDit: false, ignoreTravel: false);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            BuildWhere.AddSecurityChecks();
            
            var retriever = new DataRetriever(ClientQueryableDb);

            #region Get the Auth Status Totals (this will be our "raw data")
            var sql = SqlBuilder.GetSqlAuthStatusTotals(BuildWhere, DefaultWhereClause, Clauses);

            RawDataList = retriever.GetData<RawData>(sql, BuildWhere,false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();
            RawDataList = RawDataList.GroupBy(s => s.AuthStatus, (key, recs) => new RawData
            {
                AuthStatus = key,
                NumRecs = recs.Select(s => s.RecKey).Distinct().Count()
            }).ToList();

            if (!_conditionals.DataExists(RawDataList, Globals)) return false;

            FinalData = new FinalData();
            FinalDataList.Add(FinalData);
            FinalData.Approved = RawDataList.Where(s => s.AuthStatus.EqualsIgnoreCase("A")).Sum(s => s.NumRecs);
            FinalData.Declined = RawDataList.Where(s => s.AuthStatus.EqualsIgnoreCase("D")).Sum(s => s.NumRecs);
            FinalData.Pending = RawDataList.Where(s => s.AuthStatus.EqualsIgnoreCase("P")).Sum(s => s.NumRecs);
            FinalData.Expired = RawDataList.Where(s => s.AuthStatus.EqualsIgnoreCase("E")).Sum(s => s.NumRecs);
            #endregion

            #region "Behaviour Detail by # of Tickets" 14 day domestic
            sql = SqlBuilder.GetSql14DayDomestic(BuildWhere, DefaultWhereClause, Clauses.WhereCyMth);
            var oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Adv14Daycm = oneValue.Value;
            }

            sql = SqlBuilder.GetSql14DayDomestic(BuildWhere, DefaultWhereClause, Clauses.WhereCyYtd);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Adv14Daycy = oneValue.Value;
            }

            sql = SqlBuilder.GetSql14DayDomestic(BuildWhere, DefaultWhereClause, Clauses.WherePyMth);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Adv14Daypm = oneValue.Value;
            }

            sql = SqlBuilder.GetSql14DayDomestic(BuildWhere, DefaultWhereClause, Clauses.WherePyYtd);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Adv14Daypy = oneValue.Value;
            }
            #endregion

            #region "Behaviour Detail by # of Tickets" 21 day international
            sql = SqlBuilder.GetSql21DayInternational(BuildWhere, DefaultWhereClause, Clauses.WhereCyMth);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Adv21Daycm = oneValue.Value;
            }

            sql = SqlBuilder.GetSql21DayInternational(BuildWhere, DefaultWhereClause, Clauses.WhereCyYtd);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Adv21Daycy = oneValue.Value;
            }

            sql = SqlBuilder.GetSql21DayInternational(BuildWhere, DefaultWhereClause, Clauses.WherePyMth);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Adv21Daypm = oneValue.Value;
            }

            sql = SqlBuilder.GetSql21DayInternational(BuildWhere, DefaultWhereClause, Clauses.WherePyYtd);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Adv21Daypy = oneValue.Value;
            }
            #endregion

            #region "Behaviour Detail by # of Tickets" ACCEPTED LOWEST FARE
            sql = SqlBuilder.GetSqlAcceptedLowestFare(BuildWhere, DefaultWhereClause, Clauses.WhereCyMth);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Acclfarecm = oneValue.Value;
            }

            sql = SqlBuilder.GetSqlAcceptedLowestFare(BuildWhere, DefaultWhereClause, Clauses.WhereCyYtd);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Acclfarecy = oneValue.Value;
            }

            sql = SqlBuilder.GetSqlAcceptedLowestFare(BuildWhere, DefaultWhereClause, Clauses.WherePyMth);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Acclfarepm = oneValue.Value;
            }

            sql = SqlBuilder.GetSqlAcceptedLowestFare(BuildWhere, DefaultWhereClause, Clauses.WherePyYtd);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Acclfarepy = oneValue.Value;
            }
            #endregion

            #region "Behaviour Detail by # of Tickets" Hotel Included
            sql = SqlBuilder.GetSqlHotelIncluded(BuildWhere, DefaultWhereClause, Clauses.WhereCyMth);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Hotrecscm = oneValue.Value;
            }

            sql = SqlBuilder.GetSqlHotelIncluded(BuildWhere, DefaultWhereClause, Clauses.WhereCyYtd);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Hotrecscy = oneValue.Value;
            }

            sql = SqlBuilder.GetSqlHotelIncluded(BuildWhere, DefaultWhereClause, Clauses.WherePyMth);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Hotrecspm = oneValue.Value;
            }

            sql = SqlBuilder.GetSqlHotelIncluded(BuildWhere, DefaultWhereClause, Clauses.WherePyYtd);
            oneValue = retriever.GetData<OneValue>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList().FirstOrDefault(); //should be only one record
            if (oneValue != null)
            {
                FinalData.Hotrecspy = oneValue.Value;
            }
            #endregion

            #region Top/Bottom
            sql = SqlBuilder.GetSqlTopBottomTraveler(BuildWhere, DefaultWhereClause, Clauses.WhereCyMth);
            TbTravelersRawData = retriever.GetData<KpiTravelersRawData>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();
            TbTravelersRawData = TbTravelersRawData.GroupBy(s => new { s.PassLast, s.PassFrst, s.AuthStatus }, (key, recs) => new KpiTravelersRawData
            {
                PassLast = key.PassLast.Trim(),
                PassFrst = key.PassFrst.Trim(),
                AuthStatus = key.AuthStatus.Trim(),
                NumRecs = recs.Select(s => s.RecKey).Distinct().Count()
            }).ToList();

            sql = SqlBuilder.GetSqlTopBottomApprovers(BuildWhere, DefaultWhereClause, Clauses.WhereCyMth);
            TbApproversRawData = retriever.GetData<KpiApproversRawData>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();
            TbApproversRawData = TbApproversRawData.GroupBy(s => new { s.AuthrzrNbr, s.Auth1Email, s.AuthStatus }, (key, recs) => new KpiApproversRawData
            {
                AuthrzrNbr = key.AuthrzrNbr,
                Auth1Email = key.Auth1Email.Trim(),
                AuthStatus = key.AuthStatus.Trim(),
                NumRecs = recs.Count()
            }).ToList();

            #endregion

            #region Reason codes Current Month
            //air
            sql = SqlBuilder.GetSqlReasonCodesAir(BuildWhere, DefaultWhereClause, Clauses.WhereCyMth);
            var temp = retriever.GetData<ReasonCodeRawData>(sql, BuildWhere,false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();
            ReasonsCyMth = TravAuthKpiHelpers.GroupReasons(temp);
            //car
            sql = SqlBuilder.GetSqlReasonCodesCar(BuildWhere, DefaultWhereClause, Clauses.WhereCyMth);
            temp = retriever.GetData<ReasonCodeRawData>(sql, BuildWhere,false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();
            ReasonsCyMth.AddRange(TravAuthKpiHelpers.GroupReasons(temp));
            //hotel
            sql = SqlBuilder.GetSqlReasonCodesHotel(BuildWhere, DefaultWhereClause, Clauses.WhereCyMth);
            temp = retriever.GetData<ReasonCodeRawData>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();
            ReasonsCyMth.AddRange(TravAuthKpiHelpers.GroupReasons(temp));

            #endregion Current Month

            #region Reason codes Current YTD
            //air
            sql = SqlBuilder.GetSqlReasonCodesAir(BuildWhere, DefaultWhereClause, Clauses.WhereCyYtd);
            temp = retriever.GetData<ReasonCodeRawData>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();
            ReasonsCyYtd = TravAuthKpiHelpers.GroupReasons(temp);
            //car
            sql = SqlBuilder.GetSqlReasonCodesCar(BuildWhere, DefaultWhereClause, Clauses.WhereCyYtd);
            temp = retriever.GetData<ReasonCodeRawData>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();
            ReasonsCyYtd.AddRange(TravAuthKpiHelpers.GroupReasons(temp));
            //hotel
            sql = SqlBuilder.GetSqlReasonCodesHotel(BuildWhere, DefaultWhereClause, Clauses.WhereCyYtd);
            temp = retriever.GetData<ReasonCodeRawData>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();
            ReasonsCyYtd.AddRange(TravAuthKpiHelpers.GroupReasons(temp));

            #endregion Current YTD

            #region Reason codes Prior Month
            //air
            sql = SqlBuilder.GetSqlReasonCodesAir(BuildWhere, DefaultWhereClause, Clauses.WherePyMth);
            temp = retriever.GetData<ReasonCodeRawData>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();
            ReasonsPyMth = TravAuthKpiHelpers.GroupReasons(temp);
            //car
            sql = SqlBuilder.GetSqlReasonCodesCar(BuildWhere, DefaultWhereClause, Clauses.WherePyMth);
            temp = retriever.GetData<ReasonCodeRawData>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();
            ReasonsPyMth.AddRange(TravAuthKpiHelpers.GroupReasons(temp));
            //hotel
            sql = SqlBuilder.GetSqlReasonCodesHotel(BuildWhere, DefaultWhereClause, Clauses.WherePyMth);
            temp = retriever.GetData<ReasonCodeRawData>(sql, BuildWhere,false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();
            ReasonsPyMth.AddRange(TravAuthKpiHelpers.GroupReasons(temp));

            #endregion Prior Month

            #region Reason codes Prior YTD
            //air
            sql = SqlBuilder.GetSqlReasonCodesAir(BuildWhere, DefaultWhereClause, Clauses.WherePyYtd);
            temp = retriever.GetData<ReasonCodeRawData>(sql, BuildWhere, false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();
            ReasonsPyYtd = TravAuthKpiHelpers.GroupReasons(temp);
            //car
            sql = SqlBuilder.GetSqlReasonCodesCar(BuildWhere, DefaultWhereClause, Clauses.WherePyYtd);
            temp = retriever.GetData<ReasonCodeRawData>(sql, BuildWhere, false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();
            ReasonsPyYtd.AddRange(TravAuthKpiHelpers.GroupReasons(temp));
            //hotel
            sql = SqlBuilder.GetSqlReasonCodesHotel(BuildWhere, DefaultWhereClause, Clauses.WherePyYtd);
            temp = retriever.GetData<ReasonCodeRawData>(sql, BuildWhere, false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();
            ReasonsPyYtd.AddRange(TravAuthKpiHelpers.GroupReasons(temp));

            #endregion Prior YTD

            return true;
        }
    }
}

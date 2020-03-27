using com.ciswired.libraries.CISLogger;
using Domain.Helper;
using Domain.Interfaces;
using Domain.Orm.Classes;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Logging;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.WhereRoute;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public class BuildWhere
    {
        private readonly ErrorLogger _errorLogger = new ErrorLogger();
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        readonly ClientFunctions _clientFunctions;

        private readonly IMasterDataStore _masterStore;

        private IClientDataStore _clientStore;

        public ReportGlobals ReportGlobals { get; set; }

        public BuildWhere(ClientFunctions clientFunctions)
        {
            _clientFunctions = clientFunctions;
            ReportGlobals = new ReportGlobals();
            SqlParameters = new List<SqlParameter>();
            _masterStore = new MasterDataStore();
            AdvancedParameterQueryTableRefList = new List<AvancedParameterQueryTableRef>();
            //TODO: _clientStore is null right now
        }

        public BuildWhere(ClientFunctions clientFunctions, ReportGlobals globals, IMasterDataStore masterStore, IClientDataStore clientStore)
        {
            _clientFunctions = clientFunctions;
            ReportGlobals = globals;
            SqlParameters = new List<SqlParameter>();
            _masterStore = masterStore;
            _clientStore = clientStore;
            AdvancedParameterQueryTableRefList = new List<AvancedParameterQueryTableRef>();
        }

        public bool BuildDateWhere { get; set; }
        public bool BuildTripWhere { get; set; }
        public bool BuildCarWhere { get; set; }
        public bool BuildHotelWhere { get; set; }
        public bool BuildUdidWhere { get; set; }
        public List<AdvancedColumnInformation> AdvancedColumnList { get; set; }

        public string PickNameAccountTitle { get; set; }
        public string NotInText { get; set; }
        public string InText { get; set; }

        public bool IsRoutingBidirectional { get; set; }
        public bool IgnoreTravel { get; set; }

        public bool ReturnOnlyMatchingLeg { get; set; } //TODO: is this actually needed

        public string WhereClauseDate { get; set; }
        public string WhereClauseTrip { get; set; }
        public string WhereClauseRoute { get; set; }
        public string WhereClauseCar { get; set; }
        public string WhereClauseHotel { get; set; }
        public string WhereClauseUdid { get; set; }
        public string WhereClauseSvcFee { get; set; }
        public string WhereClauseServices { get; set; }
        public string WhereClauseAdvanced { get; set; }
        public string WhereClauseServiceFeeAdvanced { get; set; }
        public string WhereClauseChanges { get; set; }
        public bool IncludeCancelled { get; set; }

        public bool IncludeVoids { get; set; }

        public bool HasFirstDestination => ReportGlobals.ParmHasValue(WhereCriteria.FIRSTDEST);
        public bool HasFirstOrigin => ReportGlobals.ParmHasValue(WhereCriteria.FIRSTORIGIN);

        public bool HasRoutingCriteria => ReportGlobals.ParmHasValue(WhereCriteria.ORIGIN) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.DESTINAT) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.METROORG) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.METRODEST) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.ORIGCOUNTRY) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.DESTCOUNTRY) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.ORIGREGION) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.DESTREGION) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.INORGS) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.INDESTS) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.INMETROORGS) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.INMETRODESTS) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.INORIGCOUNTRY) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.INDESTCOUNTRY) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.INORIGREGION) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.INDESTREGION) ||
                                          ReportGlobals.ParmHasValue(WhereCriteria.INAIRLINE);

        public List<SqlParameter> SqlParameters { get; set; }

        /// <summary>
        /// This returns the SqlParameters as an object array that is newed up from SqlParameters to avoid 
        /// errors related to the collection already being contained by another SqlParameterCollection
        /// </summary>
        public object[] Parameters
        {
            get
            {
                var parms = new object[SqlParameters.Count];
                for (var i = 0; i < SqlParameters.Count; i++)
                {
                    //we return a whole new param object each time to avoid the "already contained in a query" error. 
                    var newParam = new SqlParameter(SqlParameters[i].ParameterName, SqlParameters[i].Value);
                    parms[i] = newParam;
                }
                return parms;
            }
        }

        public string WhereClauseFull => CreateClause();

        private string CreateClause()
        {
            var clause = "";

            var builder = new WhereClauseBuilder();
            clause = builder.AddToWhereClause(clause, WhereClauseDate);
            clause = builder.AddToWhereClause(clause, WhereClauseTrip);
            clause = builder.AddToWhereClause(clause, WhereClauseCar);
            clause = builder.AddToWhereClause(clause, WhereClauseHotel);
            clause = builder.AddToWhereClause(clause, WhereClauseUdid);

            return clause;
        }
        public List<AvancedParameterQueryTableRef> AdvancedParameterQueryTableRefList { get; set; }

        //TODO: no reason for this to return false - why not just let the exception fall through?
        //TODO: legDit, buildRouteWhere are all not used and can be removed
        //TODO: InMemory parameter can go away, but it will require refactoring all the report programs. 
        public bool BuildAll(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, bool buildTripWhere, bool buildRouteWhere, bool buildCarWhere, bool buildHotelWhere,
            bool buildUdidWhere, bool buildDateWhere, bool inMemory, bool isRoutingBidirectional, bool legDit, bool ignoreTravel)
        {
            var needTranslation = false;

            //should change output language first, if it's not set use user language
            var outputLanguage = (!ReportGlobals.OutputLanguage.IsNullOrWhiteSpace())
                ? ReportGlobals.OutputLanguage
                : ReportGlobals.UserLanguage;
            needTranslation = !outputLanguage.EqualsIgnoreCase("EN");
            NotInText = BuildWhereUtilities.GetNotInTextByLangCode(needTranslation, outputLanguage, _masterStore);
            InText = BuildWhereUtilities.GetInTextByLangCode(needTranslation, outputLanguage, _masterStore);

            ClearWhereClausesInfo();

            ReportGlobals.AccountName = ReportGlobals.CompanyName;
            ReportGlobals.WhereText = string.Empty;
            PickNameAccountTitle = string.Empty;

            BuildCarWhere = buildCarWhere;
            BuildHotelWhere = buildHotelWhere;
            BuildTripWhere = buildTripWhere;
            BuildDateWhere = buildDateWhere;
            BuildUdidWhere = buildUdidWhere;

            IsRoutingBidirectional = isRoutingBidirectional;
            IgnoreTravel = ignoreTravel;
            ReturnOnlyMatchingLeg = ReportGlobals.ProcessKey == (int)ReportTitles.Arrival || ReportGlobals.ProcessKey == (int)ReportTitles.Departures;

            if (ReportGlobals.MultiUdidParameters.Parameters.Any()) ReportGlobals.WhereText += "Multiple UDID Criteria; ";

            var currency = ReportGlobals.GetParmValue(WhereCriteria.MONEYTYPE);
            if (!string.IsNullOrEmpty(currency)) ReportGlobals.WhereText += BuildWhereUtilities.GetCurrencyWhereText(currency, ReportGlobals);

            try
            {
                if (BuildDateWhere) new DateWhere().GetDateWhere(ReportGlobals, this);

                if (BuildTripWhere) GetTripWhere(getAllMasterAccountsQuery);

                if (BuildCarWhere) new CarWhere().GetCarWhere(ReportGlobals, this, NotInText);

                if (BuildHotelWhere) new HotelWhere().GetHotelWhere(ReportGlobals, this, NotInText);

                if (BuildUdidWhere) new UdidWhere().GetUdidWhere(ReportGlobals, this, InText);
            }
            catch (Exception e)
            {
                var error = new ErrorInformation
                {
                    Exception = e,
                    Agency = ReportGlobals.Agency,
                    ErrorMessage = e.ToString(),
                    UserNumber = ReportGlobals.UserNumber,
                    Version = ReportGlobals.iBankVersion,
                    ServerNumber = ReportGlobals.ServerNumber,
                    ErrorProgram = "BuildWhere.cs",
                    LineNumber = (short)(new StackTrace(e, true).GetFrame(0).GetFileLineNumber()),
                    ServerName = ReportGlobals.IsOfflineServer ? ".NET Broadcast Server" : ".NET Report Server"
                };

                _errorLogger.Log(error);
                LOG.Error(e.Message.FormatMessageWithReportLogKey(ReportGlobals.ReportLogKey), e);
                return false;
            }

            return true;
        }

        public void AddSecurityChecks()
        {
            //TODO: fix - remove this instantiation once the NULL issue is resolved
            _clientStore = new ClientDataStore(ReportGlobals.AgencyInformation.ServerName, ReportGlobals.AgencyInformation.DatabaseName);
            var secBuilder = new UserSecurityBuilder(_clientStore.ClientQueryDb, ReportGlobals.UserNumber);
            WhereClauseTrip = secBuilder.GetAllowedUserSource(ReportGlobals.ClientType, WhereClauseTrip, ReportGlobals.Agency, ReportGlobals.User.AllSources);

            WhereClauseTrip = secBuilder.GetAllowedUserAcct(WhereClauseTrip, ReportGlobals.User.AllAccounts);

            WhereClauseTrip = secBuilder.GetUserBreaks(WhereClauseTrip, ReportGlobals.User.AllBreaks1, ReportGlobals.User.AllBreaks2);
        }

        public void AddAdvancedClauses(bool keepCrit = false, bool excludeTls = false)
        {
            var clauseBuilder = new AdvancedClauseBuilder();
            if (AdvancedColumnList == null) AdvancedColumnList = SetAdvancedColumnList();

            WhereClauseAdvanced = clauseBuilder.GetAdvancedWhereClause(ReportGlobals, AdvancedColumnList, excludeTls);
        }

        public void AddAdvancedClauses(ref string serviceFeeAdvancedWhereClause, bool keepCrit = false, bool excludeTls = false)
        {
            var clauseBuilder = new AdvancedClauseBuilder();
            if (AdvancedColumnList == null) AdvancedColumnList = SetAdvancedColumnList();

            WhereClauseAdvanced = clauseBuilder.GetAdvancedWhereClause(ReportGlobals, AdvancedColumnList, excludeTls, ref serviceFeeAdvancedWhereClause);
        }

        public void BuildAdvancedClauses(bool keepCrit = false, bool excludeTls = false)
        {
            var clauseBuilder = new AdvancedClauseBuilder();
            if (AdvancedColumnList == null) AdvancedColumnList = SetAdvancedColumnList();

            clauseBuilder.BuildAdvancedWhereClause(this, AdvancedColumnList, excludeTls);
        }

        public bool AddBuildWhereChanges()
        {
            var changesWhere = new ChangesWhere();
            return changesWhere.AddBuildWhereChanges(ReportGlobals, this);
        }

        private void ClearWhereClausesInfo()
        {
            WhereClauseDate = string.Empty;
            WhereClauseTrip = string.Empty;
            WhereClauseCar = string.Empty;
            WhereClauseHotel = string.Empty;
            WhereClauseRoute = string.Empty;
            WhereClauseServices = string.Empty;
            WhereClauseSvcFee = string.Empty;
            WhereClauseUdid = string.Empty;
            WhereClauseAdvanced = string.Empty;
            SqlParameters = new List<SqlParameter>();
        }

        private void GetTripWhere(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery)
        {
            var tripWhere = new TripWhere(new ClientDataStore(ReportGlobals.AgencyInformation.ServerName, ReportGlobals.AgencyInformation.DatabaseName), new MasterDataStore());
            var domIntl = ReportGlobals.GetParmValue(WhereCriteria.DOMINTL);
            var isHistory = ReportGlobals.ParmValueEquals(WhereCriteria.PREPOST, "2");
            var isNotIn = ReportGlobals.IsParmValueOn(WhereCriteria.NOTINACCT);
            IncludeVoids = ReportGlobals.IsParmValueOn(WhereCriteria.CBINCLVOIDS);

            if (isHistory) tripWhere.HandleBackOfficeReportSpecificProcessing(ReportGlobals, this, IncludeVoids);

            tripWhere.HandleAccountCriteriaUseOutputTranslation(ReportGlobals, this, getAllMasterAccountsQuery, isNotIn, PickNameAccountTitle, _clientFunctions, InText, NotInText);

            tripWhere.HandleParentAccount(ReportGlobals, this, getAllMasterAccountsQuery, PickNameAccountTitle, _clientFunctions);

            tripWhere.HandleBreaks(ReportGlobals, this);

            tripWhere.HandleStandardWhereClauses(ReportGlobals, this, NotInText);

            //Moved for Market, which prints the DOMINTL filters after the Data Source filter. Do all reports print in the same order?
            tripWhere.HandleDomesticIntl(domIntl, ReportGlobals, this);

            tripWhere.HandleSimpleFieldsWhereClauses(ReportGlobals, this);

            tripWhere.HandleAgentCriteria(ReportGlobals, this, isNotIn);

            if (ReportGlobals.IncludeOrphanServiceFees) tripWhere.HandleOrphanServiceFees(ReportGlobals, domIntl);

            //sublog type
            WhereClauseTrip = tripWhere.AddListWhere(ReportGlobals, WhereClauseTrip, WhereCriteria.SUBLOGTYPE, WhereCriteria.INSUBLOGTYPE, WhereCriteria.NOTINSUBLOGTYPE, "SUBLOGTYPE", "T1.sublogtype",
                "Sub Log Type", NotInText);

            tripWhere.HandleAccountType(ReportGlobals, this);
        }

        public List<T> ApplyWhereRoute<T>(List<T> rawData, bool applyOriginDestAtLegLevel, bool returnAllLegs = true) where T : class, IRouteWhere
        {
            var processor = new RoutingProcessor<T>(ReportGlobals, _masterStore);
            return processor.ProcessRoutingCriteria(rawData, returnAllLegs, IsRoutingBidirectional, IgnoreTravel).ToList();
        }

        private List<AdvancedColumnInformation> SetAdvancedColumnList()
        {
            //Use output language instead of userlanguage
            var outputLanguage = ReportGlobals.OutputLanguage;
            if (string.IsNullOrWhiteSpace(outputLanguage)) outputLanguage = "EN";
            return new GetAdvancedColumnsByUserLanguageQuery(new iBankMastersQueryable(), outputLanguage).ExecuteQuery();
        }
    }
}

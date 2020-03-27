using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.ciswired.libraries.CISLogger;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.CarbonCalculations;
using iBank.Services.Implementation.Utilities.ClientData;
using iBank.Services.Implementation.Utilities.CurrencyConversion;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.DataSetHandler
{
    public class LegDataSetHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly bool _isReservationReport;

        private readonly ReportGlobals _globals;

        private readonly BuildWhere _buildWhere;

        private readonly IClientDataStore _clientStore;

        public LegDataSetHandler(bool isReservationReport, ReportGlobals globals, BuildWhere buildWhere, IClientDataStore clientStore)
        {
            _isReservationReport = isReservationReport;
            _globals = globals;
            _buildWhere = buildWhere;
            _clientStore = clientStore;
        }

        public List<LegRawData> GetLegData(bool tripTlsSwitch, bool udidExists, string whereClause)
        {
            var legDataManager = new LegDataSqlScript();
            var sql = legDataManager.GetSqlScript(tripTlsSwitch, udidExists, _isReservationReport, whereClause);       
            var legData = ClientDataRetrieval.GetRawData<LegRawData>(sql, _isReservationReport, _buildWhere, _globals, addFieldsFromLegsTable: false, includeAllLegs: false).ToList();
            LOG.Debug($"Retrieved {legData.Count} leg records.");

            return legData;
        }

        public string GetTempWhereTextForLeg(string whereText)
        {
            var whereTextHold = whereText;
            if (whereTextHold.Right(1).Equals(";")) whereTextHold = whereTextHold.RemoveLastChar();

            return whereTextHold;
        }

        public List<LegRawData> GetRoutingCriteriaFilteredData(List<LegRawData> legData)
        {
            var applyLegs = _globals.GetParmValue(WhereCriteria.RBAPPLYTOLEGORSEG) == "1";
            legData = _buildWhere.ApplyWhereRoute(legData, applyLegs, false);
            
            //because after applying route the seqno won't be in order, so we need to reorder it.
            LegSegOrderFixer.UpdateSequenceToCorrectOrder(legData);

            LOG.Debug($"LegData After Routing Count:{legData.Count}");

            return legData;
        }

        public List<RawData> RemoveFilteredOutDataFromTrip(List<int> recKeysInLegs, List<RawData> tripData)
        {
            tripData.RemoveAll(x => !recKeysInLegs.Contains(x.RecKey));
            LOG.Debug($"TripData After Routing Filter:{tripData.Count}");

            return tripData;
        }

        public string GetTravelerLocationText(string tempWhereText, RoutingCriteria routingCriteria)
        {
            var routingTextHelper = new RoutingWhereTextManager();
            return routingTextHelper.GetRoutingText(tempWhereText, routingCriteria, _globals.IsParmValueOn(WhereCriteria.CBEXCLTRAVSARRIVINGHOME));
        }

        public List<LegRawData> GetLegCarbon(List<LegRawData> legData, string carbonCalculator)
        {
            var useMileageTable = _globals.IsParmValueOn(WhereCriteria.MILEAGETABLE);
            var useMetric = _globals.IsParmValueOn(WhereCriteria.METRIC);

            if (useMileageTable) AirMileageCalculator<LegRawData>.CalculateAirMileageFromTable(legData);

            var carbonCalc = new CarbonCalculator();
            carbonCalc.SetAirCarbon(legData, useMetric, carbonCalculator);

            if (useMetric) MetricImperialConverter.ConvertMilesToKilometers(legData);

            return legData;
        }

        public List<LegRawData> GetCollapsedLegData(List<LegRawData> legData, List<UserReportColumnInformation> columns)
        {
            var reassignClass = columns.Select(s => s.Name).Contains("CLASSCAT") || columns.Select(s => s.Name).Contains("CLASSCNAM");

            LOG.Debug($"SetOtherRawData - reassignClass:[{reassignClass}] | LegDataList Count:[{legData.Count}]");
            if (_globals.Agency.Equals("GSA"))
            {
                legData = reassignClass
                    ? legData = Collapser<LegRawData>.CollapseUsingClassHierarchy(legData, Collapser<LegRawData>.CollapseType.LongestMile, true, _clientStore, _globals.Agency)
                    : legData = Collapser<LegRawData>.Collapse(legData, Collapser<LegRawData>.CollapseType.LongestMile);
            }
            else
            {
                legData = reassignClass
                        ? legData = Collapser<LegRawData>.CollapseUsingClassHierarchy(legData, Collapser<LegRawData>.CollapseType.DepartAndArrive, true, _clientStore, _globals.Agency)
                        : legData = Collapser<LegRawData>.Collapse(legData, Collapser<LegRawData>.CollapseType.DepartAndArrive);
            }

            LOG.Debug($"SetOtherRawData - reassignClass:[{reassignClass}] | after collapse LegDataList Count:[{legData.Count}]");

            return legData;
        }

        public List<LegRawData> ConvertCurrency(List<LegRawData> legData, string moneyType)
        {
            if (legData.Select(s => s.Moneytype).Distinct().Any(s => s != moneyType && moneyType != ""))
            {
                var cc = new CurrencyConverter();
                cc.ConvertCurrency(legData, moneyType);
            }

            return legData;
        }
    }
}

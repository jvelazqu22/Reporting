
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.XmlExtract.DataSqlScripts;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

using Domain.Models.ReportPrograms.XmlExtractReport;
using Domain.Models;
using System;

using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Utilities.ClientData;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers
{
    public class DataManager
    {
        private static readonly ILogger Log = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ReportGlobals _globals;
        private readonly DataSwitches _dataSwitches;
        private readonly XmlExtractParameters _xmlExtractParams;
        private readonly bool _isReservation;
        private readonly bool _udidExists;
        private readonly bool _needCancelTrips;

        private readonly BuildWhere _buildWhere;
        protected iBankClientQueryable ClientQueryableDb => new iBankClientQueryable(_globals.AgencyInformation.ServerName, _globals.AgencyInformation.DatabaseName);

        public DataManager(ReportGlobals globals, BuildWhere buildWhere, DataSwitches dataSwitches, XmlExtractParameters parameters, bool udidExists, bool isReservation, bool needCancelTrips)
        {
            _globals = globals;
            _dataSwitches = dataSwitches;
            _xmlExtractParams = parameters;
            _isReservation = isReservation;
            _udidExists = udidExists;
            _needCancelTrips = needCancelTrips;
            _buildWhere = buildWhere;
        }

        public void SetRawData(string whereClause, bool isISOS, bool isUSRISOS)
        {
            var dataSqlScript = new TripDataSqlScript();
            var sql = dataSqlScript.GetSqlScript(_udidExists, _isReservation, whereClause);
            _xmlExtractParams.TripRawDataList = ClientDataRetrieval.GetRawData<RawData>(sql, _isReservation, _buildWhere, _globals, false).ToList();

            if (!_isReservation)
            {
                foreach (var row in _xmlExtractParams.TripRawDataList)
                {
                    row.Lastupdate = DateTime.MinValue;
                    row.Changstamp = DateTime.MinValue;
                    row.Parsestamp = DateTime.MinValue;
                }
            }

            if ((_needCancelTrips || isISOS || isUSRISOS) && _isReservation) SetCancelledTrip(sql);
        }

        public void SetCancelledTrip(SqlScript sql)
        {
            var cancSql = sql;
            cancSql.FromClause = sql.FromClause.Replace("ibtrips", "ibcanctrips").Replace("ibudids", "ibcancudids");
            cancSql.FieldList = sql.FieldList.Replace("'UPD ' as", "'CANC' as");

            var cancelledData = ClientDataRetrieval.GetRawData<RawData>(cancSql, _isReservation, _buildWhere, _globals, false).ToList();
            Log.Debug($"SetCancelledTrip - cancelledTrip Count:[{cancelledData.Count}]");
            _xmlExtractParams.TripRawDataList.AddRange(cancelledData);
            Log.Debug($"SetCancelledTrip - AddRange to LTripRawDataList Count:[{_xmlExtractParams.TripRawDataList.Count}]");
        }

        public void SetXmlExtractParameters(string whereClause, bool includeVoids)
        {
            if (_dataSwitches.AirSwitch || _dataSwitches.RailSwitch)
            {
                if (Features.XmlExtractWhereClauseFull.IsEnabled() && !includeVoids)
                {
                    // this is related to US: 9676. 
                    // igonore voided trips and get all legs since legs are not marked as void or not. Only at the trip level.
                    // "invdate >= @t1BeginDate and invdate <= @t1EndDate AND T1.trantype <> @t1TranType1 AND T1.recloc = 'MATSOF' AND T1.agency = 'DEMO'"
                    SetLegData(whereClause.Replace("T1.trantype <> @t1TranType1 AND ", ""));
                }
                else
                {
                    SetLegData(whereClause);
                }
            }

            if (_dataSwitches.CarSwitch)
            {
                if (Features.XmlExtractWhereClauseFull.IsEnabled())
                {
                    // this is related to US: 9676. 
                    // Look up the TranType at the car level and not the trip level
                    //"invdate >= @t1BeginDate and invdate <= @t1EndDate AND T1.trantype <> @t1TranType1 AND T1.recloc = 'MATSOF' AND T1.agency = 'DEMO'"
                    SetCarData(whereClause.Replace("T1.trantype", "T4.CarTranTyp"));
                }
                else
                {
                    SetCarData(whereClause);
                }
            }

            if (_dataSwitches.HotelSwitch)
            {
                if (Features.XmlExtractWhereClauseFull.IsEnabled())
                {
                    // this is related to US: 9676. 
                    // Look up the TranType at the hotel level and not the trip level
                    //"invdate >= @t1BeginDate and invdate <= @t1EndDate AND T1.trantype <> @t1TranType1 AND T1.recloc = 'MATSOF' AND T1.agency = 'DEMO'"
                    SetHotelData(whereClause.Replace("T1.trantype", "T5.HotTranTyp"));
                }
                else
                {
                    SetHotelData(whereClause);
                }
            }

            if (_dataSwitches.UdidSwitch)
            {
                if (Features.XmlExtractWhereClauseFull.IsEnabled() && !includeVoids)
                {
                    // this is related to US: 9676. 
                    // igonore voided trips and get all udid since the udid table(s) are not marked as voided or not. Only at the trip level.
                    //"invdate >= @t1BeginDate and invdate <= @t1EndDate AND T1.trantype <> @t1TranType1 AND T1.recloc = 'MATSOF' AND T1.agency = 'DEMO'"
                    SetUdidData(whereClause.Replace("T1.trantype <> @t1TranType1 AND ", ""));
                }
                else
                {
                    SetUdidData(whereClause);
                }
            }

            if (_dataSwitches.SvcFeeSwitch && !_isReservation)
            {
                if (Features.XmlExtractWhereClauseFull.IsEnabled())
                {
                    // this is related to US: 9676. 
                    // Look up the TranType at the service level and not the trip level
                    //"invdate >= @t1BeginDate and invdate <= @t1EndDate AND T1.trantype <> @t1TranType1 AND T1.recloc = 'MATSOF' AND T1.agency = 'DEMO'"
                    SetSvcFeeData(whereClause.Replace("T1.trantype", "T6A.sftrantype"));
                }
                else
                {
                    SetSvcFeeData(whereClause);
                }
            }

            if (_dataSwitches.MktSegSwitch)
            {
                if (Features.XmlExtractWhereClauseFull.IsEnabled() && !includeVoids)
                {
                    // this is related to US: 9676. 
                    // igonore voided trips not 100% sire how the [ibMktSegs] data relate to the hibtrips data though.
                    SetMarketSegData(whereClause.Replace("T1.trantype <> @t1TranType1 AND ", ""));
                }
                else
                {
                    SetMarketSegData(whereClause);
                }
            }
        }

        public void SetLegData(string whereClause)
        {
            var dataSqlScript = new LegDataSqlScript(_globals, _dataSwitches.AirSwitch, _dataSwitches.RailSwitch);
            var sql = dataSqlScript.GetSqlScript(_udidExists, _isReservation, whereClause);
            _xmlExtractParams.LegRawDataList = ClientDataRetrieval.GetRawData<LegRawData>(sql, _isReservation, _buildWhere, _globals, false).ToList();

            if (_needCancelTrips && _isReservation) SetCancelledLeg(sql);
        }

        public void SetCancelledLeg(SqlScript sql)
        {
            var cancSql = sql;
            cancSql.FromClause = sql.FromClause.Replace("ibtrips", "ibcanctrips").Replace("ibudids", "ibcancudids").Replace("iblegs", "ibcanclegs");

            var cancelledData = ClientDataRetrieval.GetRawData<LegRawData>(cancSql, _isReservation, _buildWhere, _globals, false).ToList();
            Log.Debug($"SetCancelledLeg - cancelledLeg Count:[{cancelledData.Count}]");
            _xmlExtractParams.LegRawDataList.AddRange(cancelledData);
            Log.Debug($"SetCancelledLeg - AddRange to LegRawDataList Count:[{_xmlExtractParams.LegRawDataList.Count}]");
        }

        public void SetCarData(string whereClause)
        {
            var dataSqlScript = new CarDataSqlScript();
            var sql = dataSqlScript.GetSqlScript(_udidExists, _isReservation, whereClause);
            _xmlExtractParams.CarRawDataList = ClientDataRetrieval.GetRawData<CarRawData>(sql, _isReservation, _buildWhere, _globals, false).ToList();

            if (_needCancelTrips && _isReservation) SetCancelledCar(sql);
        }

        public void SetCancelledCar(SqlScript sql)
        {
            var cancSql = sql;
            cancSql.FromClause = sql.FromClause.Replace("ibtrips", "ibcanctrips").Replace("ibudids", "ibcancudids").Replace("ibcar", "ibcanccars");

            var cancelledData = ClientDataRetrieval.GetRawData<CarRawData>(cancSql, _isReservation, _buildWhere, _globals, false).ToList();
            Log.Debug($"SetCancelledCar - cancelledCar Count:[{cancelledData.Count}]");
            _xmlExtractParams.CarRawDataList.AddRange(cancelledData);
            Log.Debug($"SetCancelledCar - AddRange to CarRawDataList Count:[{_xmlExtractParams.CarRawDataList.Count}]");
        }

        public void SetHotelData(string whereClause)
        {
            var dataSqlScript = new HotelDataSqlScript();
            var sql = dataSqlScript.GetSqlScript(_udidExists, _isReservation, whereClause);
            _xmlExtractParams.HotelRawDataList = ClientDataRetrieval.GetRawData<HotelRawData>(sql, _isReservation, _buildWhere, _globals, false).ToList();

            if (_needCancelTrips && _isReservation) SetCancelledHotel(sql);
        }

        public void SetCancelledHotel(SqlScript sql)
        {
            var cancSql = sql;
            cancSql.FromClause = sql.FromClause.Replace("ibtrips", "ibcanctrips").Replace("ibudids", "ibcancudids").Replace("ibhotel", "ibcanchotels");

            var cancelledData = ClientDataRetrieval.GetRawData<HotelRawData>(cancSql, _isReservation, _buildWhere, _globals, false).ToList();
            Log.Debug($"SetCancelledHotel - cancelledHotel Count:[{cancelledData.Count}]");
            _xmlExtractParams.HotelRawDataList.AddRange(cancelledData);
            Log.Debug($"SetCancelledHotel - AddRange to HotelRawDataList Count:[{_xmlExtractParams.HotelRawDataList.Count}]");
        }

        public void SetUdidData(string whereClause)
        {
            var dataSqlScript = new UdidDataSqlScript();
            var sql = dataSqlScript.GetSqlScript(_udidExists, _isReservation, whereClause);
            _xmlExtractParams.UdidRawDataList = ClientDataRetrieval.GetRawData<UdidRawData>(sql, _isReservation, _buildWhere, _globals, false).ToList();

            if (_needCancelTrips  && _isReservation) SetCancelledUdid(sql);
        }

        public void SetCancelledUdid(SqlScript sql)
        {
            var cancSql = sql;
            cancSql.FromClause = sql.FromClause.Replace("ibtrips", "ibcanctrips").Replace("ibudids", "ibcancudids");

            var cancelledData = ClientDataRetrieval.GetRawData<UdidRawData>(cancSql, _isReservation, _buildWhere, _globals, false).ToList();
            Log.Debug($"SetCancelledUdid - cancelledUdud Count:[{cancelledData.Count}]");
            _xmlExtractParams.UdidRawDataList.AddRange(cancelledData);
            Log.Debug($"SetCancelledUdid - AddRange to UdudRawDataList Count:[{_xmlExtractParams.UdidRawDataList.Count}]");
        }

        public void SetSvcFeeData(string whereClause)
        {
            var dataSqlScript = new SvcFeeDataSqlScript();
            var sql = dataSqlScript.GetSqlScript(_udidExists, _isReservation, whereClause);
            _xmlExtractParams.SvcFeeRawDataList = ClientDataRetrieval.GetRawData<SvcFeeRawData>(sql, _isReservation, _buildWhere, _globals, false).ToList();
        }

        public void SetMarketSegData(string whereClause)
        {
            var dataSqlScript = new MktSegDataSqlScript();
            var sql = dataSqlScript.GetSqlScript(_udidExists, _isReservation, whereClause);
            _xmlExtractParams.MktSegRawDataList = ClientDataRetrieval.GetRawData<MktSegRawData>(sql, _isReservation, _buildWhere, _globals, false).ToList();
        }
    }
}

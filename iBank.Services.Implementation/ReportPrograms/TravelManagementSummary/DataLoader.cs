using Domain.Models;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iBank.Services.Implementation.Utilities;
using Domain.Models.ReportPrograms.TravelManagementSummary;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.TravelManagementSummary
{
    public class DataLoader
    {
        private readonly ReportGlobals _globals;
        private readonly bool _isReservation;
        private readonly BuildWhere _buildWhere;

        public DataLoader(bool isReservationReport, BuildWhere where, ReportGlobals globals) 
        {
            _globals = globals;
            _buildWhere = where;
            _isReservation = isReservationReport;
        }

        public List<RawData> GetAirData(SqlScript sql, bool allLegs)
        {
            return ClientDataRetrieval.GetRawData<RawData>(sql, _isReservation, _buildWhere, _globals, allLegs).ToList();
        }
        public List<LegRawData> GetLegData(SqlScript sql, bool allLegs)
        {
            return ClientDataRetrieval.GetRawData<LegRawData>(sql, _isReservation, _buildWhere, _globals, allLegs).ToList();
        }
        
        public List<HotelRawData> GetHotelData(SqlScript sql, bool allLegs)
        {
            return ClientDataRetrieval.GetRawData<HotelRawData>(sql, _isReservation, _buildWhere, _globals, allLegs).ToList();
        }
        public List<CarRawData> GetCarData(SqlScript sql, bool allLegs)
        {
            return ClientDataRetrieval.GetRawData<CarRawData>(sql, _isReservation, _buildWhere, _globals, allLegs).ToList();
        }
        public List<SvcFeeRawData> GetServiceFeeData(SqlScript sql, bool allLegs)
        {
            return ClientDataRetrieval.GetRawData<SvcFeeRawData>(sql, _isReservation, _buildWhere, _globals, allLegs).ToList();
        }
    }
}

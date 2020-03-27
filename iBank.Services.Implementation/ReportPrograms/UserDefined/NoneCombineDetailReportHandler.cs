using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class NoneCombineDetailReportHandler
    {
        private UserDefinedParameters _userDefinedParameters;
        private int _processKey;

        public NoneCombineDetailReportHandler(UserDefinedParameters userDefinedParameters, int processKey)
        {
            _userDefinedParameters = userDefinedParameters;
            _processKey = processKey;
        }

        public void CleanTripData()
        {
            switch (_processKey)
            {
                case (int)ReportTitles.AirUserDefinedReports:
                    RemoveNoneAirAndRailTrips();
                    break;
                case (int)ReportTitles.HotelUserDefinedReports:
                    RemoveNoneHotelTrips();
                    break;
                case (int)ReportTitles.CarUserDefinedReports:
                    RemoveNoneCarTrips();
                    break;
                case (int)ReportTitles.ServiceFeeUserDefinedReports:
                    RemoveNoneSvcFeeTrips();
                    break;
            }
        }

        public void RemoveNoneAirAndRailTrips()
        {
            //to allow rail data in Air Custom Report (meaning mode in A and R)
            var airData = _userDefinedParameters.LegDataList;
            var tripData = _userDefinedParameters.TripDataList;
            _userDefinedParameters.TripDataList = tripData.Where(a => airData.Any(b => a.RecKey ==b.RecKey)).ToList();
        }

        public void RemoveNoneHotelTrips()
        {
            var hotelData = _userDefinedParameters.HotelDataList;
            var tripData = _userDefinedParameters.TripDataList;
            _userDefinedParameters.TripDataList = tripData.Where(a => hotelData.Any(b => a.RecKey == b.RecKey)).ToList();
        }

        public void RemoveNoneCarTrips()
        {
            var carData = _userDefinedParameters.CarDataList;
            var tripData = _userDefinedParameters.TripDataList;
            _userDefinedParameters.TripDataList = tripData.Where(a => carData.Any(b => a.RecKey == b.RecKey)).ToList();            
        }

        public void RemoveNoneSvcFeeTrips()
        {
            var svcFeeData = _userDefinedParameters.ServiceFeeDataList;
            var tripData = _userDefinedParameters.TripDataList;
            _userDefinedParameters.TripDataList = tripData.Where(a => svcFeeData.Any(b => a.RecKey == b.RecKey)).ToList();
        }
    }
}

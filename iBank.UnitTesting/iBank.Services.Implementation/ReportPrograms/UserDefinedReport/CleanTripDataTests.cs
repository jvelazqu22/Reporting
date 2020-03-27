using System;
using System.Collections.Generic;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class CleanTripDataTests
    {
        [TestMethod]
        public void CleanTripData_CustomAirTwoTripsOneAir_RemainOneTrip()
        {
            //arrange
            var userDefinedParameters = InitialCustomAirData();

            var exp = new List<RawData>
            {
                new RawData{RecKey = 100}
            };

            var handler = new NoneCombineDetailReportHandler(userDefinedParameters, (int)ReportTitles.AirUserDefinedReports);

            //act
            handler.CleanTripData();

            //Assert
            Assert.AreEqual(exp.Count, userDefinedParameters.TripDataList.Count);
        }

        [TestMethod]
        public void CleanTripData_CustomAirTwoTripsOneHotel_RemainOneTrip()
        {
            //arrange
            var userDefinedParameters = InitialCustomHotelData();

            var exp = new List<RawData>
            {
                new RawData{RecKey = 100}
            };

            var handler = new NoneCombineDetailReportHandler(userDefinedParameters, (int)ReportTitles.HotelUserDefinedReports);

            //act
            handler.CleanTripData();

            //Assert
            Assert.AreEqual(exp.Count, userDefinedParameters.TripDataList.Count);
        }


        [TestMethod]
        public void CleanTripData_CustomCarTwoTripsOneCar_RemainOneTrip()
        {
            //arrange
            var userDefinedParameters = InitialCustomCarData();

            var exp = new List<RawData>
            {
                new RawData{RecKey = 100}
            };

            var handler = new NoneCombineDetailReportHandler(userDefinedParameters, (int)ReportTitles.CarUserDefinedReports);

            //act
            handler.CleanTripData();

            //Assert
            Assert.AreEqual(exp.Count, userDefinedParameters.TripDataList.Count);
        }


        [TestMethod]
        public void CleanTripData_CustomSvcFeeTwoTripsOneSvcFee_RemainOneTrip()
        {
            //arrange
            var userDefinedParameters = InitialCustomSvcFeeData();

            var exp = new List<RawData>
            {
                new RawData{RecKey = 100}
            };

            var handler = new NoneCombineDetailReportHandler(userDefinedParameters, (int)ReportTitles.ServiceFeeUserDefinedReports);

            //act
            handler.CleanTripData();

            //Assert
            Assert.AreEqual(exp.Count, userDefinedParameters.TripDataList.Count);
        }


        private UserDefinedParameters InitialCustomAirData()
        {
            //arrange
            var userDefinedParameters = new UserDefinedParameters();
            userDefinedParameters.TripDataList = new List<RawData>
            {
                new RawData{RecKey = 100},
                new RawData{RecKey = 200}
            };

            userDefinedParameters.AirLegDataList = new List<LegRawData>
            {
                new LegRawData{RecKey = 100, Mode="A"}
            };

            userDefinedParameters.LegDataList = new List<LegRawData>
            {
                new LegRawData{RecKey = 100, Mode="R"},
                new LegRawData{RecKey = 100, Mode="A"},
            };

            userDefinedParameters.RailLegDataList = new List<LegRawData>
            {
                new LegRawData{RecKey = 100, Mode="R"}
            };

            return userDefinedParameters;
        }

        private UserDefinedParameters InitialCustomHotelData()
        {
            //arrange
            var userDefinedParameters = new UserDefinedParameters();
            userDefinedParameters.TripDataList = new List<RawData>
            {
                new RawData{RecKey = 100},
                new RawData{RecKey = 200}
            };

            userDefinedParameters.HotelDataList = new List<HotelRawData>
            {
                new HotelRawData{RecKey = 100}
            };

            return userDefinedParameters;
        }

        private UserDefinedParameters InitialCustomCarData()
        {
            //arrange
            var userDefinedParameters = new UserDefinedParameters();
            userDefinedParameters.TripDataList = new List<RawData>
            {
                new RawData{RecKey = 100},
                new RawData{RecKey = 200}
            };

            userDefinedParameters.CarDataList = new List<CarRawData>
            {
                new CarRawData{RecKey = 100}
            };

            return userDefinedParameters;
        }
        private UserDefinedParameters InitialCustomSvcFeeData()
        {
            //arrange
            var userDefinedParameters = new UserDefinedParameters();
            userDefinedParameters.TripDataList = new List<RawData>
            {
                new RawData{RecKey = 100},
                new RawData{RecKey = 200}
            };

            userDefinedParameters.ServiceFeeDataList = new List<ServiceFeeData>
            {
                new ServiceFeeData{RecKey = 100}
            };

            return userDefinedParameters;
        }

    }
}

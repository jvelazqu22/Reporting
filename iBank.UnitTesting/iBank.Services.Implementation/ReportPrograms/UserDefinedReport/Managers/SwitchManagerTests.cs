using Domain.Helper;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using iBank.Entities.MasterEntities;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport.Managers
{
    [TestClass]
    public class SwitchManagerTests
    {
        private SwitchManager _tester { get; set; }

        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        //[TestMethod]
        public void PopulateSwitches_UserReportColumnInformationnTableNameIsTrips_AllSwitchesAreFalseExceptTripSwitchIsTrue()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "test" }
                    }
                }
            };
            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "TRIPS", Name = "something" }
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { tlsonly = false, colname = "test"  }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsFalse(_tester.CarSwitch);
            Assert.IsFalse(_tester.HotelSwitch);
            Assert.IsFalse(_tester.LegSwitch);
            Assert.IsFalse(_tester.ServiceFeeSwitch);
            Assert.IsFalse(_tester.RailLegSwitch);
            Assert.IsFalse(_tester.AirLegSwitch);
            Assert.IsFalse(_tester.MiscSegmentsSwitch);
            Assert.IsFalse(_tester.ChangeLogSwitch);
            Assert.IsFalse(_tester.MarketSegmentsSwitch);
            Assert.IsFalse(_tester.TravelAuthorizersSwitch);
            Assert.IsFalse(_tester.TraveAuthSwitch);
            Assert.IsFalse(_tester.TripTlsSwitch);
            Assert.IsFalse(_tester.HotelSwitch2);
            Assert.IsFalse(_tester.CarSwitch2);
            Assert.IsFalse(_tester.LegSwitch2);
            Assert.IsFalse(_tester.ServiceFeeSwitch2);
            Assert.IsFalse(_tester.HaveRoomType);
            Assert.IsFalse(_tester.MiscSegmentTourSwitch);
            Assert.IsFalse(_tester.MiscSegmentCruiseSwitch);
            Assert.IsFalse(_tester.MiscSegmentLimoSwitch);
            Assert.IsFalse(_tester.UdidSwitch);
            Assert.IsFalse(_tester.MiscSegmentRalSwitch);
            Assert.IsFalse(_tester.HotelOnly);
            Assert.IsFalse(_tester.CarOnly);

            Assert.IsTrue(_tester.TripSwitch);
        }

        [TestMethod]
        public void PopulateSwitches_GetAllParametersAsNeedToBeForTestTrue_AllSwitchesReturnTrueExceptCarOnlyHotelOnly()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "test" },
                        new AdvancedParameter() {  FieldName = "FLTCOUNT" },
                        new AdvancedParameter() {  FieldName = "CARCOUNT" },
                        new AdvancedParameter() {  FieldName = "ROOMTYPE" },
                        new AdvancedParameter() {  FieldName = "HTLCOUNT" }
                    }
                }
            };
            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "TRIPS", Name = "s" },
                new UserReportColumnInformation() { TableName = "a", Name = "UDID" },
                new UserReportColumnInformation() { TableName = "LEGS", Name = "something" },
                new UserReportColumnInformation() { TableName = "AUTO", Name = "something" },
                new UserReportColumnInformation() { TableName = "HOTEL", Name = "something" },
                new UserReportColumnInformation() { TableName = "a", Name = "HTLAVGRATE" },
                new UserReportColumnInformation() { TableName = "a", Name = "CARAVGRATE" },
                new UserReportColumnInformation() { TableName = "a", Name = "FLTCOUNT" },
                new UserReportColumnInformation() { TableName = "a", Name = "INVAMTNFEE" },
                new UserReportColumnInformation() { TableName = "SVCFEE", Name = "something" },
                new UserReportColumnInformation() { TableName = "AIRLEG", Name = "something" },
                new UserReportColumnInformation() { TableName = "RAIL", Name = "something" },
                new UserReportColumnInformation() { TableName = "MISCSEGS", Name = "something" },
                new UserReportColumnInformation() { TableName = "CHGLOG", Name = "CANCELCODE" },
                new UserReportColumnInformation() { TableName = "ONDMSEGS", Name = "something" },
                new UserReportColumnInformation() { TableName = "AUTHRZR", Name = "something" },
                new UserReportColumnInformation() { TableName = "TRAVAUTH", Name = "something" },
                new UserReportColumnInformation() { TableName = "MSTUR", Name = "something" },
                new UserReportColumnInformation() { TableName = "MSSEA", Name = "something" },
                new UserReportColumnInformation() { TableName = "MSLIM", Name = "something" },
                new UserReportColumnInformation() { TableName = "MSRAL", Name = "something" },
                new UserReportColumnInformation() { TableName = "ROOMTYPE", Name = "something" },
                new UserReportColumnInformation() { TableName = "s", Name = "CARCOUNT" }
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { tlsonly = true, colname = "test"  }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.CarSwitch);
            Assert.IsTrue(_tester.HotelSwitch);
            Assert.IsTrue(_tester.LegSwitch);
            Assert.IsTrue(_tester.ServiceFeeSwitch);
            Assert.IsTrue(_tester.RailLegSwitch);
            Assert.IsTrue(_tester.AirLegSwitch);
            Assert.IsTrue(_tester.MiscSegmentsSwitch);
            Assert.IsTrue(_tester.ChangeLogSwitch);
            Assert.IsTrue(_tester.MarketSegmentsSwitch);
            Assert.IsTrue(_tester.TravelAuthorizersSwitch);
            Assert.IsTrue(_tester.TraveAuthSwitch);
            Assert.IsTrue(_tester.HotelSwitch2);
            Assert.IsTrue(_tester.CarSwitch2);
            Assert.IsTrue(_tester.LegSwitch2);
            Assert.IsTrue(_tester.ServiceFeeSwitch2);
            Assert.IsTrue(_tester.HaveRoomType);
            Assert.IsTrue(_tester.MiscSegmentTourSwitch);
            Assert.IsTrue(_tester.MiscSegmentCruiseSwitch);
            Assert.IsTrue(_tester.MiscSegmentLimoSwitch);
            Assert.IsTrue(_tester.UdidSwitch);
            Assert.IsTrue(_tester.MiscSegmentRalSwitch);
            Assert.IsTrue(_tester.TripSwitch);
            Assert.IsTrue(_tester.TripTlsSwitch);

            Assert.IsFalse(_tester.HotelOnly);
            Assert.IsFalse(_tester.CarOnly);
        }

        [TestMethod]
        public void PopulateSwitches_UserReportColumnInformationOneTableNameMatches_CarSwitchIsTrue()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "test" }
                    }
                }
            };
            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "TRIPS", Name = "something" },
                new UserReportColumnInformation() { TableName = "HIBCARS", Name = "something" }
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { tlsonly = false, colname = "test"  }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.CarSwitch);
        }

        [TestMethod]
        public void PopulateSwitches_UserReportColumnInformationOneTableNameMatches_HotelSwitchIsTrue()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "test" }
                    }
                }
            };
            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "TRIPS", Name = "something" },
                new UserReportColumnInformation() { TableName = "IBHOTEL", Name = "something" }
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { tlsonly = false, colname = "test"  }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.HotelSwitch);
        }

        [TestMethod]
        public void PopulateSwitches_UserReportColumnInformationOneTableNameMatches_LegSwitchIsTrue()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "test" }
                    }
                }
            };
            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "TRIPS", Name = "something" },
                new UserReportColumnInformation() { TableName = "IBLEGS", Name = "something" }
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { tlsonly = false, colname = "test"  }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.LegSwitch);
        }

        [TestMethod]
        public void PopulateSwitches_UserReportColumnInformationOneTableNameMatches_ServiceFeeSwitchIsTrue()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "test" }
                    }
                }
            };
            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "TRIPS", Name = "something" },
                new UserReportColumnInformation() { TableName = "HIBSVCFEES", Name = "something" }
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { tlsonly = false, colname = "test"  }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.ServiceFeeSwitch);
        }


        [TestMethod]
        public void PopulateSwitches_UserReportIColumnInformationNameIsUDID_TheUdidIsSetToTrue()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "test" }
                    }
                }
            };
            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "TRIPS", Name = "UDID" }
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { tlsonly = false, colname = "test"  }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.UdidSwitch);
        }


        [TestMethod]
        public void PopulateSwitches_UserReportIColumnInformationNameIsTlsOnly_TheTripTlsIsSetToTrue()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "test"}
                    }
                }
            };
            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "TRIPS" , Name="something"}
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { tlsonly = true, colname = "test"  }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.TripTlsSwitch);
        }

        [TestMethod]
        public void PopulateSwitches_UserReportIColumnInformationTableNameIsChglog_TheChangeLogIsSetToTrue()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "test", }
                    }
                }
            };
            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "CHGLOG", Name="something"}
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { colname = "test" }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.ChangeLogSwitch);
        }

        [TestMethod]
        public void PopulateSwitches_UserReportIColumnInformationNameIsCancelCode_TheChangeLogIsSetToTrue()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "test", }
                    }
                }
            };
            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "something", Name="CANCELCODE"}
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { colname = "test" }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.ChangeLogSwitch);
        }

        [TestMethod]
        public void PopulateSwitches_UserReportIAdvancedParameterIsRoomType_TheNeedRoomTypeIsSetToTrue()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "ROOMTYPE" }
                    }
                }
            };
            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "something", Name="test"}
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { colname = "test" }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.NeedRoomType);
        }

        [TestMethod]
        public void PopulateSwitches_CustomAir_LegSwitchIsOn()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "ROOMTYPE" }
                    }
                },
                ProcessKey = (int)ReportTitles.AirUserDefinedReports
            };

            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "something", Name="CANCELCODE"}
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { colname = "test" }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.LegSwitch);
        }

        [TestMethod]
        public void PopulateSwitches_CustomCarr_CarSwitchIsOn()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() {  FieldName = "ROOMTYPE" }
                    }
                },
                ProcessKey = (int)ReportTitles.CarUserDefinedReports
            };

            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "something", Name="CANCELCODE"}
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { colname = "test" }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.CarSwitch);
        }


        [TestMethod]
        public void PopulateSwitches_CustomHotel_HotelSwitchIsOn()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                ProcessKey = (int)ReportTitles.HotelUserDefinedReports
            };

            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "something", Name="CANCELCODE"}
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { colname = "test" }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.HotelSwitch);
        }


        [TestMethod]
        public void PopulateSwitches_CustomServiceFee_ServiceFeeSwitchIsOn()
        {
            //Arrange
            ReportGlobals globals = new ReportGlobals()
            {
                ProcessKey = (int)ReportTitles.ServiceFeeUserDefinedReports
            };

            var columns = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation() { TableName = "something", Name="CANCELCODE"}
            };
            List<collist2> collist2List = new List<collist2>()
            {
                new collist2() { colname = "test" }
            };

            _tester = new SwitchManager(columns, globals);

            //Act
            _tester.PopulateSwitches(collist2List);

            //Assert
            Assert.IsTrue(_tester.ServiceFeeSwitch);
        }
    }
}

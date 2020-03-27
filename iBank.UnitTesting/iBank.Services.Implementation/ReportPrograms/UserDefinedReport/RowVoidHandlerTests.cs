using System;
using System.Collections.Generic;

using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Services.Implementation.ReportPrograms.UserDefined;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class RowVoidHandlerTests
    {
        [TestMethod]
        public void ZeroOutAirAmounts()
        {
            var row = new RawData { Airchg = 5, Offrdchg = 5, Stndchg = 5, Mktfare = 5, Basefare = 5, Faretax = 5, Tax1 = 5, Tax2 = 5, Tax3 = 5, Tax4 = 5 };

            RowVoidHandler.ZeroOutAirAmounts(row);

            Assert.AreEqual(0, row.Airchg);
            Assert.AreEqual(0, row.Offrdchg);
            Assert.AreEqual(0, row.Stndchg);
            Assert.AreEqual(0, row.Mktfare);
            Assert.AreEqual(0, row.Basefare);
            Assert.AreEqual(0, row.Faretax);
            Assert.AreEqual(0, row.Tax1);
            Assert.AreEqual(0, row.Tax2);
            Assert.AreEqual(0, row.Tax3);
            Assert.AreEqual(0, row.Tax4);
        }

        [TestMethod]
        public void AllCarAndHotelDataIsVoid_NoRecordsExist_ReturnTrue()
        {
            var carData = new List<CarRawData>();
            var hotelData = new List<HotelRawData>();

            var output = RowVoidHandler.AllCarAndHotelDataIsVoid(carData, hotelData);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void AllCarAndHotelDataIsVoid_RecordsExist_AllAreVoid_ReturnTrue()
        {
            var carData = new List<CarRawData> { new CarRawData { Cartrantyp = "V" } };
            var hotelData = new List<HotelRawData> { new HotelRawData { Hottrantyp = "V" } };

            var output = RowVoidHandler.AllCarAndHotelDataIsVoid(carData, hotelData);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void AllCarAndHotelDataIsVoid_RecordsExist_NonVoidCarDataExists_ReturnFalse()
        {
            var carData = new List<CarRawData> { new CarRawData { Cartrantyp = "I" } };
            var hotelData = new List<HotelRawData> { new HotelRawData { Hottrantyp = "V" } };

            var output = RowVoidHandler.AllCarAndHotelDataIsVoid(carData, hotelData);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void AllCarAndHotelDataIsVoid_RecordsExist_NonVoidHotelDataExists_ReturnFalse()
        {
            var carData = new List<CarRawData> { new CarRawData { Cartrantyp = "V" } };
            var hotelData = new List<HotelRawData> { new HotelRawData { Hottrantyp = "I" } };

            var output = RowVoidHandler.AllCarAndHotelDataIsVoid(carData, hotelData);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void AllOtherDataIsVoid_NoRecordsExist_ReturnTrue()
        {
            var carData = new List<CarRawData>();
            var hotelData = new List<HotelRawData>();
            var serviceFeeData = new List<ServiceFeeData>();

            var output = RowVoidHandler.AllOtherDataIsVoid(carData, hotelData, serviceFeeData);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void AllOtherDataIsVoid_RecordsExist_AllAreVoid_ReturnTrue()
        {
            var carData = new List<CarRawData> { new CarRawData { Cartrantyp = "V" } };
            var hotelData = new List<HotelRawData> { new HotelRawData { Hottrantyp = "V" } };
            var serviceFeeData = new List<ServiceFeeData>();

            var output = RowVoidHandler.AllOtherDataIsVoid(carData, hotelData, serviceFeeData);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void AllOtherDataIsVoid_RecordsExist_NonVoidCarDataExists_ReturnFalse()
        {
            var carData = new List<CarRawData> { new CarRawData { Cartrantyp = "I" } };
            var hotelData = new List<HotelRawData> { new HotelRawData { Hottrantyp = "V" } };
            var serviceFeeData = new List<ServiceFeeData>();

            var output = RowVoidHandler.AllOtherDataIsVoid(carData, hotelData, serviceFeeData);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void AllOtherDataIsVoid_RecordsExist_NonVoidHotelDataExists_ReturnFalse()
        {
            var carData = new List<CarRawData> { new CarRawData { Cartrantyp = "V" } };
            var hotelData = new List<HotelRawData> { new HotelRawData { Hottrantyp = "I" } };
            var serviceFeeData = new List<ServiceFeeData>();

            var output = RowVoidHandler.AllOtherDataIsVoid(carData, hotelData, serviceFeeData);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void AllOtherDataIsVoid_RecordsExist_NonVoidServiceFeeDataExists_ReturnFalse()
        {
            var carData = new List<CarRawData> { new CarRawData { Cartrantyp = "V" } };
            var hotelData = new List<HotelRawData> { new HotelRawData { Hottrantyp = "V" } };
            var serviceFeeData = new List<ServiceFeeData> { new ServiceFeeData { SfTranType = "I" } };

            var output = RowVoidHandler.AllOtherDataIsVoid(carData, hotelData, serviceFeeData);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void AllOtherDataIsVoid_RecordNoneExist_ReturnTrue()
        {
            var carData = new List<CarRawData> { new CarRawData { Cartrantyp = "V" } };
            var hotelData = new List<HotelRawData> { new HotelRawData { Hottrantyp = "V" } };
            var serviceFeeData = new List<ServiceFeeData> { new ServiceFeeData { SfTranType = "V" } };

            var output = RowVoidHandler.AllOtherDataIsVoid(carData, hotelData, serviceFeeData);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void AllOtherDataIsVoid_NoneVoidRecordExist_ReturnFalse()
        {
            var carData = new List<CarRawData> { new CarRawData { Cartrantyp = "I" } };
            var hotelData = new List<HotelRawData> { new HotelRawData { Hottrantyp = "I" } };
            var serviceFeeData = new List<ServiceFeeData> { new ServiceFeeData { SfTranType = "I" } };

            var output = RowVoidHandler.AllOtherDataIsVoid(carData, hotelData, serviceFeeData);

            Assert.AreEqual(false, output);
        }

        
    }
}

using System.Collections.Generic;

using Domain.Constants;
using Domain.Helper;
using Domain.Interfaces;
using Domain.Services;
using iBank.Entities.CISMasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.CarbonCalculations;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.CarbonCalculations
{
    [TestClass]
    public class CarbonCalculatorTests
    {
        private IList<FakeHotel> _hotelData;

        private IList<FakeCar> _carData;

        private IList<FakeAir> _airData;

        private ICisMastersDataStore _store;

        private ICacheService _cache;

        [TestInitialize]
        public void Init()
        {
            _hotelData = new List<FakeHotel> { new FakeHotel { HotelCo2 = 0, HPlusMin = 2, Nights = 3, Rooms = 4 } };
            _carData = new List<FakeCar>
                           {
                               new FakeCar { CarCo2 = 0, CarType = "CDAR", CPlusMin = 2, Days = 3 }, //4x4 car
                           };
            _airData = new List<FakeAir>
            {
                new FakeAir { AirCo2 = 0, AltCarCo2 = 0, AltRailCo2 = 0, ClassCat = "", DitCode = "", HaulType = "", Miles = 100 }              
            };

            var rates = GetRates();
            var hauls = GetHauls();

            var mockDb = new Mock<ICisMastersQueryable>();
            mockDb.Setup(x => x.CarbonCalculationRate).Returns(MockHelper.GetListAsQueryable(rates).Object);
            mockDb.Setup(x => x.CarbonCalculationHaul).Returns(MockHelper.GetListAsQueryable(hauls).Object);
            var mockStore = new Mock<ICisMastersDataStore>();
            mockStore.Setup(x => x.CisMastersQueryDb).Returns(mockDb.Object);
            _store = mockStore.Object;

            var mockCache = new Mock<ICacheService>();
            CarbonValue temp;
            mockCache.Setup(x => x.TryGetValue(CacheKeys.CarbonValuesLookup, out temp)).Returns(false);
            _cache = mockCache.Object;
        }

        private IList<CarbonCalculationRate> GetRates()
        {
            return new List<CarbonCalculationRate>
            {
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.UPLIFT_FACTOR, Rate = 2.5M },
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.LARGE_ENGINE_RATE, Rate = 3.5M },
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.MEDIUM_ENGINE_RATE, Rate = 4.5M },
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.LBS_PER_HOTEL_NIGHT_RATE, Rate = 5.5M },
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.ALT_CAR_RATE, Rate = 6.5M },
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.ALT_RAIL_DOMESTIC_RATE, Rate = 7.5M },
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.ALT_RAIL_INTL_RATE, Rate = 8.5M },
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.SHORT_HAUL_LBS_PER_MILE_RATE, Rate = 9.5M },
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.MED_HAUL_ECONOMY_LBS_PER_MILE_RATE, Rate = 10.5M },
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.MED_HAUL_DEFAULT_LBS_PER_MILE_RATE, Rate = 11.5M },
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.LONG_HAUL_ECONOMY_LBS_PER_MILE_RATE, Rate = 12.5M },
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.LONG_HAUL_BUSINESS_LBS_PER_MILE_RATE, Rate = 13.5M },
                new CarbonCalculationRate { RateName = CarbonCalculationKeys.LONG_HAUL_DEFAULT_LBS_PER_MILE_RATE, Rate = 14.5M }
            };
        }

        private IList<CarbonCalculationHaul> GetHauls()
        {
            return new List<CarbonCalculationHaul>
            {
                new CarbonCalculationHaul { HaulType = CarbonCalculationKeys.SHORT_HAUL, Abbreviation = "S", MileageLimit = 200 },
                new CarbonCalculationHaul { HaulType = CarbonCalculationKeys.MEDIUM_HAUL, Abbreviation = "M", MileageLimit = 500 },
                new CarbonCalculationHaul { HaulType = CarbonCalculationKeys.LONG_HAUL, Abbreviation = "L", MileageLimit = 0 }
            };
        }

        [TestMethod]
        public void SetHotelCarbon_UseImperial_IsBackOfficeData()
        {
            var sut = new CarbonCalculator(_cache, _store);

            sut.SetHotelCarbon(_hotelData, false, false);

            Assert.AreEqual(132, _hotelData[0].HotelCo2);
        }

        [TestMethod]
        public void SetHotelCarbon_UseImperial_IsReservationData()
        {
            var sut = new CarbonCalculator(_cache, _store);

            sut.SetHotelCarbon(_hotelData, false, true);

            Assert.AreEqual(66, _hotelData[0].HotelCo2);
        }

        [TestMethod]
        public void SetHotelCarbon_UseMetric_IsBackOfficeData()
        {
            var sut = new CarbonCalculator(_cache, _store);

            sut.SetHotelCarbon(_hotelData, true, false);

            Assert.AreEqual(60, _hotelData[0].HotelCo2);
        }

        [TestMethod]
        public void SetHotelCarbon_UseMetric_IsReservationData()
        {
            var sut = new CarbonCalculator(_cache, _store);

            sut.SetHotelCarbon(_hotelData, true, true);

            Assert.AreEqual(30, _hotelData[0].HotelCo2);
        }
        
        [TestMethod]
        public void SetCarCarbon_Imperial_BackOfficeData_FourByFourCar_IsLargeEngineRate()
        {
            var sut = new CarbonCalculator(_cache, _store);

            sut.SetCarCarbon(_carData, false, false);

            Assert.AreEqual(2100M, _carData[0].CarCo2);
        }
        
        [TestMethod]
        public void SetCarCarbon_Imperial_BackOfficeData_SportsCar_IsLargeEngineRate()
        {
            var sut = new CarbonCalculator(_cache, _store);
            _carData[0].CarType = "PFAR";

            sut.SetCarCarbon(_carData, false, false);

            Assert.AreEqual(2100M, _carData[0].CarCo2);
        }
        
        [TestMethod]
        public void SetCarCarbon_Imperial_BackOfficeData_DefaultLargeEngineDesignation_IsLargeEngineRate()
        {
            var sut = new CarbonCalculator(_cache, _store);
            _carData[0].CarType = "P";

            sut.SetCarCarbon(_carData, false, false);

            Assert.AreEqual(2100M, _carData[0].CarCo2);
        }
        
        [TestMethod]
        public void SetCarCarbon_Imperial_BackOfficeData_MediumEngine_IsMediumEngineRate()
        {
            var sut = new CarbonCalculator(_cache, _store);
            _carData[0].CarType = "XXX";

            sut.SetCarCarbon(_carData, false, false);

            Assert.AreEqual(2700M, _carData[0].CarCo2);
        }
        
        [TestMethod]
        public void SetCarCarbon_Imperial_ReservationData_IsMediumEngineRate()
        {
            var sut = new CarbonCalculator(_cache, _store);
            _carData[0].CarType = "XXX";

            sut.SetCarCarbon(_carData, false, true);

            Assert.AreEqual(1350M, _carData[0].CarCo2);
        }
        
        [TestMethod]
        public void SetCarCarbon_Metric_ReservationData_IsMediumEngineRate()
        {
            var sut = new CarbonCalculator(_cache, _store);
            _carData[0].CarType = "XXX";

            sut.SetCarCarbon(_carData, true, true);

            Assert.AreEqual(612.3M, _carData[0].CarCo2);
        }
        
        [TestMethod]
        public void SetCarCarbon_Metric_BackOfficeData_IsMediumEngineRate()
        {
            var sut = new CarbonCalculator(_cache, _store);
            _carData[0].CarType = "XXX";

            sut.SetCarCarbon(_carData, true, false);

            Assert.AreEqual(1224.7M, _carData[0].CarCo2);
        }
    }

    class FakeHotel : ICarbonCalculationsHotel
    {
        public int Nights { get; set; }
        public int Rooms { get; set; }
        public decimal HotelCo2 { get; set; }
        public int HPlusMin { get; set; }
    }

    class FakeCar : ICarbonCalculationsCar
    {
        public string CarType { get; set; }
        public int Days { get; set; }
        public decimal CarCo2 { get; set; }
        public int CPlusMin { get; set; }
    }

    class FakeAir : ICarbonCalculations
    {
        public int Miles { get; set; }
        public string ClassCat { get; set; }
        public string DitCode { get; set; }
        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; }
        public decimal AltRailCo2 { get; set; }
        public string HaulType { get; set; }
    }
}

using Domain.Models.ReportPrograms.TravelDetail;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.CarbonCalculations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TravelDetail.TravelDetailByUdid
{
    public class TopTravAllCarbonCalc
    {
        private readonly CarbonCalculator _carbonCalc = new CarbonCalculator();

        // Updates list with Carbon Emissions
        public void AddCarbonEmissionsToFinalData(List<TopTravAllByUdidFinalData> finalData, TravDetShared travDetShared, bool useMileageTable, bool useMetric, string carbonCalculator, bool isPreview)
        {
            CalculateCarbonEmissionForAirDataAndUpdateFinalData(finalData, travDetShared, useMileageTable, useMetric, carbonCalculator);
            CalculateCarbonEmissionForCarDataAndUpdateFinalData(finalData, travDetShared, useMetric, isPreview);
            CalculateCarbonEmissionForHotelDataAndUpdateFinalData(finalData, travDetShared, useMetric, isPreview);
        }

        // Adds Air CO2 to finaldata list
        public void CalculateCarbonEmissionForAirDataAndUpdateFinalData(List<TopTravAllByUdidFinalData> finalData, TravDetShared travDetShared, bool useMileageTable, bool useMetric, string carbonCalculator)
        {
            if(useMileageTable) AirMileageCalculator<LegRawData>.CalculateAirMileageFromTable(travDetShared.Legs);
            
            _carbonCalc.SetAirCarbon(travDetShared.Legs, useMetric, carbonCalculator);

            if (useMetric) MetricImperialConverter.ConvertMilesToKilometers(travDetShared.Legs);

            foreach (var recLoc in finalData.Select(s => s.RecLoc).Distinct().ToList())
            {
                List<TopTravAllByUdidFinalData> finalDataByRecLoc = finalData.Where(w => w.RecLoc.Equals(recLoc)).ToList();
                foreach (var item in finalDataByRecLoc)
                {
                    var co2 = Convert.ToInt32(travDetShared.Legs.Where(w => item.RecKey == w.RecKey).Sum(s => s.AirCo2));
                    item.Airco2 = item.tripcount >= 0 ? co2 : co2 * (-1);
                }
            }
        }

        // Adds Car CO2 to finaldata list
        public void CalculateCarbonEmissionForCarDataAndUpdateFinalData(List<TopTravAllByUdidFinalData> finalData, TravDetShared travDetShared, bool useMetric, bool isPreview)
        {
            _carbonCalc.SetCarCarbon(travDetShared.Cars, useMetric, isPreview);
            foreach (var recLoc in finalData.Select(s => s.RecLoc).Distinct().ToList())
            {
                List<TopTravAllByUdidFinalData> finalDataByRecLoc = finalData.Where(w => w.RecLoc.Equals(recLoc)).ToList();
                foreach (var item in finalDataByRecLoc)
                {
                    var co2 = Convert.ToInt32(travDetShared.Cars.Where(w => item.RecKey == w.RecKey).Sum(s => s.CarCo2));
                    item.Carco2 = item.cardays >= 0 ? co2 : co2 * (-1);
                }
            }
        }

        // Adds Hotel CO2 to finaldata list
        public void CalculateCarbonEmissionForHotelDataAndUpdateFinalData(List<TopTravAllByUdidFinalData> finalData, TravDetShared travDetShared, bool useMetric, bool isPreview)
        {
            _carbonCalc.SetHotelCarbon(travDetShared.Hotels, useMetric, isPreview);
            foreach (var recLoc in finalData.Select(s => s.RecLoc).Distinct().ToList())
            {
                List<TopTravAllByUdidFinalData> finalDataByRecLoc = finalData.Where(w => w.RecLoc.Equals(recLoc)).ToList();
                foreach (var item in finalDataByRecLoc)
                {
                    var co2 = Convert.ToInt32(travDetShared.Hotels.Where(w => item.RecKey == w.RecKey).Sum(s => s.HotelCo2));
                    item.HotelCo2 = item.hotnights >= 0 ? co2 : co2 * (-1);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.CarrierConcentrationReport;

namespace iBank.Services.Implementation.ReportPrograms.CarrierConcentration
{
    public class CarrierConcentrationDataProcessor
    {
        private readonly CarrierConcentrationCalculations _calc = new CarrierConcentrationCalculations();
        public void SetFlightMarkets(List<RawData> rawData)
        {
            foreach (var row in rawData)
            {
                row.Flt_mkt = row.Origin.Trim() + "-" + row.Destinat.Trim();
                row.Flt_mkt2 = row.Destinat.Trim() + "-" + row.Origin.Trim();
            }
        }

        public List<IntermediaryData> GetDataFilteredOnFlightSegments(List<IntermediaryData> data, string flightSegments, bool bidirectional)
        {
            var fsList = flightSegments.ToUpper().Split(',');
            return bidirectional
                ? data.Where(s => fsList.Contains(s.Flt_mkt) || fsList.Contains(s.Flt_mkt2)).ToList()
                : data.Where(s => fsList.Contains(s.Flt_mkt)).ToList();
        }

        public List<IntermediaryData> GetDataWithCarrierOneSegments(List<IntermediaryData> data)
        {
            return data.Where(s => s.Carr1Segs > 0).ToList();
        }

        public void AllocateAirCharge(List<RawData> rawData)
        {
            var groups = rawData.Where(s => s.ActFare == 0)
                    .GroupBy(s => new { s.RecKey, AirChg = s.Airchg - s.Faretax }, (key, recs) => new
                    {
                        key.RecKey,
                        key.AirChg,
                        NumSegs = recs.Count()
                    }).ToList();

            foreach (var group in groups)
            {
                if (group.AirChg != 0 && group.NumSegs != 0)
                {
                    var legs = rawData.Where(s => s.RecKey == group.RecKey);
                    foreach (var leg in legs)
                    {
                        leg.ActFare = _calc.GetAccountFare(group.AirChg, group.NumSegs);
                    }
                }
            }
        }

        public void SetConsistentCityPairs(List<RawData> rawData)
        {
            foreach (var row in rawData)
            {
                row.Orgdestemp = string.Compare(row.Origin, row.Destinat, StringComparison.Ordinal) < 0 ? row.Origin : row.Destinat;
                row.Destinat = string.Compare(row.Origin, row.Destinat, StringComparison.Ordinal) >= 0 ? row.Origin : row.Destinat;
                row.Origin = row.Orgdestemp;
            }
        }
    }
}
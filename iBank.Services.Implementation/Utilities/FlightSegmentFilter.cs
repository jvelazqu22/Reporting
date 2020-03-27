using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Interfaces;

namespace iBank.Services.Implementation.Utilities
{
    public class FlightSegmentFilter<T> where T : class, IFlightSegment
    {
        public IList<T> UpdateFlightMarkets(IList<T> data)
        {
            foreach (var record in data)
            {
                record.Flt_mkt = record.Origin.Trim() + "-" + record.Destinat.Trim();
                record.Flt_mkt2 = record.Destinat.Trim() + "-" + record.Origin.Trim();
            }

            return data;
        }

        public IList<T> FilterOnFlightSegment(IList<T> rawData, string textFlightSegments, bool isBothWays)
        {
            rawData = UpdateFlightMarkets(rawData);

            if (isBothWays)
            {
                rawData = rawData.Where(s => textFlightSegments.IndexOf(s.Flt_mkt, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            textFlightSegments.IndexOf(s.Flt_mkt2, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
            }
            else
            {
                rawData = rawData.Where(s => textFlightSegments.IndexOf(s.Flt_mkt, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
            }

            return rawData;
        }
    }
}

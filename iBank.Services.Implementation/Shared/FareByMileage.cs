using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Interfaces;

namespace iBank.Services.Implementation.Shared
{
    public class FareByMileage<T> where T :class,IFareByMileage
    {
        /// <summary>
        /// Recalculates the actual fare of each leg by mileage  
        /// </summary>
        /// <param name="rawData"></param>
        public static void CalculateFareByMileage(List<T> rawData)
        {
            var groupsByReckey = rawData.GroupBy(s => s.RecKey, (key, recs) =>
            {
                var recList = recs.ToList();
                return new
                {
                    RecKey = key,
                    BaseFare = recList.Max(s => s.BaseFare),
                    NumLegs = recList.Count,
                    TotalMiles = recList.Sum(s => s.Miles)
                };
            });

            foreach (var group in groupsByReckey)
            {
                if (group.BaseFare != 0 && group.NumLegs > 0 && group.TotalMiles != 0)
                {
                    //get all the legs for this key
                    var legs = rawData.Where(s => s.RecKey == group.RecKey).ToList();
                    var totalFare = 0m;
                    var legCounter = 0;
                    foreach (var leg in legs)
                    {
                        legCounter++;
                        var dMiles = new decimal(leg.Miles); //use double to get better precision
                        var factor = Math.Round(dMiles / group.TotalMiles, 12);
                        var legFare = Math.Round(group.BaseFare*factor, 2);
                        totalFare += legFare;
                        if (legCounter == legs.Count)
                        {
                            leg.ActFare = legFare + (group.BaseFare - totalFare); //rounding adjustment
                        }
                        else
                        {
                            leg.ActFare = legFare;
                        }
                    }
                }
            }
        }
    }
}

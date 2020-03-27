using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummarywithGraphs
{
    public class DataTransformer
    {
        public static void CorrectValidatingCarrierMode(List<RawData> rawData, List<CityPairRawData> cityPairRawData)
        {
            foreach (var row in rawData)
            {
                var city = cityPairRawData.FirstOrDefault(s => s.RecKey == row.RecKey && s.Airline.EqualsIgnoreCase(row.Valcarr));
                if (city != null)
                {
                    row.ValcarMode = city.Mode;
                }
                else
                {
                    city = cityPairRawData.FirstOrDefault(s => s.RecKey == row.RecKey);
                    row.ValcarMode = city == null ? "A" : city.Mode;
                }
                row.Mode = row.ValcarMode;
            }
        }

        public static void ProcessRawData(List<RawData> rawData, List<string> exlusionReasons, bool useBaseFare)
        {
            foreach (var row in rawData)
            {
                if (useBaseFare)
                {
                    //DEALING WITH BAD DATA.
                    if (row.Basefare == 0) row.Basefare = row.Airchg;

                    row.Stndchg = Math.Abs(row.Stndchg) < Math.Abs(row.Basefare) || row.Stndchg == 0 || (row.Stndchg > 0 && row.Basefare < 0)
                                      ? row.Basefare
                                      : row.Stndchg;
                    row.Offrdchg = (row.Offrdchg > 0 && row.Basefare < 0)
                                       ? 0 - row.Offrdchg
                                       : row.Offrdchg == 0 ? row.Basefare : row.Offrdchg;
                    row.Savings = row.Stndchg - row.Basefare;
                    row.Mktfare = row.Mktfare == 0 ? row.Basefare : row.Mktfare;
                    row.LostAmt = row.Basefare - row.Offrdchg;
                }
                else
                {
                    row.Stndchg = Math.Abs(row.Stndchg) < Math.Abs(row.Airchg) || row.Stndchg == 0 || (row.Stndchg > 0 && row.Airchg < 0)
                                      ? row.Airchg
                                      : row.Stndchg;
                    row.Offrdchg = (row.Offrdchg > 0 && row.Airchg < 0)
                                       ? 0 - row.Offrdchg
                                       : row.Offrdchg == 0 ? row.Airchg : row.Offrdchg;
                    row.Savings = row.Stndchg - row.Airchg;
                    row.Mktfare = row.Mktfare == 0 ? row.Airchg : row.Mktfare;
                    row.LostAmt = row.Airchg - row.Offrdchg;
                }
                row.Reascode = exlusionReasons.Contains(row.Reascode) ? string.Empty : row.Reascode;

                if (string.IsNullOrEmpty(row.Savingcode) && !string.IsNullOrEmpty(row.Reascode) && row.LostAmt == 0 && row.Savings > 0)
                {
                    row.Savingcode = row.Reascode;
                    row.Reascode = string.Empty;
                }

                if ((row.LostAmt < 0 && row.Plusmin > 0) || (row.LostAmt > 0 && row.Plusmin < 0))
                {
                    row.NegoSvngs = 0 - row.LostAmt;
                    row.LostAmt = 0;
                }
            }
        }
    }
}

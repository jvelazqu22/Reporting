using iBank.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    public class AirSummaryBuilder
    {
        public void AddTripRows(int groupSort, string groupName, List<RawData> airRawDataPy, List<RawData> airRawDataCy, List<FeeRawData> feeRawDataPy,
            List<FeeRawData> feeRawDataCy, List<LegRawData> legRawDataPy, List<LegRawData> legRawDataCy, List<FinalData> rows, DateTime begMonth,
            DateTime endDate, DateTime begMonth2, DateTime endDate2, bool carbonReporting, string poundsKilos, bool splitRail, bool excludeMiles,
            bool excludeExceptions, bool excludeServiceFees, bool excludeNegotiatedSavings, string milesKilos, bool excludeSavings)
        {
            var processingAirData = groupSort == 1;
            
            List<RawData> tripDataPreviousYear;
            List<RawData> tripDataCurrentYear;
            List<LegRawData> legDataPreviousYear;
            List<LegRawData> legDataCurrentYear;

            if (splitRail)
            {
                tripDataPreviousYear = processingAirData
                                         ? airRawDataPy.Where(s => !s.ValCarMode.EqualsIgnoreCase("R")).ToList()
                                         : airRawDataPy.Where(s => s.ValCarMode.EqualsIgnoreCase("R")).ToList();

                tripDataCurrentYear = processingAirData
                                         ? airRawDataCy.Where(s => !s.ValCarMode.EqualsIgnoreCase("R")).ToList()
                                         : airRawDataCy.Where(s => s.ValCarMode.EqualsIgnoreCase("R")).ToList();

                legDataPreviousYear = processingAirData
                                          ? legRawDataPy.Where(s => !s.Mode.EqualsIgnoreCase("R")).ToList()
                                          : legRawDataPy.Where(s => s.Mode.EqualsIgnoreCase("R")).ToList();

                legDataCurrentYear = processingAirData
                                         ? legRawDataCy.Where(s => !s.Mode.EqualsIgnoreCase("R")).ToList()
                                         : legRawDataCy.Where(s => s.Mode.EqualsIgnoreCase("R")).ToList();
            }
            else
            {
                tripDataPreviousYear = airRawDataPy;
                tripDataCurrentYear = airRawDataCy;

                legDataPreviousYear = legRawDataPy;
                legDataCurrentYear = legRawDataCy;
            }
            
            var totTickets = GetTicketCount(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, tripDataPreviousYear, tripDataCurrentYear);
            rows.Add(totTickets);
            
            var invoiceCount = GetInvoiceCount(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, tripDataPreviousYear, tripDataCurrentYear);
            rows.Add(invoiceCount);
            
            var creditCount = GetCreditCount(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, tripDataPreviousYear, tripDataCurrentYear);
            rows.Add(creditCount);

            //Net # of Tickets (invoice - credit)
            var netTickets = GetNetNumberTickets(groupSort, groupName, invoiceCount, creditCount);
            rows.Add(netTickets);

            //Exchanges 
            var exchangeCount = GetNetNumberExchanges(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, tripDataPreviousYear, tripDataCurrentYear);
            rows.Add(exchangeCount);

            //original ticket (invoices - credits - exchanges)
            rows.Add(GetNetNumberOriginalTickets(groupSort, groupName, invoiceCount, creditCount, exchangeCount));
            
            var sameDayCount = GetNetNumberSameDayTrips(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, tripDataPreviousYear, tripDataCurrentYear);
            rows.Add(sameDayCount);

            //original ticket (invoices - credits - sameday)
            rows.Add(GetNetNumberOvernightTrips(groupSort, groupName, invoiceCount, creditCount, sameDayCount));

            var advanceCount = GetAverageDaysPurchasedInAdvance(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, invoiceCount, tripDataPreviousYear, tripDataCurrentYear);
            rows.Add(advanceCount);

            var totAirCharges = GetTotalAirCharges(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, tripDataPreviousYear, tripDataCurrentYear);
            rows.Add(totAirCharges);

            rows.Add(GetAverageTicketPrice(groupSort, groupName, netTickets, totAirCharges));

            if (!excludeSavings)
            {
                var savings = GetSavingsFromStandardFare(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, tripDataPreviousYear, tripDataCurrentYear);
                rows.Add(savings);
            }

            if (!excludeNegotiatedSavings)
            {
                var negoSavings = GetNegotiatedSavings(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, tripDataPreviousYear, tripDataCurrentYear);
                rows.Add(negoSavings);
            }


            if (!excludeExceptions)
            {
                var lostSavings = GetLostSavings(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, tripDataPreviousYear, tripDataCurrentYear);
                rows.Add(lostSavings);

                rows.Add(GetPercentageLost(groupSort, groupName, totAirCharges, lostSavings));
            }

            if (!excludeServiceFees)
            {
                List<FeeRawData> feeDataPreviousYear;
                List<FeeRawData> feeDataCurrentYear;
                if (splitRail)
                {
                    feeDataPreviousYear = processingAirData
                                    ? feeRawDataPy.Where(s => !s.ValcarMode.EqualsIgnoreCase("R")).ToList()
                                    : feeRawDataPy.Where(s => s.ValcarMode.EqualsIgnoreCase("R")).ToList();

                    feeDataCurrentYear = processingAirData
                                    ? feeRawDataCy.Where(s => !s.ValcarMode.EqualsIgnoreCase("R")).ToList()
                                    : feeRawDataCy.Where(s => s.ValcarMode.EqualsIgnoreCase("R")).ToList();
                }
                else
                {
                    feeDataPreviousYear = feeRawDataPy;
                    feeDataCurrentYear = feeRawDataCy;
                }

                rows.Add(GetServiceFees(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, feeDataPreviousYear, feeDataCurrentYear));
            }

            if (carbonReporting)
            {
                var totCo2 = GetTotalCarbonEmissions(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, poundsKilos, legDataPreviousYear, 
                    legDataCurrentYear);
                rows.Add(totCo2);

                var avgCo2 = GetAvgCarbonEmissions(groupSort, groupName, poundsKilos, totTickets, totCo2);
                rows.Add(avgCo2);
            }

            if (!excludeMiles)
            {
                var totalMiles = GetTotalMiles(groupSort, groupName, begMonth, endDate, begMonth2, endDate2, milesKilos, legDataPreviousYear, legDataCurrentYear);
                rows.Add(totalMiles);

                var averageMilesPerTrip = GetAverageMilesPerTrip(groupSort, groupName, milesKilos, netTickets, totalMiles);
                rows.Add(averageMilesPerTrip);

                var avgCostPerMile = GetAverageCostPerMile(groupSort, groupName, milesKilos, totalMiles, totAirCharges);
                rows.Add(avgCostPerMile);
            }
        }
        private static FinalData GetTotalCarbonEmissions(int groupSort, string groupName, DateTime beginMonth, DateTime endDate, DateTime beginMonth2, DateTime endDate2, 
            string poundsKilos, List<LegRawData> legDataPy, List<LegRawData> legDataCy)
        {
            return new FinalData
                       {
                           RowNum = 17,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = $"Total CO2 Emissions ({poundsKilos}):",
                           DisplayType = DisplayType.Integer,
                           MonthValuePreviousYear = legDataPy.Where(s => s.UseDate >= beginMonth && s.UseDate <= endDate).Sum(s => s.AirCo2),
                           YearToDatePreviousYear = legDataPy.Sum(s => s.AirCo2),
                           MonthValueCurrentYear = legDataCy.Where(s => s.UseDate >= beginMonth2 && s.UseDate <= endDate2).Sum(s => s.AirCo2),
                           YearToDateCurrentYear = legDataCy.Sum(s => s.AirCo2),
                       };
        }

        private static FinalData GetAvgCarbonEmissions(int groupSort, string groupName, string poundsKilos, FinalData totTickets, FinalData totCo2)
        {
            return new FinalData
                       {
                           RowNum = 18,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = $"Average CO2 per Trip ({poundsKilos}):",
                           DisplayType = DisplayType.Integer,
                           MonthValuePreviousYear = totTickets.MonthValuePreviousYear == 0 ? 0 : totCo2.MonthValuePreviousYear / totTickets.MonthValuePreviousYear,
                           YearToDatePreviousYear = totTickets.YearToDatePreviousYear == 0 ? 0 : totCo2.YearToDatePreviousYear / totTickets.YearToDatePreviousYear,
                           MonthValueCurrentYear = totTickets.MonthValueCurrentYear == 0 ? 0 : totCo2.MonthValueCurrentYear / totTickets.MonthValueCurrentYear,
                           YearToDateCurrentYear = totTickets.YearToDateCurrentYear == 0 ? 0 : totCo2.YearToDateCurrentYear / totTickets.YearToDateCurrentYear
                       };
        }

        private static FinalData GetTotalMiles(int groupSort, string groupName, DateTime beginMonth, DateTime endDate, DateTime beginMonth2, DateTime endDate2,
            string milesKilos, List<LegRawData> legDataPrevYr, List<LegRawData> legDataCurrYr)
        {
            return new FinalData
                       {
                           RowNum = 19,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = groupSort == 1 ? $"Total {milesKilos}s Flown:" : $"Total {milesKilos}s:",
                           DisplayType = DisplayType.Integer,
                           MonthValuePreviousYear = legDataPrevYr.Where(s => s.UseDate >= beginMonth && s.UseDate <= endDate)
                                                        .Sum(s => Math.Abs(s.Miles) * s.PlusMin),
                           YearToDatePreviousYear = legDataPrevYr.Sum(s => Math.Abs(s.Miles) * s.PlusMin),
                           MonthValueCurrentYear = legDataCurrYr.Where(s => s.UseDate >= beginMonth2 && s.UseDate <= endDate2)
                                                        .Sum(s => Math.Abs(s.Miles) * s.PlusMin),
                           YearToDateCurrentYear = legDataCurrYr.Sum(s => Math.Abs(s.Miles) * s.PlusMin)
                       };
        }

        private static FinalData GetAverageMilesPerTrip(int groupSort, string groupName, string milesKilos, FinalData netTickets, FinalData totMiles)
        {
            var averageMilesPerTrip = new FinalData
                                          {
                                              RowNum = 20,
                                              Grp = groupName,
                                              GrpSort = groupSort,
                                              SubGrp = 1,
                                              Descrip = $"Average {milesKilos}s per Trip:",
                                              DisplayType = DisplayType.Integer,
                                              MonthValuePreviousYear = netTickets.MonthValuePreviousYear == 0 ? 0 : totMiles.MonthValuePreviousYear / netTickets.MonthValuePreviousYear,
                                              YearToDatePreviousYear = netTickets.YearToDatePreviousYear == 0 ? 0 : totMiles.YearToDatePreviousYear / netTickets.YearToDatePreviousYear,
                                              MonthValueCurrentYear = netTickets.MonthValueCurrentYear == 0 ? 0 : totMiles.MonthValueCurrentYear / netTickets.MonthValueCurrentYear,
                                              YearToDateCurrentYear = netTickets.YearToDateCurrentYear == 0 ? 0 : totMiles.YearToDateCurrentYear / netTickets.YearToDateCurrentYear
                                          };

            return averageMilesPerTrip;
        }

        private static FinalData GetAverageCostPerMile(int groupSort, string groupName, string milesKilos, FinalData totMiles, FinalData totAirCharges)
        {
            return new FinalData
                       {
                           RowNum = 21,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = $"Average Cost per {milesKilos}:",
                           DisplayType = DisplayType.Currency,
                           MonthValuePreviousYear = totMiles.MonthValuePreviousYear == 0 ? 0 : totAirCharges.MonthValuePreviousYear / totMiles.MonthValuePreviousYear,
                           YearToDatePreviousYear = totMiles.YearToDatePreviousYear == 0 ? 0 : totAirCharges.YearToDatePreviousYear / totMiles.YearToDatePreviousYear,
                           MonthValueCurrentYear = totMiles.MonthValueCurrentYear == 0 ? 0 : totAirCharges.MonthValueCurrentYear / totMiles.MonthValueCurrentYear,
                           YearToDateCurrentYear = totMiles.YearToDateCurrentYear == 0 ? 0 : totAirCharges.YearToDateCurrentYear / totMiles.YearToDateCurrentYear
                       };
        }

        private static FinalData GetServiceFees(int groupSort, string groupName, DateTime begMonth, DateTime endDate, DateTime begMonth2, DateTime endDate2,
            List<FeeRawData> feeDataPreviousYear, List<FeeRawData> feeDataCurrentYear)
        {
            return new FinalData
                       {
                           RowNum = 16,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = "Service Fees:",
                           DisplayType = DisplayType.Currency,
                           MonthValuePreviousYear = feeDataPreviousYear.Where(s => s.UseDate >= begMonth && s.UseDate <= endDate).Sum(s => s.SvcAmt),
                           YearToDatePreviousYear = feeDataPreviousYear.Sum(s => s.SvcAmt),
                           MonthValueCurrentYear = feeDataCurrentYear.Where(s => s.UseDate >= begMonth2 && s.UseDate <= endDate2).Sum(s => s.SvcAmt),
                           YearToDateCurrentYear = feeDataCurrentYear.Sum(s => s.SvcAmt)
                       };
        }

        private static FinalData GetPercentageLost(int groupSort, string groupName, FinalData totAirCharges, FinalData lostSavings)
        {
            return new FinalData
                       {
                           RowNum = 14,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = "% Lost:",
                           DisplayType = DisplayType.Percentage,
                           MonthValuePreviousYear = totAirCharges.MonthValuePreviousYear == 0 ? 0 : lostSavings.MonthValuePreviousYear / totAirCharges.MonthValuePreviousYear,
                           YearToDatePreviousYear = totAirCharges.YearToDatePreviousYear == 0 ? 0 : lostSavings.YearToDatePreviousYear / totAirCharges.YearToDatePreviousYear,
                           MonthValueCurrentYear = totAirCharges.MonthValueCurrentYear == 0 ? 0 : lostSavings.MonthValueCurrentYear / totAirCharges.MonthValueCurrentYear,
                           YearToDateCurrentYear = totAirCharges.YearToDateCurrentYear == 0 ? 0 : lostSavings.YearToDateCurrentYear / totAirCharges.YearToDateCurrentYear
                       };
        }

        private static FinalData GetLostSavings(int groupSort, string groupName, DateTime begMonth, DateTime endDate, DateTime begMonth2, DateTime endDate2,
                                                List<RawData> tripDataPreviousYear, List<RawData> tripDataCurrentYear)
        {
            return new FinalData
                        {
                            RowNum = 13,
                            Grp = groupName,
                            GrpSort = groupSort,
                            SubGrp = 1,
                            Descrip = "Lost Savings:",
                            DisplayType = DisplayType.Currency,
                            MonthValuePreviousYear = tripDataPreviousYear.Where(s => s.UseDate >= begMonth && s.UseDate <= endDate).Sum(s => s.LostAmt),
                            YearToDatePreviousYear = tripDataPreviousYear.Sum(s => s.LostAmt),
                            MonthValueCurrentYear = tripDataCurrentYear.Where(s => s.UseDate >= begMonth2 && s.UseDate <= endDate2).Sum(s => s.LostAmt),
                            YearToDateCurrentYear = tripDataCurrentYear.Sum(s => s.LostAmt)
                        };
        }

        private static FinalData GetNegotiatedSavings(int groupSort, string groupName, DateTime begMonth, DateTime endDate, DateTime begMonth2, DateTime endDate2,
                                                      List<RawData> tripDataPreviousYear, List<RawData> tripDataCurrentYear)
        {
            return new FinalData
                            {
                                RowNum = 13,
                                Grp = groupName,
                                GrpSort = groupSort,
                                SubGrp = 1,
                                Descrip = "Negotiated Savings:",
                                DisplayType = DisplayType.Currency,
                                MonthValuePreviousYear = tripDataPreviousYear.Where(s => s.UseDate >= begMonth && s.UseDate <= endDate).Sum(s => s.NegoSvngs),
                                YearToDatePreviousYear = tripDataPreviousYear.Sum(s => s.NegoSvngs),
                                MonthValueCurrentYear = tripDataCurrentYear.Where(s => s.UseDate >= begMonth2 && s.UseDate <= endDate2).Sum(s => s.NegoSvngs),
                                YearToDateCurrentYear = tripDataCurrentYear.Sum(s => s.NegoSvngs),
                            };
        }

        private static FinalData GetSavingsFromStandardFare(int groupSort, string groupName, DateTime begMonth, DateTime endDate, DateTime begMonth2, DateTime endDate2,
                                                            List<RawData> tripDataPreviousYear, List<RawData> tripDataCurrentYear)
        {
            return new FinalData
                        {
                            RowNum = 12,
                            Grp = groupName,
                            GrpSort = groupSort,
                            SubGrp = 1,
                            Descrip = "Savings from Std Fare:",
                            DisplayType = DisplayType.Currency,
                            MonthValuePreviousYear = tripDataPreviousYear.Where(s => s.UseDate >= begMonth && s.UseDate <= endDate).Sum(s => s.Savings),
                            YearToDatePreviousYear = tripDataPreviousYear.Sum(s => s.Savings),
                            MonthValueCurrentYear = tripDataCurrentYear.Where(s => s.UseDate >= begMonth2 && s.UseDate <= endDate2).Sum(s => s.Savings),
                            YearToDateCurrentYear = tripDataCurrentYear.Sum(s => s.Savings),
                        };
        }

        private static FinalData GetAverageTicketPrice(int groupSort, string groupName, FinalData netTickets, FinalData totAirCharges)
        {
            return new FinalData
                       {
                           RowNum = 11,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = "Average Ticket Price:",
                           DisplayType = DisplayType.Currency,
                           MonthValuePreviousYear = netTickets.MonthValuePreviousYear == 0 ? 0 : totAirCharges.MonthValuePreviousYear / netTickets.MonthValuePreviousYear,
                           YearToDatePreviousYear = netTickets.YearToDatePreviousYear == 0 ? 0 : totAirCharges.YearToDatePreviousYear / netTickets.YearToDatePreviousYear,
                           MonthValueCurrentYear = netTickets.MonthValueCurrentYear == 0 ? 0 : totAirCharges.MonthValueCurrentYear / netTickets.MonthValueCurrentYear,
                           YearToDateCurrentYear = netTickets.YearToDateCurrentYear == 0 ? 0 : totAirCharges.YearToDateCurrentYear / netTickets.YearToDateCurrentYear
                       };
        }

        private static FinalData GetTotalAirCharges(int groupSort, string groupName, DateTime begMonth, DateTime endDate, DateTime begMonth2, DateTime endDate2,
                                                    List<RawData> tripDataPreviousYear, List<RawData> tripDataCurrentYear)
        {
            return new FinalData
                        {
                            RowNum = 10,
                            Grp = groupName,
                            GrpSort = groupSort,
                            SubGrp = 1,
                            Descrip = "Total Air Charges:",
                            DisplayType = DisplayType.Currency,
                            MonthValuePreviousYear = tripDataPreviousYear.Where(s => s.UseDate >= begMonth && s.UseDate <= endDate).Sum(s => s.Airchg),
                            YearToDatePreviousYear = tripDataPreviousYear.Sum(s => s.Airchg),
                            MonthValueCurrentYear = tripDataCurrentYear.Where(s => s.UseDate >= begMonth2 && s.UseDate <= endDate2).Sum(s => s.Airchg),
                            YearToDateCurrentYear = tripDataCurrentYear.Sum(s => s.Airchg),
                        };
        }

        private static FinalData GetAverageDaysPurchasedInAdvance(int groupSort, string groupName, DateTime begMonth, DateTime endDate, DateTime begMonth2,
                                                                  DateTime endDate2, FinalData invoiceCount, List<RawData> tripDataPreviousYear, List<RawData> tripDataCurrentYear)
        {
            var advanceCount = new FinalData();
            advanceCount.RowNum = 9;
            advanceCount.Grp = groupName;
            advanceCount.GrpSort = groupSort;
            advanceCount.SubGrp = 1;
            advanceCount.Descrip = "Avg Days Purchased in Advance:";
            advanceCount.DisplayType = DisplayType.Integer;
            advanceCount.MonthValuePreviousYear = invoiceCount.MonthValuePreviousYear == 0
                                              ? 0
                                              : tripDataPreviousYear.Where(s => s.UseDate >= begMonth && s.UseDate <= endDate && s.Plusmin == 1)
                                                                    .Sum(s => s.Advance)
                                                / invoiceCount.MonthValuePreviousYear;
            advanceCount.YearToDatePreviousYear = invoiceCount.YearToDatePreviousYear == 0
                                              ? 0
                                              : tripDataPreviousYear.Where(s => s.Plusmin == 1)
                                                                    .Sum(s => s.Advance) 
                                                / invoiceCount.YearToDatePreviousYear;
            advanceCount.MonthValueCurrentYear = invoiceCount.MonthValueCurrentYear == 0
                                              ? 0
                                              : tripDataCurrentYear.Where(s => s.UseDate >= begMonth2 && s.UseDate <= endDate2 && s.Plusmin == 1)
                                                                    .Sum(s => s.Advance)
                                                / invoiceCount.MonthValueCurrentYear;
            advanceCount.YearToDateCurrentYear = invoiceCount.YearToDateCurrentYear == 0
                                              ? 0
                                              : tripDataCurrentYear.Where(s => s.Plusmin == 1)
                                                                    .Sum(s => s.Advance)
                                                / invoiceCount.YearToDateCurrentYear;
            return advanceCount;
        }

        private static FinalData GetNetNumberOvernightTrips(int groupSort, string groupName, FinalData invoiceCount, FinalData creditCount, FinalData sameDayCount)
        {
            return new FinalData
                       {
                           RowNum = 8,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = "Net # Overnight Trips (Estimated):",
                           DisplayType = DisplayType.Integer,
                           MonthValuePreviousYear = invoiceCount.MonthValuePreviousYear - creditCount.MonthValuePreviousYear - sameDayCount.MonthValuePreviousYear,
                           YearToDatePreviousYear = invoiceCount.YearToDatePreviousYear - creditCount.YearToDatePreviousYear - sameDayCount.YearToDatePreviousYear,
                           MonthValueCurrentYear = invoiceCount.MonthValueCurrentYear - creditCount.MonthValueCurrentYear - sameDayCount.MonthValueCurrentYear,
                           YearToDateCurrentYear = invoiceCount.YearToDateCurrentYear - creditCount.YearToDateCurrentYear - sameDayCount.YearToDateCurrentYear
                       };
        }

        private static FinalData GetNetNumberSameDayTrips(int groupSort, string groupName, DateTime begMonth, DateTime endDate, DateTime begMonth2, DateTime endDate2, 
            List<RawData> tripDataPreviousYear, List<RawData> tripDataCurrentYear)
        {
            return new FinalData
                       {
                           RowNum = 7,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = "Net # Same Day RoundTrips / OneWays (Est'd):",
                           DisplayType = DisplayType.Integer,
                           MonthValuePreviousYear = tripDataPreviousYear.Where(s => s.UseDate >= begMonth && s.UseDate <= endDate && s.Depdate == s.ArrDate).Sum(s => s.Plusmin),
                           YearToDatePreviousYear = tripDataPreviousYear.Where(s => s.Depdate == s.ArrDate).Sum(s => s.Plusmin),
                           MonthValueCurrentYear = tripDataCurrentYear.Where(s => s.UseDate >= begMonth2 && s.UseDate <= endDate2 && s.Depdate == s.ArrDate).Sum(s => s.Plusmin),
                           YearToDateCurrentYear = tripDataCurrentYear.Where(s => s.Depdate == s.ArrDate).Sum(s => s.Plusmin),
                       };
        }

        private static FinalData GetNetNumberOriginalTickets(int groupSort, string groupName, FinalData invoiceCount, FinalData creditCount, FinalData exchangeCount)
        {
            return new FinalData
                       {
                           RowNum = 5,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = "Net # of Original Tickets:",
                           DisplayType = DisplayType.Integer,
                           MonthValuePreviousYear = invoiceCount.MonthValuePreviousYear - creditCount.MonthValuePreviousYear - exchangeCount.MonthValuePreviousYear,
                           YearToDatePreviousYear = invoiceCount.YearToDatePreviousYear - creditCount.YearToDatePreviousYear - exchangeCount.YearToDatePreviousYear,
                           MonthValueCurrentYear = invoiceCount.MonthValueCurrentYear - creditCount.MonthValueCurrentYear - exchangeCount.MonthValueCurrentYear,
                           YearToDateCurrentYear = invoiceCount.YearToDateCurrentYear - creditCount.YearToDateCurrentYear - exchangeCount.YearToDateCurrentYear
                       };
        }

        private static FinalData GetNetNumberExchanges(int groupSort, string groupName, DateTime begMonth, DateTime endDate, DateTime begMonth2, DateTime endDate2, 
            List<RawData> tripDataPreviousYear, List<RawData> tripDataCurrentYear)
        {
            return new FinalData
                       {
                           RowNum = 6,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = "Net # of Exchanges:",
                           DisplayType = DisplayType.Integer,
                           MonthValuePreviousYear = tripDataPreviousYear.Where(s => s.UseDate >= begMonth && s.UseDate <= endDate && s.Exchange).Sum(s => s.Plusmin),
                           YearToDatePreviousYear = tripDataPreviousYear.Where(s => s.Exchange).Sum(s => s.Plusmin),
                           MonthValueCurrentYear = tripDataCurrentYear.Where(s => s.UseDate >= begMonth2 && s.UseDate <= endDate2 && s.Exchange).Sum(s => s.Plusmin),
                           YearToDateCurrentYear = tripDataCurrentYear.Where(s => s.Exchange).Sum(s => s.Plusmin),
                       };
        }

        private static FinalData GetNetNumberTickets(int groupSort, string groupName, FinalData invoiceCount, FinalData creditCount)
        {
            return new FinalData
                       {
                           RowNum = 4,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = "Net # of Tickets:",
                           DisplayType = DisplayType.Integer,
                           MonthValuePreviousYear = invoiceCount.MonthValuePreviousYear - creditCount.MonthValuePreviousYear,
                           YearToDatePreviousYear = invoiceCount.YearToDatePreviousYear - creditCount.YearToDatePreviousYear,
                           MonthValueCurrentYear = invoiceCount.MonthValueCurrentYear - creditCount.MonthValueCurrentYear,
                           YearToDateCurrentYear = invoiceCount.YearToDateCurrentYear - creditCount.YearToDateCurrentYear,
                       };
        }

        private static FinalData GetCreditCount(int groupSort, string groupName, DateTime begMonth, DateTime endDate, DateTime begMonth2, DateTime endDate2, 
            List<RawData> tripDataPreviousYear, List<RawData> tripDataCurrentYear)
        {
            return new FinalData
                       {
                           RowNum = 3,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = "Credits:",
                           DisplayType = DisplayType.Integer,
                           MonthValuePreviousYear = tripDataPreviousYear.Count(s => s.UseDate >= begMonth && s.UseDate <= endDate && s.Plusmin == -1),
                           YearToDatePreviousYear = tripDataPreviousYear.Count(s => s.Plusmin == -1),
                           MonthValueCurrentYear = tripDataCurrentYear.Count(s => s.UseDate >= begMonth2 && s.UseDate <= endDate2 && s.Plusmin == -1),
                           YearToDateCurrentYear = tripDataCurrentYear.Count(s => s.Plusmin == -1),
                       };
        }

        private static FinalData GetInvoiceCount(int groupSort, string groupName, DateTime begMonth, DateTime endDate, DateTime begMonth2, DateTime endDate2, 
            List<RawData> tripDataPreviousYear, List<RawData> tripDataCurrentYear)
        {
            return new FinalData
                       {
                           RowNum = 2,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = "Transactions - Invoices:",
                           DisplayType = DisplayType.Integer,
                           MonthValuePreviousYear = tripDataPreviousYear.Count(s => s.UseDate >= begMonth && s.UseDate <= endDate && s.Plusmin == 1),
                           YearToDatePreviousYear = tripDataPreviousYear.Count(s => s.Plusmin == 1),
                           MonthValueCurrentYear = tripDataCurrentYear.Count(s => s.UseDate >= begMonth2 && s.UseDate <= endDate2 && s.Plusmin == 1),
                           YearToDateCurrentYear = tripDataCurrentYear.Count(s => s.Plusmin == 1),
                       };
        }

        private static FinalData GetTicketCount(int groupSort, string groupName, DateTime begMonth, DateTime endDate, DateTime begMonth2, DateTime endDate2, 
            List<RawData> tripDataPreviousYear, List<RawData> tripDataCurrentYear)
        {
            return new FinalData
                       {
                           RowNum = 1,
                           Grp = groupName,
                           GrpSort = groupSort,
                           SubGrp = 1,
                           Descrip = "# of Tickets:",
                           DisplayType = DisplayType.Integer,
                           MonthValuePreviousYear = tripDataPreviousYear.Count(s => s.UseDate >= begMonth && s.UseDate <= endDate),
                           YearToDatePreviousYear = tripDataPreviousYear.Count,
                           MonthValueCurrentYear = tripDataCurrentYear.Count(s => s.UseDate >= begMonth2 && s.UseDate <= endDate2),
                           YearToDateCurrentYear = tripDataCurrentYear.Count
                       };
        }
    }
}

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Domain.Models.ReportPrograms.QuickSummaryReport;

using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.QuickSummary
{
    public static class RowBuilder
    {
        public static List<FinalData> GetTripRows(List<RawData> tripDataList, Translations translations, bool separateRail, bool excludeExceptions, bool excludeSavings, bool excludeNegoSvngs, string currencySymbol, string symbolPosition)
        {
            var rows = new List<FinalData>();

            var tripData = tripDataList.Where(s => !separateRail || !s.ValcarMode.EqualsIgnoreCase("R")).ToList();
            if (!tripData.Any())
            {
                tripData.Add(new RawData());
            }

            var trips = tripData.Sum(s => s.Plusmin);
            var airChg = tripData.Sum(s => s.Airchg);
            var stndChg = tripData.Sum(s => s.Stndchg);
            var excepts = tripData.Sum(s => s.Offrdchg != 0 && s.Lostamt != 0 ? s.Plusmin : 0);
            var lostAmt = tripData.Sum(s => s.Lostamt);
            var negoSvngs = tripData.Sum(s => s.Negosvngs);

            var avgAir =  trips == 0?0: airChg/trips;
            var avgSavings = trips == 0 ? 0 : (stndChg - airChg)/trips;
            var svgsPcnt = stndChg == 0 ? 0 : (stndChg - airChg)/ stndChg;
            var avgLost = trips == 0 ? 0 : lostAmt/trips;
            var lostPcnt = airChg == 0 ? 0 : lostAmt/ airChg;
            

            rows.Add(new FinalData
            {
                Rownum = 1,
                Grp = translations.xAir,
                Grpsort = 1,
                Subgrp = 1,
                Descrip = translations.xNbrOfTrips,
                Tots = trips.ToString()
            });
            rows.Add(new FinalData
            {
                Rownum = 2,
                Grp = translations.xAir,
                Grpsort = 1,
                Subgrp = 2,
                Descrip = translations.xAirCharges,
                Tots = airChg.AddCurrency(currencySymbol,symbolPosition),
                Avgs = avgAir.AddCurrency(currencySymbol, symbolPosition),
                TotsDecimal = airChg
            });

            if (!excludeSavings)
            {
                rows.Add(new FinalData
                {
                    Rownum = 3,
                    Grp = translations.xAir,
                    Grpsort = 1,
                    Subgrp = 3,
                    Descrip = translations.xSavings,
                    Tots = (stndChg  - airChg).AddCurrency(currencySymbol,symbolPosition),
                    Avgs = avgSavings.AddCurrency(currencySymbol,symbolPosition),
                    Svgs = svgsPcnt.ToString("P")
                });
            }

            if (!excludeExceptions)
            {
                rows.Add(new FinalData
                {
                    Rownum = 4,
                    Grp = translations.xAir,
                    Grpsort = 1,
                    Subgrp = 4,
                    Descrip = translations.xNbrOfExcepts,
                    Tots = excepts.ToString(),
                    TotsDecimal = excepts
                });
                rows.Add(new FinalData
                {
                    Rownum = 5,
                    Grp = translations.xAir,
                    Grpsort = 1,
                    Subgrp = 5,
                    Descrip = translations.xLostSvgs,
                    Tots = lostAmt.AddCurrency(currencySymbol,symbolPosition),
                    Avgs = avgLost.AddCurrency(currencySymbol,symbolPosition),
                    Svgs = lostPcnt.ToString("P"),
                    TotsDecimal = lostAmt
                });
            }

            if (!excludeNegoSvngs)
            {
                rows.Add(new FinalData
                {
                    Rownum = 6,
                    Grp = translations.xAir,
                    Grpsort = 1,
                    Subgrp = 6,
                    Descrip = translations.xNegoSvgs,
                    Tots = negoSvngs.AddCurrency(currencySymbol,symbolPosition)
                });
            }


            if (separateRail)
            {
                tripData = tripDataList.Where(s => s.ValcarMode.EqualsIgnoreCase("R")).ToList();
                if (!tripData.Any())
                {
                    tripData.Add(new RawData());
                }

                trips = tripData.Sum(s => s.Plusmin);
                airChg = tripData.Sum(s => s.Airchg);
                stndChg = tripData.Sum(s => s.Stndchg);
                excepts = tripData.Sum(s => s.Offrdchg != 0 && s.Lostamt != 0 ? s.Plusmin : 0);
                lostAmt = tripData.Sum(s => s.Lostamt);
                negoSvngs = tripData.Sum(s => s.Negosvngs);

                avgAir = trips == 0 ? 0 : airChg / trips;
                avgSavings = trips == 0 ? 0 : (stndChg - airChg) / trips;
                svgsPcnt = stndChg == 0 ? 0 : (stndChg - airChg) / stndChg;
                avgLost = trips == 0 ? 0 : lostAmt / trips;
                lostPcnt = airChg == 0 ? 0 : lostAmt / airChg;

                rows.Add(new FinalData
                {
                    Rownum = 7,
                    Grp = translations.xRail,
                    Grpsort = 2,
                    Subgrp = 1,
                    Descrip = translations.xNbrOfTrips,
                    Tots = trips.ToString()
                });
                rows.Add(new FinalData
                {
                    Rownum = 8,
                    Grp = translations.xRail,
                    Grpsort = 2,
                    Subgrp = 2,
                    Descrip = translations.xAirCharges,
                    Tots = airChg.AddCurrency(currencySymbol,symbolPosition),
                    Avgs = avgAir.AddCurrency(currencySymbol,symbolPosition),
                    TotsDecimal = airChg
                });

                if (!excludeSavings)
                {
                    rows.Add(new FinalData
                    {
                        Rownum = 9,
                        Grp = translations.xRail,
                        Grpsort = 2,
                        Subgrp = 3,
                        Descrip = translations.xSavings,
                        Tots = (stndChg - airChg).AddCurrency(currencySymbol,symbolPosition),
                        Avgs = avgSavings.AddCurrency(currencySymbol,symbolPosition),
                        Svgs = svgsPcnt.ToString("P")
                    });
                }

                if (!excludeExceptions)
                {
                    rows.Add(new FinalData
                    {
                        Rownum = 10,
                        Grp = translations.xRail,
                        Grpsort = 2,
                        Subgrp = 4,
                        Descrip = translations.xNbrOfExcepts,
                        Tots = excepts.ToString(),
                        TotsDecimal = excepts
                    });
                    rows.Add(new FinalData
                    {
                        Rownum = 11,
                        Grp = translations.xRail,
                        Grpsort = 2,
                        Subgrp = 5,
                        Descrip = translations.xLostSvgs,
                        Tots = lostAmt.AddCurrency(currencySymbol,symbolPosition),
                        Avgs = avgLost.AddCurrency(currencySymbol,symbolPosition),
                        Svgs = lostPcnt.ToString("P"),
                        TotsDecimal = lostAmt
                    });
                }

                if (!excludeNegoSvngs)
                {
                    rows.Add(new FinalData
                    {
                        Rownum = 12,
                        Grp = translations.xRail,
                        Grpsort = 2,
                        Subgrp = 6,
                        Descrip = translations.xNegoSvgs,
                        Tots = negoSvngs.AddCurrency(currencySymbol,symbolPosition)
                    });
                }

            }


            return rows;
        }

        public static List<FinalData> GetCarRows(List<CarRawData> carDataList, Translations translations, bool excludeExceptions, string currencySymbol, string symbolPosition)
        {
            var rows = new List<FinalData>();

            var rentals = carDataList.Sum(s => s.Cplusmin);
            var days = carDataList.Sum(s => s.Cplusmin*s.Days);
            var carCost = carDataList.Sum(s => s.Days*s.Abookrat);
            var bookRate = carDataList.Sum(s => s.Abookrat);
            var excepts = carDataList.Sum(s => string.IsNullOrEmpty(s.Reascoda.Trim()) ? 0 : s.Cplusmin);
            var excptnAmtLost = carDataList.Sum(s => string.IsNullOrEmpty(s.Reascoda.Trim()) ? 0 : (s.Abookrat - s.Aexcprat)*s.Days);
            
            var bookRateCount = carDataList.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin);
            var bookRateNights = carDataList.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin * s.Days);

            var avgDays = rentals == 0 ? 0m : days/(decimal)rentals;
            var avgCost = bookRateCount == 0 ? 0m : bookRate/ bookRateCount;
            var dayCost = bookRateNights == 0 ? 0m : carCost/ bookRateNights;
            var excCntPcnt = rentals == 0 ? 0m : excepts/ (decimal)rentals;
            var excDolPcnt = carCost == 0 ? 0m : excptnAmtLost / carCost;


            rows.Add(new FinalData
            {
                Rownum = 15,
                Grp = translations.xCarRental,
                Grpsort = 3,
                Subgrp = 1,
                Descrip = translations.xNbrRentals,
                Tots = rentals.ToString()
            });
            rows.Add(new FinalData
            {
                Rownum = 16,
                Grp = translations.xCarRental,
                Grpsort = 3,
                Subgrp = 2,
                Descrip = translations.xNbrDays,
                Tots = days.ToString(),
                Avgs = avgDays.ToString("0.00")
            });
            rows.Add(new FinalData
            {
                Rownum = 17,
                Grp = translations.xCarRental,
                Grpsort = 3,
                Subgrp = 3,
                Descrip = translations.xCostBkRate,
                Tots = carCost.AddCurrency(currencySymbol,symbolPosition),
                Avgs = avgCost.AddCurrency(currencySymbol,symbolPosition),
                TotsDecimal = carCost
            });
            rows.Add(new FinalData
            {
                Rownum = 18,
                Grp = translations.xCarRental,
                Grpsort = 3,
                Subgrp = 4,
                Descrip = translations.xCostPerDay,
                Tots = "N/A",
                Avgs = dayCost.AddCurrency(currencySymbol,symbolPosition)
            });
            if (!excludeExceptions)
            {
                rows.Add(new FinalData
                {
                    Rownum = 19,
                    Grp = translations.xCarRental,
                    Grpsort = 3,
                    Subgrp = 5,
                    Descrip = translations.xNbrOfExcepts,
                    Tots = excepts.ToString(),
                    Svgs = excCntPcnt.ToString("P"),
                    TotsDecimal = excepts
                });
                rows.Add(new FinalData
                {
                    Rownum = 20,
                    Grp = translations.xCarRental,
                    Grpsort = 3,
                    Subgrp = 6,
                    Descrip = translations.xExcpnAmtLost,
                    Tots = excptnAmtLost.AddCurrency(currencySymbol,symbolPosition),
                    Svgs = excDolPcnt.ToString("P"),
                    TotsDecimal = excptnAmtLost
                });
            }

            return rows;
        }

        public static List<FinalData> GetHotelRows(List<HotelRawData> hotelDataList, Translations translations, bool excludeExceptions, string currencySymbol, string symbolPosition)
        {
            var rows = new List<FinalData>();

            var rentals = hotelDataList.Sum(s => s.Hplusmin);
            var nights = hotelDataList.Sum(s => s.Hplusmin * s.Nights * s.Rooms);
            var cost = hotelDataList.Sum(s => s.Nights * s.Bookrate * s.Rooms);
            var bookRate = hotelDataList.Sum(s => s.Bookrate);
            var excepts = hotelDataList.Sum(s => string.IsNullOrEmpty(s.Reascodh.Trim()) ? 0 : s.Hplusmin);
            var excptnAmtLost = hotelDataList.Sum(s => string.IsNullOrEmpty(s.Reascodh.Trim()) ? 0 : (s.Bookrate - s.Hexcprat) * s.Nights * s.Rooms);

            var bookRateCount = hotelDataList.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin);
            var bookRateNights = hotelDataList.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin * s.Nights * s.Rooms);

            var avgDays = rentals == 0 ? 0m :(decimal) nights / rentals;
            var avgCost = bookRateCount == 0 ? 0m : bookRate / bookRateCount;
            var dayCost = bookRateNights == 0 ? 0m : cost / bookRateNights;
            var excCntPcnt = rentals == 0 ? 0m : (decimal) excepts / rentals;
            var excDolPcnt = cost == 0 ? 0m : excptnAmtLost / cost;

            rows.Add(new FinalData
            {
                Rownum = 21,
                Grp = translations.xHotelBkgs,
                Grpsort = 4,
                Subgrp = 1,
                Descrip = translations.xNbrBkngs,
                Tots = rentals.ToString()
            });
            rows.Add(new FinalData
            {
                Rownum = 22,
                Grp = translations.xHotelBkgs,
                Grpsort = 4,
                Subgrp = 2,
                Descrip = translations.xNbrNights,
                Tots = nights.ToString(),
                Avgs = avgDays.ToString("0.00")
            });
            rows.Add(new FinalData
            {
                Rownum = 23,
                Grp = translations.xHotelBkgs,
                Grpsort = 4,
                Subgrp = 3,
                Descrip = translations.xCostBkRate,
                Tots = cost.AddCurrency(currencySymbol,symbolPosition),
                Avgs = avgCost.AddCurrency(currencySymbol,symbolPosition),
                TotsDecimal = cost
            });
            rows.Add(new FinalData
            {
                Rownum = 24,
                Grp = translations.xHotelBkgs,
                Grpsort = 4,
                Subgrp = 4,
                Descrip = translations.xCostPerNight,
                Tots = "N/A",
                Avgs = dayCost.AddCurrency(currencySymbol,symbolPosition)
            });
            if (!excludeExceptions)
            {
                rows.Add(new FinalData
                {
                    Rownum = 25,
                    Grp = translations.xHotelBkgs,
                    Grpsort = 4,
                    Subgrp = 5,
                    Descrip = translations.xNbrOfExcepts,
                    Tots = excepts.ToString(),
                    Svgs = excCntPcnt.ToString("P"),
                    TotsDecimal = excepts
                });
                rows.Add(new FinalData
                {
                    Rownum = 26,
                    Grp = translations.xHotelBkgs,
                    Grpsort = 4,
                    Subgrp = 6,
                    Descrip = translations.xExcpnAmtLost,
                    Tots = excptnAmtLost.AddCurrency(currencySymbol,symbolPosition),
                    Svgs = excDolPcnt.ToString("P"),
                    TotsDecimal = excptnAmtLost
                });
            }

            return rows;
        }

        public static List<FinalData> GetTotalRows(List<FinalData> finalDataList, Translations translations, bool excludeExceptions, string currencySymbol, string symbolPosition)
        {
            var rows = new List<FinalData>();

            //totals
            var total = finalDataList.Where(s => s.Rownum == 2 || s.Rownum == 8 || s.Rownum == 17 || s.Rownum == 23).Sum(s => s.TotsDecimal);
            rows.Add(new FinalData
            {
                Rownum = 27,
                Grp = translations.xRptTotals,
                Grpsort = 5,
                Subgrp = 1,
                Descrip = translations.xTotCharges,
                Tots = total.AddCurrency(currencySymbol,symbolPosition)
            });

            if (!excludeExceptions)
            {
                var totalExceptions = finalDataList.Where(s => s.Rownum == 4 || s.Rownum == 10 || s.Rownum == 19 || s.Rownum == 25).Sum(s => s.TotsDecimal);
                var totalLost = finalDataList.Where(s => s.Rownum == 5 || s.Rownum == 11 || s.Rownum == 20 || s.Rownum == 26).Sum(s => s.TotsDecimal);

                rows.Add(new FinalData
                {
                    Rownum = 28,
                    Grp = translations.xRptTotals,
                    Grpsort = 5,
                    Subgrp = 2,
                    Descrip = translations.xTotExcepts,
                    Tots = totalExceptions.ToString("#")
                });
                rows.Add(new FinalData
                {
                    Rownum = 29,
                    Grp = translations.xRptTotals,
                    Grpsort = 5,
                    Subgrp = 3,
                    Descrip = translations.xTotLost,
                    Tots = totalLost.AddCurrency(currencySymbol,symbolPosition)
                });
            }
           

            return rows;
        }

        public static List<FinalData> GetTripRowsDit(List<RawData> tripDataList, Translations translations, bool separateRail, bool excludeExceptions, bool excludeSavings, bool excludeNegoSvngs, string currencySymbol, string symbolPosition)
        {
            var rows = new List<FinalData>();

            var tripData = tripDataList.Where(s => !separateRail || !s.ValcarMode.EqualsIgnoreCase("R")).ToList();
            if (!tripData.Any())
            {
                tripData.Add(new RawData());
            }

            var tripDataDom = tripData.Where(s => s.Domintl.EqualsIgnoreCase("D")).ToList();
            var tripDataIntl = tripData.Where(s => !s.Domintl.EqualsIgnoreCase("D")).ToList();

            var trips1 = tripDataDom.Sum(s => s.Plusmin);
            var trips2 = tripDataIntl.Sum(s => s.Plusmin);
            var trips3 = tripData.Sum(s => s.Plusmin);

            var airChg1 = tripDataDom.Sum(s => s.Airchg);
            var airChg2 = tripDataIntl.Sum(s => s.Airchg);
            var airChg3 = tripData.Sum(s => s.Airchg);

            var stndChg1 = tripDataDom.Sum(s => s.Stndchg);
            var stndChg2 = tripDataIntl.Sum(s => s.Stndchg);
            var stndChg3 = tripData.Sum(s => s.Stndchg);


            var excepts1 = tripDataDom.Sum(s => s.Offrdchg != 0 && s.Lostamt != 0 ? s.Plusmin : 0);
            var excepts2 = tripDataIntl.Sum(s => s.Offrdchg != 0 && s.Lostamt != 0 ? s.Plusmin : 0);
            var excepts3 = tripData.Sum(s => s.Offrdchg != 0 && s.Lostamt != 0 ? s.Plusmin : 0);

            var lostAmt1 = tripDataDom.Sum(s => s.Lostamt);
            var lostAmt2 = tripDataIntl.Sum(s => s.Lostamt);
            var lostAmt3 = tripData.Sum(s => s.Lostamt);

            var negoSvngs1 = tripDataDom.Sum(s => s.Negosvngs);
            var negoSvngs2 = tripDataIntl.Sum(s => s.Negosvngs);
            var negoSvngs3 = tripData.Sum(s => s.Negosvngs);

            var avgAir1 = trips1 == 0 ? 0 : airChg1 / trips1;
            var avgAir2 = trips2 == 0 ? 0 : airChg2 / trips2;
            var avgAir3 = trips3 == 0 ? 0 : airChg3 / trips3;

            var avgSavings1 = trips1 == 0 ? 0 : (stndChg1 - airChg1) / trips1;
            var avgSavings2 = trips2 == 0 ? 0 : (stndChg2 - airChg2) / trips2;
            var avgSavings3 = trips3 == 0 ? 0 : (stndChg3 - airChg3) / trips3;

            var svgsPcnt1 = stndChg1 == 0 ? 0 : (stndChg1 - airChg1) / stndChg1;
            var svgsPcnt2 = stndChg2 == 0 ? 0 : (stndChg2 - airChg2) / stndChg2;
            var svgsPcnt3 = stndChg3 == 0 ? 0 : (stndChg3 - airChg3) / stndChg3;


            var avgLost1 = trips1 == 0 ? 0 : lostAmt1 / trips1;
            var avgLost2 = trips2 == 0 ? 0 : lostAmt2 / trips2;
            var avgLost3 = trips3 == 0 ? 0 : lostAmt3 / trips3;

            var lostPcnt1 = airChg1 == 0 ? 0 : lostAmt1 / airChg1;
            var lostPcnt2 = airChg2 == 0 ? 0 : lostAmt2 / airChg2;
            var lostPcnt3 = airChg3 == 0 ? 0 : lostAmt3 / airChg3;


            rows.Add(new FinalData
            {
                Rownum = 1,
                Grp = translations.xAir,
                Grpsort = 1,
                Subgrp = 1,
                Descrip = translations.xNbrOfTrips,
                Tots1 = trips1.ToString(),
                Tots2 = trips2.ToString(),
                Tots3 = trips3.ToString()
            });
            rows.Add(new FinalData
            {
                Rownum = 2,
                Grp = translations.xAir,
                Grpsort = 1,
                Subgrp = 2,
                Descrip = translations.xAirCharges,
                Tots1 = airChg1.AddCurrency(currencySymbol,symbolPosition),
                Avgs1 = avgAir1.AddCurrency(currencySymbol,symbolPosition),
                Tots1Decimal = airChg1,
                Tots2 = airChg2.AddCurrency(currencySymbol,symbolPosition),
                Avgs2 = avgAir2.AddCurrency(currencySymbol,symbolPosition),
                Tots2Decimal = airChg2,
                Tots3 = airChg3.AddCurrency(currencySymbol,symbolPosition),
                Avgs3 = avgAir3.AddCurrency(currencySymbol,symbolPosition),
                Tots3Decimal = airChg3
            });

            if (!excludeSavings)
            {
                rows.Add(new FinalData
                {
                    Rownum = 3,
                    Grp = translations.xAir,
                    Grpsort = 1,
                    Subgrp = 3,
                    Descrip = translations.xSavings,
                    Tots1 = (stndChg1 - airChg1).AddCurrency(currencySymbol,symbolPosition),
                    Avgs1 = avgSavings1.AddCurrency(currencySymbol,symbolPosition),
                    Svgs1 = svgsPcnt1.ToString("P"),
                    Tots2 = (stndChg2 - airChg2).AddCurrency(currencySymbol,symbolPosition),
                    Avgs2 = avgSavings2.AddCurrency(currencySymbol,symbolPosition),
                    Svgs2 = svgsPcnt2.ToString("P"),
                    Tots3 = (stndChg3 - airChg3).AddCurrency(currencySymbol,symbolPosition),
                    Avgs3 = avgSavings3.AddCurrency(currencySymbol,symbolPosition),
                    Svgs3 = svgsPcnt3.ToString("P")
                });
            }

            if (!excludeExceptions)
            {
                rows.Add(new FinalData
                {
                    Rownum = 4,
                    Grp = translations.xAir,
                    Grpsort = 1,
                    Subgrp = 4,
                    Descrip = translations.xNbrOfExcepts,
                    Tots1 = excepts1.ToString(),
                    Tots1Decimal = excepts1,
                    Tots2 = excepts2.ToString(),
                    Tots2Decimal = excepts2,
                    Tots3 = excepts3.ToString(),
                    Tots3Decimal = excepts3
                });
                rows.Add(new FinalData
                {
                    Rownum = 5,
                    Grp = translations.xAir,
                    Grpsort = 1,
                    Subgrp = 5,
                    Descrip = translations.xLostSvgs,
                    Tots1 = lostAmt1.AddCurrency(currencySymbol,symbolPosition),
                    Avgs1 = avgLost1.AddCurrency(currencySymbol,symbolPosition),
                    Svgs1 = lostPcnt1.ToString("P"),
                    Tots1Decimal = lostAmt1,
                    Tots2 = lostAmt2.AddCurrency(currencySymbol,symbolPosition),
                    Avgs2 = avgLost2.AddCurrency(currencySymbol,symbolPosition),
                    Svgs2 = lostPcnt2.ToString("P"),
                    Tots2Decimal = lostAmt2,
                    Tots3 = lostAmt3.AddCurrency(currencySymbol,symbolPosition),
                    Avgs3 = avgLost3.AddCurrency(currencySymbol,symbolPosition),
                    Svgs3 = lostPcnt3.ToString("P"),
                    Tots3Decimal = lostAmt3
                });
            }

            if (!excludeNegoSvngs)
            {
                rows.Add(new FinalData
                {
                    Rownum = 6,
                    Grp = translations.xAir,
                    Grpsort = 1,
                    Subgrp = 6,
                    Descrip = translations.xNegoSvgs,
                    Tots1 = negoSvngs1.AddCurrency(currencySymbol,symbolPosition),
                    Tots2 = negoSvngs2.AddCurrency(currencySymbol,symbolPosition),
                    Tots3 = negoSvngs3.AddCurrency(currencySymbol,symbolPosition)
                });
            }


            if (separateRail)
            {
                tripData = tripDataList.Where(s => s.ValcarMode.EqualsIgnoreCase("R")).ToList();
                if (!tripData.Any())
                {
                    tripData.Add(new RawData());
                }

                tripDataDom = tripData.Where(s => s.Domintl.EqualsIgnoreCase("D")).ToList();
                tripDataIntl = tripData.Where(s => !s.Domintl.EqualsIgnoreCase("D")).ToList();

                trips1 = tripDataDom.Sum(s => s.Plusmin);
                trips2 = tripDataIntl.Sum(s => s.Plusmin);
                trips3 = tripData.Sum(s => s.Plusmin);

                airChg1 = tripDataDom.Sum(s => s.Airchg);
                airChg2 = tripDataIntl.Sum(s => s.Airchg);
                airChg3 = tripData.Sum(s => s.Airchg);

                stndChg1 = tripDataDom.Sum(s => s.Stndchg);
                stndChg2 = tripDataIntl.Sum(s => s.Stndchg);
                stndChg3 = tripData.Sum(s => s.Stndchg);


                excepts1 = tripDataDom.Sum(s => s.Offrdchg != 0 && s.Lostamt != 0 ? s.Plusmin : 0);
                excepts2 = tripDataIntl.Sum(s => s.Offrdchg != 0 && s.Lostamt != 0 ? s.Plusmin : 0);
                excepts3 = tripData.Sum(s => s.Offrdchg != 0 && s.Lostamt != 0 ? s.Plusmin : 0);

                lostAmt1 = tripDataDom.Sum(s => s.Lostamt);
                lostAmt2 = tripDataIntl.Sum(s => s.Lostamt);
                lostAmt3 = tripData.Sum(s => s.Lostamt);

                negoSvngs1 = tripDataDom.Sum(s => s.Negosvngs);
                negoSvngs2 = tripDataIntl.Sum(s => s.Negosvngs);
                negoSvngs3 = tripData.Sum(s => s.Negosvngs);

                avgAir1 = trips1 == 0 ? 0 : airChg1 / trips1;
                avgAir2 = trips2 == 0 ? 0 : airChg2 / trips2;
                avgAir3 = trips3 == 0 ? 0 : airChg3 / trips3;

                avgSavings1 = trips1 == 0 ? 0 : (stndChg1 - airChg1) / trips1;
                avgSavings2 = trips2 == 0 ? 0 : (stndChg2 - airChg2) / trips2;
                avgSavings3 = trips3 == 0 ? 0 : (stndChg3 - airChg3) / trips3;

                svgsPcnt1 = stndChg1 == 0 ? 0 : (stndChg1 - airChg1) / stndChg1;
                svgsPcnt2 = stndChg2 == 0 ? 0 : (stndChg2 - airChg2) / stndChg2;
                svgsPcnt3 = stndChg3 == 0 ? 0 : (stndChg3 - airChg3) / stndChg3;

                avgLost1 = trips1 == 0 ? 0 : lostAmt1 / trips1;
                avgLost2 = trips2 == 0 ? 0 : lostAmt2 / trips2;
                avgLost3 = trips3 == 0 ? 0 : lostAmt3 / trips3;

                lostPcnt1 = airChg1 == 0 ? 0 : lostAmt1 / airChg1;
                lostPcnt2 = airChg2 == 0 ? 0 : lostAmt2 / airChg2;
                lostPcnt3 = airChg3 == 0 ? 0 : lostAmt3 / airChg3;


                rows.Add(new FinalData
                {
                    Rownum = 7,
                    Grp = translations.xRail,
                    Grpsort = 2,
                    Subgrp = 1,
                    Descrip = translations.xNbrOfTrips,
                    Tots1 = trips1.ToString(),
                    Tots2 = trips2.ToString(),
                    Tots3 = trips3.ToString()
                });
                rows.Add(new FinalData
                {
                    Rownum = 8,
                    Grp = translations.xRail,
                    Grpsort = 2,
                    Subgrp = 2,
                    Descrip = translations.xAirCharges,
                    Tots1 = airChg1.AddCurrency(currencySymbol,symbolPosition),
                    Avgs1 = avgAir1.AddCurrency(currencySymbol,symbolPosition),
                    Tots1Decimal = airChg1,
                    Tots2 = airChg2.AddCurrency(currencySymbol,symbolPosition),
                    Avgs2 = avgAir2.AddCurrency(currencySymbol,symbolPosition),
                    Tots2Decimal = airChg2,
                    Tots3 = airChg3.AddCurrency(currencySymbol,symbolPosition),
                    Avgs3 = avgAir3.AddCurrency(currencySymbol,symbolPosition),
                    Tots3Decimal = airChg3
                });

                if (!excludeSavings)
                {
                    rows.Add(new FinalData
                    {
                        Rownum = 9,
                        Grp = translations.xRail,
                        Grpsort = 2,
                        Subgrp = 3,
                        Descrip = translations.xSavings,
                        Tots1 = (stndChg1 - airChg1).AddCurrency(currencySymbol,symbolPosition),
                        Avgs1 = avgSavings1.AddCurrency(currencySymbol,symbolPosition),
                        Svgs1 = svgsPcnt1.ToString("P"),
                        Tots2 = (stndChg2 - airChg2).AddCurrency(currencySymbol,symbolPosition),
                        Avgs2 = avgSavings2.AddCurrency(currencySymbol,symbolPosition),
                        Svgs2 = svgsPcnt2.ToString("P"),
                        Tots3 = (stndChg3 - airChg3).AddCurrency(currencySymbol,symbolPosition),
                        Avgs3 = avgSavings3.AddCurrency(currencySymbol,symbolPosition),
                        Svgs3 = svgsPcnt3.ToString("P")
                    });
                }

                if (!excludeExceptions)
                {
                    rows.Add(new FinalData
                    {
                        Rownum = 10,
                        Grp = translations.xRail,
                        Grpsort = 2,
                        Subgrp = 4,
                        Descrip = translations.xNbrOfExcepts,
                        Tots1 = excepts1.ToString(),
                        Tots1Decimal = excepts1,
                        Tots2 = excepts2.ToString(),
                        Tots2Decimal = excepts2,
                        Tots3 = excepts3.ToString(),
                        Tots3Decimal = excepts3
                    });
                    rows.Add(new FinalData
                    {
                        Rownum = 11,
                        Grp = translations.xRail,
                        Grpsort = 2,
                        Subgrp = 5,
                        Descrip = translations.xLostSvgs,
                        Tots1 = lostAmt1.AddCurrency(currencySymbol,symbolPosition),
                        Avgs1 = avgLost1.AddCurrency(currencySymbol,symbolPosition),
                        Svgs1 = lostPcnt1.ToString("P"),
                        Tots1Decimal = lostAmt1,
                        Tots2 = lostAmt2.AddCurrency(currencySymbol,symbolPosition),
                        Avgs2 = avgLost2.AddCurrency(currencySymbol,symbolPosition),
                        Svgs2 = lostPcnt2.ToString("P"),
                        Tots2Decimal = lostAmt2,
                        Tots3 = lostAmt3.AddCurrency(currencySymbol,symbolPosition),
                        Avgs3 = avgLost3.AddCurrency(currencySymbol,symbolPosition),
                        Svgs3 = lostPcnt3.ToString("P"),
                        Tots3Decimal = lostAmt3
                    });
                }

                if (!excludeNegoSvngs)
                {
                    rows.Add(new FinalData
                    {
                        Rownum = 12,
                        Grp = translations.xRail,
                        Grpsort = 2,
                        Subgrp = 6,
                        Descrip = translations.xNegoSvgs,
                        Tots1 = negoSvngs1.AddCurrency(currencySymbol,symbolPosition),
                        Tots2 = negoSvngs2.AddCurrency(currencySymbol,symbolPosition),
                        Tots3 = negoSvngs3.AddCurrency(currencySymbol,symbolPosition)
                    });
                }

            }


            return rows;
        }

        public static List<FinalData> GetCarRowsDit(List<CarRawData> carDataList, Translations translations, bool excludeExceptions, string currencySymbol, string symbolPosition)
        {
            var rows = new List<FinalData>();
            var carDataListDom = carDataList.Where(s => s.Domintl.EqualsIgnoreCase("D")).ToList();
            var carDataListIntl = carDataList.Where(s => !s.Domintl.EqualsIgnoreCase("D")).ToList();

            var rentals1 = carDataListDom.Sum(s => s.Cplusmin);
            var rentals2 = carDataListIntl.Sum(s => s.Cplusmin);
            var rentals3 = carDataList.Sum(s => s.Cplusmin);

            var days1 = carDataListDom.Sum(s => s.Cplusmin * s.Days);
            var days2 = carDataListIntl.Sum(s => s.Cplusmin * s.Days);
            var days3 = carDataList.Sum(s => s.Cplusmin * s.Days);


            var carCost1 = carDataListDom.Sum(s => s.Days * s.Abookrat);
            var carCost2 = carDataListIntl.Sum(s => s.Days * s.Abookrat);
            var carCost3 = carDataList.Sum(s => s.Days * s.Abookrat);

            var bookRate1 = carDataListDom.Sum(s => s.Abookrat);
            var bookRate2 = carDataListIntl.Sum(s => s.Abookrat);
            var bookRate3 = carDataList.Sum(s => s.Abookrat);

            var excepts1 = carDataListDom.Sum(s => string.IsNullOrEmpty(s.Reascoda.Trim()) ? 0 : s.Cplusmin);
            var excepts2 = carDataListIntl.Sum(s => string.IsNullOrEmpty(s.Reascoda.Trim()) ? 0 : s.Cplusmin);
            var excepts3 = carDataList.Sum(s => string.IsNullOrEmpty(s.Reascoda.Trim()) ? 0 : s.Cplusmin);


            var excptnAmtLost1 = carDataListDom.Sum(s => string.IsNullOrEmpty(s.Reascoda.Trim()) ? 0 : (s.Abookrat - s.Aexcprat) * s.Days);
            var excptnAmtLost2 = carDataListIntl.Sum(s => string.IsNullOrEmpty(s.Reascoda.Trim()) ? 0 : (s.Abookrat - s.Aexcprat) * s.Days);
            var excptnAmtLost3 = carDataList.Sum(s => string.IsNullOrEmpty(s.Reascoda.Trim()) ? 0 : (s.Abookrat - s.Aexcprat) * s.Days);

            var bookRateCount1 = carDataListDom.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin);
            var bookRateCount2= carDataListIntl.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin);
            var bookRateCount3 = carDataList.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin);


            var bookRateNights1 = carDataListDom.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin * s.Days);
            var bookRateNights2 = carDataListIntl.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin * s.Days);
            var bookRateNights3 = carDataList.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin * s.Days);
            
            var avgDays1 = rentals1 == 0 ? 0m : days1 / (decimal)rentals1;
            var avgDays2 = rentals2 == 0 ? 0m : days2 / (decimal)rentals2;
            var avgDays3 = rentals3 == 0 ? 0m : days3 / (decimal)rentals3;


            var avgCost1 = bookRateCount1 == 0 ? 0m : bookRate1 / bookRateCount1;
            var avgCost2 = bookRateCount2 == 0 ? 0m : bookRate2 / bookRateCount2;
            var avgCost3 = bookRateCount3 == 0 ? 0m : bookRate3 / bookRateCount3;

            var dayCost1 = bookRateNights1 == 0 ? 0m : carCost1 / bookRateNights1;
            var dayCost2 = bookRateNights2 == 0 ? 0m : carCost2 / bookRateNights2;
            var dayCost3 = bookRateNights3 == 0 ? 0m : carCost3 / bookRateNights3;

            var excCntPcnt1 = rentals1 == 0 ? 0m : excepts1 / (decimal)rentals1;
            var excCntPcnt2 = rentals2 == 0 ? 0m : excepts2 / (decimal)rentals2;
            var excCntPcnt3 = rentals3 == 0 ? 0m : excepts3 / (decimal)rentals3;


            var excDolPcnt1 = carCost1 == 0 ? 0m : excptnAmtLost1 / carCost1;
            var excDolPcnt2 = carCost2 == 0 ? 0m : excptnAmtLost2 / carCost2;
            var excDolPcnt3 = carCost3 == 0 ? 0m : excptnAmtLost3 / carCost3;


            rows.Add(new FinalData
            {
                Rownum = 15,
                Grp = translations.xCarRental,
                Grpsort = 3,
                Subgrp = 1,
                Descrip = translations.xNbrRentals,
                Tots1 = rentals1.ToString(),
                Tots2 = rentals2.ToString(),
                Tots3 = rentals3.ToString()
            });
            rows.Add(new FinalData
            {
                Rownum = 16,
                Grp = translations.xCarRental,
                Grpsort = 3,
                Subgrp = 2,
                Descrip = translations.xNbrDays,
                Tots1 = days1.ToString(),
                Avgs1 = avgDays1.ToString("0.00"),
                Tots2 = days2.ToString(),
                Avgs2 = avgDays2.ToString("0.00"),
                Tots3 = days3.ToString(),
                Avgs3 = avgDays3.ToString("0.00")
            });
            rows.Add(new FinalData
            {
                Rownum = 17,
                Grp = translations.xCarRental,
                Grpsort = 3,
                Subgrp = 3,
                Descrip = translations.xCostBkRate,
                Tots1 = carCost1.AddCurrency(currencySymbol,symbolPosition),
                Avgs1 = avgCost1.AddCurrency(currencySymbol,symbolPosition),
                Tots1Decimal = carCost1,
                Tots2 = carCost2.AddCurrency(currencySymbol,symbolPosition),
                Avgs2 = avgCost2.AddCurrency(currencySymbol,symbolPosition),
                Tots2Decimal = carCost2,
                Tots3 = carCost3.AddCurrency(currencySymbol,symbolPosition),
                Avgs3 = avgCost3.AddCurrency(currencySymbol,symbolPosition),
                Tots3Decimal = carCost3
            });
            rows.Add(new FinalData
            {
                Rownum = 18,
                Grp = translations.xCarRental,
                Grpsort = 3,
                Subgrp = 4,
                Descrip = translations.xCostPerDay,
                Tots1 = "N/A",
                Avgs1 = dayCost1.AddCurrency(currencySymbol,symbolPosition),
                Tots2 = "N/A",
                Avgs2= dayCost2.AddCurrency(currencySymbol,symbolPosition),
                Tots3 = "N/A",
                Avgs3 = dayCost3.AddCurrency(currencySymbol,symbolPosition)
            });
            if (!excludeExceptions)
            {
                rows.Add(new FinalData
                {
                    Rownum = 19,
                    Grp = translations.xCarRental,
                    Grpsort = 3,
                    Subgrp = 5,
                    Descrip = translations.xNbrOfExcepts,
                    Tots1 = excepts1.ToString(),
                    Svgs1 = excCntPcnt1.ToString("P"),
                    Tots1Decimal = excepts1,
                    Tots2 = excepts2.ToString(),
                    Svgs2 = excCntPcnt2.ToString("P"),
                    Tots2Decimal = excepts2,
                    Tots3 = excepts3.ToString(),
                    Svgs3 = excCntPcnt3.ToString("P"),
                    Tots3Decimal = excepts3
                });
                rows.Add(new FinalData
                {
                    Rownum = 20,
                    Grp = translations.xCarRental,
                    Grpsort = 3,
                    Subgrp = 6,
                    Descrip = translations.xExcpnAmtLost,
                    Tots1 = excptnAmtLost1.AddCurrency(currencySymbol,symbolPosition),
                    Svgs1 = excDolPcnt1.ToString("P"),
                    Tots1Decimal = excptnAmtLost1,
                    Tots2 = excptnAmtLost2.AddCurrency(currencySymbol,symbolPosition),
                    Svgs2 = excDolPcnt2.ToString("P"),
                    Tots2Decimal = excptnAmtLost2,
                    Tots3= excptnAmtLost3.AddCurrency(currencySymbol,symbolPosition),
                    Svgs3 = excDolPcnt3.ToString("P"),
                    Tots3Decimal = excptnAmtLost3
                });
            }

            return rows;
        }

        public static List<FinalData> GetHotelRowsDit(List<HotelRawData> hotelDataList, Translations translations, bool excludeExceptions, string currencySymbol, string symbolPosition)
        {
            var rows = new List<FinalData>();

            var hotelDataListDom = hotelDataList.Where(s => s.Domintl.EqualsIgnoreCase("D")).ToList();
            var hotelDataListIntl = hotelDataList.Where(s => !s.Domintl.EqualsIgnoreCase("D")).ToList();

            var rentals1 = hotelDataListDom.Sum(s => s.Hplusmin);
            var rentals2 = hotelDataListIntl.Sum(s => s.Hplusmin);
            var rentals3 = hotelDataList.Sum(s => s.Hplusmin);


            var nights1 = hotelDataListDom.Sum(s => s.Hplusmin * s.Nights * s.Rooms);
            var nights2 = hotelDataListIntl.Sum(s => s.Hplusmin * s.Nights * s.Rooms);
            var nights3 = hotelDataList.Sum(s => s.Hplusmin * s.Nights * s.Rooms);

            var cost1 = hotelDataListDom.Sum(s => s.Nights * s.Bookrate * s.Rooms);
            var cost2 = hotelDataListIntl.Sum(s => s.Nights * s.Bookrate * s.Rooms);
            var cost3 = hotelDataList.Sum(s => s.Nights * s.Bookrate * s.Rooms);

            var bookRate1 = hotelDataListDom.Sum(s => s.Bookrate);
            var bookRate2 = hotelDataListIntl.Sum(s => s.Bookrate);
            var bookRate3 = hotelDataList.Sum(s => s.Bookrate);

            var excepts1 = hotelDataListDom.Sum(s => string.IsNullOrEmpty(s.Reascodh.Trim()) ? 0 : s.Hplusmin);
            var excepts2 = hotelDataListIntl.Sum(s => string.IsNullOrEmpty(s.Reascodh.Trim()) ? 0 : s.Hplusmin);
            var excepts3 = hotelDataList.Sum(s => string.IsNullOrEmpty(s.Reascodh.Trim()) ? 0 : s.Hplusmin);

            var excptnAmtLost1 = hotelDataListDom.Sum(s => string.IsNullOrEmpty(s.Reascodh.Trim()) ? 0 : (s.Bookrate - s.Hexcprat) * s.Nights * s.Rooms);
            var excptnAmtLost2 = hotelDataListIntl.Sum(s => string.IsNullOrEmpty(s.Reascodh.Trim()) ? 0 : (s.Bookrate - s.Hexcprat) * s.Nights * s.Rooms);
            var excptnAmtLost3 = hotelDataList.Sum(s => string.IsNullOrEmpty(s.Reascodh.Trim()) ? 0 : (s.Bookrate - s.Hexcprat) * s.Nights * s.Rooms);

            var bookRateCount1 = hotelDataListDom.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin);
            var bookRateCount2 = hotelDataListIntl.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin);
            var bookRateCount3 = hotelDataList.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin);

            var bookRateNights1 = hotelDataListDom.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin * s.Nights * s.Rooms);
            var bookRateNights2 = hotelDataListIntl.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin * s.Nights * s.Rooms);
            var bookRateNights3 = hotelDataList.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin * s.Nights * s.Rooms);


            var avgDays1 = rentals1 == 0 ? 0m : (decimal)nights1 / rentals1;
            var avgDays2 = rentals2 == 0 ? 0m : (decimal)nights2 / rentals2;
            var avgDays3 = rentals3 == 0 ? 0m : (decimal)nights3 / rentals3;

            var avgCost1 = bookRateCount1 == 0 ? 0m : bookRate1 / bookRateCount1;
            var avgCost2 = bookRateCount2 == 0 ? 0m : bookRate2 / bookRateCount2;
            var avgCost3 = bookRateCount3 == 0 ? 0m : bookRate3 / bookRateCount3;

            var dayCost1 = bookRateNights1 == 0 ? 0m : cost1 / bookRateNights1;
            var dayCost2 = bookRateNights2 == 0 ? 0m : cost2 / bookRateNights2;
            var dayCost3 = bookRateNights3 == 0 ? 0m : cost3 / bookRateNights3;

            var excCntPcnt1 = rentals1 == 0 ? 0m : (decimal)excepts1 / rentals1;
            var excCntPcnt2 = rentals2 == 0 ? 0m : (decimal)excepts2 / rentals2;
            var excCntPcnt3 = rentals3 == 0 ? 0m : (decimal)excepts3 / rentals3;

            var excDolPcnt1 = cost1 == 0 ? 0m : excptnAmtLost1 / cost1;
            var excDolPcnt2 = cost2 == 0 ? 0m : excptnAmtLost2 / cost2;
            var excDolPcnt3 = cost3 == 0 ? 0m : excptnAmtLost3 / cost3;

            rows.Add(new FinalData
            {
                Rownum = 21,
                Grp = translations.xHotelBkgs,
                Grpsort = 4,
                Subgrp = 1,
                Descrip = translations.xNbrBkngs,
                Tots1 = rentals1.ToString(),
                Tots2 = rentals2.ToString(),
                Tots3 = rentals3.ToString()
            });
            rows.Add(new FinalData
            {
                Rownum = 22,
                Grp = translations.xHotelBkgs,
                Grpsort = 4,
                Subgrp = 2,
                Descrip = translations.xNbrNights,
                Tots1 = nights1.ToString(),
                Avgs1 = avgDays1.ToString("0.00"),
                Tots2 = nights2.ToString(),
                Avgs2 = avgDays2.ToString("0.00"),
                Tots3 = nights3.ToString(),
                Avgs3 = avgDays3.ToString("0.00")
            });
            rows.Add(new FinalData
            {
                Rownum = 23,
                Grp = translations.xHotelBkgs,
                Grpsort = 4,
                Subgrp = 3,
                Descrip = translations.xCostBkRate,
                Tots1 = cost1.AddCurrency(currencySymbol,symbolPosition),
                Avgs1 = avgCost1.AddCurrency(currencySymbol,symbolPosition),
                Tots1Decimal = cost1,
                Tots2 = cost2.AddCurrency(currencySymbol,symbolPosition),
                Avgs2 = avgCost2.AddCurrency(currencySymbol,symbolPosition),
                Tots2Decimal = cost2,
                Tots3 = cost3.AddCurrency(currencySymbol,symbolPosition),
                Avgs3 = avgCost3.AddCurrency(currencySymbol,symbolPosition),
                Tots3Decimal = cost3
            });
            rows.Add(new FinalData
            {
                Rownum = 24,
                Grp = translations.xHotelBkgs,
                Grpsort = 4,
                Subgrp = 4,
                Descrip = translations.xCostPerNight,
                Tots1 = "N/A",
                Avgs1 = dayCost1.AddCurrency(currencySymbol,symbolPosition),
                Tots2 = "N/A",
                Avgs2 = dayCost2.AddCurrency(currencySymbol,symbolPosition),
                Tots3 = "N/A",
                Avgs3 = dayCost3.AddCurrency(currencySymbol,symbolPosition)
            });
            if (!excludeExceptions)
            {
                rows.Add(new FinalData
                {
                    Rownum = 25,
                    Grp = translations.xHotelBkgs,
                    Grpsort = 4,
                    Subgrp = 5,
                    Descrip = translations.xNbrOfExcepts,
                    Tots1 = excepts1.ToString(),
                    Svgs1 = excCntPcnt1.ToString("P"),
                    Tots1Decimal = excepts1,
                    Tots2 = excepts2.ToString(),
                    Svgs2 = excCntPcnt2.ToString("P"),
                    Tots2Decimal = excepts2,
                    Tots3 = excepts3.ToString(),
                    Svgs3 = excCntPcnt3.ToString("P"),
                    Tots3Decimal = excepts3
                });
                rows.Add(new FinalData
                {
                    Rownum = 26,
                    Grp = translations.xHotelBkgs,
                    Grpsort = 4,
                    Subgrp = 6,
                    Descrip = translations.xExcpnAmtLost,
                    Tots1 = excptnAmtLost1.AddCurrency(currencySymbol,symbolPosition),
                    Svgs1 = excDolPcnt1.ToString("P"),
                    Tots1Decimal = excptnAmtLost1,
                    Tots2 = excptnAmtLost2.AddCurrency(currencySymbol,symbolPosition),
                    Svgs2 = excDolPcnt2.ToString("P"),
                    Tots2Decimal = excptnAmtLost2,
                    Tots3 = excptnAmtLost3.AddCurrency(currencySymbol,symbolPosition),
                    Svgs3 = excDolPcnt3.ToString("P"),
                    Tots3Decimal = excptnAmtLost3
                });
            }

            return rows;
        }

        public static List<FinalData> GetTotalRowsDit(List<FinalData> finalDataList, Translations translations, bool excludeExceptions, string currencySymbol, string symbolPosition)
        {
            var rows = new List<FinalData>();

            //totals
            var total1 = finalDataList.Where(s => s.Rownum == 2 || s.Rownum == 8 || s.Rownum == 17 || s.Rownum == 23).Sum(s => s.Tots1Decimal);
            var total2 = finalDataList.Where(s => s.Rownum == 2 || s.Rownum == 8 || s.Rownum == 17 || s.Rownum == 23).Sum(s => s.Tots2Decimal);
            var total3 = finalDataList.Where(s => s.Rownum == 2 || s.Rownum == 8 || s.Rownum == 17 || s.Rownum == 23).Sum(s => s.Tots3Decimal);
            rows.Add(new FinalData
            {
                Rownum = 27,
                Grp = translations.xRptTotals,
                Grpsort = 5,
                Subgrp = 1,
                Descrip = translations.xTotCharges,
                Tots1 = total1.AddCurrency(currencySymbol,symbolPosition),
                Tots2 = total2.AddCurrency(currencySymbol,symbolPosition),
                Tots3 = total3.AddCurrency(currencySymbol,symbolPosition)
            });

            if (!excludeExceptions)
            {
                var totalExceptions1 = finalDataList.Where(s => s.Rownum == 4 || s.Rownum == 10 || s.Rownum == 19 || s.Rownum == 25).Sum(s => s.Tots1Decimal);
                var totalExceptions2 = finalDataList.Where(s => s.Rownum == 4 || s.Rownum == 10 || s.Rownum == 19 || s.Rownum == 25).Sum(s => s.Tots2Decimal);
                var totalExceptions3 = finalDataList.Where(s => s.Rownum == 4 || s.Rownum == 10 || s.Rownum == 19 || s.Rownum == 25).Sum(s => s.Tots3Decimal);

                var totalLost1 = finalDataList.Where(s => s.Rownum == 5 || s.Rownum == 11 || s.Rownum == 20 || s.Rownum == 26).Sum(s => s.Tots1Decimal);
                var totalLost2 = finalDataList.Where(s => s.Rownum == 5 || s.Rownum == 11 || s.Rownum == 20 || s.Rownum == 26).Sum(s => s.Tots2Decimal);
                var totalLost3 = finalDataList.Where(s => s.Rownum == 5 || s.Rownum == 11 || s.Rownum == 20 || s.Rownum == 26).Sum(s => s.Tots3Decimal);

                rows.Add(new FinalData
                {
                    Rownum = 28,
                    Grp = translations.xRptTotals,
                    Grpsort = 5,
                    Subgrp = 2,
                    Descrip = translations.xTotExcepts,
                    Tots1 = totalExceptions1.ToString("#"),
                    Tots2 = totalExceptions2.ToString("#"),
                    Tots3 = totalExceptions3.ToString("#")
                });
                rows.Add(new FinalData
                {
                    Rownum = 29,
                    Grp = translations.xRptTotals,
                    Grpsort = 5,
                    Subgrp = 3,
                    Descrip = translations.xTotLost,
                    Tots1 = totalLost1.AddCurrency(currencySymbol,symbolPosition),
                    Tots2 = totalLost2.AddCurrency(currencySymbol,symbolPosition),
                    Tots3 = totalLost3.AddCurrency(currencySymbol,symbolPosition)
                });
            }


            return rows;
        }

        public static List<FinalData> GetTripRowsComp(List<RawData> tripDataList, List<RawData> tripDataListComp, Translations translations, bool separateRail, bool excludeExceptions, bool excludeSavings, bool excludeNegoSvngs, string currencySymbol, string symbolPosition)
        {
            var rows = new List<FinalData>();

            var tripData = tripDataList.Where(s => !separateRail || !s.ValcarMode.EqualsIgnoreCase("R")).ToList();
            if (!tripData.Any())
            {
                tripData.Add(new RawData());
            }

            var tripDataComp = tripDataListComp.Where(s => !separateRail || !s.ValcarMode.EqualsIgnoreCase("R")).ToList();

            var trips = tripData.Sum(s => s.Plusmin);
            var tripsComp = tripDataComp.Sum(s => s.Plusmin);
            var tripsDelta = tripsComp - trips;

            var airChg = tripData.Sum(s => s.Airchg);
            var airChgComp = tripDataComp.Sum(s => s.Airchg);
            var airChgDelta = airChgComp - airChg;


            var stndChg = tripData.Sum(s => s.Stndchg);
            var stndChgComp = tripDataComp.Sum(s => s.Stndchg);
            var stndChgDelta = stndChgComp - stndChg;

            var excepts = tripData.Sum(s => s.Offrdchg != 0 && s.Lostamt != 0 ? s.Plusmin : 0);
            var exceptsComp = tripDataComp.Sum(s => s.Offrdchg != 0 && s.Lostamt != 0 ? s.Plusmin : 0);
            var exceptsDelta = exceptsComp - excepts;

            var lostAmt = tripData.Sum(s => s.Lostamt);
            var lostAmtComp = tripDataComp.Sum(s => s.Lostamt);
            var lostAmtDelta = lostAmtComp - lostAmt;

            var negoSvngs = tripData.Sum(s => s.Negosvngs);
            var negoSvngsComp = tripDataComp.Sum(s => s.Negosvngs);
            var negoSvngsDelta = negoSvngsComp - negoSvngs;

            var avgAir = trips == 0 ? 0 : airChg / trips;
            var avgAirComp = tripsComp == 0 ? 0 : airChgComp / tripsComp;
            var avgAirDelta = avgAirComp - avgAir;


            var avgSavings = trips == 0 ? 0 : (stndChg - airChg) / trips;
            var avgSavingsComp = tripsComp == 0 ? 0 : (stndChgComp - airChgComp) / tripsComp;
            var avgSavingsDelta = avgSavingsComp - avgSavings;

            var svgsPcnt = stndChg == 0 ? 0 : (stndChg - airChg) / stndChg;
            var svgsPcntComp = stndChgComp == 0 ? 0 : (stndChgComp - airChgComp) / stndChgComp;
            var svgsPcntDelta = svgsPcntComp - svgsPcnt;

            var avgLost = trips == 0 ? 0 : lostAmt / trips;
            var avgLostComp = tripsComp == 0 ? 0 : lostAmtComp / tripsComp;
            var avgLostDelta = avgLostComp - avgLost;

            var lostPcnt = airChg == 0 ? 0 : lostAmt / airChg;
            var lostPcntComp = airChgComp == 0 ? 0 : lostAmtComp / airChgComp;
            var lostPcntDelta = lostPcntComp - lostPcnt;

            rows.Add(new FinalData
            {
                Rownum = 1,
                Grp = translations.xAir,
                Grpsort = 1,
                Subgrp = 1,
                Descrip = translations.xNbrOfTrips,
                Tots = trips.ToString(),
                Tots2 = tripsComp.ToString(),
                Tots3 = tripsDelta.ToString()
            });
            rows.Add(new FinalData
            {
                Rownum = 2,
                Grp = translations.xAir,
                Grpsort = 1,
                Subgrp = 2,
                Descrip = translations.xAirCharges,
                Tots = airChg.AddCurrency(currencySymbol,symbolPosition),
                Avgs = avgAir.AddCurrency(currencySymbol,symbolPosition),
                TotsDecimal = airChg,
                Tots2 = airChgComp.AddCurrency(currencySymbol,symbolPosition),
                Avgs2 = avgAirComp.AddCurrency(currencySymbol,symbolPosition),
                Tots2Decimal = airChgComp,
                Tots3 = airChgDelta.AddCurrency(currencySymbol,symbolPosition),
                Avgs3 = avgAirDelta.AddCurrency(currencySymbol,symbolPosition),
                Tots3Decimal = airChgDelta
            });

            if (!excludeSavings)
            {
                rows.Add(new FinalData
                {
                    Rownum = 3,
                    Grp = translations.xAir,
                    Grpsort = 1,
                    Subgrp = 3,
                    Descrip = translations.xSavings,
                    Tots = (stndChg - airChg).AddCurrency(currencySymbol,symbolPosition),
                    Avgs = avgSavings.AddCurrency(currencySymbol,symbolPosition),
                    Svgs = svgsPcnt.ToString("P"),
                    Tots2 = (stndChgComp - airChgComp).AddCurrency(currencySymbol,symbolPosition),
                    Avgs2 = avgSavingsComp.AddCurrency(currencySymbol,symbolPosition),
                    Svgs2 = svgsPcntComp.ToString("P"),
                    Tots3 = (stndChgDelta - airChgDelta).AddCurrency(currencySymbol,symbolPosition),
                    Avgs3 = avgSavingsDelta.AddCurrency(currencySymbol,symbolPosition),
                    Svgs3 = svgsPcntDelta.ToString("P")
                });
            }

            if (!excludeExceptions)
            {
                rows.Add(new FinalData
                {
                    Rownum = 4,
                    Grp = translations.xAir,
                    Grpsort = 1,
                    Subgrp = 4,
                    Descrip = translations.xNbrOfExcepts,
                    Tots = excepts.ToString(),
                    TotsDecimal = excepts,
                    Tots2 = exceptsComp.ToString(),
                    Tots2Decimal = exceptsComp,
                    Tots3 = exceptsDelta.ToString(),
                    Tots3Decimal = exceptsDelta
                });
                rows.Add(new FinalData
                {
                    Rownum = 5,
                    Grp = translations.xAir,
                    Grpsort = 1,
                    Subgrp = 5,
                    Descrip = translations.xLostSvgs,
                    Tots = lostAmt.AddCurrency(currencySymbol,symbolPosition),
                    Avgs = avgLost.AddCurrency(currencySymbol,symbolPosition),
                    Svgs = lostPcnt.ToString("P"),
                    TotsDecimal = lostAmt,
                    Tots2 = lostAmtComp.AddCurrency(currencySymbol,symbolPosition),
                    Avgs2 = avgLostComp.AddCurrency(currencySymbol,symbolPosition),
                    Svgs2 = lostPcntComp.ToString("P"),
                    Tots2Decimal = lostAmtComp,
                    Tots3 = lostAmtDelta.AddCurrency(currencySymbol,symbolPosition),
                    Avgs3 = avgLostDelta.AddCurrency(currencySymbol,symbolPosition),
                    Svgs3 = lostPcntDelta.ToString("P"),
                    Tots3Decimal = lostAmtDelta
                });
            }

            if (!excludeNegoSvngs)
            {
                rows.Add(new FinalData
                {
                    Rownum = 6,
                    Grp = translations.xAir,
                    Grpsort = 1,
                    Subgrp = 6,
                    Descrip = translations.xNegoSvgs,
                    Tots = negoSvngs.AddCurrency(currencySymbol,symbolPosition),
                    Tots2 = negoSvngsComp.AddCurrency(currencySymbol,symbolPosition),
                    Tots3 = negoSvngsDelta.AddCurrency(currencySymbol,symbolPosition)
                });
            }


            if (separateRail)
            {
                tripData = tripDataList.Where(s => s.ValcarMode.EqualsIgnoreCase("R")).ToList();
                if (!tripData.Any())
                {
                    tripData.Add(new RawData());
                }

                tripDataComp = tripDataListComp.Where(s => s.ValcarMode.EqualsIgnoreCase("R")).ToList();

                trips = tripData.Sum(s => s.Plusmin);
                tripsComp = tripDataComp.Sum(s => s.Plusmin);
                tripsDelta = tripsComp - trips;

                airChg = tripData.Sum(s => s.Airchg);
                airChgComp = tripDataComp.Sum(s => s.Airchg);
                airChgDelta = airChgComp - airChg;


                stndChg = tripData.Sum(s => s.Stndchg);
                stndChgComp = tripDataComp.Sum(s => s.Stndchg);
                stndChgDelta = stndChgComp - stndChg;

                excepts = tripData.Sum(s => s.Offrdchg != 0 && s.Lostamt != 0 ? s.Plusmin : 0);
                exceptsComp = tripDataComp.Sum(s => s.Offrdchg != 0 && s.Lostamt != 0 ? s.Plusmin : 0);
                exceptsDelta = exceptsComp - excepts;

                lostAmt = tripData.Sum(s => s.Lostamt);
                lostAmtComp = tripDataComp.Sum(s => s.Lostamt);
                lostAmtDelta = lostAmtComp - lostAmt;

                negoSvngs = tripData.Sum(s => s.Negosvngs);
                negoSvngsComp = tripDataComp.Sum(s => s.Negosvngs);
                negoSvngsDelta = negoSvngsComp - negoSvngs;

                avgAir = trips == 0 ? 0 : airChg / trips;
                avgAirComp = tripsComp == 0 ? 0 : airChgComp / tripsComp;
                avgAirDelta = avgAirComp - avgAir;

                avgSavings = trips == 0 ? 0 : (stndChg - airChg) / trips;
                avgSavingsComp = tripsComp == 0 ? 0 : (stndChgComp - airChgComp) / tripsComp;
                avgSavingsDelta = avgSavingsComp - avgSavings;

                svgsPcnt = stndChg == 0 ? 0 : (stndChg - airChg) / stndChg;
                svgsPcntComp = stndChgComp == 0 ? 0 : (stndChgComp - airChgComp) / stndChgComp;
                svgsPcntDelta = svgsPcntComp - svgsPcnt;

                avgLost = trips == 0 ? 0 : lostAmt / trips;
                avgLostComp = tripsComp == 0 ? 0 : lostAmtComp / tripsComp;
                avgLostDelta = avgLostComp - avgLost;

                lostPcnt = airChg == 0 ? 0 : lostAmt / airChg;
                lostPcntComp = airChgComp == 0 ? 0 : lostAmtComp / airChgComp;
                lostPcntDelta = lostPcntComp - lostPcnt;

                rows.Add(new FinalData
                {
                    Rownum = 7,
                    Grp = translations.xRail,
                    Grpsort = 2,
                    Subgrp = 1,
                    Descrip = translations.xNbrOfTrips,
                    Tots = trips.ToString(),
                    Tots2 = tripsComp.ToString(),
                    Tots3 = tripsDelta.ToString()
                });
                rows.Add(new FinalData
                {
                    Rownum = 8,
                    Grp = translations.xRail,
                    Grpsort = 2,
                    Subgrp = 2,
                    Descrip = translations.xAirCharges,
                    Tots = airChg.AddCurrency(currencySymbol,symbolPosition),
                    Avgs = avgAir.AddCurrency(currencySymbol,symbolPosition),
                    TotsDecimal = airChg,
                    Tots2 = airChgComp.AddCurrency(currencySymbol,symbolPosition),
                    Avgs2 = avgAirComp.AddCurrency(currencySymbol,symbolPosition),
                    Tots2Decimal = airChgComp,
                    Tots3 = airChgDelta.AddCurrency(currencySymbol,symbolPosition),
                    Avgs3 = avgAirDelta.AddCurrency(currencySymbol,symbolPosition),
                    Tots3Decimal = airChgDelta
                });

                if (!excludeSavings)
                {
                    rows.Add(new FinalData
                    {
                        Rownum = 9,
                        Grp = translations.xRail,
                        Grpsort = 2,
                        Subgrp = 3,
                        Descrip = translations.xSavings,
                        Tots = (stndChg - airChg).AddCurrency(currencySymbol,symbolPosition),
                        Avgs = avgSavings.AddCurrency(currencySymbol,symbolPosition),
                        Svgs = svgsPcnt.ToString("P"),
                        Tots2 = (stndChgComp - airChgComp).AddCurrency(currencySymbol,symbolPosition),
                        Avgs2 = avgSavingsComp.AddCurrency(currencySymbol,symbolPosition),
                        Svgs2 = svgsPcntComp.ToString("P"),
                        Tots3 = (stndChgDelta - airChgDelta).AddCurrency(currencySymbol,symbolPosition),
                        Avgs3 = avgSavingsDelta.AddCurrency(currencySymbol,symbolPosition),
                        Svgs3 = svgsPcntDelta.ToString("P")
                    });
                }

                if (!excludeExceptions)
                {
                    rows.Add(new FinalData
                    {
                        Rownum = 10,
                        Grp = translations.xRail,
                        Grpsort = 2,
                        Subgrp = 4,
                        Descrip = translations.xNbrOfExcepts,
                        Tots = excepts.ToString(),
                        TotsDecimal = excepts,
                        Tots2 = exceptsComp.ToString(),
                        Tots2Decimal = exceptsComp,
                        Tots3 = exceptsDelta.ToString(),
                        Tots3Decimal = exceptsDelta
                    });
                    rows.Add(new FinalData
                    {
                        Rownum = 11,
                        Grp = translations.xRail,
                        Grpsort = 2,
                        Subgrp = 5,
                        Descrip = translations.xLostSvgs,
                        Tots = lostAmt.AddCurrency(currencySymbol,symbolPosition),
                        Avgs = avgLost.AddCurrency(currencySymbol,symbolPosition),
                        Svgs = lostPcnt.ToString("P"),
                        TotsDecimal = lostAmt,
                        Tots2 = lostAmtComp.AddCurrency(currencySymbol,symbolPosition),
                        Avgs2 = avgLostComp.AddCurrency(currencySymbol,symbolPosition),
                        Svgs2 = lostPcntComp.ToString("P"),
                        Tots2Decimal = lostAmtComp,
                        Tots3 = lostAmtDelta.AddCurrency(currencySymbol,symbolPosition),
                        Avgs3 = avgLostDelta.AddCurrency(currencySymbol,symbolPosition),
                        Svgs3 = lostPcntDelta.ToString("P"),
                        Tots3Decimal = lostAmtDelta
                    });
                }

                if (!excludeNegoSvngs)
                {
                    rows.Add(new FinalData
                    {
                        Rownum = 12,
                        Grp = translations.xRail,
                        Grpsort = 2,
                        Subgrp = 6,
                        Descrip = translations.xNegoSvgs,
                        Tots = negoSvngs.AddCurrency(currencySymbol,symbolPosition),
                        Tots2 = negoSvngsComp.AddCurrency(currencySymbol,symbolPosition),
                        Tots3 = negoSvngsDelta.AddCurrency(currencySymbol,symbolPosition)
                    });
                }

            }


            return rows;
        }

        public static List<FinalData> GetCarRowsComp(List<CarRawData> carDataList, List<CarRawData> carDataListComp, Translations translations, bool excludeExceptions, string currencySymbol, string symbolPosition)
        {
            var rows = new List<FinalData>();

            var rentals = carDataList.Sum(s => s.Cplusmin);
            var rentalsComp = carDataListComp.Sum(s => s.Cplusmin);
            var rentalsDelta = rentalsComp - rentals;

            var days = carDataList.Sum(s => s.Cplusmin * s.Days);
            var daysComp = carDataListComp.Sum(s => s.Cplusmin * s.Days);
            var daysDelta = daysComp - days;

            var carCost = carDataList.Sum(s => s.Days * s.Abookrat);
            var carCostComp = carDataListComp.Sum(s => s.Days * s.Abookrat);
            var carCostDelta = carCostComp - carCost;

            var bookRate = carDataList.Sum(s => s.Abookrat);
            var bookRateComp = carDataListComp.Sum(s => s.Abookrat);

            var excepts = carDataList.Sum(s => string.IsNullOrEmpty(s.Reascoda.Trim()) ? 0 : s.Cplusmin);
            var exceptsComp = carDataListComp.Sum(s => string.IsNullOrEmpty(s.Reascoda.Trim()) ? 0 : s.Cplusmin);
            var exceptsDelta = exceptsComp - excepts;

            var excptnAmtLost = carDataList.Sum(s => string.IsNullOrEmpty(s.Reascoda.Trim()) ? 0 : (s.Abookrat - s.Aexcprat) * s.Days);
            var excptnAmtLostComp = carDataListComp.Sum(s => string.IsNullOrEmpty(s.Reascoda.Trim()) ? 0 : (s.Abookrat - s.Aexcprat) * s.Days);
            var excptnAmtLostDelta = excptnAmtLostComp - excptnAmtLost;

            var bookRateCount = carDataList.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin);
            var bookRateCountComp = carDataListComp.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin);

            var bookRateNights = carDataList.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin * s.Days);
            var bookRateNightsComp = carDataListComp.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin * s.Days);

            var avgDays = rentals == 0 ? 0m : days / (decimal)rentals;
            var avgDaysComp = rentalsComp == 0 ? 0m : daysComp / (decimal)rentalsComp;
            var avgDaysDelta = avgDaysComp - avgDays;

            var avgCost = bookRateCount == 0 ? 0m : bookRate / bookRateCount;
            var avgCostComp = bookRateCountComp == 0 ? 0m : bookRateComp / bookRateCountComp;
            var avgCostDelta = avgCostComp - avgCost;

            var dayCost = bookRateNights == 0 ? 0m : carCost / bookRateNights;
            var dayCostComp = bookRateNightsComp == 0 ? 0m : carCostComp / bookRateNightsComp;
            var dayCostDelta = dayCostComp - dayCost;

            
            var excCntPcnt = rentals == 0 ? 0m : excepts / (decimal)rentals;
            var excCntPcntComp = rentalsComp == 0 ? 0m : exceptsComp / (decimal)rentalsComp;
            var excCntPcntDelta = excCntPcntComp - excCntPcnt;

            var excDolPcnt = carCost == 0 ? 0m : excptnAmtLost / carCost;
            var excDolPcntComp = carCostComp == 0 ? 0m : excptnAmtLostComp / carCostComp;
            var excDolPcntDelta = excDolPcntComp - excDolPcnt;


            rows.Add(new FinalData
            {
                Rownum = 15,
                Grp = translations.xCarRental,
                Grpsort = 3,
                Subgrp = 1,
                Descrip = translations.xNbrRentals,
                Tots = rentals.ToString(),
                Tots2 = rentalsComp.ToString(),
                Tots3 = rentalsDelta.ToString()
            });
            rows.Add(new FinalData
            {
                Rownum = 16,
                Grp = translations.xCarRental,
                Grpsort = 3,
                Subgrp = 2,
                Descrip = translations.xNbrDays,
                Tots = days.ToString(),
                Avgs = avgDays.ToString("0.00"),
                Tots2 = daysComp.ToString(),
                Avgs2 = avgDaysComp.ToString("0.00"),
                Tots3 = daysDelta.ToString(),
                Avgs3 = avgDaysDelta.ToString("0.00")
            });
            rows.Add(new FinalData
            {
                Rownum = 17,
                Grp = translations.xCarRental,
                Grpsort = 3,
                Subgrp = 3,
                Descrip = translations.xCostBkRate,
                Tots = carCost.AddCurrency(currencySymbol,symbolPosition),
                Avgs = avgCost.AddCurrency(currencySymbol,symbolPosition),
                TotsDecimal = carCost,
                Tots2 = carCostComp.AddCurrency(currencySymbol,symbolPosition),
                Avgs2 = avgCostComp.AddCurrency(currencySymbol,symbolPosition),
                Tots2Decimal = carCostComp,
                Tots3 = carCostDelta.AddCurrency(currencySymbol,symbolPosition),
                Avgs3 = avgCostDelta.AddCurrency(currencySymbol,symbolPosition),
                Tots3Decimal = carCostDelta
            });
            rows.Add(new FinalData
            {
                Rownum = 18,
                Grp = translations.xCarRental,
                Grpsort = 3,
                Subgrp = 4,
                Descrip = translations.xCostPerDay,
                Tots = "N/A",
                Avgs = dayCost.AddCurrency(currencySymbol,symbolPosition),
                Tots2 = "N/A",
                Avgs2 = dayCostComp.AddCurrency(currencySymbol,symbolPosition),
                Tots3 = "N/A",
                Avgs3 = dayCostDelta.AddCurrency(currencySymbol,symbolPosition)
            });
            if (!excludeExceptions)
            {
                rows.Add(new FinalData
                {
                    Rownum = 19,
                    Grp = translations.xCarRental,
                    Grpsort = 3,
                    Subgrp = 5,
                    Descrip = translations.xNbrOfExcepts,
                    Tots = excepts.ToString(),
                    Svgs = excCntPcnt.ToString("P"),
                    TotsDecimal = excepts,
                    Tots2 = exceptsComp.ToString(),
                    Svgs2 = excCntPcntComp.ToString("P"),
                    Tots2Decimal = exceptsComp,
                    Tots3 = exceptsDelta.ToString(),
                    Svgs3 = excCntPcntDelta.ToString("P"),
                    Tots3Decimal = exceptsDelta
                });
                rows.Add(new FinalData
                {
                    Rownum = 20,
                    Grp = translations.xCarRental,
                    Grpsort = 3,
                    Subgrp = 6,
                    Descrip = translations.xExcpnAmtLost,
                    Tots = excptnAmtLost.AddCurrency(currencySymbol,symbolPosition),
                    Svgs = excDolPcnt.ToString("P"),
                    TotsDecimal = excptnAmtLost,
                    Tots2 = excptnAmtLostComp.AddCurrency(currencySymbol,symbolPosition),
                    Svgs2 = excDolPcntComp.ToString("P"),
                    Tots2Decimal = excptnAmtLostComp,
                    Tots3 = excptnAmtLostDelta.AddCurrency(currencySymbol,symbolPosition),
                    Svgs3 = excDolPcntDelta.ToString("P"),
                    Tots3Decimal = excptnAmtLostDelta
                });
            }

            return rows;
        }

        public static List<FinalData> GetHotelRowsComp(List<HotelRawData> hotelDataList, List<HotelRawData> hotelDataListComp, Translations translations, bool excludeExceptions, string currencySymbol, string symbolPosition)
        {
            var rows = new List<FinalData>();

            var rentals = hotelDataList.Sum(s => s.Hplusmin);
            var rentalsComp = hotelDataListComp.Sum(s => s.Hplusmin);
            var rentalsDelta = rentalsComp - rentals;

            var nights = hotelDataList.Sum(s => s.Hplusmin * s.Nights * s.Rooms);
            var nightsComp = hotelDataListComp.Sum(s => s.Hplusmin * s.Nights * s.Rooms);
            var nightsDelta = nightsComp - nights;

            var cost = hotelDataList.Sum(s => s.Nights * s.Bookrate * s.Rooms);
            var costComp = hotelDataListComp.Sum(s => s.Nights * s.Bookrate * s.Rooms);
            var costDelta = costComp - cost;

            var bookRate = hotelDataList.Sum(s => s.Bookrate);
            var bookRateComp = hotelDataListComp.Sum(s => s.Bookrate);

            var excepts = hotelDataList.Sum(s => string.IsNullOrEmpty(s.Reascodh.Trim()) ? 0 : s.Hplusmin);
            var exceptsComp = hotelDataListComp.Sum(s => string.IsNullOrEmpty(s.Reascodh.Trim()) ? 0 : s.Hplusmin);
            var exceptsDelta = exceptsComp - excepts;

            var excptnAmtLost = hotelDataList.Sum(s => string.IsNullOrEmpty(s.Reascodh.Trim()) ? 0 : (s.Bookrate - s.Hexcprat) * s.Nights * s.Rooms);
            var excptnAmtLostComp = hotelDataListComp.Sum(s => string.IsNullOrEmpty(s.Reascodh.Trim()) ? 0 : (s.Bookrate - s.Hexcprat) * s.Nights * s.Rooms);
            var excptnAmtLostDelta = excptnAmtLostComp - excptnAmtLost;

            var bookRateCount = hotelDataList.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin);
            var bookRateCountComp = hotelDataListComp.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin);

            var bookRateNights = hotelDataList.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin * s.Nights * s.Rooms);
            var bookRateNightsComp = hotelDataListComp.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin * s.Nights * s.Rooms);

            var avgDays = rentals == 0 ? 0m : (decimal)nights / rentals;
            var avgDaysComp = rentalsComp == 0 ? 0m : (decimal)nightsComp / rentalsComp;
            var avgDaysDelta = avgDaysComp - avgDays;

            var avgCost = bookRateCount == 0 ? 0m : bookRate / bookRateCount;
            var avgCostComp = bookRateCountComp == 0 ? 0m : bookRateComp / bookRateCountComp;
            var avgCostDelta = avgCostComp - avgCost;

            var dayCost = bookRateNights == 0 ? 0m : cost / bookRateNights;
            var dayCostComp = bookRateNightsComp == 0 ? 0m : costComp / bookRateNightsComp;
            var dayCostDelta = dayCostComp - dayCost;

            var excCntPcnt = rentals == 0 ? 0m : (decimal)excepts / rentals;
            var excCntPcntComp = rentalsComp == 0 ? 0m : (decimal)exceptsComp / rentalsComp;
            var excCntPcntDelta = excCntPcntComp - excCntPcnt;

            var excDolPcnt = cost == 0 ? 0m : excptnAmtLost / cost;
            var excDolPcntComp = costComp == 0 ? 0m : excptnAmtLostComp / costComp;
            var excDelPcntDelta = excDolPcntComp - excCntPcnt;

            rows.Add(new FinalData
            {
                Rownum = 21,
                Grp = translations.xHotelBkgs,
                Grpsort = 4,
                Subgrp = 1,
                Descrip = translations.xNbrBkngs,
                Tots = rentals.ToString(),
                Tots2 = rentalsComp.ToString(),
                Tots3 = rentalsDelta.ToString()
            });
            rows.Add(new FinalData
            {
                Rownum = 22,
                Grp = translations.xHotelBkgs,
                Grpsort = 4,
                Subgrp = 2,
                Descrip = translations.xNbrNights,
                Tots = nights.ToString(),
                Avgs = avgDays.ToString("0.00"),
                Tots2 = nightsComp.ToString(),
                Avgs2 = avgDaysComp.ToString("0.00"),
                Tots3 = nightsDelta.ToString(),
                Avgs3 = avgDaysDelta.ToString("0.00")
            });
            rows.Add(new FinalData
            {
                Rownum = 23,
                Grp = translations.xHotelBkgs,
                Grpsort = 4,
                Subgrp = 3,
                Descrip = translations.xCostBkRate,
                Tots = cost.AddCurrency(currencySymbol,symbolPosition),
                Avgs = avgCost.AddCurrency(currencySymbol,symbolPosition),
                TotsDecimal = cost,
                Tots2 = costComp.AddCurrency(currencySymbol,symbolPosition),
                Avgs2 = avgCostComp.AddCurrency(currencySymbol,symbolPosition),
                Tots2Decimal = costComp,
                Tots3 = costDelta.AddCurrency(currencySymbol,symbolPosition),
                Avgs3 = avgCostDelta.AddCurrency(currencySymbol,symbolPosition),
                Tots3Decimal = costDelta
            });
            rows.Add(new FinalData
            {
                Rownum = 24,
                Grp = translations.xHotelBkgs,
                Grpsort = 4,
                Subgrp = 4,
                Descrip = translations.xCostPerNight,
                Tots = "N/A",
                Avgs = dayCost.AddCurrency(currencySymbol,symbolPosition),
                Tots2 = "N/A",
                Avgs2 = dayCostComp.AddCurrency(currencySymbol,symbolPosition),
                Tots3 = "N/A",
                Avgs3 = dayCostDelta.AddCurrency(currencySymbol,symbolPosition)
            });
            if (!excludeExceptions)
            {
                rows.Add(new FinalData
                {
                    Rownum = 25,
                    Grp = translations.xHotelBkgs,
                    Grpsort = 4,
                    Subgrp = 5,
                    Descrip = translations.xNbrOfExcepts,
                    Tots = excepts.ToString(),
                    Svgs = excCntPcnt.ToString("P"),
                    TotsDecimal = excepts,
                    Tots2 = exceptsComp.ToString(),
                    Svgs2 = excCntPcntComp.ToString("P"),
                    Tots2Decimal = exceptsComp,
                    Tots3 = exceptsDelta.ToString(),
                    Svgs3 = excCntPcntDelta.ToString("P"),
                    Tots3Decimal = exceptsDelta
                });
                rows.Add(new FinalData
                {
                    Rownum = 26,
                    Grp = translations.xHotelBkgs,
                    Grpsort = 4,
                    Subgrp = 6,
                    Descrip = translations.xExcpnAmtLost,
                    Tots = excptnAmtLost.AddCurrency(currencySymbol,symbolPosition),
                    Svgs = excDolPcnt.ToString("P"),
                    TotsDecimal = excptnAmtLost,
                    Tots2 = excptnAmtLostComp.AddCurrency(currencySymbol,symbolPosition),
                    Svgs2 = excDolPcntComp.ToString("P"),
                    Tots2Decimal = excptnAmtLostComp,
                    Tots3 = excptnAmtLostDelta.AddCurrency(currencySymbol,symbolPosition),
                    Svgs3 = excDelPcntDelta.ToString("P"),
                    Tots3Decimal = excptnAmtLostDelta
                });
            }

            return rows;
        }

        public static List<FinalData> GetTotalRowsComp(List<FinalData> finalDataList, Translations translations, bool excludeExceptions, string currencySymbol, string symbolPosition)
        {
            var rows = new List<FinalData>();

            //totals
            var total = finalDataList.Where(s => s.Rownum == 2 || s.Rownum == 8 || s.Rownum == 17 || s.Rownum == 23).Sum(s => s.TotsDecimal);
            var totalComp = finalDataList.Where(s => s.Rownum == 2 || s.Rownum == 8 || s.Rownum == 17 || s.Rownum == 23).Sum(s => s.Tots2Decimal);
            var totalDelta = finalDataList.Where(s => s.Rownum == 2 || s.Rownum == 8 || s.Rownum == 17 || s.Rownum == 23).Sum(s => s.Tots3Decimal);

            rows.Add(new FinalData
            {
                Rownum = 27,
                Grp = translations.xRptTotals,
                Grpsort = 5,
                Subgrp = 1,
                Descrip = translations.xTotCharges,
                Tots = total.AddCurrency(currencySymbol,symbolPosition),
                Tots2 = totalComp.AddCurrency(currencySymbol,symbolPosition),
                Tots3 = totalDelta.AddCurrency(currencySymbol,symbolPosition)
            });

            if (!excludeExceptions)
            {
                var totalExceptions = finalDataList.Where(s => s.Rownum == 4 || s.Rownum == 10 || s.Rownum == 19 || s.Rownum == 25).Sum(s => s.TotsDecimal);
                var totalExceptionsComp = finalDataList.Where(s => s.Rownum == 4 || s.Rownum == 10 || s.Rownum == 19 || s.Rownum == 25).Sum(s => s.Tots2Decimal);
                var totalExceptionsDelta = finalDataList.Where(s => s.Rownum == 4 || s.Rownum == 10 || s.Rownum == 19 || s.Rownum == 25).Sum(s => s.Tots3Decimal);

                var totalLost = finalDataList.Where(s => s.Rownum == 5 || s.Rownum == 11 || s.Rownum == 20 || s.Rownum == 26).Sum(s => s.TotsDecimal);
                var totalLostComp = finalDataList.Where(s => s.Rownum == 5 || s.Rownum == 11 || s.Rownum == 20 || s.Rownum == 26).Sum(s => s.Tots2Decimal);
                var totalLostDelta = finalDataList.Where(s => s.Rownum == 5 || s.Rownum == 11 || s.Rownum == 20 || s.Rownum == 26).Sum(s => s.Tots3Decimal);

                rows.Add(new FinalData
                {
                    Rownum = 28,
                    Grp = translations.xRptTotals,
                    Grpsort = 5,
                    Subgrp = 2,
                    Descrip = translations.xTotExcepts,
                    Tots = totalExceptions.ToString("#"),
                    Tots2 = totalExceptionsComp.ToString("#"),
                    Tots3 = totalExceptionsDelta.ToString("#")
                });
                rows.Add(new FinalData
                {
                    Rownum = 29,
                    Grp = translations.xRptTotals,
                    Grpsort = 5,
                    Subgrp = 3,
                    Descrip = translations.xTotLost,
                    Tots = totalLost.AddCurrency(currencySymbol,symbolPosition),
                    Tots2 = totalLostComp.AddCurrency(currencySymbol,symbolPosition),
                    Tots3 = totalLostDelta.AddCurrency(currencySymbol,symbolPosition)
                });
            }


            return rows;
        }

        public static string AddCurrency(this decimal val, string symbol, string position)
        {
            NumberFormatInfo currencyFormat = new CultureInfo(CultureInfo.CurrentCulture.ToString()).NumberFormat;
            currencyFormat.CurrencyNegativePattern = 1;

            return position == "L"
            ? symbol.Trim() + val.ToString("C", currencyFormat).Trim().Replace("$",string.Empty)
            : val.ToString("C",currencyFormat).Trim().Replace("$", string.Empty) + symbol.Trim();

        }
    }
}

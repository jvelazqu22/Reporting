using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    public class ExecSummaryYrToYrDateCalculator
    {
        public DateTime GetBeginDate(int fiscalYearStartMonthNumber, int startMonthNumber, int startYear)
        {
            return fiscalYearStartMonthNumber > startMonthNumber
                ? new DateTime(startYear - 2, fiscalYearStartMonthNumber, 1)
                : new DateTime(startYear - 1, fiscalYearStartMonthNumber, 1);
        }

        public DateTime GetBeginDate2(int fiscalYearStartMonthNumber, int startMonthNumber, int startYear)
        {
            return fiscalYearStartMonthNumber > startMonthNumber
                ? new DateTime(startYear - 1, fiscalYearStartMonthNumber, 1)
                : new DateTime(startYear, fiscalYearStartMonthNumber, 1);
        }

        public DateTime GetBeginMonth2(int startYear, int startMonthNumber, bool isQuarterToQuarterOption, int fiscalYearStartMonthNumber)
        {
            var beginMonth2 = new DateTime(startYear, startMonthNumber, 1);

            if (isQuarterToQuarterOption)
            {
                var calc = new ExecutiveSummaryYearToYearCalculations();
                var startQtrMonth = calc.GetStartQuarterMonth(fiscalYearStartMonthNumber, startMonthNumber);
                beginMonth2 = new DateTime(startYear, startQtrMonth, 1);
            }

            return beginMonth2;
        }

        public DateTime GetOneYearPrior(DateTime begMonth2)
        {
            return begMonth2.AddMonths(-12);
        }

        public DateTime GetEndDate2(DateTime begMonth2, bool isQuarterToQuarterOption)
        {
            return isQuarterToQuarterOption
                ? begMonth2.AddMonths(3).AddSeconds(-1)
                : begMonth2.AddMonths(1).AddSeconds(-1);
        }

        public bool DataExists(IEnumerable<RawData> cyAirData, IEnumerable<RawData> pyAirData, IEnumerable<LegRawData> cyLegData, IEnumerable<LegRawData> pyLegData,
                          IEnumerable<CarRawData> cyCarData, IEnumerable<CarRawData> pyCarData, IEnumerable<HotelRawData> cyHotelData, IEnumerable<HotelRawData> pyHotelData)
        {
            return cyAirData.Any() || pyAirData.Any() || cyLegData.Any() || pyLegData.Any() || cyCarData.Any() || pyCarData.Any() || cyHotelData.Any() || pyHotelData.Any();
        }

        public void AssignCurrency(string moneyType, List<FinalData> FinalDataList)
        {
            //Crystal Report does not assign the currency symbol and position, as all values are strings, so we do it here. 
            var currencySymbol = "$";
            var currencyPosition = "L";
            var addCurrency = false;
            if (!string.IsNullOrEmpty(moneyType) && !moneyType.Equals("USD"))
            {
                var curSettings = new GetCurrencySettingsByMoneyTypeQuery(new iBankMastersQueryable(), moneyType).ExecuteQuery();
                currencySymbol = curSettings.csymbol.Trim();
                currencyPosition = curSettings.cleftright.Trim();
                addCurrency = true;
            }

            if (addCurrency)
            {
                foreach (var row in FinalDataList)
                {
                    row.CurrencySymbol = currencySymbol;
                    row.SymbolPosition = currencyPosition;
                }
            }
        }

        public void ProcessTripData(bool useBaseFare, IEnumerable<RawData> airData, List<string> reasExclude)
        {
            //var reasExclude = Globals.AgencyInformation.ReasonExclude.Split(',').ToList();
            foreach (var row in airData)
            {
                if (useBaseFare)
                {
                    //DEALING WITH BAD DATA.
                    if (row.Basefare == 0)
                        row.Basefare = row.Airchg;

                    row.Stndchg = Math.Abs(row.Stndchg) < Math.Abs(row.Basefare) || row.Stndchg == 0 ||
                                  (row.Stndchg > 0 && row.Basefare < 0)
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
                    row.Stndchg = Math.Abs(row.Stndchg) < Math.Abs(row.Airchg) || row.Stndchg == 0 ||
                                  (row.Stndchg > 0 && row.Airchg < 0)
                        ? row.Airchg
                        : row.Stndchg;
                    row.Offrdchg = (row.Offrdchg > 0 && row.Airchg < 0)
                        ? 0 - row.Offrdchg
                        : row.Offrdchg == 0 ? row.Airchg : row.Offrdchg;
                    row.Savings = row.Stndchg - row.Airchg;
                    row.Mktfare = row.Mktfare == 0 ? row.Airchg : row.Mktfare;
                    row.LostAmt = row.Airchg - row.Offrdchg;
                }
                row.Reascode = reasExclude.Contains(row.Reascode) ? string.Empty : row.Reascode;
                row.Advance = (row.Depdate.GetValueOrDefault() - row.Invdate.GetValueOrDefault()).Days;

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

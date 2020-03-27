using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.AccountSummary12MonthTrend;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.AccountSummaryAir12MonthTrend
{
    public class AcctSum12MonthDataProcessor
    {
        public IList<GroupedRecord> GroupData(IList<RawData> rawData)
        {
            return rawData.GroupBy(s => new { s.Acct, s.UseDate }, s => new { s.PlusMin, s.AirChg }, (key, g) =>
            {
                var temp = g.ToList();
                return new GroupedRecord
                {
                    Acct = key.Acct,
                    Trips = temp.Sum(s => s.PlusMin),
                    RecordCount = temp.Count,
                    Amount = temp.Sum(s => s.AirChg),
                    UsedDate = key.UseDate

                };
            }).ToList();
        }

        public IList<FinalData> FillAmounts(IList<GroupedRecord> groupedRecs, int[,] months)
        {
            var list = groupedRecs.GroupBy(s => s.Acct, s => s, (key, g) =>
            {
                var temp = g.ToList();
                return new FinalData
                {
                    Acct = key,
                    //AcctDesc = key.Name,
                    Trips1 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[0, 1] && s.UsedDate.Value.Month == months[0, 0]).Sum(s => s.Trips),
                    Trips2 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[1, 1] && s.UsedDate.Value.Month == months[1, 0]).Sum(s => s.Trips),
                    Trips3 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[2, 1] && s.UsedDate.Value.Month == months[2, 0]).Sum(s => s.Trips),
                    Trips4 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[3, 1] && s.UsedDate.Value.Month == months[3, 0]).Sum(s => s.Trips),
                    Trips5 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[4, 1] && s.UsedDate.Value.Month == months[4, 0]).Sum(s => s.Trips),
                    Trips6 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[5, 1] && s.UsedDate.Value.Month == months[5, 0]).Sum(s => s.Trips),
                    Trips7 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[6, 1] && s.UsedDate.Value.Month == months[6, 0]).Sum(s => s.Trips),
                    Trips8 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[7, 1] && s.UsedDate.Value.Month == months[7, 0]).Sum(s => s.Trips),
                    Trips9 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[8, 1] && s.UsedDate.Value.Month == months[8, 0]).Sum(s => s.Trips),
                    Trips10 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[9, 1] && s.UsedDate.Value.Month == months[9, 0]).Sum(s => s.Trips),
                    Trips11 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[10, 1] && s.UsedDate.Value.Month == months[10, 0]).Sum(s => s.Trips),
                    Trips12 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[11, 1] && s.UsedDate.Value.Month == months[11, 0]).Sum(s => s.Trips),
                    Amt1 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[0, 1] && s.UsedDate.Value.Month == months[0, 0]).Sum(s => s.Amount),
                    Amt2 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[1, 1] && s.UsedDate.Value.Month == months[1, 0]).Sum(s => s.Amount),
                    Amt3 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[2, 1] && s.UsedDate.Value.Month == months[2, 0]).Sum(s => s.Amount),
                    Amt4 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[3, 1] && s.UsedDate.Value.Month == months[3, 0]).Sum(s => s.Amount),
                    Amt5 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[4, 1] && s.UsedDate.Value.Month == months[4, 0]).Sum(s => s.Amount),
                    Amt6 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[5, 1] && s.UsedDate.Value.Month == months[5, 0]).Sum(s => s.Amount),
                    Amt7 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[6, 1] && s.UsedDate.Value.Month == months[6, 0]).Sum(s => s.Amount),
                    Amt8 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[7, 1] && s.UsedDate.Value.Month == months[7, 0]).Sum(s => s.Amount),
                    Amt9 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[8, 1] && s.UsedDate.Value.Month == months[8, 0]).Sum(s => s.Amount),
                    Amt10 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[9, 1] && s.UsedDate.Value.Month == months[9, 0]).Sum(s => s.Amount),
                    Amt11 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[10, 1] && s.UsedDate.Value.Month == months[10, 0]).Sum(s => s.Amount),
                    Amt12 = temp.Where(s => s.UsedDate.HasValue && s.UsedDate.Value.Year == months[11, 1] && s.UsedDate.Value.Month == months[11, 0]).Sum(s => s.Amount),
                };
            })
            .ToList();

            return list;
        }

        public IList<FinalData> FillFinalData(IList<FinalData> finalData, IClientQueryable clientQueryDb, string agency, ClientFunctions clientFunctions, bool isGroupByParentAccount)
        {
            var calc = new AcctSum12MonthCalculations();
            foreach (var rec in finalData)
            {
                rec.YrTrips = calc.GetYearTrips(rec);
                rec.YrAmt = calc.GetYearAmount(rec);

                var name = new GetParentAccountQuery(clientQueryDb.Clone() as IClientQueryable, rec.Acct, agency).ExecuteQuery();
                if (isGroupByParentAccount)
                {
                    if (name != null)
                    {
                        var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientQueryDb.Clone() as IClientQueryable, agency);
                        var getAllParentAccountsQuery = new GetAllParentAccountsQuery(clientQueryDb.Clone() as IClientQueryable);
                        var parent = clientFunctions.LookupParent(getAllMasterAccountsQuery, name.acct.Trim(), getAllParentAccountsQuery, agency);
                        rec.Acct = parent.AccountId;
                        rec.AcctDesc = parent.AccountDescription;
                    }
                }
                else
                {
                    rec.AcctDesc = (name == null) ? $"{rec.Acct.Trim()}  NOT FOUND" : name.acctname;
                }
            }

            return GroupFinalDataToIncludeParentAccounts(finalData);
            //return finalData;
        }

        public IList<FinalData> GroupFinalDataToIncludeParentAccounts(IList<FinalData> finalData)
        {
            var list = finalData.GroupBy(s => s.Acct, s => s, (key, g) =>
            {
                var temp = g.ToList();
                return new FinalData
                {
                    Acct = key,
                    AcctDesc = temp.First().AcctDesc,
                    //AcctDesc = key.Name,
                    Trips1 = temp.Sum(s => s.Trips1),
                    Trips2 = temp.Sum(s => s.Trips2),
                    Trips3 = temp.Sum(s => s.Trips3),
                    Trips4 = temp.Sum(s => s.Trips4),
                    Trips5 = temp.Sum(s => s.Trips5),
                    Trips6 = temp.Sum(s => s.Trips6),
                    Trips7 = temp.Sum(s => s.Trips7),
                    Trips8 = temp.Sum(s => s.Trips8),
                    Trips9 = temp.Sum(s => s.Trips9),
                    Trips10 = temp.Sum(s => s.Trips10),
                    Trips11 = temp.Sum(s => s.Trips11),
                    Trips12 = temp.Sum(s => s.Trips12),
                    Amt1 = temp.Sum(s => s.Amt1),
                    Amt2 = temp.Sum(s => s.Amt2),
                    Amt3 = temp.Sum(s => s.Amt3),
                    Amt4 = temp.Sum(s => s.Amt4),
                    Amt5 = temp.Sum(s => s.Amt5),
                    Amt6 = temp.Sum(s => s.Amt6),
                    Amt7 = temp.Sum(s => s.Amt7),
                    Amt8 = temp.Sum(s => s.Amt8),
                    Amt9 = temp.Sum(s => s.Amt9),
                    Amt10 = temp.Sum(s => s.Amt10),
                    Amt11 = temp.Sum(s => s.Amt11),
                    Amt12 = temp.Sum(s => s.Amt12),
                    YrTrips = temp.Sum(s => s.YrTrips),
                    YrAmt = temp.Sum(s => s.YrAmt)
                };
            })
            .ToList();

            return list;
        }

        public IList<FinalData> SortData(IList<FinalData> finalData, string sortBy)
        {
            switch (sortBy)
            {
                case "2":
                    return finalData.OrderByDescending(s => s.YrTrips).ToList();
                case "3":
                    return finalData.OrderByDescending(s => s.YrAmt).ToList();
                default:
                    return finalData.OrderBy(s => s.AcctDesc).ToList();
            }
            
        }
    }
}

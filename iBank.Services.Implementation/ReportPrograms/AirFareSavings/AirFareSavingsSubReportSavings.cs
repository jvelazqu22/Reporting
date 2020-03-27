using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.AirFareSavingsReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.AirFareSavings
{
    public class AirFareSavingsSubReportSavings
    {
        public List<SummaryDataInformation> GetSavingCodesSummaryData(ClientFunctions clientFunctions, List<FinalData> finalDataList, ReportGlobals globals,
            IClientDataStore clientStore, IMasterDataStore masterStore)
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, globals.Agency);
            var returnValue = finalDataList.Where(w => w.Seqno == 1 && (!w.Savingcode.IsNullOrWhiteSpace() || w.Savings != 0))
                .GroupBy(s => new { s.Savingcode, s.Origacct },
                    (key, g) =>
                    {
                        return new SummaryDataInformation()
                        {
                            Description = clientFunctions.LookupReason(getAllMasterAccountsQuery, key.Savingcode, key.Origacct, clientStore, globals, masterStore.MastersQueryDb),
                            Count = g.Sum(s => s.Plusmin),
                            Amount = g.Sum(s => s.Savings)
                        };
                    }).ToList();

                returnValue = returnValue.GroupBy(s => new { s.Description },
                (key, g) =>
                {
                    var listGroupByDescription = g.ToList();

                    return new SummaryDataInformation()
                    {
                        Description = listGroupByDescription.First().Description,
                        Count = listGroupByDescription.Sum(s => s.Count),
                        Amount = listGroupByDescription.Sum(s => s.Amount)
                    };
                }).ToList();

            return returnValue;
        }

        public List<SummaryDataInformation> GetNegotiatedSavingsSummaryData(List<FinalData> FinalDataList)
        {
            List<NegotiatedSavingsModel> list = GetDistinctNegotiatedSavingsModelListFromFinalDataList(FinalDataList);

            return list.Where(w => w.Negosvngs != 0).GroupBy(s => 1,
                    (key, g) =>
                    {
                        return new SummaryDataInformation() { Count = g.Sum(x => x.Plusmin), Amount = g.Sum(x => x.Negosvngs), Description = "Negotiated Savings" };
                    }).ToList();
        }

        public List<NegotiatedSavingsModel> GetDistinctNegotiatedSavingsModelListFromFinalDataList(List<FinalData> FinalDataList)
        {
            return FinalDataList.GroupBy(s =>
                        new
                        {
                            s.Reckey,
                            s.Savingcode,
                            s.Reascode,
                            s.Airchg,
                            s.Offrdchg,
                            s.Stndchg,
                            s.Lostamt,
                            s.Savings,
                            s.Negosvngs,
                            s.Plusmin,
                            s.Origacct
                        })
                        .Select(s => new NegotiatedSavingsModel()
                        {
                            Reckey = s.First().Reckey,
                            Savingcode = s.First().Savingcode.Trim(),
                            Reascode = s.First().Reascode.Trim(),
                            Airchg = s.First().Airchg,
                            Offrdchg = s.First().Offrdchg,
                            Stndchg = s.First().Stndchg,
                            Lostamt = s.First().Lostamt,
                            Savings = s.First().Savings,
                            Negosvngs = s.First().Negosvngs,
                            Plusmin = s.First().Plusmin,
                            Origacct = s.First().Origacct.Trim()
                        }
                        ).OrderBy(o => o.Airchg).ToList();
        }

        public void SavingsCodesFirstLossCodesSecondSetUp(List<SummaryDataInformation> aSavCodes, List<SummaryDataInformation> aLossCodes, List<SubReportData> subReportDataList)
        {
            int lossCodesCounter = aLossCodes.Count;
            int index = 0;
            foreach (var aSavCode in aSavCodes)
            {
                var subReportData = new SubReportData();
                subReportData.Savingdesc = aSavCode.Description;
                subReportData.Svgamt = aSavCode.Amount;
                subReportData.Svgcount = aSavCode.Count;
                if (lossCodesCounter > 0)
                {
                    subReportData.Lossamt = aLossCodes[index].Amount;
                    subReportData.Losscount = aLossCodes[index].Count;
                    subReportData.Lossdesc = aLossCodes[index].Description;
                    lossCodesCounter--;
                    index++;
                }
                subReportDataList.Add(subReportData);
            }
        }

        public void NegotiatedSavings(List<SummaryDataInformation> aNegoSvngs, List<SubReportData> subReportDataList)
        {
            foreach (var item in aNegoSvngs)
            {
                subReportDataList.Add(new SubReportData()
                {
                    Svgcount = item.Count,
                    Svgamt = item.Amount,
                    Savingdesc = item.Description
                });
            }
        }

    }
}

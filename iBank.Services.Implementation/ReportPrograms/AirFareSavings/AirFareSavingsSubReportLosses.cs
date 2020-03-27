using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.AirFareSavingsReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.AirFareSavings
{
    public class AirFareSavingsSubReportLosses
    {
        public List<SummaryDataInformation> GetLossCodesSummaryData(ClientFunctions clientFunctions, List<FinalData> finalDataList, ReportGlobals globals,
                                                                     IClientDataStore clientStore, IMasterDataStore masterStore)
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, globals.Agency);

            //var temp = finalDataList.Where(w =>
            //    w.Negosvngs == 0 && w.Seqno == 1 &&
            //    (!w.Reascode.IsNullOrWhiteSpace() || (w.Offrdchg != 0 && (w.Plusmin * w.Lostamt) > 0))).ToList();

            var finalListWithNoSavingsAndSeqNo1 = finalDataList.Where(w => w.Negosvngs == 0 && w.Seqno == 1).ToList();
            //var temp3 = temp2.Select(s => s.Reascode).Distinct().ToList();
            //var strReasonCodes = string.Join(",", temp3);

            var returnValue = finalListWithNoSavingsAndSeqNo1
                .GroupBy(s => new { s.Reascode, s.Origacct },
                            (key, g) =>
                            {
                                return new SummaryDataInformation()
                                {
                                    Description = clientFunctions.LookupReason(getAllMasterAccountsQuery, key.Reascode, key.Origacct, clientStore, globals, masterStore.MastersQueryDb),
                                    Count = g.Sum(s => s.Plusmin),
                                    Amount = g.Sum(s => s.Lostamt)
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

        public void LossCodesFirstSavingsCodesSecondSetUp(List<SummaryDataInformation> aSavCodes, List<SummaryDataInformation> aLossCodes, bool lExSavings, List<SubReportData> subReportDataList)
        {
            int index = 0, saveCodesCounter = aSavCodes.Count;
            foreach (var aLossCode in aLossCodes)
            {
                var subReportData = new SubReportData();
                subReportData.Lossamt = aLossCode.Amount;
                subReportData.Losscount = aLossCode.Count;
                subReportData.Lossdesc = aLossCode.Description;
                if (!lExSavings && saveCodesCounter > 0)
                {
                    subReportData.Savingdesc = aSavCodes[index].Description;
                    subReportData.Svgamt = aSavCodes[index].Amount;
                    subReportData.Svgcount = aSavCodes[index].Count;
                    saveCodesCounter--;
                    index++;
                }
                subReportDataList.Add(subReportData);
            }
        }
    }
}

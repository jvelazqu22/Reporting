using Domain.Models.ReportPrograms.HotelFareSavings;
using Domain.Orm.Classes;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.HotelFareSavings
{
    public class HotelSavingsSummaryData
    {
        public List<SubReportData> AddSummaryData(List<SubReportData> SubReportList, List<SubReportData> savingsCodes, List<SubReportData> lossCodes)
        {
            var subRows = Math.Max(savingsCodes.Count, lossCodes.Count);
            for (var i = 1; i <= subRows; i++)
            {
                var newRow = new SubReportData();

                if (savingsCodes.Count >= i)
                {
                    newRow.Savingdesc = savingsCodes[i - 1].Savingdesc;
                    newRow.Svgcount = savingsCodes[i - 1].Svgcount;
                    newRow.Svgamt = savingsCodes[i - 1].Svgamt;
                }

                if (lossCodes.Count >= i)
                {
                    newRow.Lossdesc = lossCodes[i - 1].Lossdesc;
                    newRow.Losscount = lossCodes[i - 1].Losscount;
                    newRow.Lossamt = lossCodes[i - 1].Lossamt;
                }
                SubReportList.Add(newRow);
            }

            return SubReportList;
        }

        public List<SubReportData> GetSavingCodes(bool excludeSavings, List<FinalData> finalDataList, ClientFunctions clientFunctions,
            IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, ReportGlobals globals, IClientDataStore clientStore, IMasterDataStore masterStore)
        {
            var results = new List<SubReportData>();
            if (!excludeSavings)
            {
                var filterDataResults = finalDataList.Where(s => !string.IsNullOrEmpty(s.Hotsvgcode.Trim()) || s.Savings != 0);
                results = filterDataResults
                .GroupBy(s => clientFunctions.LookupReason(getAllMasterAccountsQuery, s.Hotsvgcode, s.Origacct, clientStore, globals, masterStore.MastersQueryDb),
                        (key, recs) =>
                        {
                            var reclist = recs as IList<FinalData> ?? recs.ToList();
                            return new SubReportData
                            {
                                Savingdesc = key,
                                Svgcount = reclist.Sum(s => s.Hplusmin),
                                Svgamt = reclist.Sum(s => s.Savings)
                            };
                        })
                        .OrderBy(s => s.Savingdesc)
                        .ToList();
            }
            return results;
        }

        public List<SubReportData> GetLossCodes(List<FinalData> finalDataList, ClientFunctions clientFunctions,
            IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, ReportGlobals globals, IClientDataStore clientStore, IMasterDataStore masterStore)
        {
            var results = new List<SubReportData>();

            var filterDataResults = finalDataList.Where(s => !string.IsNullOrEmpty(s.Reascodh.Trim()) || (s.Hexcprat != 0 && s.Lostamt * s.Hplusmin != 0));
            results = filterDataResults
                .GroupBy(s => clientFunctions.LookupReason(getAllMasterAccountsQuery, s.Reascodh, s.Origacct, clientStore, globals, masterStore.MastersQueryDb),
                        (key, recs) =>
                        {
                            var reclist = recs as IList<FinalData> ?? recs.ToList();
                            return new SubReportData
                            {
                                Lossdesc = key,
                                Losscount = reclist.Sum(s => s.Hplusmin),
                                Lossamt = reclist.Sum(s => s.Lostamt)
                            };
                        })
                        .OrderBy(s => s.Lossdesc)
                        .ToList();

            return results;
        }

    }
}

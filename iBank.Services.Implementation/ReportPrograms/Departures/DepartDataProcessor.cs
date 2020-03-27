using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;

using Domain.Models.ReportPrograms.DeparturesReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using Domain.Helper;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.Departures
{
    public class DepartDataProcessor
    {
        public IList<FinalData> ProcessRawIntoFinal(IList<RawData> rawData, IMasterDataStore store, UserInformation user, UserBreaks userBreaks, bool flightSort, bool sortByName, bool includeAllLegs,
                                                    bool includePassengerCountByFlight, bool includePageBreakByDate, ReportGlobals globals)
        {
            var finalData = rawData.Select(x => new FinalData
            {
                RecKey = x.RecKey,
                Acct = !user.AccountBreak ? "^na^" : x.Acct,
                Break1 = !userBreaks.UserBreak1 ? "^na^" : ((string.IsNullOrEmpty(x.Break1.Trim()) ? "NONE".PadRight(30) : x.Break1.Trim())),
                Break2 = !userBreaks.UserBreak2 ? "^na^" : ((string.IsNullOrEmpty(x.Break2.Trim()) ? "NONE".PadRight(30) : x.Break2.Trim())),
                Break3 = !userBreaks.UserBreak3 ? "^na^" : ((string.IsNullOrEmpty(x.Break3.Trim()) ? "NONE".PadRight(16) : x.Break3.Trim())),
                Destinat = x.Destinat,
                Destdesc = AportLookup.LookupAport(new MasterDataStore(), x.Destinat, x.Mode, globals.Agency),
                Invoice = x.Invoice,
                Recloc = x.RecLoc,
                PlusMin = x.PlusMin,
                RdepDate = x.RDepDate ?? DateTime.MinValue,
                PassLast = x.PassLast,
                PassFrst = x.PassFrst,
                Airline = x.Airline,
                AlineDesc = LookupFunctions.LookupAline(store, x.Airline, x.Mode),
                Pseudocity = x.PseudoCity,
                FltNo = x.fltno,
                Origin = x.Origin,
                OrgDesc = AportLookup.LookupAport(store, x.Origin, x.Mode, globals.Agency),
                ArrTime = SharedProcedures.ConvertTime(x.ArrTime),
                DepTime = SharedProcedures.ConvertTime(x.DepTime),
                FltSort = flightSort ? x.fltno : "X",
                SortDepTim = x.DepTime.Trim().PadLeft(5, '0'),
                Seg_Cntr = x.Seg_Cntr,
                SegNum = x.SegNum,
                TxtDepDate = x.RDepDate.GetValueOrDefault().ToShortDateString(),
                SeqNo = x.SeqNo,
                Mode = x.Mode
            }).ToList();

            finalData.ForEach(x => x.CrysPgBrk = includePageBreakByDate ? x.OrgDesc + x.RdepDate.ToShortDateString() : "1");

            if (includePassengerCountByFlight && !includeAllLegs && !sortByName)
            {
                return finalData.OrderBy(x => x.OrgDesc)
                    .ThenBy(x => x.RdepDate)
                    .ThenBy(x => x.SortDepTim)
                    .ThenBy(x => x.FltSort)
                    .ThenBy(x => x.FltNo)
                    .ThenBy(x => x.PassLast)
                    .ThenBy(x => x.PassFrst)
                    .ToList();
            }
            else
            {
                if (sortByName)
                {
                    return finalData.OrderBy(x => x.OrgDesc)
                            .ThenBy(x => x.RdepDate)
                            .ThenBy(x => x.PassLast)
                            .ThenBy(x => x.PassFrst)
                            .ThenBy(x => x.SortDepTim)
                            .ToList();
                }
                else
                {
                    return finalData.OrderBy(x => x.OrgDesc)
                            .ThenBy(x => x.RdepDate)
                            .ThenBy(x => x.SortDepTim)
                            .ThenBy(x => x.PassLast)
                            .ThenBy(x => x.PassFrst)
                            .ToList();
                }
            }
        }

        public IList<FinalData> CombineFinalDataWithLegData(IList<FinalData> finalData, IList<RawData> legData, IMasterDataStore store, bool sortByName, ReportGlobals globals)
        {
            var newFinal = from leg in legData
                           join final in finalData
                               on new
                                {
                                    leg.RecKey,
                                    leg.Seg_Cntr
                                }
                               equals new
                                {
                                    final.RecKey,
                                    final.Seg_Cntr
                                }
                               into temp
                           from final in temp
                           select new FinalData
                            {
                               RecKey = leg.RecKey,
                               Acct = final.Acct,
                               Break1 = final.Break1,
                               Break2 = final.Break2,
                               Break3 = final.Break3,
                               FrstOrigin = final == null ? string.Empty : final.Origin,
                               FrstDesc = final == null ? string.Empty : final.OrgDesc,
                               Pseudocity = final == null ? string.Empty : final.Pseudocity,
                               Recloc = final == null ? string.Empty : final.Recloc,
                               RdepDate = final == null ? DateTime.MinValue : final.RdepDate,
                               PassLast = final == null ? string.Empty : final.PassLast,
                               PassFrst = final == null ? string.Empty : final.PassFrst,
                               SortDepTim = final == null ? string.Empty : final.SortDepTim,
                               Airline = leg.Airline,
                               AlineDesc = LookupFunctions.LookupAline(store, leg.Airline, leg.Mode),
                               FltNo = leg.fltno,
                               DepTime = SharedProcedures.ConvertTime(leg.DepTime),
                               Origin = leg.Origin,
                               OrgDesc = AportLookup.LookupAport(store, leg.Origin, leg.Mode, globals.Agency),
                               Destinat = leg.Destinat,
                               Destdesc = AportLookup.LookupAport(store, leg.Destinat, leg.Mode, globals.Agency),
                               ArrTime = SharedProcedures.ConvertTime(leg.ArrTime),
                               TxtDepDate = leg.RDepDate.GetValueOrDefault().ToShortDateString(),
                               SeqNo = leg.SeqNo
                           };

            return newFinal.ToList();
        }

        public IList<FinalData> SortData(IList<FinalData> finalData, bool sortByName, bool includePassengerCountByFlight, bool includeAllLegs)
        {
            if (includeAllLegs)
            {
                if (sortByName)
                {
                    return finalData.OrderBy(x => x.FrstDesc)
                        .ThenBy(x => x.RdepDate)
                        .ThenBy(x => x.PassLast)
                        .ThenBy(x => x.PassFrst)
                        .ThenBy(x => x.RecKey)
                        .ThenBy(x => x.SeqNo)
                        .ToList();
                }
                else
                {
                    return finalData.OrderBy(x => x.FrstDesc)
                        .ThenBy(x => x.RdepDate)
                        .ThenBy(x => x.SortDepTim)
                        .ThenBy(x => x.PassLast)
                        .ThenBy(x => x.PassFrst)
                        .ThenBy(x => x.RecKey)
                        .ThenBy(x => x.SeqNo)
                        .ToList();
                }
            }
            if (includePassengerCountByFlight && !sortByName)
            {
                return finalData.OrderBy(x => x.OrgDesc)
                    .ThenBy(x => x.RdepDate)
                    .ThenBy(x => x.SortDepTim)
                    .ThenBy(x => x.FltSort)
                    .ThenBy(x => x.FltNo)
                    .ThenBy(x => x.PassLast)
                    .ThenBy(x => x.PassFrst)
                    .ToList();
            }
            else
            {
                if (sortByName)
                {
                    return finalData.OrderBy(x => x.OrgDesc)
                            .ThenBy(x => x.RdepDate)
                            .ThenBy(x => x.PassLast)
                            .ThenBy(x => x.PassFrst)
                            .ThenBy(x => x.SortDepTim)
                            .ToList();
                }
                else
                {
                    return finalData.OrderBy(x => x.OrgDesc)
                            .ThenBy(x => x.RdepDate)
                            .ThenBy(x => x.SortDepTim)
                            .ThenBy(x => x.PassLast)
                            .ThenBy(x => x.PassFrst)
                            .ToList();
                }
            }
        }
    }
}

using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.ArrivalReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using Domain.Helper;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.Arrival
{
    public class ArrivalDataProcessor
    {
        readonly ClientFunctions _clientFunctions;
        public ArrivalDataProcessor(ClientFunctions clientFunctions)
        {
            _clientFunctions = clientFunctions;
        }

        public IList<FinalData> ProcessRawIntoFinal(IList<RawData> rawData, IMasterDataStore store, UserBreaks userBreaks, UserInformation user, bool flightSort, 
            bool sortByName, bool includeAllLegs, bool includePassengerCountByFlight, bool includePageBreakByDate, IClientQueryable clientQueryDb, ReportGlobals globals)
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientQueryDb, globals.Agency);

            var finalData = rawData.Select(x => new FinalData
            {
                Acct = !user.AccountBreak ? "^na^" : x.Acct,
                AcctDesc = !user.AccountBreak ? "^na^" : _clientFunctions.LookupCname(getAllMasterAccountsQuery, x.Acct, globals),
                Break1 = !userBreaks.UserBreak1 ? "^na^" : ((string.IsNullOrEmpty(x.Break1.Trim()) ? "NONE".PadRight(30) : x.Break1.Trim())),
                Break2 = !userBreaks.UserBreak2 ? "^na^" : ((string.IsNullOrEmpty(x.Break2.Trim()) ? "NONE".PadRight(30) : x.Break2.Trim())),
                Break3 = !userBreaks.UserBreak3 ? "^na^" : ((string.IsNullOrEmpty(x.Break3.Trim()) ? "NONE".PadRight(16) : x.Break3.Trim())),
                RecKey = x.RecKey,
                Destinat = x.Destinat,
                Destdesc = AportLookup.LookupAport(new MasterDataStore(), x.Destinat, x.Mode, globals.Agency),
                Invoice = x.Invoice,
                Recloc = x.Recloc,
                PlusMin = x.PlusMin,
                RArrDate = x.RArrDate ?? DateTime.MinValue,
                PassLast = x.Passlast,
                PassFrst = x.Passfrst,
                Airline = x.Airline,
                AlineDesc = LookupFunctions.LookupAline(store, x.Airline, x.Mode),
                Pseudocity = x.Pseudocity,
                FltNo = x.fltno,
                Origin = x.Origin,
                OrgDesc = AportLookup.LookupAport(new MasterDataStore(), x.Origin, x.Mode, globals.Agency),
                ArrTime = string.IsNullOrEmpty(x.ArrTime.Trim()) ? "" : SharedProcedures.ConvertTime(x.ArrTime),
                DepTime = SharedProcedures.ConvertTime(x.DepTime),
                FltSort = flightSort ?  x.fltno : "X",
                SortArrTim = x.ArrTime.Trim().PadLeft(5, '0'),
                Seg_Cntr = x.Seg_Cntr,
                SegNum = x.SegNum,
                TxtArrDate = x.RArrDate.GetValueOrDefault().ToShortDateString(),
                SeqNo = x.SeqNo,
                Mode = x.Mode
            }).ToList();

            finalData.ForEach(x => x.CrysPgBrk = includePageBreakByDate ? x.Destdesc + x.RArrDate : "1");

            if (includePassengerCountByFlight && !includeAllLegs && !sortByName)
            {
                return finalData.OrderBy(x => x.Destdesc)
                    .ThenBy(x => x.RArrDate)
                    .ThenBy(x => x.SortArrTim)
                    .ThenBy(x => x.FltSort)
                    .ThenBy(x => x.PassLast)
                    .ThenBy(x => x.PassFrst)
                    .ToList();
            }
            else
            {
                if (sortByName)
                {
                    return finalData.OrderBy(x => x.Destdesc)
                    .ThenBy(x => x.RArrDate)
                    .ThenBy(x => x.PassLast)
                    .ThenBy(x => x.PassFrst)
                    .ThenBy(x => x.SortArrTim)
                    .ToList();
                }
                else
                {
                    return finalData.OrderBy(x => x.Destdesc)
                   .ThenBy(x => x.RArrDate)
                   .ThenBy(x => x.SortArrTim)
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
                               Acct = final.Acct,
                               Break1 = final.Break1,
                               Break2 = final.Break2,
                               Break3 = final.Break3,
                               RecKey = final.RecKey,
                               FinalDest = final == null ? "" : final.Destinat,
                               FinalDesc = final == null ? "" : final.Destdesc,
                               Pseudocity = final == null ? "" : final.Pseudocity,
                               Recloc = final == null ? "" : final.Recloc,
                               RArrDate = final == null ? DateTime.MinValue : final.RArrDate,
                               PassLast = final == null ? "" : final.PassLast,
                               PassFrst = final == null ? "" : final.PassFrst,
                               SortArrTim = final == null ? string.Empty : final.SortArrTim,
                               Airline = leg.Airline,
                               AlineDesc = LookupFunctions.LookupAline(store, leg.Airline, leg.Mode),
                               FltNo = leg.fltno,
                               DepTime = SharedProcedures.ConvertTime(leg.DepTime),
                               Origin = leg.Origin,
                               OrgDesc = AportLookup.LookupAport(store, leg.Origin, leg.Mode, globals.Agency),
                               Destinat = leg.Destinat,
                               Destdesc = AportLookup.LookupAport(store, leg.Destinat, leg.Mode, globals.Agency),
                               ArrTime = SharedProcedures.ConvertTime(leg.ArrTime),
                               TxtArrDate = leg.RArrDate.GetValueOrDefault().ToShortDateString()
                           };

            if (sortByName)
            {
                return newFinal.OrderBy(x => x.FinalDesc)
                    .ThenBy(x => x.RArrDate)
                    .ThenBy(x => x.PassLast)
                    .ThenBy(x => x.PassFrst)
                    .ThenBy(x => x.RecKey)
                    .ThenBy(x => x.SortArrTim)
                    .ToList();
            }
            else
            {
                return newFinal.OrderBy(x => x.FinalDesc)
                    .ThenBy(x => x.RArrDate)
                    .ThenBy(x => x.SortArrTim)
                    .ThenBy(x => x.PassLast)
                    .ThenBy(x => x.PassFrst)
                    .ThenBy(x => x.RecKey)
                    .ThenBy(x => x.SeqNo)
                    .ToList();
            }
        }
    }
}

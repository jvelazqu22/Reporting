using System;

using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;

using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.SameCityReport;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.SameCity
{
    public class SameCityDataProcessor
    {
        public IList<FinalData> MapRawToFinalData(IList<RawData> rawData, IMasterDataStore store, ReportGlobals globals)
        {
            return rawData.Select(s => new FinalData
            {
                Recloc = s.Recloc,
                Invoice = s.Invoice,
                Ticket = s.Ticket,
                Plusmin = s.Plusmin,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Destinat = s.Destinat,
                Origin = s.Origin,
                Orgcity = AportLookup.LookupAport(store, s.Origin, s.Mode, globals.Agency),
                Destcity = AportLookup.LookupAport(store, s.Destinat, s.Mode, globals.Agency),
                Airline = s.Airline,
                Rarrdate = s.RArrDate ?? DateTime.MinValue,
                Sorttime = s.RArrDate ?? DateTime.MinValue + SharedProcedures.ConvertTimeToTimeSpan(s.Arrtime),
                Arrtime = SharedProcedures.ConvertTime(s.Arrtime),
                Fltno = s.fltno,
                Class = s.Class,
                Carrier = LookupFunctions.LookupAline(store, s.Airline),

            })
            .OrderBy(s => s.Destinat)
            .ThenBy(s => s.Rarrdate)
            .ThenBy(s => s.Destcity)
            .ThenBy(s => s.Passlast)
            .ThenBy(s => s.Passfrst)
            .ToList();
        }

        public IList<FinalData> RemoveMatchingCreditAndInvoiceRecords(IList<FinalData> finalData)
        {
            var credits = finalData.Where(s => s.Plusmin == -1).Select(s => s.Invoice.Trim()).Distinct().ToList();
            finalData.ToList().RemoveAll(s => credits.Contains(s.Invoice.Trim()));

            return finalData;
        }

        public IList<FinalData> FilterToCitiesWithMoreThanOneTraveler(IList<FinalData> finalData, int numberOfTravelers)
        {
            var tempData = finalData.GroupBy(s => new { s.Rarrdate, s.Destinat },
                (g, r) => new { g.Rarrdate, g.Destinat, Seg_Cntr = r.Count() })
                .Where(t => t.Seg_Cntr >= numberOfTravelers).ToList();

            return (finalData.Join(tempData, s => new { s.Rarrdate, s.Destinat },
                t => new { t.Rarrdate, t.Destinat }, (s, t) => new { s, t })
                .OrderBy(@t1 => @t1.s.Destcity)
                .ThenBy(@t1 => @t1.s.Rarrdate)
                .ThenBy(@t1 => @t1.s.Sorttime)
                .ThenBy(@t1 => @t1.s.Passlast)
                .ThenBy(@t1 => @t1.s.Passfrst)
                .Select(@t1 => new FinalData
                                   {
                                       Recloc = @t1.s.Recloc,
                                       Invoice = @t1.s.Invoice,
                                       Ticket = @t1.s.Ticket,
                                       Passlast = @t1.s.Passlast,
                                       Passfrst = @t1.s.Passfrst,
                                       Destinat = @t1.s.Destinat,
                                       Origin = @t1.s.Origin,
                                       Airline = @t1.s.Airline,
                                       Rarrdate = @t1.s.Rarrdate,
                                       Arrtime = @t1.s.Arrtime,
                                       Sorttime = @t1.s.Sorttime,
                                       Fltno = @t1.s.Fltno,
                                       Class = @t1.s.Class,
                                       Orgcity = @t1.s.Orgcity,
                                       Destcity = @t1.s.Destcity,
                                       Carrier = @t1.s.Carrier,
                                       Sgrpcnt = @t1.t.Seg_Cntr
                                   })
                   ).ToList();
        }
    }
}
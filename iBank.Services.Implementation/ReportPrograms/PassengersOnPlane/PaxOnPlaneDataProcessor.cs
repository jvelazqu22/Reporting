using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.PassengersOnPlaneReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.PassengersOnPlane
{
    public class PaxOnPlaneDataProcessor
    {
        private readonly PaxOnPlaneCalculations _calc = new PaxOnPlaneCalculations();

        public IList<FinalData> MapRawToFinalData(IList<RawData> rawData, bool accountBreak, UserBreaks userBreaks, ReportGlobals globals)
        {
            return rawData.Select(s => new FinalData
            {
                Reckey = s.RecKey,
                Recloc = s.Recloc.Trim(),
                Ticket = s.Ticket.Trim(),
                Agentid = s.Agentid.Trim(),
                Passlast = s.Passlast.Trim(),
                Passfrst = s.Passfrst.Trim(),
                Pseudocity = s.Pseudocity.Trim(),
                Bookdate = s.Bookdate ?? DateTime.MinValue,
                Trantype = s.Trantype.Trim(),
                Acct = !accountBreak ? Constants.NotApplicable : s.Acct,
                Break1 = !userBreaks.UserBreak1 ? Constants.NotApplicable : string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1.Trim(),
                Break2 = !userBreaks.UserBreak2 ? Constants.NotApplicable : string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2.Trim(),
                Break3 = !userBreaks.UserBreak3 ? Constants.NotApplicable : string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(16) : s.Break3.Trim(),
                Airline = s.Airline.Trim(),
                Depdate = s.Depdate ?? DateTime.MinValue,
                Fltno = s.Fltno.Trim(),
                Origin = s.Origin.Trim(),
                Destinat = s.Destinat.Trim(),
                Mode = s.Mode.Trim(),
                Rdepdate = s.RDepDate ?? DateTime.MinValue,
                Deptime = SharedProcedures.ConvertTime(s.Deptime),
                Arrtime = SharedProcedures.ConvertTime(s.Arrtime),
                ClassCode = s.ClassCode.Trim(),
                BreakCombo = _calc.GetBreakCombo(s, userBreaks, globals.IsParmValueOn(WhereCriteria.CBPRINTBRKINFOINBODY))
            }).ToList();
        }

        public IList<RawData> FilterForDomesticInternational(IList<RawData> rawData, int domesticInternationalValue)
        {
            switch (domesticInternationalValue)
            {
                case 2: // Domestic
                    rawData = rawData.Where(s => (s.DitCode == "D" || string.IsNullOrEmpty(s.DitCode)) && s.Domintl == "D").ToList();
                    break;
                case 3: // International
                    rawData = rawData.Where(s => (s.DitCode == "I" || string.IsNullOrEmpty(s.DitCode)) && s.Domintl == "I").ToList();
                    break;
                case 4: // Transborder
                    rawData = rawData.Where(s => (s.DitCode == "T" || string.IsNullOrEmpty(s.DitCode)) && s.Domintl == "T").ToList();
                    break;
                case 5: // Exclude Domestic
                    rawData = rawData.Where(s => (s.DitCode != "D" && !string.IsNullOrEmpty(s.DitCode)) || (string.IsNullOrEmpty(s.DitCode) && s.Domintl != "D")).ToList();
                    break;
                case 6: // Exclude International
                    rawData = rawData.Where(s => (s.DitCode != "I" && !string.IsNullOrEmpty(s.DitCode)) || (string.IsNullOrEmpty(s.DitCode) && s.Domintl != "I")).ToList();
                    break;
                case 7: // Exclude Transborder
                    rawData = rawData.Where(s => (s.DitCode != "T" && !string.IsNullOrEmpty(s.DitCode)) || (string.IsNullOrEmpty(s.DitCode) && s.Domintl != "T")).ToList();
                    break;
            }

            return rawData;
        }


        public IList<GroupedData> GroupFinalData(IList<FinalData> finalData, int numberOfPassengers, ReportGlobals globals)
        {
            return finalData.GroupBy(s => new { s.Acct, s.Break1, s.Break2, s.Break3, s.Airline, s.Rdepdate, s.Fltno, s.Origin, s.Destinat, s.Mode },
                (key, s) => new GroupedData
                {
                    Acct = key.Acct,
                    Break1 = key.Break1,
                    Break2 = key.Break2,
                    Break3 = key.Break3,
                    Airline = key.Airline,
                    Fltno = key.Fltno,
                    Origin = key.Origin,
                    Destinat = key.Destinat,
                    Mode = key.Mode,
                    Rdepdate = key.Rdepdate,
                    Orgdesc = AportLookup.LookupAport(new MasterDataStore(), key.Origin, key.Mode, globals.Agency),
                    Destdesc = AportLookup.LookupAport(new MasterDataStore(), key.Destinat, key.Mode, globals.Agency),
                    Pax = s
                })  
                .Where(s => s.Pax.Select(x => new { FirstName = x.Passfrst, LastName = x.Passlast }).Distinct().Count() >= numberOfPassengers)
                .ToList();
        }

        public void SetDataRange(IList<GroupedData> groupedDepart, DateTime? beginDate, DateTime? endDate)
        {
            foreach (var depart in groupedDepart)
            {
                if (depart.Rdepdate< beginDate || depart.Rdepdate > endDate ) depart.InRange = false;
                
                var pax = depart.Pax.Select(x => x.Rdepdate).Where(y => y.CompareTo(beginDate) >= 0  );
                if (!pax.Any()) depart.InRange = false;
            }            
        }


        public IList<FinalData> MapGroupedDataToFinalData(IList<GroupedData> groupedData, ClientFunctions clientFunctions, IMasterDataStore store, IClientDataStore clientStore, string agency, ReportGlobals globals)
        {
            var finalData = new List<FinalData>();
            foreach (var g in groupedData)
            {
                foreach (var p in g.Pax)
                {
                    if (g.InRange)
                    {
                        finalData.Add(new FinalData
                        {
                            Reckey = p.Reckey,
                            Recloc = p.Recloc,
                            Acct = p.Acct,
                            Acctdesc = clientFunctions.LookupCname(new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, agency), p.Acct, globals),
                            Break1 = g.Break1,
                            Break2 = g.Break2,
                            Break3 = g.Break3,
                            Passlast = p.Passlast,
                            Passfrst = p.Passfrst,
                            Trantype = p.Trantype,
                            Pseudocity = p.Pseudocity,
                            Origin = p.Origin,
                            Destinat = p.Destinat,
                            Orgdesc = g.Orgdesc,
                            Destdesc = g.Destdesc,
                            Airline = p.Airline,
                            Alinedesc = LookupFunctions.LookupAline(store, p.Airline, p.Mode),
                            Depdate = p.Depdate,
                            Deptime = p.Deptime,
                            Arrtime = p.Arrtime,
                            Fltno = p.Fltno,
                            ClassCode = p.ClassCode,
                            Ticket = p.Ticket,
                            Agentid = p.Agentid,
                            Rdepdate = p.Rdepdate,
                            Bookdate = p.Bookdate,
                            BreakCombo = p.BreakCombo
                        });
                    }
                }
            }

            return finalData;
        }

        public IList<FinalData> OrderFinalData(IList<FinalData> finalData, bool isPrintBreakInfoInBodyOn)
        {
            if (isPrintBreakInfoInBodyOn)
            {
                return finalData.OrderBy(s => s.Acctdesc)
                    .ThenBy(s => s.Acct)
                    .ThenBy(s => s.Rdepdate)
                    .ThenBy(s => s.Destinat)
                    .ThenBy(s => s.Orgdesc)
                    .ThenBy(s => s.Airline)
                    .ThenBy(s => s.Deptime)
                    .ThenBy(s => s.Fltno)
                    .ToList();
            }

            return finalData.OrderBy(s => s.Acctdesc)
                .ThenBy(s => s.Acct)
                .ThenBy(s => s.Break1)
                .ThenBy(s => s.Break2)
                .ThenBy(s => s.Break3)
                .ThenBy(s => s.Rdepdate)
                .ThenBy(s => s.Destinat)
                .ThenBy(s => s.Orgdesc)
                .ThenBy(s => s.Airline)
                .ThenBy(s => s.Deptime)
                .ThenBy(s => s.Fltno)
                .ToList();
        }
    }
}

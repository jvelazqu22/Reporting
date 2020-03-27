using System.Collections.Generic;
using System.Linq;

using CODE.Framework.Core.Utilities;

using Domain.Helper;
using Domain.Models.ReportPrograms.AirFareSavingsReport;
using Domain.Orm.iBankClientQueries;

using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.AirFareSavings
{
    public static class AirFareSavingsExtensions
    {
        private static readonly AirFareSavingsCalculations _afsCalculations = new AirFareSavingsCalculations();
        public static List<FinalData> ToFinalData(this List<RawData> rawData, IMasterDataStore masterStore, IClientDataStore clientStore, 
            ReportGlobals globals, ReportOptions options, ClientFunctions clientFunctions)
        {
            var finalDataList = new List<FinalData>();
            foreach (var s in rawData)
            {
                var finalData = new FinalData();

                finalData.Reckey = s.RecKey;
                finalData.Seqno = s.SeqNo;
                finalData.Ticket = s.Ticket;
                finalData.Homectry = options.UsePageBreakHomeCountry ? LookupFunctions.LookupHomeCountryName(s.SourceAbbr, globals, masterStore) : Constants.NotApplicable;
                finalData.Acct = options.UseAccountBreak ? s.Acct : Constants.NotApplicable;
                finalData.Acctdesc = globals.User.AccountBreak
                                         ? clientFunctions.LookupCname(new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, globals.Agency), s.Acct, globals)
                                         : Constants.NotApplicable;
                finalData.Break1 = GetBreakText(options.UserBreaks.UserBreak1, s.Break1, 30); 
                finalData.Break2 = GetBreakText(options.UserBreaks.UserBreak2, s.Break2, 30); 
                finalData.Break3 = GetBreakText(options.UserBreaks.UserBreak3, s.Break3, 16); 
                finalData.Invdate = s.InvDate.ToDateTimeSafe();
                finalData.Passlast = s.Passlast;
                finalData.Passfrst = s.Passfrst;
                finalData.Origin = s.Origin;
                finalData.Orgdesc = AportLookup.LookupAport(masterStore, s.Origin, s.Mode, s.Airline);
                finalData.Destinat = s.Destinat;
                finalData.Destdesc = AportLookup.LookupAport(masterStore, s.Destinat, s.Mode, s.Airline);
                finalData.Rdepdate = s.RDepDate.ToDateTimeSafe();
                finalData.Connect = s.Connect;
                finalData.Airline = s.Airline;
                finalData.Class = s.Class;
                finalData.Airchg = options.UseBaseFare ? s.Basefare : s.Airchg;
                finalData.Plusmin = s.Plusmin;
                
                finalData.Offrdchg = _afsCalculations.GetOffRdChange(options.UseDerivedSavingsCode, s.Offrdchg, s.Airchg);
                finalData.Stndchg = _afsCalculations.GetStndCharge(options.UseDerivedSavingsCode, s.Stndchg, finalData.Airchg);

                finalData.Negosvngs = _afsCalculations.GetNegotiatedSavings(s.Airchg, s.Offrdchg, s.Plusmin, s.Basefare, options.UseBaseFare);

                var lostAmount = _afsCalculations.GetLostAmount(s.Airchg, finalData.Offrdchg, s.Plusmin, s.Basefare, options.UseBaseFare);
                finalData.Lostamt = lostAmount;
                finalData.Reascode = s.Reascode;
                //finalData.Reascode = finalData.Lostamt > 0 ? s.Reascode : string.Empty;
                // 4936 Enhancement 00165066 -  Fare Savings - Air - Populate Code for $0 Savings and no Losses/Negotiated Savings Records

                finalData.Savings = _afsCalculations.GetSavings(finalData.Stndchg, s.Airchg, s.Basefare, options.UseBaseFare);
                finalData.Savingcode = _afsCalculations.GetSavingsCode(s.Savingcode, options.UseDerivedSavingsCode, s.Reascode,
                    _afsCalculations.GetLostAmount(s.Airchg, s.Offrdchg, s.Plusmin, s.Basefare, options.UseBaseFare),
                    _afsCalculations.GetSavings(s.Stndchg, s.Airchg, s.Basefare, options.UseBaseFare));

                finalData.Lostpct = _afsCalculations.GetLossPercentage(s.Airchg, finalData.Offrdchg, s.Basefare, options.UseBaseFare);
                finalData.Origacct = s.Acct;
                finalData.Sourceabbr = s.SourceAbbr;
                finalData.Invdatetxt = globals.TranslateDDMMMDate(s.InvDate);
                finalData.Rdepdattxt = globals.TranslateDDMMMDate(s.RDepDate);

                finalDataList.Add(finalData);
            }

            return finalDataList;
        }

        private static string GetBreakText(bool useBreak, string text, int pad)
        {
            return useBreak
                       ? string.IsNullOrEmpty(text.Trim())
                             ? "NONE".PadRight(pad)
                             : text
                       : Constants.NotApplicable;
        }

        public static List<FinalData> Sort(this List<FinalData> finalDataList, bool deriveSavingsCode)
        {
            if (deriveSavingsCode)
            {
                finalDataList = finalDataList
                    .OrderBy(x => x.Acct.Trim())
                    .ThenBy(x => x.Acctdesc.Trim())
                    .ThenBy(x => x.Break1.Trim())
                    .ThenBy(x => x.Break2.Trim())
                    .ThenBy(x => x.Break3.Trim())
                    .ThenBy(x => x.Passlast.Trim())
                    .ThenBy(x => x.Passfrst.Trim())
                    .ThenBy(x => x.Reckey)
                    .ThenBy(x => x.Invdate)
                    .ThenBy(x => x.Rdepdate)
                    .ThenBy(x => x.Seqno)
                    .ToList();
            }
            else
            {
                finalDataList = finalDataList
                    .OrderBy(x => x.Acct.Trim())
                    .ThenBy(x => x.Acctdesc.Trim())
                    .ThenBy(x => x.Break1.Trim())
                    .ThenBy(x => x.Break2.Trim())
                    .ThenBy(x => x.Break3.Trim())
                    .ThenBy(x => x.Passlast.Trim())
                    .ThenBy(x => x.Passfrst.Trim())
                    .ThenBy(x => x.Invdate)
                    .ThenBy(x => x.Rdepdate)
                    .ThenBy(x => x.Origin)
                    .ToList();
            }

            return finalDataList;
        }
    }
}

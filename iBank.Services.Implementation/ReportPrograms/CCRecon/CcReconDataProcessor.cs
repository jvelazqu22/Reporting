using Domain.Helper;
using Domain.Models.ReportPrograms.CCReconReport;
using Domain.Orm.Classes;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.CCRecon
{
    public class CcReconDataProcessor
    {
        private readonly CcReconCalculations _calc = new CcReconCalculations();

        public IList<FinalData> MapRawToFinalData(IList<RawData> rawData, Dictionary<int, string> routeItineraries, bool useAccountBreaks,
            ClientFunctions clientFunctions, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, ReportGlobals globals, UserBreaks userBreaks, bool displayBreaks, IMasterDataStore store)
        {
            var finalData = new List<FinalData>();

            foreach (var row in rawData)
            {
                var airlineLookup = LookupFunctions.LookupAirline(store, row.Valcarr.Trim(), row.Mode.Trim());

                string itinerary;
                if (!routeItineraries.TryGetValue(row.RecKey, out itinerary))
                {
                    itinerary = new string(' ', 240);
                }

                if (!finalData.Exists(s => s.RecKey == row.RecKey))
                {
                    var tempFinalData = new FinalData();
                    tempFinalData.AirCurrTyp = row.AirCurrTyp;
                    tempFinalData.RecKey = row.RecKey;
                    tempFinalData.Acct = row.Acct;
                    tempFinalData.Acctdesc = !useAccountBreaks ? Constants.NotApplicable : clientFunctions.LookupCname(getAllMasterAccountsQuery, row.Acct, globals);
                    //uncommented for US 7312 - Defect 00185144 ..Only display breaks per User Settings
                    tempFinalData.Break1 = !userBreaks.UserBreak1 ? Constants.NotApplicable : string.IsNullOrEmpty(row.Break1.Trim()) ? "NONE".PadRight(30) : row.Break1;
                    tempFinalData.Break2 = !userBreaks.UserBreak2 ? Constants.NotApplicable : string.IsNullOrEmpty(row.Break2.Trim()) ? "NONE".PadRight(30) : row.Break2;
                    tempFinalData.Break3 = !userBreaks.UserBreak3 ? Constants.NotApplicable : string.IsNullOrEmpty(row.Break3.Trim()) ? "NONE".PadRight(30) : row.Break3;
                    tempFinalData.BreaksCol = GetBreaksCol(displayBreaks, tempFinalData.Break1, tempFinalData.Break2, tempFinalData.Break3);
                    tempFinalData.RecLoc = row.RecLoc;
                    tempFinalData.Cardnum = row.Cardnum;
                    tempFinalData.Valcarr = row.Valcarr;
                    tempFinalData.Airlndesc = airlineLookup.Item2;
                    tempFinalData.Airlinenbr = airlineLookup.Item1.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0');
                    tempFinalData.Ticket = row.Ticket;
                    tempFinalData.Trantype = row.Trantype;
                    tempFinalData.Trandate = row.Trandate ?? DateTime.MinValue;
                    tempFinalData.Invoice = row.Invoice;
                    tempFinalData.Passlast = row.Passlast;
                    tempFinalData.Passfrst = row.Passfrst;
                    tempFinalData.Airchg = row.Airchg ?? 0;
                    tempFinalData.Descript = itinerary;
                    tempFinalData.Depdate = row.Depdate ?? DateTime.MinValue;
                    tempFinalData.Arrdate = row.Arrdate ?? DateTime.MinValue;

                    finalData.Add(tempFinalData);
                }
            }

            return finalData;
        }

        public string GetBreaksCol(bool displayBreaks, string break1, string break2, string break3)
        {
            string retValue = string.Empty;

            if (string.IsNullOrWhiteSpace(break1)) break1 = "NONE";
            if (string.IsNullOrWhiteSpace(break2)) break2 = "NONE";
            if (string.IsNullOrWhiteSpace(break3)) break3 = "NONE";

            if (displayBreaks)
            {
                if (break1 != Constants.NotApplicable) {
                    retValue += break1.Trim();
                }
                if (break2 != Constants.NotApplicable)
                {
                    if (!string.IsNullOrEmpty(retValue)) retValue += "/";
                    retValue += break2.Trim();
                }
                if (break3 != Constants.NotApplicable)
                {
                    if (!string.IsNullOrEmpty(retValue)) retValue += "/";
                    retValue += break3.Trim();
                }
            }

            return retValue;
        }

        public IList<FinalData> MapSvcFeeDataToFinalData(IList<FinalData> finalData, IList<ServiceFee> svcFees, UserBreaks userBreaks, bool displayBreaks, 
            ReportGlobals globals, bool useAccountBreak, ClientFunctions clientFunctions, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery)
        {
            var mappedData = new List<FinalData>();

            foreach (var row in svcFees)
            {
                var break1 = !userBreaks.UserBreak1 ? Constants.NotApplicable : string.IsNullOrEmpty(row.Break1.Trim()) ? "NONE".PadRight(30) : row.Break1;
                var break2 = !userBreaks.UserBreak2 ? Constants.NotApplicable : string.IsNullOrEmpty(row.Break2.Trim()) ? "NONE".PadRight(30) : row.Break2;
                var break3 = !userBreaks.UserBreak3 ? Constants.NotApplicable : string.IsNullOrEmpty(row.Break3.Trim()) ? "NONE".PadRight(30) : row.Break3;
                var breakcol = GetBreaksCol(displayBreaks, break1, break2, break3);

                var tempFinalData = new FinalData();
                tempFinalData.RecKey = row.RecKey;
                tempFinalData.AirCurrTyp = row.FeeCurrTyp;
                tempFinalData.Acct = row.Acct;
                tempFinalData.Acctdesc = !useAccountBreak ? Constants.NotApplicable : clientFunctions.LookupCname(getAllMasterAccountsQuery, row.Acct, globals);
                tempFinalData.Break1 = break1;
                tempFinalData.Break2 = break2;
                tempFinalData.Break3 = break3;
                tempFinalData.BreaksCol = breakcol;
                tempFinalData.RecLoc = row.RecLoc;
                tempFinalData.Cardnum = row.SfCardNum;
                tempFinalData.Airlndesc = "Service Fee".PadRight(30);
                tempFinalData.Airlinenbr = "890";
                tempFinalData.Ticket = row.Mco.PadRight(20);
                tempFinalData.Trantype = row.Trantype;
                tempFinalData.Trandate = row.Trandate;
                tempFinalData.Invoice = row.Invoice;
                tempFinalData.Passlast = row.Passlast;
                tempFinalData.Passfrst = row.Passfrst;
                tempFinalData.Airchg = row.SvcFee;
                tempFinalData.Descript = row.Descript;
                tempFinalData.Depdate = DateTime.MinValue;
                tempFinalData.Arrdate = DateTime.MinValue;
                mappedData.Add(tempFinalData);
             
            }

            return mappedData;
        }

        public IList<FinalData> MapCreditCardDataToFinalData(IList<FinalData> finalData, IList<CcData> creditCardData, List<int> udidNumber, IMasterDataStore store)
        {
            foreach (var row in creditCardData)
            {
                var finalDataRow = finalData.FirstOrDefault(s => (s.Airlinenbr + s.Ticket.Trim()) == row.Ticket.Trim());

                if (finalDataRow != null)
                {
                    finalDataRow.Cctransamt = row.TransAmt;
                    finalDataRow.Ccrefnbr = row.RefNbr;
                }
                else
                {
                    finalData.Add(new FinalData
                    {
                        Grpcode = "B",
                        RecKey = row.RecKey,
                        Acct = Constants.NotApplicable,
                        Acctdesc = Constants.NotApplicable,
                        Break1 = Constants.NotApplicable,
                        Break2 = Constants.NotApplicable,
                        Break3 = Constants.NotApplicable,
                        Breakfld = "Z",
                        Cardnum = row.CardNum,
                        Airlinenbr = row.Ticket.Left(3),
                        Ticket = row.Ticket.Substring(3),
                        Airlndesc = LookupFunctions.LookupAirlineByNumber(store, row.Ticket.Left(3)),
                        Trantype = row.Trantype,
                        Trandate = row.TranDate,
                        Passname = row.PassName,
                        Descript = row.Descript,
                        Cctransamt = row.TransAmt,
                        Ccrefnbr = row.RefNbr,
                        Udidnbr1 = udidNumber[0],
                        Udidnbr2 = udidNumber[1],
                        Udidnbr3 = udidNumber[2],
                        Udidnbr4 = udidNumber[3],
                        Udidnbr5 = udidNumber[4],
                        Udidnbr6 = udidNumber[5],
                        Udidnbr7 = udidNumber[6],
                        Udidnbr8 = udidNumber[7],
                        Udidnbr9 = udidNumber[8],
                        Udidnbr10 = udidNumber[9],
                        Depdate = finalData.FirstOrDefault(x => x.RecKey == row.RecKey).Depdate,
                        Arrdate = finalData.FirstOrDefault(x => x.RecKey == row.RecKey).Arrdate
                });
                }
            }

            return finalData;
        }

        public IList<CcData> SortCreditCardData(IEnumerable<CcData> source, string sortBy)
        {
            switch (sortBy)
            {
                case "2":
                case "8":
                case "9":
                    return source.OrderBy(s => s.CardNum)
                        .ThenBy(s => s.PassName)
                        .ThenBy(s => s.Ticket)
                        .ToList();
                case "3":
                case "5":
                case "6":
                    return source.OrderBy(s => s.CardNum)
                        .ThenBy(s => s.TranDate)
                        .ThenBy(s => s.Ticket)
                        .ToList();
                case "7":
                case "11":
                    return source.OrderBy(s => s.CardNum)
                        .ThenBy(s => s.TranDate)
                        .ThenBy(s => s.PassName)
                        .ThenBy(s => s.Ticket)
                        .ToList();
                case "10":
                    return source.OrderBy(s => s.CardNum)
                       .ThenBy(s => s.PassName)
                       .ThenBy(s => s.TranDate)
                       .ThenBy(s => s.Ticket)
                       .ToList();

                default:
                    return source.OrderBy(s => s.CardNum)
                       .ThenBy(s => s.Ticket)
                       .ToList();
            }
        }

        public List<CcData> MapItineraryToCreditCardData(List<CcData> ccData)
        {
            var routeItineraries = SharedProcedures.GetRouteItinerary(ccData, true);
            ccData.ForEach(s =>
            {
                string itinerary;
                if (!routeItineraries.TryGetValue(s.RecKey, out itinerary))
                {
                    itinerary = new string(' ', 240);
                }
                s.Descript = itinerary;
            });

            return ccData;
        }

        public IList<FinalData> RemapFinalData(IList<FinalData> finalData, bool useAcctBrks, IList<Udid> udids, List<int> udidNumber, string sortBy)
        {
            var ccrefnbrstring = new string(' ', 24);
            return finalData.Select(s => new FinalData
            {
                Grpcode = "A",
                Acct = useAcctBrks ? s.Acct : Constants.NotApplicable,
                Acctdesc = s.Acctdesc.PadRight(60),
                AirCurrTyp = s.AirCurrTyp,
                Break1 = s.Break1.Trim(),
                Break2 = s.Break2.Trim(),
                Break3 = s.Break3.Trim(),
                Breakfld = _calc.GetBreakField(sortBy, s),
                BreaksCol = s.BreaksCol,
                Cardnum = s.Cardnum,
                Airlinenbr = s.Airlinenbr,
                Ticket = s.Ticket,
                Airlndesc = s.Airlndesc,
                Trantype = s.Trantype,
                Invoice = s.Invoice,
                Trandate = s.Trandate,
                Passname = string.Format("{0}/{1}", s.Passlast.Trim(), s.Passfrst.Trim()),
                Descript = s.Descript,
                Airchg = s.Airchg,
                Cctransamt = 0,
                Ccrefnbr = ccrefnbrstring,
                Udidnbr1 = udidNumber[0],
                Udidnbr2 = udidNumber[1],
                Udidnbr3 = udidNumber[2],
                Udidnbr4 = udidNumber[3],
                Udidnbr5 = udidNumber[4],
                Udidnbr6 = udidNumber[5],
                Udidnbr7 = udidNumber[6],
                Udidnbr8 = udidNumber[7],
                Udidnbr9 = udidNumber[8],
                Udidnbr10 = udidNumber[9],
                Udidtext1 = _calc.GetUdids(s.RecKey, udidNumber[0], udids),
                Udidtext2 = _calc.GetUdids(s.RecKey, udidNumber[1], udids),
                Udidtext3 = _calc.GetUdids(s.RecKey, udidNumber[2], udids),
                Udidtext4 = _calc.GetUdids(s.RecKey, udidNumber[3], udids),
                Udidtext5 = _calc.GetUdids(s.RecKey, udidNumber[4], udids),
                Udidtext6 = _calc.GetUdids(s.RecKey, udidNumber[5], udids),
                Udidtext7 = _calc.GetUdids(s.RecKey, udidNumber[6], udids),
                Udidtext8 = _calc.GetUdids(s.RecKey, udidNumber[7], udids),
                Udidtext9 = _calc.GetUdids(s.RecKey, udidNumber[8], udids),
                Udidtext10 = _calc.GetUdids(s.RecKey, udidNumber[9], udids),
                Depdate = s.Depdate,
                Arrdate = s.Arrdate
            }).ToList();
        }
    }
}
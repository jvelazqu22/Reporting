using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Domain.Models.ReportPrograms.PTATravelersDetailReport;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.PTATravelersDetail
{
    public class RowBuilderTripData
    {
        public void SetTripDta(int recKey, int travAuthNo, ref FinalData firstRow, ReportGlobals globals, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, 
            IClientDataStore clientStore, IMasterDataStore masterStore, ClientFunctions clientFunctions, ref List<RawData> rawDataList, 
            bool includeLostSavings, ref string lostSvgsMsg, InternationalSettingsInformation intl, ref List<FinalData> newRows)
        {
            var counter = 0;
            var curRow = firstRow;
            foreach (var rawRow in rawDataList.Where(s => s.RecKey == recKey && s.TravAuthNo == travAuthNo).OrderBy(s => s.RDepDate))
            {
                counter++;
                if (counter == 1)
                {
                    firstRow.Tottripchg = rawRow.AirChg;
                    curRow.Tottripchg = rawRow.AirChg;
                    curRow.Airchg = rawRow.AirChg;
                    curRow.Exchange = rawRow.Exchange;
                    curRow.Penaltyamt = rawRow.PenaltyAmt;
                    curRow.Airfarebkd = rawRow.AirChg - rawRow.PenaltyAmt;
                    curRow.Tktorgamt = rawRow.TktOrgAmt;
                    curRow.Addcollamt = rawRow.AddCollAmt;

                    curRow.Airlowfare = rawRow.OffrdChg == 0
                        ? rawRow.AirChg
                        : rawRow.OffrdChg;

                    if (includeLostSavings && curRow.Airlowfare < rawRow.AirChg)
                    {
                        curRow.Airlostsvg = rawRow.AirChg - curRow.Airlowfare;
                        curRow.Airexcreas = clientFunctions.LookupReason(getAllMasterAccountsQuery, rawRow.ReasCode, rawRow.Acct, clientStore, globals, masterStore.MastersQueryDb, true);
                    }
                    if (curRow.Airlostsvg > 0)
                    {
                        var pt1 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsgPt1", "Lost Savings - Air:  Lower Fare Offered", globals.LanguageVariables);
                        var pt2 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsgPt2", "Lost Savings = ", globals.LanguageVariables);
                        var reason = LookupFunctions.LookupLanguageTranslation("xReason", "Reason", globals.LanguageVariables);

                        lostSvgsMsg = @"** " + pt1 + " = " + curRow.Airlowfare.CurrencyTransform(intl) + ": " + pt2 + " = " +
                            curRow.Airlostsvg.CurrencyTransform(intl) + "; " + reason + ": " + curRow.Airexcreas + Environment.NewLine;
                    }
                }
                else
                {
                    //if a row exists, use it. Otherwise, create a new one. 
                    if (newRows.Count >= counter)
                    {
                        curRow = newRows[counter - 1];
                    }
                    else
                    {
                        curRow = new FinalData();
                        Mapper.Map(firstRow, curRow);
                        newRows.Add(curRow);
                    }
                }

                curRow.Airline = rawRow.Airline;
                curRow.Rdepdate = rawRow.RDepDate ?? DateTime.MinValue;
                curRow.Origin = rawRow.Origin;
                curRow.Orgdesc = AportLookup.LookupAport(masterStore, rawRow.Origin, rawRow.Mode, globals.Agency);
                curRow.Destinat = rawRow.Destinat;
                curRow.Destdesc = AportLookup.LookupAport(masterStore, rawRow.Destinat, rawRow.Mode, globals.Agency);
                curRow.Legdata = true;
                curRow.Class = rawRow.Class; //TODO: Do we need classcat and classcat description? Not on report. 
            }
        }

        public void SetTripDta1(ref FinalData firstRow, ReportGlobals globals, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery,
            IClientDataStore clientStore, IMasterDataStore masterStore, ClientFunctions clientFunctions, RawData rawRow,
            bool includeLostSavings, ref string lostSvgsMsg, InternationalSettingsInformation intl, ref FinalData curRow)
        {
                firstRow.Tottripchg = rawRow.AirChg;
                curRow.Tottripchg = rawRow.AirChg;
                curRow.Airchg = rawRow.AirChg;
                curRow.Exchange = rawRow.Exchange;
                curRow.Penaltyamt = rawRow.PenaltyAmt;
                curRow.Airfarebkd = rawRow.AirChg - rawRow.PenaltyAmt;
                curRow.Tktorgamt = rawRow.TktOrgAmt;
                curRow.Addcollamt = rawRow.AddCollAmt;

                curRow.Airlowfare = rawRow.OffrdChg == 0
                    ? rawRow.AirChg
                    : rawRow.OffrdChg;

                if (includeLostSavings && curRow.Airlowfare < rawRow.AirChg)
                {
                    curRow.Airlostsvg = rawRow.AirChg - curRow.Airlowfare;
                    curRow.Airexcreas = clientFunctions.LookupReason(getAllMasterAccountsQuery, rawRow.ReasCode, rawRow.Acct, clientStore, globals, masterStore.MastersQueryDb, true);
                }
                if (curRow.Airlostsvg > 0)
                {
                    var pt1 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsgPt1", "Lost Savings - Air:  Lower Fare Offered", globals.LanguageVariables);
                    var pt2 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsgPt2", "Lost Savings = ", globals.LanguageVariables);
                    var reason = LookupFunctions.LookupLanguageTranslation("xReason", "Reason", globals.LanguageVariables);

                    lostSvgsMsg = @"** " + pt1 + " = " + curRow.Airlowfare.CurrencyTransform(intl) + ": " + pt2 + " = " +
                        curRow.Airlostsvg.CurrencyTransform(intl) + "; " + reason + ": " + curRow.Airexcreas + Environment.NewLine;
                }
        }

        public void SetTripDta2(IMasterDataStore masterStore, RawData rawRow, InternationalSettingsInformation intl, ref FinalData curRow, ReportGlobals globals)
        {
            curRow.Airline = rawRow.Airline;
            curRow.Rdepdate = rawRow.RDepDate ?? DateTime.MinValue;
            curRow.Origin = rawRow.Origin;
            curRow.Orgdesc = AportLookup.LookupAport(masterStore, rawRow.Origin, rawRow.Mode, globals.Agency);
            curRow.Destinat = rawRow.Destinat;
            curRow.Destdesc = AportLookup.LookupAport(masterStore, rawRow.Destinat, rawRow.Mode, globals.Agency);
            curRow.Legdata = true;
            curRow.Class = rawRow.Class; //TODO: Do we need classcat and classcat description? Not on report. 
        }

    }
}

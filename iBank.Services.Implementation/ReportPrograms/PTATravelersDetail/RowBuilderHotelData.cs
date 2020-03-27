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
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.PTATravelersDetail
{
    public class RowBuilderHotelData
    {
        public void SetHotelData1(int recKey, ref FinalData firstRow, ReportGlobals globals, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, 
            IClientDataStore clientStore, IMasterDataStore masterStore, ClientFunctions clientFunctions, bool includeLostSavings, int travAuthNo, ref int counter,
            ref string lostSvgsMsg, InternationalSettingsInformation intl, ref List<FinalData> newRows, List<HotelRawData> hotelRawDataList, FinalData curRow)
        {
            foreach (var hotRow in hotelRawDataList.Where(s => s.RecKey == recKey && s.TravAuthNo == travAuthNo))
            {
                counter++;
                if (counter >= 1)
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
                curRow.Hotelnam = hotRow.HotelNam;
                curRow.Hotcity = hotRow.HotCity;
                curRow.Metro = hotRow.Metro;
                curRow.Datein = hotRow.DateIn ?? DateTime.MinValue;
                curRow.Rooms = hotRow.Rooms;
                curRow.Nights = hotRow.Nights;
                curRow.Hotbookrat = hotRow.BookRate;
                curRow.Hotelcost = hotRow.Nights * hotRow.Rooms * hotRow.BookRate;

                curRow.Hotlowrate = hotRow.HExcpRat == 0
                    ? hotRow.BookRate
                    : hotRow.HExcpRat;

                if (includeLostSavings && curRow.Hotlowrate < hotRow.BookRate)
                {
                    curRow.Hotlostsvg = (hotRow.BookRate - curRow.Hotlowrate) * hotRow.Nights * hotRow.Rooms;
                    curRow.Hotexcreas = clientFunctions.LookupReason(getAllMasterAccountsQuery, hotRow.ReasCodH, firstRow.Acct, clientStore, globals, masterStore.MastersQueryDb, true);
                }
                if (curRow.Hotlostsvg > 0)
                {
                    var msg3Pt1 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsg3Pt1", "Lost Savings - Hotel Booking for", globals.LanguageVariables);
                    var msg2Pt2 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsg2Pt2", "Lower Rate Available", globals.LanguageVariables);
                    var msg1Pt2 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsgPt2", "Lost Savings", globals.LanguageVariables);
                    var reason = LookupFunctions.LookupLanguageTranslation("xReason", "Reason", globals.LanguageVariables);

                    lostSvgsMsg += @"** " + msg3Pt1 + " = " + curRow.Datein + ": " + msg2Pt2 + " = " +
                        curRow.Hotlowrate.CurrencyTransform(intl) + "; " + msg1Pt2 + curRow.Hotlostsvg.CurrencyTransform(intl) + "; "
                        + reason + ": " + curRow.Hotexcreas + Environment.NewLine;

                }
                firstRow.Tottripchg += curRow.Hotelcost;
                curRow.Hoteldata = true;
            }
        }

        public void SetHotelData2(int recKey, ref FinalData firstRow, ReportGlobals globals, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery,
            IClientDataStore clientStore, IMasterDataStore masterStore, ClientFunctions clientFunctions, bool includeLostSavings, int travAuthNo, 
            ref string lostSvgsMsg, InternationalSettingsInformation intl, ref List<FinalData> newRows, HotelRawData hotRow, FinalData curRow)
        {
            curRow.Hotelnam = hotRow.HotelNam;
            curRow.Hotcity = hotRow.HotCity;
            curRow.Metro = hotRow.Metro;
            curRow.Datein = hotRow.DateIn ?? DateTime.MinValue;
            curRow.Rooms = hotRow.Rooms;
            curRow.Nights = hotRow.Nights;
            curRow.Hotbookrat = hotRow.BookRate;
            curRow.Hotelcost = hotRow.Nights * hotRow.Rooms * hotRow.BookRate;

            curRow.Hotlowrate = hotRow.HExcpRat == 0
                ? hotRow.BookRate
                : hotRow.HExcpRat;

            if (includeLostSavings && curRow.Hotlowrate < hotRow.BookRate)
            {
                curRow.Hotlostsvg = (hotRow.BookRate - curRow.Hotlowrate) * hotRow.Nights * hotRow.Rooms;
                curRow.Hotexcreas = clientFunctions.LookupReason(getAllMasterAccountsQuery, hotRow.ReasCodH, firstRow.Acct, clientStore, globals, masterStore.MastersQueryDb, true);
            }
            if (curRow.Hotlostsvg > 0)
            {
                var msg3Pt1 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsg3Pt1", "Lost Savings - Hotel Booking for", globals.LanguageVariables);
                var msg2Pt2 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsg2Pt2", "Lower Rate Available", globals.LanguageVariables);
                var msg1Pt2 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsgPt2", "Lost Savings", globals.LanguageVariables);
                var reason = LookupFunctions.LookupLanguageTranslation("xReason", "Reason", globals.LanguageVariables);

                lostSvgsMsg += @"** " + msg3Pt1 + " = " + curRow.Datein + ": " + msg2Pt2 + " = " +
                    curRow.Hotlowrate.CurrencyTransform(intl) + "; " + msg1Pt2 + curRow.Hotlostsvg.CurrencyTransform(intl) + "; "
                    + reason + ": " + curRow.Hotexcreas + Environment.NewLine;

            }
            firstRow.Tottripchg += curRow.Hotelcost;
            curRow.Hoteldata = true;
        }
    }
}

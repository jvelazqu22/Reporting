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
    public class RowBuilderCarData
    {
        public void SetCarDta(int recKey, ref FinalData firstRow, ReportGlobals globals, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, 
            IClientDataStore clientStore, IMasterDataStore masterStore, ClientFunctions clientFunctions, bool includeLostSavings, int travAuthNo, ref int counter,
            ref string lostSvgsMsg, InternationalSettingsInformation intl, ref List<FinalData> newRows, List<CarRawData> carRawDataList, FinalData curRow)
        {
            foreach (var carRow in carRawDataList.Where(s => s.RecKey == recKey && s.TravAuthNo == travAuthNo))
            {
                counter++;
                if (counter > 1)
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
                curRow.Carcompany = carRow.Company;
                curRow.Rentdate = carRow.RentDate ?? DateTime.MinValue;
                curRow.Days = carRow.Days;
                curRow.Carcost = carRow.Days * carRow.ABookRat;
                curRow.Carlowrate = carRow.AExcpRat == 0
                    ? carRow.ABookRat
                    : carRow.AExcpRat;
                if (includeLostSavings && curRow.Carlowrate < carRow.ABookRat)
                {
                    curRow.Carlostsvg = (carRow.ABookRat - curRow.Carlowrate) * carRow.Days;
                    curRow.Carexcreas = clientFunctions.LookupReason(getAllMasterAccountsQuery, carRow.ReasCodA, firstRow.Acct, clientStore, globals, masterStore.MastersQueryDb, true);
                }
                if (curRow.Carlostsvg > 0)
                {
                    var msg2Pt1 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsg2Pt1", "Lost Savings - Car Rental on", globals.LanguageVariables);
                    var msg2Pt2 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsg2Pt2", "Lower Rate Available", globals.LanguageVariables);
                    var msg1Pt2 = LookupFunctions.LookupLanguageTranslation("xLostSvgsMsgPt2", "Lost Savings", globals.LanguageVariables);
                    var reason = LookupFunctions.LookupLanguageTranslation("xReason", "Reason", globals.LanguageVariables);

                    lostSvgsMsg += @"** " + msg2Pt1 + " = " + curRow.Rentdate + ": " + msg2Pt2 + " = " +
                        curRow.Carlowrate.CurrencyTransform(intl) + "; " + msg1Pt2 + curRow.Carlostsvg.CurrencyTransform(intl) + "; "
                        + reason + ": " + curRow.Carexcreas + Environment.NewLine;

                }
                curRow.Cardata = true;
                firstRow.Tottripchg += curRow.Carcost;
            }
        }

    }
}

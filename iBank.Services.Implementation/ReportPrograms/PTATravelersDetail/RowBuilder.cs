using System;
using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities;
using Domain.Helper;
using Domain.Models.ReportPrograms.PTATravelersDetailReport;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.PTATravelersDetail
{
    public class RowBuilder
    {
        public List<RawData> RawDataList = new List<RawData>();
        public List<TripAuthorizerRawData> TripAuthorizerRawDataList { get; set; }
        public List<CarRawData> CarRawDataList { get; set; }
        public List<HotelRawData> HotelRawDataList { get; set; }
        public List<GroupedTripAuthData> GroupedTripAuthData { get; set; }

        public ReportGlobals Globals { get; set; }
        public ClientFunctions ClientFunctions { get; set; }

        public double OffsetHours { get; set; }

        public List<FinalData> BuildRows(IClientDataStore clientStore, IMasterDataStore masterStore)
        {
            var userBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
            var authStatuses = new GetMiscParamListQuery(masterStore.MastersQueryDb, "TRAVAUTHSTAT", Globals.UserLanguage).ExecuteQuery().ToList();
            var notRequired = new List<string> { "A", "D", "C" };
            var includeAuthComm = Globals.IsParmValueOn(WhereCriteria.CBINCLAUTHCOMMS);
            var includeLostSavings = Globals.IsParmValueOn(WhereCriteria.CBINCLLOWFARELOSTSVGS);
            var includeNotifyOnly = Globals.IsParmValueOn(WhereCriteria.CBINCLNOTIFONLY);

            var country = Globals.GetParmValue(WhereCriteria.COUNTRY);
            var intl = new GetSettingsByCountryAndLangCodeQuery(masterStore.MastersQueryDb, country, Globals.UserLanguage).ExecuteQuery();

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, Globals.Agency);
            var accts = RawDataList.Select(s => s.Acct).Distinct().ToList();
            var acctLookups = accts.Select(s => new Tuple<string, string>(s, ClientFunctions.LookupCname(getAllMasterAccountsQuery, s, Globals))).ToList();

            var finalDataList = new List<FinalData>();
            var rowBuilderTripData = new RowBuilderTripData();
            var rowBuilderCarData = new RowBuilderCarData();
            var rowBuilderHotelData = new RowBuilderHotelData();

            var rowCounter = 1;
            foreach (var row in GroupedTripAuthData)
            {
                //Create a set of rows for each "group". Data will be combined from trip, car, and hotel. 
                var newRows = new List<FinalData>();

                var recKey = row.RecKey;
                var travAuthNo = row.TravAuthNo;
                var lostSvgsMsg = string.Empty;
                var authCounter = 0;

                var firstRow = new FinalData();
                newRows.Add(firstRow);

                #region Authorizer data

                new RowBuilderAuthorizerData().SetAuthorizerDta(TripAuthorizerRawDataList, recKey, travAuthNo, includeNotifyOnly,
                    firstRow, row, ref rowCounter, Globals, acctLookups, userBreaks, getAllMasterAccountsQuery,
                    OffsetHours, clientStore, masterStore, authStatuses, notRequired, includeAuthComm, ClientFunctions);

                #endregion

                #region Trip data

                //new RowBuilderTripData().SetTripDta(recKey, travAuthNo, ref firstRow, Globals, getAllMasterAccountsQuery, clientStore,
                //    masterStore, ClientFunctions, ref RawDataList, includeLostSavings, ref lostSvgsMsg, intl, ref newRows);

                var counter = 0;
                var curRow = firstRow;
                foreach (var rawRow in RawDataList.Where(s => s.RecKey == recKey && s.TravAuthNo == travAuthNo).OrderBy(s => s.RDepDate))
                {
                    counter++;
                    if (counter == 1)
                    {
                        rowBuilderTripData.SetTripDta1(ref firstRow, Globals, getAllMasterAccountsQuery, clientStore, masterStore, ClientFunctions, rawRow,
                        includeLostSavings, ref lostSvgsMsg, intl, ref curRow);
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

                    rowBuilderTripData.SetTripDta2(masterStore, rawRow, intl, ref curRow, Globals);
                }
                #endregion

                #region Car data
                counter = 0;
                curRow = firstRow;
                //Add this code back to display hotel data if there is none in the result set. 
                //CarRawDataList.AddRange(ReportMockData<CarRawData>.GetMockData(2));
                //foreach (var carRow in CarRawDataList)
                //{
                //    carRow.RecKey = recKey;
                //    carRow.TravAuthNo = travAuthNo;
                //    carRow.Company = "FAKE DATA";
                //}

                rowBuilderCarData.SetCarDta( recKey, ref firstRow, Globals, getAllMasterAccountsQuery, clientStore, masterStore, ClientFunctions, 
                    includeLostSavings, travAuthNo, ref counter, ref lostSvgsMsg, intl, ref newRows, CarRawDataList, curRow);

                #endregion

                #region Hotel Data

                counter = 0;
                curRow = firstRow;
                //Add this code back to display hotel data if there is none in the result set. 
                //HotelRawDataList.AddRange(ReportMockData<HotelRawData>.GetMockData(2));
                //foreach (var hotRow in HotelRawDataList)
                //{
                //    hotRow.RecKey = recKey;903911
                //    hotRow.TravAuthNo = travAuthNo;
                //}


                //  Get diffrent results when I use SetHotelData1 than when I use SetHotelData1
                //rowBuilderHotelData.SetHotelData1(recKey, ref firstRow, Globals, getAllMasterAccountsQuery, clientStore, masterStore, 
                //    ClientFunctions, includeLostSavings, travAuthNo, ref counter, ref lostSvgsMsg, intl, ref newRows, HotelRawDataList, curRow);

                foreach (var hotRow in HotelRawDataList.Where(s => s.RecKey == recKey && s.TravAuthNo == travAuthNo))
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

                    rowBuilderHotelData.SetHotelData2(recKey, ref firstRow, Globals, getAllMasterAccountsQuery, clientStore, masterStore, ClientFunctions, 
                        includeLostSavings, travAuthNo, ref lostSvgsMsg, intl, ref newRows, hotRow, curRow);

                }
                #endregion

                foreach (var finalData in newRows)
                {
                    finalData.Tottripchg = firstRow.Tottripchg;
                    finalData.Lostsvgmsg = lostSvgsMsg;
                }
                finalDataList.AddRange(newRows.OrderBy(s => s.Rdepdate));
            }

            return finalDataList;
        }

    }
}

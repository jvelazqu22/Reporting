using Domain.Helper;
using Domain.Models.ReportPrograms.InvoiceReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;
using System.Collections.Generic;
using System.Linq;
using Domain;
using iBank.Services.Implementation.Shared.AdvancedClause;
using Domain.Models;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class InvoiceDataRetriever
    {
        public ReportGlobals Globals { get; set; }
        public bool IsReservation { get; set; }

        private IClientQueryable ClientQueryableDb
        {
            get
            {
                return new iBankClientQueryable(Globals.AgencyInformation.ServerName,
                    Globals.AgencyInformation.DatabaseName);
            }
        }
        

        private readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();

        public List<LegRawData> GetLegData(string whereClause, ReportGlobals globals, BuildWhere buildWhere, bool checkForDuplicatesAndRemoveThem = false)
        {
            var fieldList = "T1.reckey, T2.*";
            var legwhereClause = whereClause + " and airline != 'ZZ'";
            var fromClause = "hibtrips T1, hiblegs T2";
            var sortBy = "order by T2.reckey, seqno";

            var sql = new SqlScript();
            sql.FieldList = fieldList;
            sql.FromClause = fromClause;
            sql.KeyWhereClause = "T1.reckey = T2.reckey and ";
            sql.WhereClause = sql.KeyWhereClause + legwhereClause;
            sql.OrderBy = sortBy;

            sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, true);

            var retriever = new DataRetriever(ClientQueryableDb);

            return retriever.GetData<LegRawData>(sql, buildWhere, false, false, IsReservation, true, checkForDuplicatesAndRemoveThem, true).ToList();

         }

        public List<CarRawData> GetCarData(string whereClause9, ReportGlobals globals, BuildWhere buildWhere, string acceptableTransactionTypes, bool checkForDuplicatesAndRemoveThem = false)
        {
            var fieldList = "T1.invdate, T1.reckey, company, autocity, autostat, convert(int,days) as days, rentdate, cartype, abookrat,convert(int,cplusmin) as cplusmin, numcars, confirmno, invbyagcy, CarTranTyp, T4.MoneyType, DateBack ";
            var carwhereClause = whereClause9 + "  and T1.reckey = T4.reckey and T4.CarTranTyp in (" +
                                 acceptableTransactionTypes + ")";
            switch (globals.GetParmValue(WhereCriteria.DDINVBYAGCY))
            {
                case "2":
                    carwhereClause += " and T4.invByAgcy = 0";
                    break;
                case "3":
                    carwhereClause += " and T4.invByAgcy = 1";
                    break;
            }
            var fromClause = "hibTrips T1, hibcars T4";
            var sortBy = "order by T1.reckey";
            var fullSql = SqlProcessor.ProcessSql(fieldList, false, fromClause, carwhereClause, sortBy, globals);
            
            return checkForDuplicatesAndRemoveThem
                ? ClientDataRetrieval.GetUdidFilteredOpenQueryData<CarRawData>(fullSql, globals, buildWhere.Parameters, false).RemoveDuplicates().ToList()
                : ClientDataRetrieval.GetUdidFilteredOpenQueryData<CarRawData>(fullSql, globals, buildWhere.Parameters, false).ToList();
            
        }

        public List<HotelRawData> GetHotelData(string whereClause9, ReportGlobals globals, BuildWhere buildWhere, string acceptableTransactionTypes, bool checkForDuplicatesAndRemoveThem = false)
        {
            var fieldList = "T1.invdate, T1.reckey,  hotelnam, hotcity, hotstate, convert(int,nights) as nights, convert(int, rooms) as rooms, datein, bookrate, compamt, T5.moneytype, guarante,convert(int,hplusmin) as hplusmin, hotphone, confirmno, invbyagcy, HotTranTyp, roomtype ";
            var hotelWhereClause = whereClause9 + " and T1.reckey = T5.reckey and T5.HotTranTyp in (" +
                                   acceptableTransactionTypes + ")";
            switch (globals.GetParmValue(WhereCriteria.DDINVBYAGCY))
            {
                case "2":
                    hotelWhereClause += " and T5.invByAgcy = 0";
                    break;
                case "3":
                    hotelWhereClause += " and T5.invByAgcy = 1";
                    break;
            }
            var sortBy = "order by T1.reckey";
            var fromClause = "hibTrips T1, hibhotel T5";

            var fullSql = SqlProcessor.ProcessSql(fieldList, false, fromClause, hotelWhereClause, sortBy, globals);
            
            return checkForDuplicatesAndRemoveThem 
                ? ClientDataRetrieval.GetUdidFilteredOpenQueryData<HotelRawData>(fullSql, globals, buildWhere.Parameters, false).RemoveDuplicates().ToList()
                : ClientDataRetrieval.GetUdidFilteredOpenQueryData<HotelRawData>(fullSql, globals, buildWhere.Parameters, false).ToList();
           
        }

        public List<SvcFeeRawData> GetServiceFeeData(string whereClause, ReportGlobals globals, BuildWhere buildWhere, string originalWhere, bool checkForDuplicatesAndRemoveThem = false)
        {
            var svcFeeDataList = new List<SvcFeeRawData>();
            if (globals.AgencyInformation.UseServiceFees)
            {
                var svcFeeWhere = buildWhere.HasRoutingCriteria ? originalWhere : whereClause;
                var fieldList = "T1.reckey, recloc, invoice, acct, T6A.sfTrantype, passlast, " +
                            "passfrst, T6A.trandate, T6A.svcAmt, T6A.svcDesc, T6A.mco, T6A.sfCardnum, " +
                            "T6A.tax1, T6A.tax2, T6A.tax3, T6A.tax4";

                var svcFeewhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.sfTrantype <> 'V' and " +
                                        "T6A.svcCode = 'TSF'and " + svcFeeWhere;
                var fromClause = "hibtrips T1, hibServices T6A";
                var sortBy = string.Empty;
                var fullSql = SqlProcessor.ProcessSql(fieldList, false, fromClause, svcFeewhereClause, sortBy, globals);
                svcFeeDataList = ClientDataRetrieval.GetUdidFilteredOpenQueryData<SvcFeeRawData>(fullSql, globals, buildWhere.Parameters, false).ToList();
            }

            return checkForDuplicatesAndRemoveThem? svcFeeDataList.RemoveDuplicates().ToList() : svcFeeDataList;
        }

        public Udid GetUdid(string whereX, ReportGlobals globals, BuildWhere buildWhere, int udidNumber, string udidText)
        {
            var udid = new Udid
                           {
                               UdidNumber = udidNumber,
                               UdidText = udidText
                           };

            if (udid.UdidNumber > 0)
            {
                if (string.IsNullOrEmpty(udid.UdidText))
                {
                    udid.UdidText = "Udid # " + udid.UdidNumber + " text:";
                }
                if (!udid.UdidText.Right(1).Equals(":"))
                {
                    udid.UdidText += ":";
                }

                var udidWhere = $"T1.reckey = T3.reckey and UdidNo = {udid.UdidNumber} and {whereX}";
                var fieldList = "T1.reckey, convert(int,UdidNo) as UdidNbr, UdidText as UdidText, count(*) as dummy";
                
                var fromClause = "hibtrips T1, hibudids T3";
                var fullSql = SqlProcessor.ProcessSql(fieldList, false, fromClause, udidWhere, "", globals);
                fullSql += " group by T1.reckey, UdidNo, UdidText";
                udid.UdidData = ClientDataRetrieval.GetUdidFilteredOpenQueryData<UdidRawData>(fullSql, globals, buildWhere.Parameters, false).ToList();
            }

            return udid;
        }

    }
}

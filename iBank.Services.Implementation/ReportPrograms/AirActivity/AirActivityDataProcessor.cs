using System;
using System.Collections.Generic;
using System.Linq;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.AirActivityReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using iBank.Services.Implementation.Shared.SpecifyUdid;

namespace iBank.Services.Implementation.ReportPrograms.AirActivity
{
    public class AirActivityDataProcessor
    {
        ClientFunctions _clientFunctions;
        public AirActivityDataProcessor(ClientFunctions clientFunctions)
        {
            _clientFunctions = clientFunctions;
        }
        private string DateRange { get; set; }
        
        public IList<FinalData> ConvertRawDataToFinalData(IList<RawData> rawData, IList<UdidRecord> udids, List<int> udidNumber, ReportGlobals globals, UserBreaks userBreaks, bool isDateSort, string dateRange,
            IClientQueryable clientQueryDb, IMasterDataStore store)
        {
            DateRange = dateRange;
            const string VOID = "    ** VOID **    ";

            var user = globals.User;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientQueryDb, globals.Agency);

            var calc = new UdidCalculator();
            var query = rawData.Select(s =>
               new FinalData
               {
                   RecKey = s.RecKey,
                   Recloc = s.Recloc,
                   Invdate = s.Invdate ?? DateTime.MinValue,
                   Bookdate = s.Bookdate ?? DateTime.MinValue,
                   Invoice = s.Invoice.Trim(),
                   Ticket = s.Ticket,
                   SeqNo = s.SeqNo,
                   Pseudocity = s.Pseudocity,
                   Acct = !user.AccountBreak ? "^na^" : s.Acct,
                   AcctDesc = !user.AccountBreak ? "^na^" : _clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, globals),
                   Break1 = !userBreaks.UserBreak1 ? "^na^" : ((string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1.Trim())),
                   Break2 = !userBreaks.UserBreak2 ? "^na^" : ((string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2.Trim())),
                   Break3 = !userBreaks.UserBreak3 ? "^na^" : ((string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(16) : s.Break3.Trim())),
                   Passlast = s.Passlast.Trim(),
                   Passfrst = s.Passfrst.Trim(),
                   Origin = s.Origin,
                   OrgDesc = AportLookup.LookupAport(store, s.Origin, s.Mode, globals.Agency),
                   Destinat = s.Destinat,
                   DestDesc = AportLookup.LookupAport(store, s.Destinat, s.Mode, globals.Agency),
                   Connect = s.Connect,
                   Trantype = s.Trantype.Trim(),
                   Cardnum = (s.Trantype.Trim().EqualsIgnoreCase("V")) ? VOID : s.Cardnum,
                   Depdate = s.Depdate ?? DateTime.MinValue,
                   RDepDate = s.RDepDate ?? DateTime.MinValue,
                   Airline = LookupFunctions.LookupAlineCode(store, s.Airline, s.Mode),
                   fltno = s.fltno,
                   Tktdesig = s.Tktdesig,
                   ClassCode = s.ClassCode,
                   Airchg = s.Airchg,
                   Offrdchg = s.Offrdchg,
                   Svcfee = s.Svcfee,
                   SfTranType = s.SfTranType,
                   Exchange = s.Exchange,
                   Origticket = s.Origticket,
                   AirCo2 = SetAirCo2(s.Miles, s.AirCo2),
                   AltCarCo2 = s.AltCarCo2,
                   AltRailCo2 = s.AltRailCo2,
                   HaulType = s.HaulType,
                   Plusmin = s.Plusmin,
                   Sortdate = (isDateSort) ? GetSortDate(s) : new DateTime(),
                   Miles = s.Miles,
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
                   Udidtext1 = calc.GetUdids(s.RecKey, udidNumber[0], udids),
                   Udidtext2 = calc.GetUdids(s.RecKey, udidNumber[1], udids),
                   Udidtext3 = calc.GetUdids(s.RecKey, udidNumber[2], udids),
                   Udidtext4 = calc.GetUdids(s.RecKey, udidNumber[3], udids),
                   Udidtext5 = calc.GetUdids(s.RecKey, udidNumber[4], udids),
                   Udidtext6 = calc.GetUdids(s.RecKey, udidNumber[5], udids),
                   Udidtext7 = calc.GetUdids(s.RecKey, udidNumber[6], udids),
                   Udidtext8 = calc.GetUdids(s.RecKey, udidNumber[7], udids),
                   Udidtext9 = calc.GetUdids(s.RecKey, udidNumber[8], udids),
                   Udidtext10 = calc.GetUdids(s.RecKey, udidNumber[9], udids)
               }).OrderBy(s => s.Pseudocity)
               .ThenBy(s => s.AcctDesc)
               .ThenBy(s => s.Acct)
               .ThenBy(s => s.Break1)
               .ThenBy(s => s.Break2)
               .ThenBy(s => s.Break3);

            if (isDateSort) query = query.ThenBy(GetSortDate);

            var finalData = query.ThenBy(s => s.Passlast)
            .ThenBy(s => s.Passfrst)
            .ThenBy(s => s.Invoice)
            .ThenBy(s => s.RecKey)
            .ThenBy(s => s.SeqNo)
            .ToList();    

            return finalData;
        }

        private decimal SetAirCo2(int inmiles, decimal airCo2)
        {
            if (inmiles.GetIntSafe() >= 0) return airCo2;
            var result = airCo2 * -1;
            return result;
        }

        public IList<string> GetExportFields(bool excludeServiceFee, bool isCarbonReport, bool includeAlternateEmissions,
                                             bool useMetric, UserBreaks userBreaks, bool accountBreak, bool isReservationReport, UserInformation user, 
                                             List<int> udidNumbers, List<string> udidLabels)
        {
            var fieldList = new List<string>();

            if (isReservationReport) fieldList.Add("bookdate");

            if (accountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (userBreaks.UserBreak1)
            {
                var break1 = string.IsNullOrEmpty(user.Break1Name) ? "break_1" : user.Break1Name;
                fieldList.Add($"break1 as {break1}");
            }
            if (userBreaks.UserBreak2)
            {
                var break2 = string.IsNullOrEmpty(user.Break2Name) ? "break_2" : user.Break2Name;
                fieldList.Add($"break2 as {break2}");
            }
            if (userBreaks.UserBreak3)
            {
                var break3 = string.IsNullOrEmpty(user.Break3Name) ? "break_3" : user.Break3Name;
                fieldList.Add($"break3 as {break3}");
            }

            if (!isReservationReport) fieldList.Add("invdate");

            fieldList.Add("invoice");
            fieldList.Add("recloc");
            fieldList.Add("ticket");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");

            if (isReservationReport && !excludeServiceFee)
            {
                fieldList.Add("pseudocity");
            }

            fieldList.Add("reckey");
            fieldList.Add("cardnum");
            fieldList.Add("seqno");
            fieldList.Add("origin");
            fieldList.Add("orgdesc");
            fieldList.Add("destinat");
            fieldList.Add("destdesc");
            fieldList.Add("rdepdate");
            fieldList.Add("airline");
            fieldList.Add("fltno");
            
            if (isCarbonReport)
            {
                fieldList.Add(useMetric ? "miles as kms" : "miles");
                fieldList.Add("airco2");
                
                if (includeAlternateEmissions)
                {
                    fieldList.Add("altrailco2");
                    fieldList.Add("altcarco2");
                }
            }
            
            fieldList.Add("tktdesig");
            fieldList.Add("classcode as class");
            fieldList.Add("airchg");
            if (!excludeServiceFee) fieldList.Add("svcfee");
            fieldList.Add("exchange");
            fieldList.Add("origticket");

            ExportSpecifyUdidFields.AddUdidFieldList(fieldList, udidNumbers, udidLabels);

            return fieldList;
        }

        public IList<string> GetZeroOutFields(bool excludeServiceFees)
        {
            return excludeServiceFees ? new List<string> { "airchg" } : new List<string> { "airchg", "svcfee" };
        }

        public ReportDocument SetParameterValues(ReportDocument reportSource, ReportDocumentParameters documentParameters, bool isCarbonReport, bool isReservationReport)
        {
            reportSource.SetParameterValue(documentParameters.ColumnHeader, documentParameters.Parameters[documentParameters.ColumnHeader]);
            reportSource.SetParameterValue(documentParameters.TotalAirCharge, documentParameters.Parameters[documentParameters.TotalAirCharge]);

            if (isCarbonReport)
            {
                reportSource.SetParameterValue(documentParameters.CarbonCalcReportFooter, documentParameters.Parameters[documentParameters.CarbonCalcReportFooter]);
                reportSource.SetParameterValue(documentParameters.UnitOfMeasurement, documentParameters.Parameters[documentParameters.UnitOfMeasurement]);
            }

            if (!isReservationReport)
            {
                reportSource.SetParameterValue(documentParameters.IncludeVoids, documentParameters.Parameters[documentParameters.IncludeVoids]);
                reportSource.SetParameterValue(documentParameters.ExcludeServiceFees, documentParameters.Parameters[documentParameters.ExcludeServiceFees]);
            }

            return reportSource;
        }
        
        private DateTime GetSortDate(RawData rec)
        {
            switch ((DateType)Convert.ToInt32(DateRange))
            {
                case DateType.InvoiceDate:
                    return rec.Invdate.GetValueOrDefault();
                case DateType.BookedDate:
                    return rec.Bookdate.GetValueOrDefault();
                default:
                    return rec.Depdate.GetValueOrDefault();
            }
        }

        private DateTime GetSortDate(FinalData rec)
        {
            switch ((DateType)Convert.ToInt32(DateRange))
            {
                case DateType.InvoiceDate:
                    return rec.Invdate;
                case DateType.BookedDate:
                    return rec.Bookdate;
                default:
                    return rec.Depdate;
            }
        }
    }
}

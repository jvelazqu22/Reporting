using Domain.Models.ReportPrograms.InvoiceReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class InvoiceFinalLegData
    {

        public static bool LegDataExists(List<LegRawData> legData, RawData row, string acceptableTransactionTypes)
        {
            return legData.Any() && acceptableTransactionTypes.Contains(row.Trantype.Trim());
        }

        public static void AddLegDataToFinalData(List<FinalData> finalData, List<SubReportData> subReportData, List<LegRawData> legData, RawData row, ReportGlobals globals, AccountAddressInfo acctInfo, string validatingCarrier,
            Udid udidOne, Udid udidTwo, string exchangeInfo, IMasterDataStore store, string xTicket)
        {
            foreach (var leg in legData)
            {
                var orgDesc = AportLookup.LookupAport(store, leg.Origin, leg.Mode, globals.Agency);
                var destDesc = AportLookup.LookupAport(store, leg.Destinat, leg.Mode, globals.Agency);
                var alineDesc = LookupFunctions.LookupAline(store, leg.Airline, leg.Mode);

                finalData.Add(new FinalData
                                      {
                                          Reckey = row.RecKey,
                                          Rectype = "A",
                                          Acctname = acctInfo.Name,
                                          Brklvl1 = globals.User.Break1Name,
                                          Brklvl2 = globals.User.Break2Name,
                                          Brklvl3 = globals.User.Break3Name,
                                          Acctaddr1 = acctInfo.Address1,
                                          Acctaddr2 = acctInfo.Address2,
                                          Acctaddr3 = acctInfo.Address3,
                                          Acctaddr4 = acctInfo.Address4,
                                          Invoice = row.Invoice,
                                          Invdate = row.Invdate ?? DateTime.MinValue,
                                          Bookeddate = row.Bookdate ?? DateTime.MinValue,
                                          Agentid = row.Agentid,
                                          Cardnum = row.Cardnum,
                                          Recloc = row.Recloc,
                                          Ticket = row.Ticket,
                                          Passlast = row.Passlast,
                                          Passfrst = row.Passfrst,
                                          Break1 = row.Break1,
                                          Break2 = row.Break2,
                                          Break3 = row.Break3,
                                          Valcarr = validatingCarrier,
                                          Airchg = row.Airchg,
                                          Svcfee = row.SvcFee,
                                          Activdate = leg.RDepDate ?? DateTime.MinValue,
                                          Orgdesc = orgDesc,
                                          Destdesc = destDesc,
                                          Alinedesc = alineDesc,
                                          Fltno = leg.FltNo,
                                          Deptime = SharedProcedures.ConvertTime(leg.Deptime),
                                          Arrtime = SharedProcedures.ConvertTime(leg.Arrtime),
                                          Class = leg.Class,
                                          Exchinfo = exchangeInfo,
                                          Tax1 = row.Tax1,
                                          Tax2 = row.Tax2,
                                          Tax3 = row.Tax3,
                                          Tax4 = row.Tax4,
                                          Udidnbr1 = udidOne.UdidNumber,
                                          Udidnbr2 = udidTwo.UdidNumber,
                                          Udidtext1 = udidOne.UdidText,
                                          Udidtext2 = udidTwo.UdidText
                                      });
            }

            if (row.Airchg != 0)
            {
                subReportData.Add(GetLegDataForSubReport(row, store, xTicket));
            }
        }

        public static FinalData GetDefaultData(RawData row, AccountAddressInfo acctInfo, ReportGlobals globals, string validatingCarrier, string xNoAirTravel, Udid udidOne, Udid udidTwo)
        {
            return new FinalData
                       {
                           Reckey = row.RecKey,
                           Rectype = "A",
                           Acctname = acctInfo.Name,
                           Brklvl1 = globals.User.Break1Name,
                           Brklvl2 = globals.User.Break2Name,
                           Brklvl3 = globals.User.Break3Name,
                           Acctaddr1 = acctInfo.Address1,
                           Acctaddr2 = acctInfo.Address2,
                           Acctaddr3 = acctInfo.Address3,
                           Acctaddr4 = acctInfo.Address4,
                           Invoice = row.Invoice,
                           Invdate = row.Invdate ?? DateTime.MinValue,
                           Bookeddate = row.Bookdate ?? DateTime.MinValue,
                           Agentid = row.Agentid,
                           Cardnum = row.Cardnum,
                           Recloc = row.Recloc,
                           Ticket = row.Ticket,
                           Passlast = row.Passlast,
                           Passfrst = row.Passfrst,
                           Break1 = row.Break1,
                           Break2 = row.Break2,
                           Break3 = row.Break3,
                           Valcarr = validatingCarrier,
                           Airchg = row.Airchg,
                           Svcfee = row.SvcFee,
                           Orgdesc = xNoAirTravel,
                           Tax1 = row.Tax1,
                           Tax2 = row.Tax2,
                           Tax3 = row.Tax3,
                           Tax4 = row.Tax4,
                           Udidnbr1 = udidOne.UdidNumber,
                           Udidnbr2 = udidTwo.UdidNumber,
                           Udidtext1 = udidOne.UdidText,
                           Udidtext2 = udidTwo.UdidText,
                           Vendaddr = "NOAIR"
                       };
                
        }

        public static SubReportData GetLegDataForSubReport(RawData row, IMasterDataStore store, string xTicket)
        {
            var airline = LookupFunctions.LookupAline(store, row.Valcarr, row.Mode);

            return new SubReportData
                       {
                           Reckey = row.RecKey,
                           Trandate = row.Invdate ?? DateTime.MinValue,
                           Chargedesc = $"{airline.Trim()} -- {xTicket}: {row.Ticket}:",
                           Charge = row.Airchg,
                           Taxname = "UNUSED",
                           Taxamt = 0
                       };
        }
    }
}

using Domain.Models.ReportPrograms.InvoiceReport;
using iBank.Server.Utilities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class InvoiceFinalCarData
    {

        public static bool CarDataExists(List<CarRawData> carData, RawData row, string acceptableTransactionTypes)
        {
            return carData.Any() && acceptableTransactionTypes.Contains(row.Trantype.Trim());
        }
        
        public static void AddCarData(List<FinalData> finalData, List<SubReportData> subReportData, List<CarRawData> carData, RawData row, ReportGlobals globals,
                               AccountAddressInfo acctInfo, string valCarr, Udid udidOne, Udid udidTwo, bool includeCosts, string xBookingFor, string xDay, string xDays)
        {
            foreach (var car in carData)
            {
                string vendAddress;
                if (string.IsNullOrEmpty(car.Autocity.Trim()) || string.IsNullOrEmpty(car.Autostat.Trim()))
                {
                    vendAddress = car.Autocity.Trim() + car.Autostat.Trim();
                }
                else
                {
                    vendAddress = car.Autocity.Trim() + ", " + car.Autostat.Trim();
                }

                finalData.Add(new FinalData
                                      {
                                          Reckey = row.RecKey,
                                          Rectype = "B",
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
                                          Valcarr = valCarr,
                                          Airchg = row.Airchg,
                                          Svcfee = row.SvcFee,
                                          Activdate = car.Rentdate ?? DateTime.MinValue,
                                          Vendor = car.Company,
                                          Vendaddr = vendAddress,
                                          Days = car.Days,
                                          Bookedrate = car.Abookrat,
                                          Ch_type = car.Cartype,
                                          Udidnbr1 = udidOne.UdidNumber,
                                          Udidnbr2 = udidTwo.UdidNumber,
                                          Udidtext1 = udidOne.UdidText,
                                          Udidtext2 = udidTwo.UdidText,
                                          Confirmno = car.Confirmno
                                      });

                if (car.Invbyagcy || includeCosts)
                {
                    var daysDescription = car.Days == 1 ? $"{xDay}:" : $"{xDays}:";
                    var chargeDescription = $"{xBookingFor} {car.Company.Trim()}, {car.Days} {daysDescription}";

                    subReportData.Add(new SubReportData
                                              {
                                                  Reckey = row.RecKey,
                                                  Trandate = row.Invdate ?? DateTime.MinValue,
                                                  Chargedesc = chargeDescription,
                                                  Charge = car.Abookrat * car.Days,
                                                  Taxname = "UNUSED",
                                                  Taxamt = 0
                                              });
                }
            }

        }

        public static FinalData GetDefaultData(RawData row, ReportGlobals globals, AccountAddressInfo acctInfo, string valCarr, Udid udidOne, Udid udidTwo, string xNoCarRentals)
        {
            return new FinalData
                       {
                           Reckey = row.RecKey,
                           Rectype = "B",
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
                           Valcarr = valCarr,
                           Airchg = row.Airchg,
                           Svcfee = row.SvcFee,
                           Vendor = xNoCarRentals,
                           Vendaddr = "NOBOOKING",
                           Udidnbr1 = udidOne.UdidNumber,
                           Udidnbr2 = udidTwo.UdidNumber,
                           Udidtext1 = udidOne.UdidText,
                           Udidtext2 = udidTwo.UdidText
                       };
        }
    }
}

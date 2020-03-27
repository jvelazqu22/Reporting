using Domain.Models.ReportPrograms.InvoiceReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class InvoiceFinalHotelData
    {
        public static bool HotelDataExists(List<HotelRawData> hotelData, RawData row, string acceptableTranslations)
        {
            return hotelData.Any() && acceptableTranslations.Contains(row.Trantype.Trim());
        }

        public static void AddHotelDataToFinalData(List<FinalData> finalData, List<SubReportData> subReportData, List<HotelRawData> hotelData, RawData row, AccountAddressInfo acctInfo, ReportGlobals globals, 
            string valCarr, Udid udidOne, Udid udidTwo, bool includeCosts, string xBookingFor, string xNight, string xNights)
        {
            foreach (var hotel in hotelData)
            {
                string vendAddress;
                if (string.IsNullOrEmpty(hotel.Hotcity.Trim()) || string.IsNullOrEmpty(hotel.Hotstate.Trim()))
                {
                    vendAddress = hotel.Hotcity.Trim() + hotel.Hotstate.Trim();
                }
                else
                {
                    vendAddress = hotel.Hotcity.Trim() + ", " + hotel.Hotstate.Trim();
                }

                finalData.Add(new FinalData
                                      {
                                          Reckey = row.RecKey,
                                          Rectype = "C",
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
                                          Activdate = hotel.Datein ?? DateTime.MinValue,
                                          Vendor = hotel.Hotelnam,
                                          Vendaddr = vendAddress,
                                          Days = hotel.Nights,
                                          Bookedrate = hotel.Bookrate,
                                          Ch_type = hotel.Roomtype,
                                          Hotphone = hotel.Hotphone,
                                          Guaranteed = hotel.Guarante.EqualsIgnoreCase("Y"),
                                          Udidnbr1 = udidOne.UdidNumber,
                                          Udidnbr2 = udidTwo.UdidNumber,
                                          Udidtext1 = udidOne.UdidText,
                                          Udidtext2 = udidTwo.UdidText,
                                          Confirmno = hotel.Confirmno,
                                          Rooms = hotel.Rooms
                                      });

                if (hotel.Invbyagcy || includeCosts)
                {
                    var nightsDescription = hotel.Nights == 1 ? $"{xNight}:" : $"{xNights}:";
                    var chargeDescription = $"{xBookingFor} {hotel.Hotelnam.Trim()}, {hotel.Nights} {nightsDescription}";

                    subReportData.Add(new SubReportData
                                              {
                                                  Reckey = row.RecKey,
                                                  Trandate = row.Invdate ?? DateTime.MinValue,
                                                  Chargedesc = chargeDescription,
                                                  Charge = hotel.Bookrate * hotel.Nights,
                                                  Taxname = "UNUSED",
                                                  Taxamt = 0
                                              });
                }
            }
        }

        public static FinalData GetDefaultData(RawData row, AccountAddressInfo acctInfo, ReportGlobals globals, string valCarr, string xNoHotelBkngs, Udid udidOne, Udid udidTwo)
        {
            return new FinalData
                       {
                           Reckey = row.RecKey,
                           Rectype = "C",
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
                           Vendor = xNoHotelBkngs,
                           Vendaddr = "NOBOOKING",
                           Udidnbr1 = udidOne.UdidNumber,
                           Udidnbr2 = udidTwo.UdidNumber,
                           Udidtext1 = udidOne.UdidText,
                           Udidtext2 = udidTwo.UdidText,
                       };
        }
    }
}

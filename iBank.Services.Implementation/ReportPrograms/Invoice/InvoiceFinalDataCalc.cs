using Domain.Helper;
using Domain.Models.ReportPrograms.InvoiceReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;
//using Domain.Models.ReportPrograms.TravelDetail;
//using CarRawData = Domain.Models.ReportPrograms.InvoiceReport.CarRawData;
//using HotelRawData = Domain.Models.ReportPrograms.InvoiceReport.HotelRawData;
//using LegRawData = Domain.Models.ReportPrograms.InvoiceReport.LegRawData;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class FinalDataMapper
    {
        public InvoiceReportDataLists MapToFinalData(ReportGlobals globals, InvoiceDataSources sources, string acceptableTransactionTypes, IClientDataStore clientStore, IMasterDataStore masterStore)
        {
            //translations
            var xTicket = GetTranslation("xTicket", "Ticket #", globals);
            var xDay = GetTranslation("xDay", "Day", globals);
            var xDays = GetTranslation("xDays", "Days", globals);
            var xBookingFor = GetTranslation("xBookingFor", "Booking For", globals);
            var xNight = GetTranslation("xNight", "Night", globals);
            var xNights = GetTranslation("xNights", "Nights", globals);
            var xServiceFee = GetTranslation("xServiceFee", "Service Fee", globals);
            var xNoAirTravel = GetTranslation("xNoAirTravel", "No Air Travel", globals);
            var xNoCarRentals = GetTranslation("xNoCarRentals", "No Car Rentals", globals);
            var xNoHotelBkngs = GetTranslation("xNoHotelBkngs", "No Hotel Bookings", globals);
            var xExchangeBelow = GetTranslation("xExchangeBelow", "This is an \"Exchange\" record.", globals);
            var xOrigTicketWas = GetTranslation("xOrigTicketWas", "Original Ticket # was", globals);

            var includeCosts = globals.IsParmValueOn(WhereCriteria.CBINCLHOTCARCOSTS);
            var userBreaks = SharedProcedures.SetUserBreaks(globals.User.ReportBreaks);
            var accts = new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, globals.Agency).ExecuteQuery().ToList();

            var finalData = new List<FinalData>();
            var subReportData = new List<SubReportData>();
            var svcFeeTaxes = new List<SubReportData>();

            foreach (var row in sources.RawData)
            {
                var legs = sources.LegData.Where(x => x.RecKey == row.RecKey).ToList();
                var cars = sources.CarData.Where(x => x.RecKey == row.RecKey).ToList();
                var hotels = sources.HotelData.Where(x => x.RecKey == row.RecKey).ToList();
                var services = sources.SvcFeeData.Where(x => x.RecKey == row.RecKey).ToList();

                if (!DataExistsForRecKey(legs, cars, hotels, services, acceptableTransactionTypes, row)) continue;
                
                row.Break1 = GetBreakText(userBreaks.UserBreak1, row.Break1);
                row.Break2 = GetBreakText(userBreaks.UserBreak2, row.Break2);
                row.Break3 = GetBreakText(userBreaks.UserBreak3, row.Break3);
                
                var acct = accts.FirstOrDefault(s => s.AccountId.EqualsIgnoreCase(row.Acct));
                var acctInfo = new AccountAddressInfo(acct, row.Acct.Trim());
                var valCarr = LookupFunctions.LookupAline(masterStore, row.Valcarr, row.ValcarMode);

                var exchInfo = row.Exchange ? GetExchangeInfo(xExchangeBelow, row.Origticket, xOrigTicketWas) : "";

                sources.UdidOne.UdidText = GetUdidText(sources.UdidOne.UdidData, sources.UdidOne.UdidNumber, row);
                sources.UdidTwo.UdidText = GetUdidText(sources.UdidTwo.UdidData, sources.UdidTwo.UdidNumber, row);
                
                AddLegData(finalData, subReportData, sources, legs, row, acceptableTransactionTypes, globals, acctInfo, valCarr, exchInfo, masterStore, xTicket,
                    xNoAirTravel);
                AddCarData(finalData, subReportData, sources, cars, row, acceptableTransactionTypes, globals, acctInfo, valCarr, includeCosts, xBookingFor, xDay,
                    xDays, xNoCarRentals);
                AddHotelData(finalData, subReportData, sources, hotels, row, acceptableTransactionTypes, globals, acctInfo, valCarr, includeCosts, xBookingFor,
                    xNoHotelBkngs, xNight, xNights);
                AddSvcFeeData(subReportData, svcFeeTaxes, sources, row, globals, xServiceFee, acceptableTransactionTypes);
            }
            
            if(globals.IsParmValueOn(WhereCriteria.CBINCLTAXBRKDN))
            {
                InvoiceFinalTaxesData.AddTaxes(subReportData, sources.RawData, svcFeeTaxes.ToList(), globals);
                InvoiceFinalTaxesData.AddServiceFeeTaxes(subReportData, svcFeeTaxes.ToList());
            }

            return new InvoiceReportDataLists(finalData.ToList(), subReportData);
        }

        private string GetTranslation(string key, string defaultValue, ReportGlobals globals)
        {
            var translation = globals.LanguageVariables.FirstOrDefault(s => s.VariableName.ToUpper() == key);
            return translation == null ? defaultValue : translation.Translation;
        }

        private bool DataExistsForRecKey(List<LegRawData> legData, List<CarRawData> carData, List<HotelRawData> hotelData, List<SvcFeeRawData> serviceData, string acceptableTransactionTypes, RawData row)
        {
            return (legData.Any() || hotelData.Any() || carData.Any() || serviceData.Any()) && acceptableTransactionTypes.Contains(row.Trantype.Trim());
        }

        private string GetExchangeInfo(string xExchangeBelow, string originalTicket, string xOrigTicketWas)
        {
            var exchangeInfo = $"* {xExchangeBelow}";
            if (!string.IsNullOrEmpty(originalTicket.Trim()))
            {
                exchangeInfo += $" {xOrigTicketWas} {originalTicket.Trim()}";
            }
            return exchangeInfo;
        }

        private string GetBreakText(bool userBreak, string existingBreakText)
        {
            return userBreak
                       ? string.IsNullOrEmpty(existingBreakText.Trim()) ? "NONE" : existingBreakText
                       : "^na^";
        }

        private string GetUdidText(List<UdidRawData> udidData, int udidNumber, RawData row)
        {
            var udidText = string.Empty;
            if (udidNumber > 0)
            {
                var udidRec = udidData.FirstOrDefault(s => s.RecKey == row.RecKey && s.UdidNbr == udidNumber);
                if (udidRec != null)
                {
                    udidText = udidRec.UdidText;
                }
            }

            return udidText;
        }

        private void AddLegData(List<FinalData> finalData, List<SubReportData> subReportData, InvoiceDataSources sources, List<LegRawData> legs, RawData row, string acceptableTransactionTypes, ReportGlobals globals, AccountAddressInfo acctInfo,
            string valCarr, string exchInfo, IMasterDataStore masterStore, string xTicket, string xNoAirTravel)
        {
            if (InvoiceFinalLegData.LegDataExists(legs, row, acceptableTransactionTypes))
            {
                InvoiceFinalLegData.AddLegDataToFinalData(finalData, subReportData, legs, row, globals, acctInfo, valCarr, sources.UdidOne, sources.UdidTwo,
                    exchInfo, masterStore, xTicket);
            }
            else
            {
                finalData.Add(InvoiceFinalLegData.GetDefaultData(row, acctInfo, globals, valCarr, xNoAirTravel, sources.UdidOne, sources.UdidTwo));
            }
        }

        private void AddCarData(List<FinalData> finalData, List<SubReportData> subReportData, InvoiceDataSources sources, List<CarRawData> cars, RawData row, string acceptableTransactionTypes, ReportGlobals globals, AccountAddressInfo acctInfo,
            string valCarr, bool includeCosts, string xBookingFor, string xDay, string xDays, string xNoCarRentals)
        {
            if (InvoiceFinalCarData.CarDataExists(cars, row, acceptableTransactionTypes))
            {
                InvoiceFinalCarData.AddCarData(finalData, subReportData, cars, row, globals, acctInfo, valCarr, sources.UdidOne, sources.UdidTwo, includeCosts, xBookingFor, xDay, xDays);
            }
            else
            {
                finalData.Add(InvoiceFinalCarData.GetDefaultData(row, globals, acctInfo, valCarr, sources.UdidOne, sources.UdidTwo, xNoCarRentals));
            }
        }

        private void AddHotelData(List<FinalData> finalData, List<SubReportData> subReportData, InvoiceDataSources sources, List<HotelRawData> hotels, RawData row, string acceptableTransactionTypes, ReportGlobals globals, AccountAddressInfo acctInfo,
            string valCarr, bool includeCosts, string xBookingFor, string xNoHotelBkngs, string xNight, string xNights)
        {
            if (InvoiceFinalHotelData.HotelDataExists(hotels, row, acceptableTransactionTypes))
            {
                InvoiceFinalHotelData.AddHotelDataToFinalData(finalData, subReportData, hotels, row, acctInfo, globals, valCarr, sources.UdidOne, sources.UdidTwo, includeCosts, xBookingFor, xNight, xNights);
            }
            else
            { 
                finalData.Add(InvoiceFinalHotelData.GetDefaultData(row, acctInfo, globals, valCarr, xNoHotelBkngs, sources.UdidOne, sources.UdidTwo));
            }
        }

        private void AddSvcFeeData(List<SubReportData> subReportData, List<SubReportData> svcFeeTaxList, InvoiceDataSources sources, RawData row, ReportGlobals globals, string xServiceFee, string acceptableTransactionTypes)
        {
            if (globals.AgencyInformation.UseServiceFees)
            {
                InvoiceFinalSvcFeeData.AddSvcFeeDataToSubReport(subReportData, sources.SvcFeeData, row, acceptableTransactionTypes, globals, svcFeeTaxList, xServiceFee);
            }
            else if(acceptableTransactionTypes.Contains(row.Trantype.Trim()))
            {
                subReportData.Add(InvoiceFinalSvcFeeData.GetDefaultData(row, acceptableTransactionTypes, globals, xServiceFee));
            }
        }
        
        public List<FinalData> SortFinalDataList(string whereCriteria, List<FinalData> finalDataList)
        {
            switch (whereCriteria)
            {
                case "2":
                    return finalDataList.OrderBy(s => s.Passlast)
                                        .ThenBy(s => s.Passfrst)
                                        .ToList();
                case "3":
                    return finalDataList.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ToList();
                case "4":
                    return finalDataList.OrderBy(s => s.Break1)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ToList();
                default:
                    return finalDataList.OrderBy(s => s.Invoice)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ToList();
            }
        }
    }
}

using Domain.Models.ReportPrograms.CCReconReport;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.CCRecon
{
    public class CcReconSortHandler
    {
        public List<FinalData> SortFinalData(List<FinalData> finalData, string sortBy, bool useAcctBrks, bool onlyDisplayBreaksPerUserSettings)
        {
            switch (sortBy)
            {
                //airline number, ticket number
                case "1":
                    return SortByAirlineNumberAndTicketNumber(finalData, onlyDisplayBreaksPerUserSettings);
                //airline number, traveler
                case "2":
                    return SortByAirlineNumberAndTraveler(finalData, onlyDisplayBreaksPerUserSettings);
                //airline number, transaction date
                case "3":
                    return SortByAirlineNumberAndTransactionDate(finalData, onlyDisplayBreaksPerUserSettings);
                //ticket 
                case "4":
                    return SortByTicket(finalData, onlyDisplayBreaksPerUserSettings);
                //transaction date
                case "5":
                    return SortByTransactionDate(finalData, useAcctBrks, onlyDisplayBreaksPerUserSettings);
                //transaction date, airline 
                case "6":
                    return SortByTransactionDateAndAirline(finalData, onlyDisplayBreaksPerUserSettings);
                //transaction date, airline, traveler
                case "7":
                    return SortByTransactionDateAndAirlineAndTraveler(finalData, onlyDisplayBreaksPerUserSettings);
                //traveler
                case "8":
                    return SortByTraveler(finalData, onlyDisplayBreaksPerUserSettings);
                //traveler, airline
                case "9":
                    return SortByTravelerAndAirline(finalData, onlyDisplayBreaksPerUserSettings);
                //traveler, airline, transaction date
                case "10":
                    return SortByTravelerAndAirlineAndTransactionDate(finalData, onlyDisplayBreaksPerUserSettings);
                //transaction date, traveler, airline
                case "11":
                    return SortByTransactionDateAndTravelerAndAirline(finalData, onlyDisplayBreaksPerUserSettings);
                default:
                    return DefaultSort(finalData, useAcctBrks, onlyDisplayBreaksPerUserSettings);
            }

        }

        private List<FinalData> SortByTransactionDateAndTravelerAndAirline(List<FinalData> finalData, bool onlyDisplayBreaksPerUserSettings)
        {
            if (onlyDisplayBreaksPerUserSettings)
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Trandate)
                        .ThenBy(s => s.Passname)
                        .ThenBy(s => s.Airlinenbr)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
            else
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.Trandate)
                        .ThenBy(s => s.Passname)
                        .ThenBy(s => s.Airlinenbr)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
        }

        private List<FinalData> SortByTravelerAndAirlineAndTransactionDate(List<FinalData> finalData, bool onlyDisplayBreaksPerUserSettings)
        {
            if (onlyDisplayBreaksPerUserSettings)
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Passname)
                        .ThenBy(s => s.Airlinenbr)
                        .ThenBy(s => s.Trandate)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
            else
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.Passname)
                        .ThenBy(s => s.Airlinenbr)
                        .ThenBy(s => s.Trandate)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
        }

        private List<FinalData> SortByTravelerAndAirline(List<FinalData> finalData, bool onlyDisplayBreaksPerUserSettings)
        {
            if (onlyDisplayBreaksPerUserSettings)
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Passname)
                        .ThenBy(s => s.Airlinenbr)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
            else
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.Passname)
                        .ThenBy(s => s.Airlinenbr)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
        }

        private List<FinalData> SortByTraveler(List<FinalData> finalData, bool onlyDisplayBreaksPerUserSettings)
        {
            if (onlyDisplayBreaksPerUserSettings)
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Passname)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
            else
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.Passname)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
        }

        private List<FinalData> SortByTransactionDateAndAirlineAndTraveler(List<FinalData> finalData, bool onlyDisplayBreaksPerUserSettings)
        {
            if (onlyDisplayBreaksPerUserSettings)
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Trandate)
                        .ThenBy(s => s.Airlinenbr)
                        .ThenBy(s => s.Passname)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
            else
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.Trandate)
                        .ThenBy(s => s.Airlinenbr)
                        .ThenBy(s => s.Passname)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
        }

        private List<FinalData> SortByTransactionDateAndAirline(List<FinalData> finalData, bool onlyDisplayBreaksPerUserSettings)
        {
            if (onlyDisplayBreaksPerUserSettings)
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Trandate)
                        .ThenBy(s => s.Airlinenbr)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
            else
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.Trandate)
                        .ThenBy(s => s.Airlinenbr)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
        }

        private List<FinalData> SortByTicket(List<FinalData> finalData, bool onlyDisplayBreaksPerUserSettings)
        {
            if (onlyDisplayBreaksPerUserSettings)
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
            else
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
        }

        private List<FinalData> SortByAirlineNumberAndTransactionDate(List<FinalData> finalData, bool onlyDisplayBreaksPerUserSettings)
        {
            if (onlyDisplayBreaksPerUserSettings)
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Airlinenbr)
                        .ThenBy(s => s.Trandate)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
            else
            {
                return finalData.OrderBy(s => s.Cardnum)
                      .ThenBy(s => s.Acct)
                      .ThenBy(s => s.Break1)
                      .ThenBy(s => s.Break2)
                      .ThenBy(s => s.Break3)
                      .ThenBy(s => s.Airlinenbr)
                      .ThenBy(s => s.Trandate)
                      .ThenBy(s => s.Ticket)
                      .ToList();
            }
        }

        private List<FinalData> SortByAirlineNumberAndTraveler(List<FinalData> finalData, bool onlyDisplayBreaksPerUserSettings)
        {
            if (onlyDisplayBreaksPerUserSettings)
            {
                return finalData.OrderBy(s => s.Cardnum)
                        .ThenBy(s => s.Airlinenbr)
                        .ThenBy(s => s.Passname)
                        .ThenBy(s => s.Ticket)
                        .ToList();
            }
            else
            {
                return finalData.OrderBy(s => s.Cardnum)
                          .ThenBy(s => s.Acct)
                          .ThenBy(s => s.Break1)
                          .ThenBy(s => s.Break2)
                          .ThenBy(s => s.Break3)
                          .ThenBy(s => s.Airlinenbr)
                          .ThenBy(s => s.Passname)
                          .ThenBy(s => s.Ticket)
                          .ToList();
            }
        }

        private List<FinalData> SortByAirlineNumberAndTicketNumber(List<FinalData> finalData, bool onlyDisplayBreaksPerUserSettings)
        {
            if (onlyDisplayBreaksPerUserSettings)
            {
                return finalData.OrderBy(s => s.Cardnum)
                    .ThenBy(s => s.Airlinenbr)
                    .ThenBy(s => s.Ticket)
                    .ToList();
            }
            else
            {
                return finalData.OrderBy(s => s.Cardnum)
                    .ThenBy(s => s.Acct)
                    .ThenBy(s => s.Break1)
                    .ThenBy(s => s.Break2)
                    .ThenBy(s => s.Break3)
                    .ThenBy(s => s.Airlinenbr)
                    .ThenBy(s => s.Ticket)
                    .ToList();
            }
        }

        private List<FinalData> SortByTransactionDate(List<FinalData> finalData, bool useAcctBrks, bool onlyDisplayBreaksPerUserSettings)
        {
            if (onlyDisplayBreaksPerUserSettings)
            {
                return finalData.OrderBy(s => s.Cardnum)
                    .ThenBy(s => s.Trandate)
                    .ThenBy(s => s.Ticket)
                    .ToList();

            }
            else if (useAcctBrks)
            {
                return finalData.OrderBy(s => s.Cardnum)
                    .ThenBy(s => s.Acct)
                    .ThenBy(s => s.Break1)
                    .ThenBy(s => s.Break2)
                    .ThenBy(s => s.Break3)
                    .ThenBy(s => s.Trandate)
                    .ThenBy(s => s.Ticket)
                    .ToList();
            }

            return finalData.OrderBy(s => s.Cardnum)
                .ThenBy(s => s.Acct)
                .ThenBy(s => s.Trandate)
                .ThenBy(s => s.Ticket)
                .ToList();
        }

        private List<FinalData> DefaultSort(List<FinalData> finalData, bool useAcctBrks, bool onlyDisplayBreaksPerUserSettings)
        {
            if (onlyDisplayBreaksPerUserSettings)
            {
                return finalData.OrderBy(s => s.Cardnum)
                  .ThenBy(s => s.Airlinenbr)
                  .ThenBy(s => s.Ticket)
                  .ToList();

            }
            else if (useAcctBrks)
            {
                return finalData.OrderBy(s => s.Cardnum)
                    .ThenBy(s => s.Acct)
                    .ThenBy(s => s.Break1)
                    .ThenBy(s => s.Break2)
                    .ThenBy(s => s.Break3)
                    .ThenBy(s => s.Airlinenbr)
                    .ThenBy(s => s.Ticket)
                    .ToList();
            }

            return finalData.OrderBy(s => s.Cardnum)
                .ThenBy(s => s.Acct)
                .ThenBy(s => s.Airlinenbr)
                .ThenBy(s => s.Ticket)
                .ToList();
        }
    }
}
using CODE.Framework.Core.Utilities;
using Domain.Helper;
using Domain.Models.ReportPrograms.HotelActivity;
using Domain.Orm.Classes;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.HotelActivity
{
    public class HotelActivityCalculator
    {
        public List<FinalData> GetFinalDataFromRawData(List<RawData> RawDataList, ReportGlobals Globals, UserBreaks UserBreaks, 
            ClientFunctions clientFunctions, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery)
        {
            DateTime sundayDate = GetSundayDate(Globals.BeginDate.ToDateTimeSafe());
            return RawDataList.Select(s => new FinalData
            {
                Acct = !Globals.User.AccountBreak ? Constants.NotApplicable : s.Acct,
                Acctdesc = !Globals.User.AccountBreak ? Constants.NotApplicable : clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, Globals),
                Break1 = !UserBreaks.UserBreak1
                                ? Constants.NotApplicable
                                : string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1,
                Break2 = !UserBreaks.UserBreak2
                                ? Constants.NotApplicable
                                : string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2,
                Break3 = !UserBreaks.UserBreak3
                                ? Constants.NotApplicable
                                : string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(30) : s.Break3,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Recloc = s.Recloc,
                Hotcity = s.Hotcity,
                Hotstate = s.Hotstate,
                Hotcityst = s.Hotcityst,
                Datein = s.Datein ?? DateTime.MinValue,
                Hotelnam = string.IsNullOrWhiteSpace(s.Hotelnam) ? "[No Hotel Provided]" : s.Hotelnam,
                Chaincod = s.Chaincod,
                Roomtype = s.Roomtype,
                Nights = s.Nights,
                Rooms = s.Rooms,
                Bookrate = s.Bookrate,
                Weeknum = GetWeekNum(s.Datein, sundayDate),
                WeekNumTxt = GetWeekNumberDescription(s.Datein, sundayDate, Globals),
                Hplusmin = s.Hplusmin

            }).ToList();
        }

        public string GetCrystalReportNameAndSetIsReservationVariable(string reportValue, ref bool isReservation)
        {
            switch (reportValue)
            {
                case "80":
                    return "ibHotelCity";
                case "82":
                    isReservation = true;
                    return "ibHotelAdvRes";
                case "84":
                    return "ibHotelVendor";
                case "86":
                    return "ibHotelActivity";
                default:
                    return string.Empty;
            }
        }

        public DateTime GetSundayDate(DateTime reportDate)
        {
            DateTime sundayDate = reportDate;
            while (sundayDate.DayOfWeekNumber() > 1)
                sundayDate = sundayDate.AddDays(-1);

            return sundayDate;
        }

        public DateTime GetSaturdayDate(DateTime reportDate)
        {
            DateTime saturdayDate = reportDate;
            while (saturdayDate.DayOfWeekNumber() < 7)
                saturdayDate = saturdayDate.AddDays(1);

            return saturdayDate;
        }

        public decimal GetWeekNum(DateTime? reportDate, DateTime? sundayDate)
        {
            return ((reportDate.GetValueOrDefault() - sundayDate.GetValueOrDefault()).Days / 7) + 1;
        }

        public string GetWeekNumberDescription(DateTime? dateIn, DateTime? sundayDate, ReportGlobals Globals)
        {
            return "Week # " + GetWeekNum(dateIn, sundayDate) + " - " + GetSundayDate(Globals.BeginDate.ToDateTimeSafe()).ToShortDateString()
                + " - " + GetSaturdayDate(Globals.BeginDate.ToDateTimeSafe()).ToShortDateString();
        }

    }
}

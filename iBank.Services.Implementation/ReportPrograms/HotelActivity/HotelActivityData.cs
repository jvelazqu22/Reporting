using Domain.Helper;
using Domain.Models.ReportPrograms.HotelActivity;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.HotelActivity
{
    public class HotelActivityData
    {
        public List<FinalData> SortFinalData(string reportId, List<FinalData> tempList)
        {
            switch (reportId)
            {
                case "80": // ibHotelCity
                    return GetSortedFinalDataByHotelCity(tempList);
                case "82": // ibHotelAdvRes
                    return GetSortedFinalDataByHotelAdvanceReservation(tempList);
                case "84": // ibHotelVendor
                    return GetSortedFinalDataByHotelVendor(tempList);
                default: //86 ibHotelActivity
                    return GetSortedFinalDataByHotelActivity(tempList);
            }
        }

        public List<FinalData> GetSortedFinalDataByHotelCity(List<FinalData> tempList)
        {
            return tempList.OrderBy(x => x.Acct)
                .ThenBy(x => x.Break1)
                .ThenBy(x => x.Break2)
                .ThenBy(x => x.Break3)
                .ThenBy(x => x.Hotcity)
                .ThenBy(x => x.Hotstate)
                .ThenBy(x => x.Datein)
                .ThenBy(x => x.Hotelnam)
                .ThenBy(x => x.Passlast)
                .ThenBy(x => x.Passfrst).ToList();
        }

        public List<FinalData> GetSortedFinalDataByHotelAdvanceReservation(List<FinalData> tempList)
        {
            return tempList.OrderBy(x => x.Acctdesc)
                .ThenBy(x => x.Break1)
                .ThenBy(x => x.Break2)
                .ThenBy(x => x.Break3)
                .ThenBy(x => x.Hotcity)
                .ThenBy(x => x.Hotstate)
                .ThenBy(x => x.Datein)
                .ThenBy(x => x.Passlast)
                .ThenBy(x => x.Passfrst).ToList();
        }

        public List<FinalData> GetSortedFinalDataByHotelVendor(List<FinalData> tempList)
        {
            return tempList.OrderBy(x => x.Hotelnam)
                .ThenBy(x => x.Hotcity)
                .ThenBy(x => x.Hotstate)
                .ThenBy(x => x.Passlast)
                .ThenBy(x => x.Passfrst)
                .ThenBy(x => x.Datein).ToList();
        }

        public List<FinalData> GetSortedFinalDataByHotelActivity(List<FinalData> tempList)
        {
            return tempList.OrderBy(x => x.Acctdesc)
                .ThenBy(x => x.Break1)
                .ThenBy(x => x.Break2)
                .ThenBy(x => x.Break3)
                .ThenBy(x => x.Passlast)
                .ThenBy(x => x.Passfrst)
                .ThenBy(x => x.Datein)
                .ThenBy(x => x.Hotcity)
                .ThenBy(x => x.Hotstate).ToList();
        }

        public List<string> GetExportFields(ReportGlobals globals, UserBreaks userBreaks)
        {
            string reportId = globals.ProcessKey.ToString();
            var fieldList = new List<string>();

            switch (reportId)
            {
                case "80": // ibHotelCity
                    return GetHotelCityExportFields();
                case "82": // ibHotelAdvRes
                    return GetHotelAdvanceReservationExportFields(globals, userBreaks);
                case "84": // ibHotelVendor
                    return GetHotelVendorExportFields();
                default: //86 ibHotelActivity
                    return GetHotelActivityExportFields(globals, userBreaks);
            }
        }

        public List<string> GetHotelAdvanceReservationExportFields(ReportGlobals globals, UserBreaks userBreaks)
        {
            var fieldList = new List<string>();

            if (globals.User.AccountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (userBreaks.UserBreak1)
            {
                var break1 = string.IsNullOrEmpty(globals.User.Break1Name) ? "break_1" : globals.User.Break1Name;
                fieldList.Add($"break1 as {break1}");
            }
            if (userBreaks.UserBreak2)
            {
                var break2 = string.IsNullOrEmpty(globals.User.Break2Name) ? "break_2" : globals.User.Break2Name;
                fieldList.Add($"break2 as {break2}");
            }
            if (userBreaks.UserBreak3)
            {
                var break3 = string.IsNullOrEmpty(globals.User.Break3Name) ? "break_3" : globals.User.Break3Name;
                fieldList.Add($"break3 as {break3}");
            }

            fieldList.Add("hotcity");
            fieldList.Add("hotstate");
            fieldList.Add("weeknum");
            fieldList.Add("datein");
            fieldList.Add("hotelnam");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("recloc");
            fieldList.Add("roomtype");
            fieldList.Add("nights");
            fieldList.Add("rooms");
            fieldList.Add("bookrate");

            return fieldList;
        }

        public List<string> GetHotelCityExportFields()
        {
            var fieldList = new List<string>();

            fieldList.Add("hotcity");
            fieldList.Add("datein");
            fieldList.Add("hotelnam");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("recloc");
            fieldList.Add("roomtype");
            fieldList.Add("nights");
            fieldList.Add("rooms");
            fieldList.Add("bookrate");

            return fieldList;
        }

        public List<string> GetHotelVendorExportFields()
        {
            var fieldList = new List<string>();

            fieldList.Add("hotelnam");
            fieldList.Add("hotcity");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("recloc");
            fieldList.Add("datein");
            fieldList.Add("roomtype");
            fieldList.Add("nights");
            fieldList.Add("rooms");
            fieldList.Add("bookrate");

            return fieldList;
        }

        public List<string> GetHotelActivityExportFields(ReportGlobals globals, UserBreaks userBreaks)
        {
            var fieldList = new List<string>();

            if (globals.User.AccountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (userBreaks.UserBreak1)
            {
                var break1 = string.IsNullOrEmpty(globals.User.Break1Name) ? "break_1" : globals.User.Break1Name;
                fieldList.Add($"break1 as {break1}");
            }
            if (userBreaks.UserBreak2)
            {
                var break2 = string.IsNullOrEmpty(globals.User.Break2Name) ? "break_2" : globals.User.Break2Name;
                fieldList.Add($"break2 as {break2}");
            }
            if (userBreaks.UserBreak3)
            {
                var break3 = string.IsNullOrEmpty(globals.User.Break3Name) ? "break_3" : globals.User.Break3Name;
                fieldList.Add($"break3 as {break3}");
            }

            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("recloc");
            fieldList.Add("hotelnam");
            fieldList.Add("hotcityst");
            fieldList.Add("datein");
            fieldList.Add("roomtype");
            fieldList.Add("nights");
            fieldList.Add("rooms");
            fieldList.Add("bookrate");

            return fieldList;
        }
    }
}

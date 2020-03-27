using Domain.Helper;
using Domain.Models.ReportPrograms.CarFareSavings;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.CarFareSavings
{
    public class CarFareSavingsFinalData
    {
        public List<FinalData> GetFinalDataList(ReportGlobals Globals, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, bool deriveSavingCode,
            List<RawData> RawDataList, bool homeCountryBreak, UserBreaks _userBreaks, IMasterDataStore MasterStore, ClientFunctions clientFunctions)
        {
            var FinalDataList = new List<FinalData>();

            if (deriveSavingCode)
            {
                FinalDataList = GetFinalDataListUsingDeriveSavingCode(Globals, getAllMasterAccountsQuery, deriveSavingCode, RawDataList, homeCountryBreak,
                    _userBreaks, MasterStore, clientFunctions);
            }
            else
            {
                FinalDataList = GetFinalDataListWithoutUsingDeriveSavingCode(Globals, getAllMasterAccountsQuery, deriveSavingCode, RawDataList, homeCountryBreak,
                    _userBreaks, MasterStore, clientFunctions);
            }

            FinalDataList = FinalDataList.OrderBy(s => s.Homectry)
                .ThenBy(s => s.Acctdesc)
                .ThenBy(s => s.Break1)
                .ThenBy(s => s.Break2)
                .ThenBy(s => s.Break3)
                .ThenBy(s => s.Passlast)
                .ThenBy(s => s.Passfrst)
                .ThenBy(s => s.Reckey)
                .ToList();

            return FinalDataList;
        }

        public List<FinalData> GetFinalDataListUsingDeriveSavingCode(ReportGlobals Globals, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, bool deriveSavingCode,
            List<RawData> RawDataList, bool homeCountryBreak, UserBreaks _userBreaks, IMasterDataStore MasterStore, ClientFunctions clientFunctions)
        {
            return RawDataList.Select(s => new FinalData
            {
                Reckey = s.RecKey,
                Homectry = homeCountryBreak ? LookupFunctions.LookupHomeCountryName(s.SourceAbbr, Globals, MasterStore) : "^na^",
                Acct = Globals.User.AccountBreak ? s.Acct : "^na^",
                Acctdesc = Globals.User.AccountBreak ? clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, Globals) : "^na^",
                Break1 = _userBreaks.UserBreak1 ? string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1 : "^na^",
                Break2 = _userBreaks.UserBreak2 ? string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2 : "^na^",
                Break3 = _userBreaks.UserBreak3 ? string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(16) : s.Break3 : "^na^",
                Confirmno = s.ConfirmNo,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Invdate = s.Invdate ?? DateTime.MinValue,
                Rentdate = s.RentDate ?? DateTime.MinValue,
                Company = s.Company,
                Autocity = s.Autocity,
                Autostat = s.Autostat,
                Days = s.Cplusmin * s.Days,
                Cplusmin = s.Cplusmin,
                Cartype = s.Cartype,
                Carstdrate = Math.Abs(s.CarStdRate) < Math.Abs(s.ABookRat) || s.CarStdRate == 0 || (s.CarStdRate > 0 && s.ABookRat < 0)
                    ? s.ABookRat
                    : s.CarStdRate,
                Aexcprat = s.AExcpRat > 0 && s.ABookRat < 0
                    ? 0 - s.AExcpRat
                    : s.AExcpRat == 0 ? s.ABookRat : s.AExcpRat,
                Abookrat = s.ABookRat,
                Carsvgcode = s.CarSvgCode,
                Savings = 0,
                Reascode = s.Reascoda,
                Lostamt = 0,
                Lostpct = 0,
                Origacct = s.Acct,
                Sourceabbr = s.SourceAbbr,
                UdidLbl1 = new string(' ', 25),
                UdidLbl2 = new string(' ', 25),
                UdidLbl3 = new string(' ', 25),
                UdidLbl4 = new string(' ', 25),
                UdidLbl5 = new string(' ', 25),
                UdidLbl6 = new string(' ', 25),
                UdidLbl7 = new string(' ', 25),
                UdidLbl8 = new string(' ', 25),
                UdidLbl9 = new string(' ', 25),
                UdidLbl10 = new string(' ', 80),
                UdidText1 = new string(' ', 80),
                UdidText2 = new string(' ', 80),
                UdidText3 = new string(' ', 80),
                UdidText4 = new string(' ', 80),
                UdidText5 = new string(' ', 80),
                UdidText6 = new string(' ', 80),
                UdidText7 = new string(' ', 80),
                UdidText8 = new string(' ', 80),
                UdidText9 = new string(' ', 80),
                UdidText10 = new string(' ', 80)
            }).ToList();
        }

        public List<FinalData> GetFinalDataListWithoutUsingDeriveSavingCode(ReportGlobals Globals, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, bool deriveSavingCode,
            List<RawData> RawDataList, bool homeCountryBreak, UserBreaks _userBreaks, IMasterDataStore MasterStore, ClientFunctions clientFunctions)
        {
            return RawDataList.Select(s => new FinalData
            {
                Reckey = s.RecKey,
                Homectry = homeCountryBreak ? LookupFunctions.LookupHomeCountryName(s.SourceAbbr, Globals, MasterStore) : "^na^",
                Acct = Globals.User.AccountBreak ? s.Acct : "^na^",
                Acctdesc = Globals.User.AccountBreak ? clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, Globals) : "^na^",
                Break1 = _userBreaks.UserBreak1 ? string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1 : "^na^",
                Break2 = _userBreaks.UserBreak2 ? string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2 : "^na^",
                Break3 = _userBreaks.UserBreak3 ? string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(16) : s.Break3 : "^na^",
                Confirmno = s.ConfirmNo,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Invdate = s.Invdate ?? DateTime.MinValue,
                Rentdate = s.RentDate ?? DateTime.MinValue,
                Company = s.Company,
                Autocity = s.Autocity,
                Autostat = s.Autostat,
                Days = s.Cplusmin * s.Days,
                Cplusmin = s.Cplusmin,
                Cartype = s.Cartype,
                Carstdrate = s.CarStdRate > 0 && s.ABookRat < 0
                        ? 0 - s.CarStdRate
                        : s.CarStdRate,
                Aexcprat = s.AExcpRat > 0 && s.ABookRat < 0
                        ? 0 - s.AExcpRat
                        : s.AExcpRat,
                Abookrat = s.ABookRat,
                Carsvgcode = s.CarSvgCode,
                Savings = 0,
                Reascode = s.Reascoda,
                Lostamt = 0,
                Lostpct = 0,
                Origacct = s.Acct,
                Sourceabbr = s.SourceAbbr,
                UdidLbl1 = new string(' ', 25),
                UdidLbl2 = new string(' ', 25),
                UdidLbl3 = new string(' ', 25),
                UdidLbl4 = new string(' ', 25),
                UdidLbl5 = new string(' ', 25),
                UdidLbl6 = new string(' ', 25),
                UdidLbl7 = new string(' ', 25),
                UdidLbl8 = new string(' ', 25),
                UdidLbl9 = new string(' ', 25),
                UdidLbl10 = new string(' ', 80),
                UdidText1 = new string(' ', 80),
                UdidText2 = new string(' ', 80),
                UdidText3 = new string(' ', 80),
                UdidText4 = new string(' ', 80),
                UdidText5 = new string(' ', 80),
                UdidText6 = new string(' ', 80),
                UdidText7 = new string(' ', 80),
                UdidText8 = new string(' ', 80),
                UdidText9 = new string(' ', 80),
                UdidText10 = new string(' ', 80)
            }).ToList();
        }

        public List<FinalData> UpdateFinalDataList(List<FinalData> FinalDataList, ReportGlobals Globals, bool deriveSavingCode)
        {
            foreach (var row in FinalDataList)
            {
                if (Globals.AgencyInformation.ReasonExclude.Contains(row.Reascode))
                    row.Reascode = string.Empty;

                row.Lostamt = Math.Abs(row.Days) * (row.Abookrat - row.Aexcprat);
                row.Savings = Math.Abs(row.Days) * (row.Carstdrate - row.Abookrat);

                if (row.Abookrat != 0 && row.Lostamt != 0 && row.Days != 0)
                    row.Lostpct = Math.Round(100 * (row.Lostamt / Math.Abs(row.Days) * row.Abookrat), 2);

                if (deriveSavingCode && string.IsNullOrEmpty(row.Carsvgcode) && !string.IsNullOrEmpty(row.Reascode) && row.Lostamt == 0 && row.Savings > 0)
                {
                    row.Carsvgcode = row.Reascode;
                    row.Reascode = string.Empty;
                }

                if ((row.Lostamt < 0 && row.Cplusmin > 0) || (row.Lostamt > 0 && row.Cplusmin < 0))
                    row.Lostamt = 0;
            }

            return FinalDataList;
        }
    }
}

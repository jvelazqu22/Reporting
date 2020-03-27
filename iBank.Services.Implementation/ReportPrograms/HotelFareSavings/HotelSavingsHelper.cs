using Domain.Helper;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Utilities;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.HotelFareSavings
{
    public class HotelSavingsHelper
    {
        public List<string> GetExportFields(bool acctBrk, UserBreaks userBreaks, ReportGlobals Globals)
        {
            var fieldList = new List<string>();

            if (acctBrk)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (userBreaks.UserBreak1)
                fieldList.Add("break1 as " + Globals.User.Break1Name);
            if (userBreaks.UserBreak2)
                fieldList.Add("break2 as " + Globals.User.Break2Name);
            if (userBreaks.UserBreak3)
                fieldList.Add("break3 as " + Globals.User.Break3Name);
            if (Globals.IsParmValueOn(WhereCriteria.CBPGBRKHOMECTRY))
                fieldList.Add("HomeCtry");

            fieldList.Add("confirmNo");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("reckey");
            fieldList.Add("invdate");
            fieldList.Add("datein");
            fieldList.Add("hotelnam");
            fieldList.Add("hotcity");
            fieldList.Add("hotstate");
            fieldList.Add("nights");
            fieldList.Add("rooms");
            fieldList.Add("roomtype");
            if (!Globals.IsParmValueOn(WhereCriteria.CBEXCLPUBFARE))
                fieldList.Add("hotstdrate");
            fieldList.Add("hexcprat");
            fieldList.Add("bookrate");
            fieldList.Add("hotsvgcode");
            fieldList.Add("savings");            
            fieldList.Add("reascodh");
            fieldList.Add("lostamt");
            if (Globals.IsParmValueOn(WhereCriteria.CBINCLPCTLOSS))                
                fieldList.Add("lostpct");

            return fieldList;
        }

        public List<string> AddUdidsToFieldList(ReportGlobals Globals, List<string> fieldList)
        {
            var udids = UdidLoader.GatherUdids(Globals);
            for (int i = 1; i <= udids.Count; i++)
            {
                switch (i)
                {
                    case 1:
                        fieldList.Add("UdidText1 as " + udids[i - 1].Item3.Replace(" ", "_"));
                        break;
                    case 2:
                        fieldList.Add("UdidText2 as " + udids[i - 1].Item3.Replace(" ", "_"));
                        break;
                    case 3:
                        fieldList.Add("UdidText3 as " + udids[i - 1].Item3.Replace(" ", "_"));
                        break;
                    case 4:
                        fieldList.Add("UdidText4 as " + udids[i - 1].Item3.Replace(" ", "_"));
                        break;
                    case 5:
                        fieldList.Add("UdidText5 as " + udids[i - 1].Item3.Replace(" ", "_"));
                        break;
                    case 6:
                        fieldList.Add("UdidText6 as " + udids[i - 1].Item3.Replace(" ", "_"));
                        break;
                    case 7:
                        fieldList.Add("UdidText7 as " + udids[i - 1].Item3.Replace(" ", "_"));
                        break;
                    case 8:
                        fieldList.Add("UdidText8 as " + udids[i - 1].Item3.Replace(" ", "_"));
                        break;
                    case 9:
                        fieldList.Add("UdidText9 as " + udids[i - 1].Item3.Replace(" ", "_"));
                        break;
                    case 10:
                        fieldList.Add("UdidText10 as " + udids[i - 1].Item3.Replace(" ", "_"));
                        break;
                }
            }
            return fieldList;
        }
    }
}

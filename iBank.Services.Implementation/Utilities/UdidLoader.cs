using System;
using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.ReportPrograms.CarFareSavings;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms;

namespace iBank.Services.Implementation.Utilities
{
    public static class UdidLoader
    {
        public static void LoadUdids<T,TU>(List<T> dataList, ReportRunner<TU,T> rpt, SqlScript script) where T : class, IUdidData
        {
            var udids = GatherUdids(rpt.Globals);

            if (udids.Any())
            {
                var udidData = rpt.RetrieveRawData<UdidData>(script, rpt.GlobalCalc.IsReservationReport(), false, false).ToList();
                var udidsGrouped = udidData.GroupBy(s => new {s.RecKey, s.UdidNo, s.UdidText});
                foreach (var group in udidsGrouped)
                {
                    //get the label 
                    var udid = udids.FirstOrDefault(s => s.Item2 == group.Key.UdidNo);
                    if (udid == null) continue;

                    var label = udid.Item3;
                    var key = group.Key.RecKey;
                    var val = group.Key.UdidText;

                    //get the records that match the reckey
                    var recs = dataList.Where(s => s.Reckey == key);
                    foreach (var rec in recs)
                    {
                        switch (udid.Item1)
                        {
                            case 1:
                                rec.UdidLbl1 = label;
                                rec.UdidText1 = val;
                                break;
                            case 2:
                                rec.UdidLbl2 = label;
                                rec.UdidText2 = val;
                                break;
                            case 3:
                                rec.UdidLbl3 = label;
                                rec.UdidText3 = val;
                                break;
                            case 4:
                                rec.UdidLbl4 = label;
                                rec.UdidText4 = val;
                                break;
                            case 5:
                                rec.UdidLbl5 = label;
                                rec.UdidText5 = val;
                                break;
                            case 6:
                                rec.UdidLbl6 = label;
                                rec.UdidText6 = val;
                                break;
                            case 7:
                                rec.UdidLbl7 = label;
                                rec.UdidText7 = val;
                                break;
                            case 8:
                                rec.UdidLbl8 = label;
                                rec.UdidText8 = val;
                                break;
                            case 9:
                                rec.UdidLbl9 = label;
                                rec.UdidText9 = val;
                                break;
                            case 10:
                                rec.UdidLbl10 = label;
                                rec.UdidText10 = val;
                                break;
                        }
                        
                    }
                }
            }
        }

        public static List<Tuple<int, int, string>> GatherUdids(ReportGlobals globals)
        {
            var udids = new List<Tuple<int, int, string>>();

            var udidNumber = globals.GetParmValue(WhereCriteria.UDIDONRPT1).TryIntParse(-1);
            if (udidNumber > 0)
            {
                string lable = globals.GetParmValue(WhereCriteria.UDIDLBL1);
                if (lable.Equals(string.Empty)) lable = $"UdidTxt:{udidNumber}";
                udids.Add(new Tuple<int, int, string>(1, udidNumber, lable));
            }

            udidNumber = globals.GetParmValue(WhereCriteria.UDIDONRPT2).TryIntParse(-1);
            if (udidNumber > 0)
            {
                string lable = globals.GetParmValue(WhereCriteria.UDIDLBL2);
                if (lable.Equals(string.Empty)) lable = $"UdidTxt:{udidNumber}";
                udids.Add(new Tuple<int, int, string>(2, udidNumber, lable));
            }

            udidNumber = globals.GetParmValue(WhereCriteria.UDIDONRPT3).TryIntParse(-1);
            if (udidNumber > 0)
            {
                string lable = globals.GetParmValue(WhereCriteria.UDIDLBL3);
                if (lable.Equals(string.Empty)) lable = $"UdidTxt:{udidNumber}";
                udids.Add(new Tuple<int, int, string>(3,udidNumber, lable));
            }

            udidNumber = globals.GetParmValue(WhereCriteria.UDIDONRPT4).TryIntParse(-1);
            if (udidNumber > 0)
            {
                string lable = globals.GetParmValue(WhereCriteria.UDIDLBL4);
                if (lable.Equals(string.Empty)) lable = $"UdidTxt:{udidNumber}";
                udids.Add(new Tuple<int, int, string>(4,udidNumber, lable));
            }

            udidNumber = globals.GetParmValue(WhereCriteria.UDIDONRPT5).TryIntParse(-1);
            if (udidNumber > 0)
            {
                string lable = globals.GetParmValue(WhereCriteria.UDIDLBL5);
                if (lable.Equals(string.Empty)) lable = $"UdidTxt:{udidNumber}";
                udids.Add(new Tuple<int, int, string>(5,udidNumber, lable));
            }

            udidNumber = globals.GetParmValue(WhereCriteria.UDIDONRPT6).TryIntParse(-1);
            if (udidNumber > 0)
            {
                string lable = globals.GetParmValue(WhereCriteria.UDIDLBL6);
                if (lable.Equals(string.Empty)) lable = $"UdidTxt:{udidNumber}";
                udids.Add(new Tuple<int, int, string>(6,udidNumber, lable));
            }

            udidNumber = globals.GetParmValue(WhereCriteria.UDIDONRPT7).TryIntParse(-1);
            if (udidNumber > 0)
            {
                string lable = globals.GetParmValue(WhereCriteria.UDIDLBL7);
                if (lable.Equals(string.Empty)) lable = $"UdidTxt:{udidNumber}";
                udids.Add(new Tuple<int, int, string>(7,udidNumber, lable));
            }

            udidNumber = globals.GetParmValue(WhereCriteria.UDIDONRPT8).TryIntParse(-1);
            if (udidNumber > 0)
            {
                string lable = globals.GetParmValue(WhereCriteria.UDIDLBL8);
                if (lable.Equals(string.Empty)) lable = $"UdidTxt:{udidNumber}";
                udids.Add(new Tuple<int, int, string>(8,udidNumber, lable));
            }

            udidNumber = globals.GetParmValue(WhereCriteria.UDIDONRPT9).TryIntParse(-1);
            if (udidNumber > 0)
            {
                string lable = globals.GetParmValue(WhereCriteria.UDIDLBL9);
                if (lable.Equals(string.Empty)) lable = $"UdidTxt:{udidNumber}";
                udids.Add(new Tuple<int, int, string>(9,udidNumber, lable));
            }

            udidNumber = globals.GetParmValue(WhereCriteria.UDIDONRPT10).TryIntParse(-1);
            if (udidNumber > 0)
            {
                string lable = globals.GetParmValue(WhereCriteria.UDIDLBL10);
                if (lable.Equals(string.Empty)) lable = $"UdidTxt:{udidNumber}";
                udids.Add(new Tuple<int, int, string>(10,udidNumber, lable));
            }

            return udids;
        }
    }
}

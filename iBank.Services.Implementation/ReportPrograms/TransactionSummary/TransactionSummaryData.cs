using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;
using Domain.Models.TransactionSummary;
using System.Linq;

using Domain.Models.ReportPrograms.TransactionSummary;

namespace iBank.Services.Implementation.ReportPrograms.TransactionSummary
{
    public class TransactionSummaryData
    {
        public List<string> GetExportFields(ReportGlobals Globals)
        {
            var fieldList = new List<string>();
            if (Globals.IsParmValueOn(WhereCriteria.CBBREAKBYSOURCE))
            {
                fieldList.Add("SourceDesc");
                fieldList.Add("SourceAbbr");
            }

            fieldList.Add("Year");
            fieldList.Add("MthName");
            if (Globals.IsParmValueOn(WhereCriteria.CBBREAKBYSOURCE))
            {
                fieldList.Add("Acct");
                fieldList.Add("AcctDesc");
            }
            fieldList.Add("PrevCount");
            fieldList.Add("HistCount");
            fieldList.Add("BothCount");
            fieldList.Add("TrackerCnt");
            fieldList.Add("ChgMgmtCnt");

            if (Globals.ParmValueEquals(WhereCriteria.OUTPUTTYPE, "2") || Globals.ParmValueEquals(WhereCriteria.OUTPUTTYPE, "5"))
            {
                fieldList.Add("PCMCount");
            }else
            {
                //show as pcmcount, but value is AuthCount
                var authCol = "AuthCount as PCMCount";
                fieldList.Add(authCol);
            }

            if (Globals.Agency == "AXI")
            {
                fieldList.Add("AuthCount");
            }

            return fieldList;
        }

        public List<FinalData> SortFinalData(List<FinalData> FinalDataList, bool logGen1, int begYearMonth, int endYearMonth)
        {
            if (logGen1)
            {
                FinalDataList = FinalDataList.Where(s => s.YearMth >= begYearMonth && s.YearMth <= endYearMonth)
                    .OrderBy(s => s.SourceDesc)
                    .ThenBy(s => s.SourceAbbr)
                    .ThenBy(s => s.YearMth)
                    .ThenBy(s => s.AcctDesc)
                    .ToList();
            }
            else
            {
                FinalDataList = FinalDataList.Where(s => s.YearMth >= begYearMonth && s.YearMth <= endYearMonth)
                    .OrderBy(s => s.YearMth)
                    .ThenBy(s => s.AcctDesc)
                    .ToList();
            }
            return FinalDataList;
        }
    }
}

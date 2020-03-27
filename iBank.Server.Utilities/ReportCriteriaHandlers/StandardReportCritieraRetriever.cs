using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using iBank.Server.Utilities.Classes;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Server.Utilities.ReportCriteriaHandlers
{
    public class StandardReportCritieraRetriever
    {
        public IList<ReportCriteria> GetReportCriteriaFromReportId(IQuery<IList<reporthandoff>> getReportCriteriaByReportIdQuery)
        {
            var reportCrit = getReportCriteriaByReportIdQuery.ExecuteQuery()
                                             .Select(x => new ReportCriteria
                                             {
                                                 VarName = x.parmname.ToUpper().Trim(),
                                                 VarValue = x.parmvalue.Trim().Replace("\n", string.Empty)
                                             })
                                             .OrderBy(x => x.VarName)
                                             .ToList();

            return reportCrit.Distinct(new CriteriaComparer()).ToList();
        }

        private readonly IList<string> _parametersThatNeedTwoAppended = new List<string> { "RPTTITLE", "TITLEACCT" };
        public IList<ReportCriteria> GetReportCriteriaFromSaved2(IQuery<IList<savedrpt2>> getSavedReport2ByRecordLinkQuery)
        {
            var savedRpt2 = getSavedReport2ByRecordLinkQuery.ExecuteQuery();
            
            if (savedRpt2 == null) throw new Exception("Record not found for saved report 2");

            savedRpt2 = FixSavedRpt2Names(savedRpt2);

            return savedRpt2.Select(s => new ReportCriteria
            {
                VarName = s.VarName.ToUpper().Trim(),
                VarValue = s.VarValue.Trim()
            }).ToList();
        }

        private IList<savedrpt2> FixSavedRpt2Names(IList<savedrpt2> savedRpt2Data)
        {
            foreach (var val in savedRpt2Data)
            {
                if (_parametersThatNeedTwoAppended.Contains(val.VarName.ToUpper().Trim()))
                {
                    val.VarName = val.VarName.ToUpper().Trim() + "2";
                }
            }

            return savedRpt2Data.DistinctBy(x => x.VarName.ToUpper().Trim()).ToList();
        }

        public Dictionary<int, ReportCriteria> GenerateStandardCriteria(Dictionary<int, string> whereCriteria, IList<ReportCriteria> reportCriteria)
        {
            return whereCriteria.Join(reportCriteria.DefaultIfEmpty(), w => w.Value, r => r.VarName.Trim(),
                                        (w, r) => new
                                        {
                                            w.Key,
                                            Name = r.VarName,
                                            Value = r.VarValue
                                        })
                  .ToDictionary(k => k.Key, v => new ReportCriteria { VarName = v.Name.Trim(), VarValue = v.Value.Trim() });
        }

        public int GetUserNumberFromReportCriteria(IList<ReportCriteria> reportCriteria)
        {
            var critRow = reportCriteria.FirstOrDefault(s => s.VarName.Trim().EqualsIgnoreCase("USERNBR"));
            if (critRow != null)
            {
                return critRow.VarValue.TryIntParse(0);
            }

            return 0;
        }

        public int GetReportLogKeyFromReportCriteria(IList<ReportCriteria> reportCriteria)
        {
            var critRow = reportCriteria.FirstOrDefault(s => s.VarName.Trim().EqualsIgnoreCase("REPORTLOGKEY"));
            if (critRow != null)
            {
                return critRow.VarValue.TryIntParse(0);
            }

            return 0;
        }

        public string GetTestReportNameFromReportCriteria(IList<ReportCriteria> reportCriteria)
        {
            var critRow = reportCriteria.FirstOrDefault(s => s.VarName.Trim().EqualsIgnoreCase("OUTPUTNAME"));
            if (critRow != null)
            {
                var value = critRow.VarValue.Trim();
                return string.IsNullOrEmpty(value) ? string.Empty : value;
            }

            return string.Empty;
        }

        public string GetTestReportPathFromReportCriteria(IList<ReportCriteria> reportCriteria)
        {
            var critRow = reportCriteria.FirstOrDefault(s => s.VarName.Trim().EqualsIgnoreCase("OUTPUTPATH"));
            if (critRow != null)
            {
                var value = critRow.VarValue.Trim();
                return string.IsNullOrEmpty(value) ? string.Empty : value;
            }

            return string.Empty;
        }
    }
}

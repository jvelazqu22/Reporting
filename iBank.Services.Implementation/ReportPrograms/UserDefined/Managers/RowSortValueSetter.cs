using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class RowSortValueSetter
    {

        //this is to try keep the same trip together, adding sort column to concatenate string by the columns by order 
        //that has marked as sort checked, then the columns by order that doesn't have sort checked
        /*  colname	 colorder	sort
            RECLOC    	4	    0
            FAREBASE  	5	    0
            ACOMMISN  	6	    0
            TICKET    	7	    0
            ACCT      	1	    0
            VALCARR   	2	    1
            SEGROUTING	3	    0
            
            so the sort field value order would be: {VALCARR} {ACCT} {SEGROUTING} {RECLOC} {FAREBASE} {ACOMMISN} {TICKET} all in string.
            We only use up to 8 columns. This may need to change when we know for customer behavior.
            
        */
        public string SortValue(List<string> row, List<UserReportColumnInformation> columns)
        {
            var result = string.Empty;
            int upLimit = 8; //this can be change to more or less

            var sortColumns = columns.Where(s => s.Sort > 0 ).OrderBy(s => s.Sort).ToList();
            if (sortColumns.Any())
            {
                for (var i = 0; i < sortColumns.Count; i++)
                {
                    var order = sortColumns[i].Order == 0 ? 0 : sortColumns[i].Order - 1;
                    if (order >= row.Count) continue;
                    result += $"{row[order]} ";
                    upLimit--;
                    if (upLimit == 0) break;
                }
            }

            //then columns that has no checked sort. Only check the first letter
            sortColumns = columns.Where(s => s.Sort == 0).OrderBy(s => s.Order).ToList();
             if (sortColumns.Any())
            {
                for (var i = 0; i < row.Count - 1; i++)
                {
                    var order = sortColumns[i].Order == 0 ? 0 : sortColumns[i].Order - 1;
                    if (order >= row.Count) continue;
                    result += $"{row[order]} ";
                    upLimit--;
                    if (upLimit == 0) break;
                }
            }

            return result;
        }

        //this is to try keep the same trip together, adding sort column to concatenate string by the columns by order 
        //that has marked as sort checked, then the columns by order that doesn't have sort checked
        /*  colname	 colorder	sort
            RECLOC    	4	    0
            FAREBASE  	5	    0
            ACOMMISN  	6	    0
            TICKET    	7	    0
            ACCT      	1	    0
            VALCARR   	2	    1
            SEGROUTING	3	    0
            RECKEY (increament after last sort)
            LEGCNTR (increament 1 after RECKEY)
            so the sort field value order would be: {VALCARR} {RECKEY} {LEGCNTR} all in string.
            We only use up to 8 columns. This may need to change when we know for customer behavior.
            
        */
        public string SortValueWithLegCntr(List<string> row, List<UserReportColumnInformation> columns)
        {
            var result = string.Empty;
            int upLimit = 8; //this can be change to more or less

            var sortColumns = columns.Where(s => s.Sort > 0).OrderBy(s => s.Sort).ToList();
            if (sortColumns.Any())
            {
                for (var i = 0; i < sortColumns.Count; i++)
                {
                    var order = sortColumns[i].Order == 0 ? 0 : sortColumns[i].Order - 1;
                    if (order >= row.Count) continue;
                    result += $"{row[order]} ";
                    upLimit--;
                    if (upLimit == 0) break;
                }
            }
     
            return result;
        }
    }
}

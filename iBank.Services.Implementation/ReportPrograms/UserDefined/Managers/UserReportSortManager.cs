using Domain.Helper;
using iBank.Services.Implementation.Shared;

using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class UserReportSortManager
    {
        private List<List<string>> _rows;
        private List<UserReportColumnInformation> _columns;
        private DestinationSwitch _outputFormat;

        public UserReportSortManager(List<List<string>> rows, List<UserReportColumnInformation> columns, DestinationSwitch outputFormat)
        {
            _rows = rows;
            _columns = columns;
            _outputFormat = outputFormat;
        }

        public List<List<string>> Sort()
        {
            IOrderedEnumerable<List<string>> sortTemp;
            var order = 0;
            //use the sort column to sort to determinte on the same trip
            //PDF has a visibility indication (True or False in last column)
            if (_outputFormat == DestinationSwitch.ClassicPdf)
            {
                order = _rows[0].Count - 2;
            }
            else
            {
                order = _rows[0].Count - 1;
            }
            sortTemp = _rows.OrderBy(s => s[order]);


            var sortColumns = _columns.Where(s => s.Sort > 0).OrderBy(s => s.Sort).ToList();
            if (sortColumns.Any())
            {                
                //run over the remaining sort columns, if any (thenby)
                for (int i = 1; i < sortColumns.Count; i++)
                {
                    var sortColNumber = sortColumns[i].Order - 1;
                    switch (sortColumns[i].ColumnType.Trim().ToUpper())
                    {
                        case "CURRENCY":
                            sortTemp = sortTemp.ThenBy(s => s[sortColNumber], new DecimalComparer());
                            break;
                        case "NUMERIC":
                            sortTemp = sortTemp.ThenBy(s => s[sortColNumber], new NumericComparer());
                            break;
                        case "DATE":
                        case "DATETIME":
                            sortTemp = sortTemp.ThenBy(s => s[sortColNumber], new DateComparer());
                            break;
                        default:
                            sortTemp = sortTemp.ThenBy(s => s[sortColNumber]);
                            break;
                    }
                }
            }

            return sortTemp.ToList();
        }

    }
}

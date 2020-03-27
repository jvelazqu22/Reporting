using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;

namespace iBank.Services.Implementation.Utilities.ExportHelpers
{
    public static class StringListToCsvHandler
    {
        private const string Quote = "\"";
        private const string Comma = ",";

        private static List<string> GetFieldList(List<UserReportColumnInformation> colList)
        {
            var fieldList = new List<string>();
            for (var i = 0; i < colList.Count; i++)
            {
                var header1 = colList[i].Header1.NormalizeColumnHeader();
                var header2 = colList[i].Header2.NormalizeColumnHeader();
                var header = header1 == "" ?
                    header2 :
                    header2 != "" ?
                        (header1 + "_" + header2) :
                        header1;
                fieldList.Add(header);
            }
            return fieldList;
        }

        public static void StringListToCsv(string filename, IReadOnlyList<List<string>> collection, List<UserReportColumnInformation> colList, ReportGlobals globals, string separator = "", bool includeHeaders = true)
        {
            var fieldList = GetFieldList(colList);
            var typeList = colList.Select(s => s.ColumnType).ToList();
            var tableList = colList.Select(s => s.TableName).ToList();

            if (collection == null || collection.Count <= 0) return;

            var suppressHandler = new TripFieldsSuppressHandler(globals);

            var currentCol = 0;
            using (TextWriter writer = File.CreateText(filename))
            {
                int pos;
                if (includeHeaders)
                {
                    var header = string.Empty;

                    foreach (var field in fieldList)
                    {
                        var fieldName = field;
                        var asIndex = fieldName.IndexOf(" AS ", StringComparison.OrdinalIgnoreCase);
                        if (asIndex >= 0)
                        {
                            fieldName = field.Substring(asIndex + 4);
                        }
                        currentCol += 1;
                        header += separator + fieldName.NormalizeColumnHeader() + separator + Comma;
                    }

                    pos = header.Length - (separator.Length + 1);
                    header = header.Remove(pos + 1, separator.Length);
                    //Remove the blank line 
                    //header += Crlf;
                    writer.WriteLine(header);
                }

                foreach (var row in collection)
                {
                    currentCol = 0;
                    var line = string.Empty;

                    foreach (var item in row)
                    {
                        var val = suppressHandler.GetDataAccordingToTheSuppressType(item.Trim(), fieldList[currentCol], typeList[currentCol], tableList[currentCol], row[row.Count - 1] == "1");

                        if (val.Contains(Comma))
                        {
                            val = Quote + val + Quote;
                        }

                        currentCol = currentCol + 1;
                        line += separator + val + separator + Comma;

                    }

                    pos = line.Length - (separator.Length + 1);
                    line = line.Remove(pos + 1, separator.Length);
                    writer.WriteLine(line);
                }

            }
        }

    }

}

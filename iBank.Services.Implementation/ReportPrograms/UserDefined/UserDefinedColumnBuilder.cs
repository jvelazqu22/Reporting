using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.ciswired.libraries.CISLogger;
using Domain;
using Domain.Helper;

using iBank.Entities.MasterEntities;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class UserDefinedColumnBuilder
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<UserReportColumnInformation> BuildColumns(ReportGlobals globals, UserReportInformation userReport, UserDefinedReportCarbonManager carbonManager, IList<collist2> columnList)
        {
            using (var timer = new CustomTimer(globals.ProcessKey, globals.UserNumber, globals.Agency, globals.ReportLogKey, LOG, "BuildColumns",  userReport.ReportKey))
            {
                var finalColumns = new List<UserReportColumnInformation>();
                LOG.Debug($"BuildColumns - Looping userReport.Columns");
                foreach (var column in userReport.Columns)
                {
                    if (!globals.ParmHasValue(WhereCriteria.CARBONCALC))
                    {
                        //if the current field is a Carbon field, and we aren't doing Carbon, skip it. 
                        if (carbonManager.IsCarbonColumn(column.Name)) continue;
                    }
                    
                    var columnConstructor = new ColumnConstructor(column);
                    var newColumn = columnConstructor.GetNewColumn();
                    
                    //check to see if the column is a udid...these don't have matching records in collist2. 
                    if (columnConstructor.IsUdidField || columnConstructor.IsUserLabeledUdid)
                    {
                        //We want to keep the UDID name the way it is, example UDID10, so it will work properly handled in UserDefinedLookupManager.HandleLookupFieldTrip and SwitchManager.UdidSwitch 
                        newColumn.Name = column.Name;
                        columnConstructor.AssignHeaders(newColumn, globals);
                        newColumn.TableName = "TRIPS";
                        //Set it up so it can be trigger in ReportLookups.SetUdids
                        newColumn.LookupTable = globals.IsReservationReport() ? "IBUDIDS" : "HIBUDIDS";
                        newColumn.Usage = "BOTH";
                        newColumn.ColumnType = columnConstructor.GetUdidFieldColumnType();
                        newColumn.Width = column.Width;
                    }
                    else
                    {
                        var currentColumn = columnConstructor.GetCurrentColumnFromCollist(userReport.ReportType, columnList);

                        columnConstructor.AssignHeaders(newColumn, globals, currentColumn);
                        newColumn.Width = column.Width > 0 ? column.Width : currentColumn.collngth;
                        newColumn.TableName = currentColumn.coltable.Trim();
                        newColumn.Usage = currentColumn.usage.Trim();
                        newColumn.TotalThisField = currentColumn.subtotal;
                        newColumn.LookupTable = currentColumn.lookuptbl.Trim();
                        newColumn.ColumnType = currentColumn.coltype.Trim();

                        newColumn.SuppressDuplicates = columnConstructor.IsSuppressDuplicates(newColumn.TableName, newColumn.ColumnType, globals.GetParmValue(WhereCriteria.DDSUPPDUPETRIPFLDS));
                        LOG.Debug($"BuildColumns - {newColumn.TableName} SuppressDuplicates:[{newColumn.SuppressDuplicates}]");
                    }
                    
                    if (columnConstructor.NeedToTranslateToNewColumnNames(globals, userReport.ReportType))
                    {
                        if (UserReportCheckLists.OldCols.Contains(newColumn.Name))
                        {
                            //drop the S
                            newColumn.Name = newColumn.Name.RemoveFirstChar();
                            //Skip if the column already exists
                            if (userReport.Columns.Any(s => s.Name.EqualsIgnoreCase(newColumn.Name))) continue;
                            newColumn.TableName = "TRIPS";
                        }
                    }
                    newColumn.HorizontalAlignment = column.HorizontalAlignment;

                    finalColumns.Add(newColumn);
                }
                LOG.Debug($"BuildColumns - {finalColumns.Count} columns added");

                //Always add reckey for car, hotel etc filters, if xlsx, cvs need to add more
                AddAdditionalColumns(globals.OutputFormat, globals.ProcessKey, finalColumns);
               
                LOG.Debug($"BuildColumns - {finalColumns.Count} columns after AddAdditionalColumns");

                return finalColumns;
            }
        }       
        
        public void AddAdditionalColumns(DestinationSwitch outputFormat, int processKey, List<UserReportColumnInformation> columns)
        {
            //assume no column has set sort
            var reckeySort = 1;

            //honor the sort the user selected. remove the sort on reckey 
            var sortColumns = columns.Where(s => s.Sort > 0).OrderBy(s => s.Sort).ToList();
            if (sortColumns.Any()) reckeySort = sortColumns.Count + 1;

            columns.Add(GetDefaultNumericColumn("RECKEY", "TRIPS", "RECKEY", columns.Count + 1, reckeySort));
            if (outputFormat == DestinationSwitch.Xls || outputFormat == DestinationSwitch.Csv)
            {
                if (processKey == (int)ReportTitles.CombinedUserDefinedReports)
                {
                    if (!columns.Any(s => s.Name == "PLUSMIN"))
                    {
                        columns.Add(GetDefaultNumericColumn("PLUSMIN", "TRIPS", "PLUSMIN", columns.Count + 1));
                    }
                    columns.Add(GetDefaultNumericColumn("LEGPLUSMIN", "DUMMY", "LEGPLUSMIN", columns.Count + 1));
                }
                columns.Add(GetDefaultNumericColumn("LEGCNTR", "DUMMY", "LEGCNTR", columns.Count + 1, reckeySort++));
            }
            else
            {
                columns.Add(GetDefaultNumericColumn("LEGCNTR", "DUMMY", "LEGCNTR", columns.Count + 1, reckeySort++));
            }
            columns.Add(GetHiddenSortColumn("SORT", "DUMMY", "SORT", columns.Count + 1, reckeySort));

        }

        private UserReportColumnInformation GetDefaultNumericColumn(string name, string tableName, string columnName, int order, int sort=0)
        {
            return new UserReportColumnInformation
            {
                Name = name,
                Header1 = "",
                Header2 = columnName,
                Width = 5,
                TableName = tableName,
                Usage = "",
                TotalThisField = false,
                LookupTable = "",
                ColumnType = "NUMERIC",
                Order = order,
                Sort = sort,
                GoodFieldType = "NUMERIC",
                BadFieldType = "NUMERIC"

            };
        }

        private UserReportColumnInformation GetHiddenSortColumn(string name, string tableName, string columnName, int order, int sort = 0)
        {
            return new UserReportColumnInformation
            {
                Name = name,
                Header1 = "",
                Header2 = columnName,
                Width = 5,
                TableName = tableName,
                Usage = "",
                TotalThisField = false,
                LookupTable = "",
                ColumnType = "TEXT",
                Order = order,
                Sort = sort,
                GoodFieldType = "TEXT",
                BadFieldType = "TEXT"

            };
        }
    }
}

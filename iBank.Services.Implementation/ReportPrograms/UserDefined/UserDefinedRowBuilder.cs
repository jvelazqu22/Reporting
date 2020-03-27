using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using com.ciswired.libraries.CISLogger;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using Domain;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class UserDefinedRowBuilder
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        

        private int GetMaxThreads()
        {
#if DEBUG
            return 1;
#else
            return 10;
#endif
        }

        public ReportRowHelper BuildReportRows(UserDefinedParameters userDefinedParams, bool originalIncludeVoids, ReportGlobals globals, UserReportInformation userReport, 
            BuildWhere buildWhere, bool isPreview, AdvancedParameters advancedParameters, SwitchManager switchManager, ColumnValueRulesFactory factory)
        {
            using (var timer = new CustomTimer(globals.ProcessKey, globals.UserNumber, globals.Agency, globals.ReportLogKey, LOG, "BuildReportRows", userReport.ReportKey))
            {
                //used to track which reckeys we have used so far to avoid duplicates -- the value field doesn't matter but HashSet is not thread safe so we use this structure
                var usedReckeys = new ConcurrentDictionary<int, byte>();
                usedReckeys.TryAdd(0, 0);

                var rowCount = 0;
                var lookupHelper = new UserDefinedLookupManager(userDefinedParams, globals.ParmValueEquals(WhereCriteria.DDTIMEFORMAT, "2"), globals, buildWhere, userReport.Columns, userReport.SegmentOrLeg);
                var isReservationReport = globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
                var colorManager = new ColorManager();

                var udidExists = globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1) > 0;
                var recKeyList = userDefinedParams.TripDataList.Select(s => s.RecKey).ToList();
                lookupHelper.LoadReportSpecificLookups(userReport.Columns, isReservationReport, switchManager, udidExists, recKeyList);

                var rowsToProcess = userDefinedParams.TripDataList.Count;
                var rowsProcessed = 0;
                LOG.Debug($"Processing [{rowsToProcess}] rows.");

                var rowAgg = new List<List<string>>();
                var obj = new object();
                Parallel.ForEach(userDefinedParams.TripDataList.ToBatch(30000), new ParallelOptions { MaxDegreeOfParallelism = GetMaxThreads() }, chunk =>
                {
                    try
                    {
                        var bag = new ConcurrentBag<List<string>>();
                        Parallel.ForEach(chunk, new ParallelOptions { MaxDegreeOfParallelism = GetMaxThreads() }, row =>
                        {
                            Interlocked.Increment(ref rowsProcessed);
                            if (rowsProcessed % 5000 == 0) LOG.Debug($"[{rowsProcessed}] of [{rowsToProcess}]");

                            //ignore duplicate trip records //ServiceFee user defined report.
                            if (usedReckeys.TryAdd(row.RecKey, 0))
                            {
                                //if we don't want to include voids, and this is a void row
                                if (!originalIncludeVoids && RowVoidHandler.IsVoid(row))
                                {
                                    if (RowVoidHandler.AllOtherDataIsVoid(userDefinedParams.CarLookup[row.RecKey].ToList(),
                                            userDefinedParams.HotelLookup[row.RecKey].ToList(), userDefinedParams.ServiceFeeLookup[row.RecKey].ToList()))
                                    {
                                        //skip this row
                                        LOG.Debug($"BuildReportRows - Remove Void:[{row.RecKey}]");
                                        return;
                                    }
                                    
                                    //if non-void car/hotel data exists then we need to still include the row - but get rid of the air amounts
                                    RowVoidHandler.ZeroOutAirAmounts(row);
                                }

                                Interlocked.Add(ref rowCount, row.PlusMin);

                                //we need to see how many Car or Hotel records there are. If there are more than one, we will need to add extra rows.
                                var totalRowsNeeded = RowCounter.GetTotalRowsNeeded(row.RecKey, userDefinedParams, switchManager, userReport);
                                
                                if (totalRowsNeeded == 0) totalRowsNeeded = 1;

                                LOG.Debug($"BuildReportRows - Reckey :[{row.RecKey}] | TotalRowsNeeded: {totalRowsNeeded}");

                                var rows = GetAllChildRows(totalRowsNeeded, row, globals, userReport, isPreview, advancedParameters, lookupHelper, colorManager, factory);
                                if (rows.Any())
                                {
                                    foreach (var item in rows)
                                    {
                                        bag.Add(item);
                                    }
                                }
                            }
                        });

                        lock (obj) rowAgg.AddRange(bag);
                    }
                    catch (AggregateException ex)
                    {
                        ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                        throw;//this throw doesn't actually get executed, but the compiler complains if it isn't there
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException != null) ExceptionDispatchInfo.Capture(e.InnerException).Throw();

                        throw;
                    }
                });

                return new ReportRowHelper { ReportRows = rowAgg, RowCount = rowCount };
            }
        }


        private List<List<string>> GetAllChildRows(int totalRowsNeeded, RawData row, ReportGlobals globals, UserReportInformation userReport, bool isPreview, 
            AdvancedParameters advancedParameters, UserDefinedLookupManager lookupHelper, ColorManager colorManager, ColumnValueRulesFactory factory)
        {
            var rows = new List<List<string>>();
            for (var i = 0; i < totalRowsNeeded; i++)
            {
                var newRow = BuildRow(row, i, globals, userReport, isPreview, lookupHelper, colorManager, factory);

                if (newRow == null) continue;
                LOG.Debug($"AddRow - Row: {i} - Reckey:{row.RecKey}");

                if (CheckAdvancedCrit(newRow, userReport, advancedParameters))
                {
                    rows.Add(newRow);
                }
            }

            return rows;
        }

        /// <summary>
        /// Can return NULL
        /// </summary>
        /// <param name="row"></param>
        /// <param name="seqNo"></param>
        /// <param name="globals"></param>
        /// <param name="userReport"></param>
        /// <param name="isPreview"></param>
        /// <param name="lookupHelper"></param>
        /// <param name="colorManager"></param>
        /// <returns></returns>
        private List<string> BuildRow(RawData row, int seqNo, ReportGlobals globals, UserReportInformation userReport, bool isPreview, UserDefinedLookupManager lookupHelper,
            ColorManager colorManager, ColumnValueRulesFactory factory)
        {
            var estCapacity = userReport.Columns.Count + 1; //add 1 since a pdf might add an additional column to hide the fields
            var newRow = new List<string>(estCapacity);
            bool blankRow = true;
            var sortValueSetter = new RowSortValueSetter();
            foreach (var column in userReport.Columns)
            {
           //     if (conditional.SuppressColumn(globals, column.Name)) continue;
                var cellValue = string.Empty;
                if (column.Name == "SORT")
                {
                   cellValue = sortValueSetter.SortValueWithLegCntr(newRow, userReport.Columns);                    
                }
                else {
                    cellValue = LookupColumnTableConductor.LookupColumnValue(column, row, seqNo, userReport, isPreview, lookupHelper, factory);
                }

                if (!string.IsNullOrEmpty(cellValue))
                {
                    if (column.ColumnType.Trim() == "DATE")
                    {
                        if(globals.Agency.Equals("GSA") && cellValue.Equals("01/01/1900 00:00:00"))
                        {
                            cellValue = string.Empty;
                        }
                        else
                        {
                            cellValue = Convert.ToDateTime(cellValue).ToString(globals.DateDisplay);
                        }
                    }
                    else if (column.ColumnType.Trim() == "DATETIME")
                    {
                        if (globals.Agency.Equals("GSA") && cellValue.Equals("01/01/1900 00:00:00"))
                        {
                            cellValue = string.Empty;
                        }
                        else
                        {
                            cellValue = Convert.ToDateTime(cellValue).ToString(globals.DateDisplay) + Convert.ToDateTime(cellValue).ToString(" hh:mm:sstt");
                        }
                    }
                    else if (column.ColumnType.Trim() == "CURRENCY" && globals.Agency.Equals("GSA"))
                    {
                        if(Features.GsaCurrencyCheck.IsEnabled())
                        {
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                var value = Convert.ToDouble(cellValue);
                                if (value < -100000 || value > 100000) cellValue = "0.00";
                            }
                        }
                    }
                }

                if (blankRow && !string.IsNullOrEmpty(cellValue))
                {
                    if (column.TableName != "DUMMY" && !(column.TableName == "TRIPS" && (column.Name == "PLUSMIN" || column.Name == "RECKEY" || column.Name == "SORT")))
                    {
                        blankRow = false;
                    }
                }

                var color = colorManager.GetColor(column, cellValue, userReport.Theme);

                cellValue += (color != "" && globals.OutputFormat != DestinationSwitch.Csv && globals.OutputFormat != DestinationSwitch.Xls) ? "-[" + color + "]" : "";
                newRow.Add(cellValue.Trim());
            }

            if (blankRow)
            {
                return null;
            }

            //if this is not the first "trip" row, we will want to hide the trip fields. 
            if (IsPdf(globals.OutputFormat))
            {
                newRow.Add(seqNo > 0 ? "True" : "False");
            }

            return newRow;
        }

        private bool IsPdf(DestinationSwitch outputFormat)
        {
            return outputFormat == DestinationSwitch.ClassicPdf || outputFormat == DestinationSwitch.PortableDocFormat;
        }

        /// <summary>
        /// Special handler for "lookup" advanced criteria that can't be handled in the initial queries. 
        /// </summary>
        /// <param name="row">the list of strings that makes up the current row. </param>
        /// <param name="userReport"></param>
        /// <param name="advancedParameters"></param>
        /// <returns>true if the row passes the advanced crit, false otherwise. </returns>
        private bool CheckAdvancedCrit(List<string> row, UserReportInformation userReport, AdvancedParameters advancedParameters)
        {
            if (!advancedParameters.Parameters.Any()) return true;

            //check each column. If a criteria fails, we skip the rest and return false. 
            for (var i = 0; i < userReport.Columns.Count; i++)
            {
                var advCrit = advancedParameters.Parameters.FirstOrDefault(s => s.FieldName.EqualsIgnoreCase(userReport.Columns[i].Name));
                if (advCrit == null) continue;
                return TwoValuesCompareManager.Compare(row[i], advCrit.Value1, advCrit.Operator, userReport.Columns[i].ColumnType, advCrit.Value2);
            }
            return true;
        }
    }
}

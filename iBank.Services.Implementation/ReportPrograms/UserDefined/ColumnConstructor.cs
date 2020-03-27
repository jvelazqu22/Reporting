using System;
using System.Collections.Generic;
using System.Linq;

using CODE.Framework.Core.Utilities;

using Domain.Exceptions;
using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Entities.MasterEntities;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class ColumnConstructor
    {
        private readonly UserReportColumnInformation _userReportColumn;

        public bool IsUdidField { get; set; }

        public bool IsUserLabeledUdid { get; set; }

        public ColumnConstructor(UserReportColumnInformation userReportColumn)
        {
            _userReportColumn = userReportColumn;
            SetIfUdid(userReportColumn);
        }

        public UserReportColumnInformation GetNewColumn()
        {
            var newColumn = new UserReportColumnInformation();
            Mapper.Map(_userReportColumn, newColumn);

            return newColumn;
        }
        

        public string GetUdidFieldColumnType()
        {
            if (_userReportColumn.UdidType == 2) return "CURRENCY";
            if (_userReportColumn.UdidType == 3) return "NUMERIC";
            if (_userReportColumn.UdidType == 4) return "DATE";
            if (_userReportColumn.UdidType == 6) return "TIME";
            return "TEXT";
        }

        public collist2 GetCurrentColumnFromCollist(string reportType, IList<collist2> columnList)
        {
            collist2 collist;
            //allow  Combined, Air, Car, Hotel and SvcFee user defined report to get definition. 
            if (reportType.EqualsIgnoreCase("COMBDET") || reportType.EqualsIgnoreCase("SVCFEE") || reportType.EqualsIgnoreCase("AIR") || reportType.EqualsIgnoreCase("CAR") || reportType.EqualsIgnoreCase("HOTEL"))
            {
                collist = columnList.FirstOrDefault(s => s.colname.Trim().Equals(_userReportColumn.Name.Trim())
                                                            && UserReportCheckLists.ComboTables.Contains(s.coltable.Trim()));
            }
            else if (reportType.EqualsIgnoreCase("CALLS") || reportType.EqualsIgnoreCase("PRODT"))
            {
                collist = columnList.FirstOrDefault(s => s.colname.Trim().Equals(_userReportColumn.Name.Trim()) 
                                                            && s.rpttype.EqualsIgnoreCase("TRACS")
                                                            && UserReportCheckLists.AcctTables.Contains(s.coltable.Trim()));
            }
            else
            {
                collist = columnList.FirstOrDefault(s => s.colname.Trim().Equals(_userReportColumn.Name.Trim())
                                                            && (s.rpttype.EqualsIgnoreCase("ALL")
                                                                   || s.rpttype.EqualsIgnoreCase(reportType)));
            }

            if(collist == null) throw new UserDefinedColumnNotFoundException($"Column [{_userReportColumn.Name.Trim()}] not found.");

            return collist;
        }

        public void AssignHeaders(UserReportColumnInformation newColumn, ReportGlobals globals, collist2 currentColumn = null)
        {
            if (!string.IsNullOrWhiteSpace(_userReportColumn.Header1) || !string.IsNullOrWhiteSpace(_userReportColumn.Header2) || IsUdidField || IsUserLabeledUdid)
            {
                newColumn.Header1 = _userReportColumn.Header1.Trim();
                newColumn.Header2 = _userReportColumn.Header2.Trim();
            }
            else
            {
                if(currentColumn == null) throw new UserDefinedColumnNotFoundException("A null collist2 value was supplied.");
                var headerConstructor = new HeaderConstructor();
                var headers = headerConstructor.GetHeadersByColumn(currentColumn, globals);
                newColumn.Header1 = headers.HeaderOne.Trim();
                newColumn.Header2 = headers.HeaderTwo.Trim();
            }
        }
        
        public bool IsSuppressDuplicates(string tableName, string type, string suppressOption)
        {
            var isTripLevelField = UserReportCheckLists.TripTables.Contains(tableName);

            if (!isTripLevelField) return false;

            switch (suppressOption)
            {
                case "1":
                    if (type == "TEXT" || type == "DATE" || type == "CURRENCY") return true;
                    break;
                case "2":
                    if (type == "TEXT" || type == "DATE") return true;
                    break;
                case "3":
                    if (type == "NUMERIC") return true;
                    break;
            }
            return false;
        }

        public bool NeedToTranslateToNewColumnNames(ReportGlobals globals, string reportType)
        {
            return globals.UseHibServices && reportType.EqualsIgnoreCase("COMBDET");
        }
        
        private void SetIfUdid(UserReportColumnInformation existingColumn)
        {
            IsUdidField = existingColumn.Name.Left(4).EqualsIgnoreCase("UDID");
            IsUserLabeledUdid = existingColumn.Name.Left(2).EqualsIgnoreCase("UC");
        }
    }
}

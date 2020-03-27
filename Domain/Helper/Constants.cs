
using System;

namespace Domain.Helper
{
    public static class Constants
    {
        public const int MaxUdids = 10;

        public const int RailCarrierLength = 4;
        public const string RailMode = "2";
        public const string RailCode = "R";
        public const string AirCode = "A";
        public const int CarrierDescLength = 25;

        //SortBy constants
        public const string SortByTotalNumberOfSegments = "2";
        public const string SortByTotalRevenue = "3";

        //Weight & Measurement Units
        public const string Kgs = "Kgs";
        public const string Lbs = "Lbs.";

        //Report Status constants
        public const string Done = "DONE";
        public const string Pending = "PENDING";
        public const string InProcess = "INPROCESS";

        //Generally used
        public const string In = "IN";
        public const string Out = "OUT";
        public const string Yes = "YES";
        public const string No = "NO";
        public const string On = "ON"; //ibTransactSum
        public const string NotApplicable = "^na^";
        public const string None = "NONE";
        //NOTE: Use this when you need a "FoxPro" null date. This is obviously not a constant, but C# has no date literals. 
        public static DateTime NullDate { get { return new DateTime(1900, 1, 1); } }
        public static DateTime ModifiedDateTimeMinValue { get { return new DateTime(1753,1,1); } }

        //Language Constants
        public const string English = "EN";

        //Report output file extensions. Echoes CrystalReports.Shared.ExportFormatType
        public const string CharacterSeparatedValuesExt = "csv";
        public const string CrystalReportExt = "rpt";
        public const string EditableRTFExt = "rtf";
        public const string ExcelExt = "xls";
        public const string ExcelRecordExt = "rec"; //ToDo: What's the correct extension?
        public const string ExcelWorkBookExt = "xlsx";
        public const string HTML32Ext = "html"; 
        public const string HTML40Ext = "html"; 
        public const string NoFormatExt = ""; //ToDo: Check this?
        public const string PortableDocFormatExt = "pdf";
        public const string RichTextExt = "rtf";
        public const string RPTRExt = "rptr";
        public const string TabSeperatedText= "txt";
        public const string TextExt = "txt";
        public const string WordForWindowsExt = "doc"; //ToDo: Or, "docx"?
        public const string XmlExt = "xml"; //ToDo: Or, "docx"?
        public const string PowerPoint = "pptx"; //ToDo: Or, "ppt"?

        public const string DefaultFontType = "Times New Roman";
        public const string UserDefinedReport = "** USER DEFINED REPORT **";

        public const string DefaultFontPlaceholder = "[DEFAULT]";

        //build where
        public const string FieldEqualsValue = "{0} = {1}; ";
        public const string FieldNotEqualsValue = "{0} <> {1}; ";
        public const string DEFAULT_WHERE_TEXT_DISPLAY = "'Caption Not Found'";

    }
}

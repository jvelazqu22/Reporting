namespace Domain.Constants
{
    public static class Configurations
    {
        public const string MAX_CONCURRENT_BROADCASTS_FOR_THIS_SERVER = "MaxConcurrentBroadcasts_Server_{0}";
        public const string MAX_CONCURRENT_REPORTS_FOR_THIS_SERVER = "MaxConcurrentReports_Server_{0}";

        /*
         * there are five records in the database that look like this:
         * ibankreport_server_IBANKREPORTS1_lastdatetime_CleaningOldExcelProcesses
         * ibankreport_server_IBANKREPORTS2_lastdatetime_CleaningOldExcelProcesses
         * ibankreport_server_IBANKREPORTS3_lastdatetime_CleaningOldExcelProcesses
         * ibankreport_server_IBANKREPORTS4_lastdatetime_CleaningOldExcelProcesses
         * ibankreport_server_IBANKREPORTS5_lastdatetime_CleaningOldExcelProcesses
         * ibankreport_server_GSA02_lastdatetime_CleaningOldExcelProcesses
         * ibankreport_server_KEYSTONEAPP4_lastdatetime_CleaningOldExcelProcesses
         */
        public const string LAST_DATETIME_CLEANING_EXCEL_PROCESSON_THIS_IBANK_SERVER = "ibankreport_server_{0}_lastdatetime_CleaningOldExcelProcesses";

        public const string MINUTES_TO_WAIT_FOR_FOXPRO_TO_RETURN = "MinutesToWaitForFoxProToReturn_BCST_{0}";

        public const string REPORT_SERVER_RE_START_FLAG = "Re_Start_Flag_For_Report_Server_{0}";
    }
}

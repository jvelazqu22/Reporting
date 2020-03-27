namespace Domain.Constants
{
    public class OutputType
    {
        public const string NONE = "0";
        public const string CRYSTAL_REPORT = "1";
        public const string XLS = "2";
        public const string PORTABLE_DOC_FORMAT = "3";
        public const string CVS = "5";
        public const string CLASSIC_PDF = "5";
    }

    public class ErrorMessages
    {
        public const string CRYSTAL_JOB_LIMIT_MAX_REACHED_MSG = "The maximum report processing jobs limit configured by your system administrator has been reached";
    }
}

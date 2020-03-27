using System.Collections.Generic;

namespace Domain.Constants
{
    public static class FieldList
    {
        public static List<string> SqlSelectSpecialCasesList = new List<string>()
        {
            "convert",
            "cast",
            "valcarr+replicate",
            "'UPD ' as ",
            "' ' as ",
        };

   }
}

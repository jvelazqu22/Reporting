using iBank.Server.Utilities;

using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public static class EnforceHeader
    { 
        /// <summary>
        /// If user is Metric, change the headers to use metric units
        /// </summary>
        public static void ChangeHeadersToMetric(List<UserReportColumnInformation> columns)
        {
            foreach (var column in columns)
            {
                //rename column headings to KM, KG, from Miles, Pounds. 
                if (column.Name.EqualsIgnoreCase("MILES"))
                {
                    column.Header1 = column.Header1.Replace("Miles", "Kilometers");
                    column.Header1 = column.Header1.Replace("MILES", "KILOMETERS");
                    column.Header2 = column.Header2.Replace("Miles", "Kilometers");
                    column.Header2 = column.Header2.Replace("MILES", "KILOMETERS");
                }
                if (UserReportCheckLists.WeightColumns.Contains(column.Name))
                {
                    column.Header1 = column.Header1.Replace("LB", "KG");
                    column.Header1 = column.Header1.Replace("POUND", "KG");
                    column.Header1 = column.Header1.Replace("Lb", "Kg");
                    column.Header1 = column.Header1.Replace("lb", "kg");
                    column.Header1 = column.Header1.Replace("Pound", "kg");
                    column.Header2 = column.Header2.Replace("LB", "KG");
                    column.Header2 = column.Header2.Replace("POUND", "KG");
                    column.Header2 = column.Header2.Replace("Lb", "Kg");
                    column.Header2 = column.Header2.Replace("lb", "kg");
                    column.Header2 = column.Header2.Replace("Pound", "kg");
                }
            }
        }
    }
}

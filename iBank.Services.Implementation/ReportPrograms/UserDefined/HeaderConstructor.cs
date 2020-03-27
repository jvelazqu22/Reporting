using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Entities.MasterEntities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class HeaderConstructor
    {
        public UserDefinedHeaders GetHeadersByColumn(collist2 column, ReportGlobals globals)
        {
            var colName = column.colname.Trim().ToUpper();

            switch (colName)
            {
                case "BREAK1":
                case "TBREAK1":
                    return new UserDefinedHeaders("", globals.User.Break1Name);
                case "BREAK2":
                case "TBREAK2":
                    return new UserDefinedHeaders("", globals.User.Break2Name);
                case "BREAK3":
                case "TBREAK3":
                    return new UserDefinedHeaders("", globals.User.Break3Name);
                case "TAX1":
                case "STAX1":
                    return GetTaxHeaders(globals.User.Tax1Name);
                case "TAX2":
                case "STAX2":
                    return GetTaxHeaders(globals.User.Tax2Name);
                default:
                    return new UserDefinedHeaders(column.head1, column.head2);
            }
        }

        private UserDefinedHeaders GetTaxHeaders(string taxName)
        {
            var headerNames = new UserDefinedHeaders();

            var headers = taxName.Split(' ');
            if (headers.Length == 2)
            {
                headerNames.HeaderOne = headers[0];
                headerNames.HeaderTwo = headers[1];
            }
            else
            {
                headerNames.HeaderOne = "";
                headerNames.HeaderTwo = taxName;
            }

            return headerNames;
        }
    }
}

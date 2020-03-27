using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iBank.Services.Implementation.ReportPrograms.UserDefinedLayout
{
    public class UserDefinedLayoutHelper
    {
        public static List<string> GetExportFields()
        {
            var fields = new List<string>();

            fields.Add("reportkey");
            fields.Add("crname");
            fields.Add("crtitle");
            fields.Add("crsubtit");
            fields.Add("crtype");
            fields.Add("lastused");
            fields.Add("colname");
            fields.Add("colorder");
            fields.Add("sort");
            fields.Add("pagebreak");
            fields.Add("subtotal");
            fields.Add("udidhdg1");
            fields.Add("udidhdg2");
            fields.Add("udidwidth");
            fields.Add("udidtype");
            fields.Add("horalign");
            fields.Add("prpbreak");
            fields.Add("lastname");
            fields.Add("firstname");
            fields.Add("bigname");

            return fields;
        }
    }
}

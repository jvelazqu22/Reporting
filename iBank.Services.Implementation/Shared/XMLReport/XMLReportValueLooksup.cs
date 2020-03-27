using CODE.Framework.Core.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    public static class XMLReportValueLooksup
    {
        public static string GetValueFromList(int idx, List<string> list)
        {
            return list == null || idx == -1 ? "" : list[idx];
        }

        public static int GetIndex(string value)
        {
            return value.TryIntParse(0) - 1;
        }
    }
}

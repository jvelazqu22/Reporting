using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iBank.Services.Implementation.Shared.SpecifyUdid
{
    public static class ExportSpecifyUdidFields
    {
        public static void AddUdidFieldList(List<string> fieldList, List<int> udidNumbers, List<string> udidLabels)
        {
            if (udidNumbers.Where(x => x > 0).ToList().Count > 0)
            {
                for (int i = 0; i < udidNumbers.Count; i++)
                {
                    if (string.IsNullOrEmpty(udidLabels[i])) break;

                    fieldList.Add($"Udidnbr{i+1} as udidnbr{i+1}");
                    fieldList.Add($"Udidtext{i+1} as {udidLabels[i]}");
                }
            }
        }
    }
}

using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;

namespace iBank.Services.Implementation.Shared.SpecifyUdid
{
    public class UdidHandler
    {
        private readonly ReportGlobals _globals;
        public List<int> UdidNo { get; set; } = new List<int>();
        public List<string> UdidLabel { get; set; } = new List<string>();

        public UdidHandler(ReportGlobals globals)
        {
            _globals = globals;
        }

        public void SetUdidOnReportProperties()
        {
            for (var i = 1; i<= 10; i++)
            {
                var udidNumber = GetUdidNumber(i);
                if (udidNumber != 0)
                {
                    UdidNo.Add(udidNumber);
                    UdidLabel.Add(GetUdidLabel(i));
                }
                else
                {
                    UdidNo.Add(0);
                    UdidLabel.Add("");
                }
            }            
        }
        
        private int GetUdidNumber(int udidNo)
        {
            int temp = 0;
            switch (udidNo)
            {
                case 1:
                    int.TryParse(_globals.GetParmValue(WhereCriteria.UDIDONRPT1), out temp);
                    break;
                case 2:
                    int.TryParse(_globals.GetParmValue(WhereCriteria.UDIDONRPT2), out temp);
                    break;
                case 3:
                    int.TryParse(_globals.GetParmValue(WhereCriteria.UDIDONRPT3), out temp);
                    break;
                case 4:
                    int.TryParse(_globals.GetParmValue(WhereCriteria.UDIDONRPT4), out temp);
                    break;
                case 5:
                    int.TryParse(_globals.GetParmValue(WhereCriteria.UDIDONRPT5), out temp);
                    break;
                case 6:
                    int.TryParse(_globals.GetParmValue(WhereCriteria.UDIDONRPT6), out temp);
                    break;
                case 7:
                    int.TryParse(_globals.GetParmValue(WhereCriteria.UDIDONRPT7), out temp);
                    break;
                case 8:
                    int.TryParse(_globals.GetParmValue(WhereCriteria.UDIDONRPT8), out temp);
                    break;
                case 9:
                    int.TryParse(_globals.GetParmValue(WhereCriteria.UDIDONRPT9), out temp);
                    break;
                case 10:
                    int.TryParse(_globals.GetParmValue(WhereCriteria.UDIDONRPT10), out temp);
                    break;
            }
            return temp;
        }

        private string GetUdidLabel(int udidNo)
        {
            var defaultLabel = $"Udid # {udidNo} text:";
            var actualLabel = string.Empty;

            switch (udidNo)
            {
                case 1:
                    actualLabel = _globals.GetParmValue(WhereCriteria.UDIDLBL1);
                    break;
                case 2:
                    actualLabel = _globals.GetParmValue(WhereCriteria.UDIDLBL2);
                    break;
                case 3:
                    actualLabel = _globals.GetParmValue(WhereCriteria.UDIDLBL3);
                    break;
                case 4:
                    actualLabel = _globals.GetParmValue(WhereCriteria.UDIDLBL4);
                    break;
                case 5:
                    actualLabel = _globals.GetParmValue(WhereCriteria.UDIDLBL5);
                    break;
                case 6:
                    actualLabel = _globals.GetParmValue(WhereCriteria.UDIDLBL6);
                    break;
                case 7:
                    actualLabel = _globals.GetParmValue(WhereCriteria.UDIDLBL7);
                    break;
                case 8:
                    actualLabel = _globals.GetParmValue(WhereCriteria.UDIDLBL8);
                    break;
                case 9:
                    actualLabel = _globals.GetParmValue(WhereCriteria.UDIDLBL9);
                    break;
                case 10:
                    actualLabel = _globals.GetParmValue(WhereCriteria.UDIDLBL10);
                    break;
            }

            if (string.IsNullOrEmpty(actualLabel)) actualLabel = defaultLabel;

            if (!actualLabel.Right(1).Equals(":"))
            {
                actualLabel += ":";
            }

            return actualLabel;
        }        
    }
}

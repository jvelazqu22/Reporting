using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.UserDefinedReport;

using MoreLinq;
using System.Globalization;
using CODE.Framework.Core.Utilities;

using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public static class LookupHelpers
    {
        public static string GetPredominantFareBasisByHighMiles(int reckey, IEnumerable<LegRawData> data)
        {
            var keyData = data.Where(x => x.RecKey == reckey).ToList();
            
            if (keyData.Any())
            {
                return keyData.MaxBy(x => x.Miles).Farebase ?? "";
            }

            return "";
        }
     
        public static string GetBaseFareBreakout(int recKey, decimal? basefare, IEnumerable<LegRawData> data)
        {
            if (!basefare.HasValue) return "0";

            var legCount = GetLegCount(recKey, data);

            return legCount > 0 
                ? MathHelper.Round((basefare.Value / legCount), 2).ToString(CultureInfo.InvariantCulture) 
                : "0";
        }

        public static string GetPaidFare(RawData rec)
        {
            var paidFare = rec.Airchg + rec.Fees + rec.Svcfee;
            var taxes = GetTotalTaxes(rec);

            return (paidFare + taxes).ToString();
        }
        
        public static string GetTaxesBreakoutByLegs(int reckey, IEnumerable<RawData> data, IEnumerable<LegRawData> legData)
        {
            var taxes = GetTotalTaxes(reckey, data);
            var cnt = GetLegCount(reckey, legData);

            return cnt > 0
                       ? MathHelper.Round(taxes / cnt, 2).ToString(CultureInfo.InvariantCulture)
                       : "0";
        }

        private static int GetLegCount(int reckey, IEnumerable<LegRawData> data)
        {
            return data.Count(s => s.RecKey == reckey);
        }

        private static decimal GetTotalTaxes(int reckey, IEnumerable<RawData> data)
        {
            var rec = data.FirstOrDefault(s => s.RecKey == reckey);

            return GetTotalTaxes(rec);
        }

        public static decimal GetTotalTaxes(RawData rec)
        {
            if (rec == null) return 0;

            var totalTaxes = rec.Tax1 + rec.Tax2 + rec.Tax3 + rec.Tax4;

            return MathHelper.Round(totalTaxes, 2);
        }

        public static string GetNumberOfTickets(string tranType, bool exchange)
        {
            if (tranType == "I" || tranType == "V")
            {
                if (!exchange) return "1";
                else if (exchange) return "0";
            }
            if (tranType == "C") return "-1";
            //missing related data
            return "99";
        }
    }
}

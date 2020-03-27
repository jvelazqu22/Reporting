using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.AgentProductivity;

namespace iBank.Services.Implementation.ReportPrograms.AgentProductivity
{
    public static class AgentProductivityHelpers
    {
        public static List<string> GetExportFields(List<Tuple<string, string>> carrierList)
        {
            var fields = new List<string> { "AgentId" };

            var counter = 1;
            foreach (var carrier in carrierList)
            {
                if (!string.IsNullOrEmpty(carrier.Item1))
                    fields.Add("Carr" + counter++ + " as " + carrier.Item1);
            }
            fields.Add("TotTrips");
            fields.Add("Airchg");
            return fields;
        }

        public static decimal GetAirCharge(bool countTrips, List<RawData> reclist, int carrierNumber, List<Tuple<string, string>> carrierList, DestinationSwitch destinationSwitch)
        {
            if (destinationSwitch != DestinationSwitch.Xls && carrierNumber > 8) return 0;

            var isOther = (destinationSwitch != DestinationSwitch.Xls && carrierNumber == 8) || carrierNumber == 14;
            return isOther
                ? reclist.Where(s => !carrierList.Select(c => c.Item1).Contains(s.Valcarr)).Sum(s => countTrips ? s.Trips : s.Airchg)
                : reclist.Where(s => carrierList[carrierNumber].Item1.Equals(s.Valcarr)).Sum(s => countTrips ? s.Trips : s.Airchg);
        }
    }
}

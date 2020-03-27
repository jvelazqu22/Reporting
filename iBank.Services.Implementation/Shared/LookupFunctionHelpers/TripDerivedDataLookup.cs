using System.Globalization;
using System.Linq;
using Domain;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;

namespace iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    public static class TripDerivedDataLookup
    {
        public static string GetTripTransactionId(int recKey, ReportLookups reportLookups)
        {
            var tripTransaction = reportLookups.HibTripsDerivedData.FirstOrDefault(w => w.reckey == recKey);

            return tripTransaction == null
                ? recKey.ToString(CultureInfo.InvariantCulture)
                : tripTransaction.tripTransactionID;
        }
    }
}

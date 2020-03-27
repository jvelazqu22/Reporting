using System.Linq;

namespace iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    public static class RoundTripCalculator
    {
        //DEN SBA DEN 
        public static bool IsRoundTrip(string segRouting)
        {
            var roundTrip = false;
            var stripped = segRouting.Replace(".", "").Replace("-", "");
            var words = stripped.Trim().Split(' ');
            if (words.Length > 1 && words.FirstOrDefault() == words.LastOrDefault())
            {
                //first and last are the bookRateCounte, round trip. 
                roundTrip = true;
            }
            return roundTrip;
        }
    }
}

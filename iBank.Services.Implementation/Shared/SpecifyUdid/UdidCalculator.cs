using Domain.Helper;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.Shared.SpecifyUdid
{
    public class UdidCalculator
    {
        public string GetUdids(int reckey, int number, IList<UdidRecord> udids)
        {
            var udidDesc = string.Empty;
            if (udids != null)
            {
                var udid = udids.FirstOrDefault(s => s.RecKey == reckey && s.UdidNumber == number);
                if (udid != null)
                {
                    udidDesc = udid.UdidText.Trim();
                }
            }

            return udidDesc.PadRight(80);
        }
    }
}

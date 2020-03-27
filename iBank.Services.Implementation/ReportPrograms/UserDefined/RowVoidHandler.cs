using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class RowVoidHandler
    {
        public static bool IsVoid(RawData row)
        {
            return row.Trantype.EqualsIgnoreCase("V");
        }

        public static bool AllCarAndHotelDataIsVoid(List<CarRawData> carData, List<HotelRawData> hotelData)
        {
            //if there are no records than count as void
            if (carData.Count == 0 && hotelData.Count == 0) return true;

            //if they are all void return true
            return carData.All(x => x.Cartrantyp.EqualsIgnoreCase("V"))
                    && hotelData.All(x => x.Hottrantyp.EqualsIgnoreCase("V"));
        }
        public static bool AllOtherDataIsVoid(List<CarRawData> carData, List<HotelRawData> hotelData, List<ServiceFeeData> svcFeeData)
        {
            //if there are no records than count as void
            if (carData.Count == 0 && hotelData.Count == 0 && svcFeeData.Count == 0) return true;

            //if they are all void return true
            return carData.All(x => x.Cartrantyp.EqualsIgnoreCase("V"))
                    && hotelData.All(x => x.Hottrantyp.EqualsIgnoreCase("V"))
                    && svcFeeData.All(x => x.SfTranType.EqualsIgnoreCase("V"));
        }
        public static void ZeroOutAirAmounts(RawData row)
        {
            row.Airchg = 0;
            row.Offrdchg = 0;
            row.Stndchg = 0;
            row.Mktfare = 0;
            row.Basefare = 0;
            row.Faretax = 0;
            row.Tax1 = 0;
            row.Tax2 = 0;
            row.Tax3 = 0;
            row.Tax4 = 0;
        }
    }
}

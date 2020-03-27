using Domain.Orm.Classes;
using iBank.Server.Utilities;

using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.XmlExtract
{
    public class DataSwitches
    {

        public DataSwitches()
        {

        }
        public void SetDataSwitches(List<XmlTag> tags)
        {
            TrvSwitch = tags.Any(s => s.TagType.EqualsIgnoreCase("TRV") && s.IsOn);
            AirSwitch = tags.Any(s => s.TagType.EqualsIgnoreCase("AIR") && s.IsOn);
            RailSwitch = tags.Any(s => s.TagType.EqualsIgnoreCase("RAIL") && s.IsOn);
            CarSwitch = tags.Any(s => s.TagType.EqualsIgnoreCase("RENT") && s.IsOn);
            HotelSwitch = tags.Any(s => s.TagType.EqualsIgnoreCase("STAY") && s.IsOn);
            UdidSwitch =
                tags.Any(s => s.TagType.EqualsIgnoreCase("UDID") && s.IsOn) ||
                tags.Any(s => s.TagType.EqualsIgnoreCase("UDX") && s.IsOn);
            SvcFeeSwitch = tags.Any(s => s.TagType.EqualsIgnoreCase("FEE") && s.IsOn);
            TripSwitch = tags.Any(s => s.TagType.EqualsIgnoreCase("TRP") && s.IsOn);
            MktSegSwitch = tags.Any(s => s.TagType.EqualsIgnoreCase("MKTSEG") && s.IsOn);
        }


        public bool TrvSwitch { get; set; }

        public bool AirSwitch { get; set; }

        public bool CarSwitch { get; set; }

        public bool UdidSwitch { get; set; }

        public bool HotelSwitch { get; set; }

        public bool RailSwitch { get; set; }

        public bool SvcFeeSwitch { get; set; }

        public bool TripSwitch { get; set; }

        public bool MktSegSwitch { get; set; }

        public bool NoSwitches
        {
            get
            {
                return !TrvSwitch && !AirSwitch && !CarSwitch && !UdidSwitch && !HotelSwitch && !RailSwitch &&
                       !SvcFeeSwitch && !TripSwitch && !MktSegSwitch;
            }
        }
    }

}

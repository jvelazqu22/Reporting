using System;
using iBank.Entities.MasterEntities;

namespace Domain.Helper
{
    public class AgencyDotNetSettings
    {
        public string AgencyName { get; set; }

        public bool IsDotNetEnabled { get; set; }

        public bool IsSharingAgency { get; set; }

        public AgencyDotNetSettings(string name, bool isDotNetEnabled, bool isSharingAgency)
        {
            AgencyName = name;
            IsDotNetEnabled = isDotNetEnabled;
            IsSharingAgency = isSharingAgency;
        }

        public AgencyDotNetSettings(ClientExtras extras)
        {
            AgencyName = extras.ClientCode.Trim();
            IsDotNetEnabled = extras.FieldFunction.Trim().Equals("DOT_NET_RPTSVR", StringComparison.OrdinalIgnoreCase) 
                                && extras.FieldData.Trim().Equals("YES", StringComparison.OrdinalIgnoreCase);
            IsSharingAgency = extras.ClientType.Trim().Equals("C", StringComparison.OrdinalIgnoreCase);
        }
    }
}

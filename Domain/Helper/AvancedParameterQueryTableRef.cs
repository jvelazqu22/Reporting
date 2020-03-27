using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Helper
{
    public class AvancedParameterQueryTableRef
    {
        public string TableName { get; set; } = string.Empty;
        public string AdvancedQuerySnip { get; set; } = string.Empty;
        public bool IsFieldInTripTable { get; set; } = true;
    }
}

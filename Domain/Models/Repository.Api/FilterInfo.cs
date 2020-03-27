using System.Collections.Generic;

namespace Domain.Models.Repository.Api
{
    public class FilterInfo
    {
        public string FilterName { get; set; }
        public List<string> FilterValues { get; set; }
    }
}

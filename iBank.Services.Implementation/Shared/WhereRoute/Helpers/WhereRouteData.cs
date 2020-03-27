using System.Collections.Generic;

using Domain.Interfaces;

namespace iBank.Services.Implementation.Shared.WhereRoute.Helpers
{
    public class WhereRouteData<T> where T : IRouteWhere
    {
        public IList<T> Data { get; set; } 

        public bool FiltersApplied { get; set; }

        public WhereRouteData()
        {
            Data = new List<T>();
            FiltersApplied = false;
        }
    }
}

using PagedList;

namespace Domain.Models.ViewModels
{
    public class LoadLogViewModel
    {
        public SearchLoadLogViewModel SearchModel { get; set; }
        public IPagedList<LoadLogViewModelDisplayData> PageList { get; set; }
    }
}

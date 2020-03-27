using System;

namespace Domain.Models.ViewModels
{
    public class SearchLoadLogViewModel
    {
        public string LoadTypeSelected { get; set; } = string.Empty;
        public DateTime FromLoadDateSelected { get; set; } = new DateTime(1900, 1, 1);
        public DateTime ToLoadDateSelected { get; set; } = DateTime.Today;
        public string GdsBo { get; set; } = string.Empty;
        public string DataSource { get; set; } = string.Empty;
    }
}

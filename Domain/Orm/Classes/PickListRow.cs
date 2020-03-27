using System;

namespace Domain.Orm.Classes
{
    public class PickListRow
    {
        public string Agency { get; set; } = string.Empty;
        public DateTime? LastUsed { get; set; } = DateTime.Now;
        public string ListData { get; set; } = string.Empty;
        public string ListName { get; set; } = string.Empty;
        public string ListType { get; set; } = string.Empty;
        public int RecordNum { get; set; } = 0;
        public int? UserNumber { get; set; } = 0;
    }
}

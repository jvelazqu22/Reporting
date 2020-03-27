using System;

namespace iBank.Services.Orm.Classes
{
    public class PickListRow
    {
        public PickListRow()
        {
            RecordNum = 0;
            ListName = string.Empty;
            ListType = string.Empty;
            UserNumber = 0;
            Agency = string.Empty;
            LastUsed = DateTime.Now;
            ListData = string.Empty;
        }

        public string Agency { get; set; }
        public DateTime? LastUsed { get; set; }
        public string ListData { get; set; }
        public string ListName { get; set; }
        public string ListType { get; set; }
        public int RecordNum { get; set; }
        public int? UserNumber { get; set; }
    }
}

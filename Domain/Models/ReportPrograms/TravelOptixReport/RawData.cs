using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ReportPrograms.TravelOptixReport
{
    public class RawData
    {
        public int Reckey { get; set; } = 0;
        public string Agency { get; set; } = string.Empty;
        public string ReportName { get; set; } = string.Empty;
        public int UserNumber { get; set; } = 0;
        public string UserId { get; set; } = string.Empty;
        public string TOXUserId { get; set; } = string.Empty;
        public string TOXUserDirectory { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public string AppId { get; set; } = string.Empty;
        public string StoryId { get; set; } = string.Empty;
        public string StoryName { get; set; } = string.Empty;
    }
}

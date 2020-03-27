using System.Collections.Generic;

namespace Domain.Models.Repository.Api
{
    public class ServiceBody
    {
        public AppInfo AppInfo { get; set; }
        public List<string> FilterNames { get; set; } = new List<string>();
        public string StoryId { get; set; }
        public string OutputType { get; set; }
        public List<FilterInfo> FilterInfo { get; set; } = new List<FilterInfo>();
        public string RequestId { get; set; }
        public PageSize PageSize { get; set; }
        public SourceSize SourceSize { get; set; }
    }
}

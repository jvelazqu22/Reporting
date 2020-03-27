using System;

namespace Domain.Orm.Classes
{
    public class XmlTag
    {
        public string TagName { get; set; } = string.Empty;
        public string AlternateTagName { get; set; } = string.Empty;
        public bool IsRenamed { get { return !string.IsNullOrEmpty(AlternateTagName) && !TagName.Equals(AlternateTagName,StringComparison.OrdinalIgnoreCase); } }
        public string ActiveName { get { return IsRenamed ? AlternateTagName : TagName; } }
        public bool Mask { get; set; } = false;
        public bool IsOn { get; set; }
        public int UdidNumber { get; set; } = 0;
        public string UdidText { get; set; } = string.Empty;
        public string TagType { get; set; } = string.Empty;
    }
}

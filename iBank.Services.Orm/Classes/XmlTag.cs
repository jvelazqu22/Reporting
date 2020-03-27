using System;

namespace iBank.Services.Orm.Classes
{
    public class XmlTag
    {
        public XmlTag()
        {
            UdidNumber = 0;
            TagName = string.Empty;
            AlternateTagName = string.Empty;
            UdidText = string.Empty;
            TagType = string.Empty;
            Mask = false;

        }

        public string TagName { get; set; }
        public string AlternateTagName { get; set; }
        public bool IsRenamed { get { return !string.IsNullOrEmpty(AlternateTagName) && !TagName.Equals(AlternateTagName,StringComparison.OrdinalIgnoreCase); } }
        public string ActiveName { get { return IsRenamed ? AlternateTagName : TagName; } }
        public bool Mask { get; set; }
        public bool IsOn { get; set; }
        public int UdidNumber { get; set; }
        public string UdidText { get; set; }
        public string TagType { get; set; }
    }
}

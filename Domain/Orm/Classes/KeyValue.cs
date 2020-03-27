using System;
using System.Collections.Generic;

namespace Domain.Orm.Classes
{
    public class KeyValue
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string LangCode { get; set; } = string.Empty;
        public string Agency { get; set; } = string.Empty;

        public List<KeyValue> ToList()
        {
            throw new NotImplementedException();
        }
    }
}

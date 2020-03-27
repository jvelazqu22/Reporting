using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Shared
{
    public static class ZeroOut<T>
    {
        /// <summary>
        /// Sets multiple occurences of same property in group of one reckey to 0 to avoid multiplying field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static List<T> Process(List<T> source, List<string> fields)
        {
            //get the type
            var tType = source[0].GetType();
            var tProperties = new List<PropertyInfo>(tType.GetProperties());
            
            //make sure there is a reckey to key off of
            var reckeyProp = tProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase("RECKEY"));
            if (reckeyProp == null)
            {
                throw new Exception("Class must have a field called reckey!");
            }

            //get any properties that match up with the passed in fields that should be set to 0
            var propsToZero = new List<PropertyInfo>();
            foreach (var field in fields)
            {
                var prop = tProperties.FirstOrDefault(s => s.Name.ToLower() == field.ToLower().Trim());
                if (prop != null && prop.PropertyType == typeof(decimal))
                {
                    propsToZero.Add(prop);
                }
            }
            if (!propsToZero.Any()) return source;

            var reckeys = new List<int>();
            foreach (var item in source)
            {
                var reckey = (int) reckeyProp.GetValue(item, null);
                if (!reckeys.Contains(reckey))
                {
                    //retain the field value for the first instance of the reckey
                    reckeys.Add(reckey);
                    continue;
                }
                foreach (var prop in propsToZero)
                {
                    //zero out the rest of the instances of that reckey
                    prop.SetValue(item,0m);
                }
            }

            return source;
        }
    }
}

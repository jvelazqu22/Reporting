
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Domain.Extensions
{
    public static class DataCopy
    {
        public static T DeepClone<T>(this T original)
        {
            var newObj = default(T);
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, original);

                ms.Position = 0;
                newObj = (T)bf.Deserialize(ms);
            }
            return newObj;
        }
    }
}

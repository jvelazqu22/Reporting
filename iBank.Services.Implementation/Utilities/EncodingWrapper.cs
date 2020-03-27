using System.Text;

using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Utilities
{
    public class EncodingWrapper
    {
        private static readonly string _unicode = "UTF-8";

        private static readonly string _windows = "Windows-1252";
        public Encoding GetEncoding(string outputLanguage)
        {
            if (!string.IsNullOrEmpty(outputLanguage))
            {
                const string JAPANESE = "JP";
                if (outputLanguage.EqualsIgnoreCase(JAPANESE)) return Encoding.GetEncoding(_unicode);
            }

            return Encoding.GetEncoding(_windows);
        }
    }
}

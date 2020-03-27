
namespace iBank.Services.Implementation.ReportPrograms.XmlExtract
{
    public static class XmlExtractHelpers
    {
        public static bool StringHasValue(this string value)
        {
            value = value?.Trim();

            return !string.IsNullOrEmpty(value);
        }
    }
}

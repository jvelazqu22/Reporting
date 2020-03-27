using System.IO;

using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Utilities.eFFECTS
{
    public class FileOutputTransformer
    {
        public static string TruncateFileName(string filePath, int fileNameLengthLimit)
        {
            //3.10.17 - some agencies are currently restricted to a 48 character file name length, including extension
            if (Path.GetFileName(filePath).Length <= fileNameLengthLimit) return filePath;
            
            var originalFileName = Path.GetFileNameWithoutExtension(filePath);
            var fileNameMaxCharacters = fileNameLengthLimit - Path.GetExtension(filePath).Length;

            var shortenedFileName = originalFileName.Truncate(fileNameMaxCharacters);

            return filePath.Replace(originalFileName, shortenedFileName);
        }
    }
}

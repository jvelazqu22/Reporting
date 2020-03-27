using System.Drawing;
using System.Drawing.Drawing2D;

namespace iBank.Services.Implementation.Utilities
{
    public class ImageSizer
    {
        public static Bitmap ResizeImage(Image image, Size desiredSize)
        {
            var originalHeight = image.Height;
            var originalWidth = image.Width;

            var widthRatio = GetRatio(desiredSize.Width, originalWidth);
            var heightRatio = GetRatio(desiredSize.Height, originalHeight);

            var ratioToUse = GetRatioToUse(widthRatio, heightRatio);

            var newWidth = GetNewSize(originalWidth, ratioToUse);
            var newHeight = GetNewSize(originalHeight, ratioToUse);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
            {
                SetGraphicsOptions(graphics);
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        private static double GetRatio(double newSize, double originalSize)
        {
            return newSize / originalSize;
        }

        private static double GetRatioToUse(double widthRatio, double heightRatio)
        {
            return heightRatio < widthRatio
                       ? heightRatio
                       : widthRatio;
        }

        private static int GetNewSize(double original, double ratio)
        {
            return (int)(original * ratio);
        }

        private static void SetGraphicsOptions(Graphics graphics)
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }
    }
}

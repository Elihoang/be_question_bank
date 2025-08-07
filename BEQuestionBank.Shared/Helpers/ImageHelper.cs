using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BEQuestionBank.Shared.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Resize ảnh từ file và lưu ra file mới với kích thước tối đa cho phép
        /// </summary>
        public static void ResizeImage(int maxWidth, int maxHeight, string fileName, string newFileName, string format = "jpg")
        {
            using var image = Image.FromFile(fileName); // Load ảnh từ file
            var size = GetDimensions(maxWidth, maxHeight, image.Width, image.Height);

            using var resized = new Bitmap(image, size.Width, size.Height);
            SaveImageFile(resized, newFileName, format);
        }

        /// <summary>
        /// Resize ảnh từ một đối tượng Image hiện có
        /// </summary>
        public static Image ResizeImage(int maxWidth, int maxHeight, Image image)
        {
            var size = GetDimensions(maxWidth, maxHeight, image.Width, image.Height);
            return new Bitmap(image, size.Width, size.Height);
        }

        /// <summary>
        /// Tính toán kích thước mới cho ảnh để không vượt quá maxWidth / maxHeight
        /// và giữ nguyên tỷ lệ
        /// </summary>
        private static (int Width, int Height) GetDimensions(int maxWidth, int maxHeight, int width, int height)
        {
            if (height <= maxHeight && width <= maxWidth)
                return (width, height);

            float ratio = (float)maxWidth / width;
            if (height * ratio <= maxHeight)
                return (maxWidth, (int)(height * ratio));

            ratio = (float)maxHeight / height;
            return ((int)(width * ratio), maxHeight);
        }

        /// <summary>
        /// Lưu ảnh ra file với định dạng tùy chọn
        /// </summary>
        private static void SaveImageFile(Image image, string newFileName, string format)
        {
            ImageFormat imageFormat = format.ToLower() switch
            {
                "png" => ImageFormat.Png,
                "bmp" => ImageFormat.Bmp,
                "gif" => ImageFormat.Gif,
                _ => ImageFormat.Jpeg
            };

            image.Save(newFileName, imageFormat);
        }
    }

    public static class ImageToString
    {
        /// <summary>
        /// Chuyển Image thành chuỗi Base64
        /// </summary>
        public static string GetStringFromImage(Image image, string format = "png")
        {
            using var ms = new MemoryStream();
            ImageFormat imageFormat = format.ToLower() switch
            {
                "jpg" or "jpeg" => ImageFormat.Jpeg,
                "bmp" => ImageFormat.Bmp,
                "gif" => ImageFormat.Gif,
                _ => ImageFormat.Png
            };

            image.Save(ms, imageFormat);
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Chuyển chuỗi Base64 thành Image
        /// </summary>
        public static Image GetImageFromString(string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            using var ms = new MemoryStream(bytes);
            return Image.FromStream(ms);
        }
    }
}

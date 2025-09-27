using System.IO;
using SixLabors.ImageSharp;                // Image, IImageFormat
using SixLabors.ImageSharp.Formats.Jpeg;   // JpegEncoder
using SixLabors.ImageSharp.Processing;     // Mutate
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Formats;           // IFormFile

namespace 記帳程式後端.Utility
{
    public class CompressImage
    {
        public static Stream Compress(IFormFile imgFile, int quality)
        {
            var output = new MemoryStream();

            using (var input = imgFile.OpenReadStream())
            {
                using (var image = Image.Load(input))
                {
                    // 這裡可以加入調整大小（選擇性）
                    // image.Mutate(x => x.Resize(new ResizeOptions
                    // {
                    //     Mode = ResizeMode.Max,
                    //     Size = new Size(1024, 1024)
                    // }));

                    var encoder = new JpegEncoder
                    {
                        Quality = quality // 0-100
                    };

                    image.Save(output, encoder);
                }
            }

            output.Position = 0;
            return output;
        }
    }
}

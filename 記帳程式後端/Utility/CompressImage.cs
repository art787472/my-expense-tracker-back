using static System.Net.Mime.MediaTypeNames;

using System;
using System.Drawing;
using System.Drawing.Imaging;


using System.IO;
namespace 記帳程式後端.Utility
{
    public class CompressImage
    {
        public static Stream Compress(IFormFile imgFile, long quality)
        {
            MemoryStream memoryStream = new MemoryStream();
            MemoryStream ms = new MemoryStream();
            imgFile.CopyTo(memoryStream);
            
            var image = System.Drawing.Image.FromStream(memoryStream);
            // 获取图像编码器（JPEG在这里被使用）
            ImageCodecInfo jpegEncoder = GetEncoder(ImageFormat.Jpeg);

            // 创建Encoder参数（质量设置）
            Encoder encoder = Encoder.Quality;
            EncoderParameters encoderParameters = new EncoderParameters(1);
            EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;

            // 将图像保存到MemoryStream中，使用给定的编码器和参数
            image.Save(ms, jpegEncoder, encoderParameters);


            

                // 从MemoryStream中创建一个新的Bitmap并返回
           

            ms.Position = 0;
            return ms;





        }

        static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}

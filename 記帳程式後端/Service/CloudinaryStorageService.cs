using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using 記帳程式後端.Dto;

namespace 記帳程式後端.Service
{
    public class CloudinaryStorageService : IImageStorageService
    {
        private readonly IConfiguration _configuration;
        private Cloudinary cloudinary;

        public CloudinaryStorageService(IConfiguration configuration)
        {
            var url = _configuration["CloudinaryUrl"];
            cloudinary = new Cloudinary(url);
            cloudinary.Api.Secure = true;
        }

        public async Task DeleteAsync(string storageKey)
        {
            
            
            await cloudinary.DeleteResourcesAsync(new string[] { storageKey });

        }

        public async Task<ImageUploadResponse> UploadAsync(Stream stream, string fileName)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, stream),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = true
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            var reponse = new ImageUploadResponse()
            {
                Success = uploadResult.Error == null,
                Url = uploadResult.SecureUrl?.ToString(),
                ErrorMessage = uploadResult.Error?.Message
            };

            
            return reponse;
        }
    }
}

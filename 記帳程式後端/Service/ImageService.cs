

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using 記帳程式後端.Dto;
using 記帳程式後端.Models;
using 記帳程式後端.Repository;

namespace 記帳程式後端.Service
{
    public class ImageService : IImageService
    {
        private IImageStorageService _storageService;
        private IImageRepository _imageRepository;
        public ImageService(IImageStorageService storageService, IImageRepository imageRepository) 
        {
            _storageService = storageService;
            _imageRepository = imageRepository;
        }

        public async Task DeleteImage(string id)
        {
            await _storageService.DeleteAsync(id);
        }

        public async Task<ImageUploadResponse> UploadImageAsync(Stream fileStream)
        {
            var response = await _storageService.UploadAsync(fileStream, "name");
            if(!response.Success)
            {
                return response;
            }
            var image = new ImageModel()
            {
                StorageProvider = "Cloudinary",
                StorageKey = response.StorageKey,
                url = response.Url

            };

            var repositaryResponse = await _imageRepository.CreateImage(image);
            response.image = repositaryResponse;
            return response;
        }

       

        
    }
}

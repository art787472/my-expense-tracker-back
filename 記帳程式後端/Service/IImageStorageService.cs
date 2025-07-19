using CloudinaryDotNet.Actions;
using 記帳程式後端.Dto;

namespace 記帳程式後端.Service
{
    public interface IImageStorageService
    {
        Task<ImageUploadResponse> UploadAsync(Stream stream, string fileName);
        Task DeleteAsync(string storageKey);
    }
}

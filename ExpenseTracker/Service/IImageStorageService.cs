using CloudinaryDotNet.Actions;
using ExpenseTracker.Dto;

namespace ExpenseTracker.Service
{
    public interface IImageStorageService
    {
        Task<ImageUploadResponse> UploadAsync(Stream stream, string fileName);
        Task DeleteAsync(string storageKey);
    }
}

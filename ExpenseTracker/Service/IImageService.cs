using ExpenseTracker.Dto;

namespace ExpenseTracker.Service
{
    public interface IImageService
    {
        Task<ImageUploadResponse> UploadImageAsync(Stream fileStream, string name);
        Task DeleteImage(string id);
    }
}

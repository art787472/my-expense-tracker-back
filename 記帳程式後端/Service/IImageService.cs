using 記帳程式後端.Dto;

namespace 記帳程式後端.Service
{
    public interface IImageService
    {
        Task<ImageUploadResponse> UploadImageAsync(Stream fileStream);
        Task DeleteImage(string id);
    }
}

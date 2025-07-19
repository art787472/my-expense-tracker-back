
using 記帳程式後端.Models;

namespace 記帳程式後端.Repository
{
    public interface IImageRepository
    {
        Task<ImageModel> CreateImage(ImageModel image);

        Task DeleteImage(string id);
    }
}

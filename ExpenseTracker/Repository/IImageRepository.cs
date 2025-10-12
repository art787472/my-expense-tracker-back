
using ExpenseTracker.Models;

namespace ExpenseTracker.Repository
{
    public interface IImageRepository
    {
        Task<ImageModel> CreateImage(ImageModel image);

        Task DeleteImage(string id);
    }
}

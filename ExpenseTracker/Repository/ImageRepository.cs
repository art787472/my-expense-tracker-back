using ExpenseTracker.DbAccess;
using ExpenseTracker.Models;

namespace ExpenseTracker.Repository
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;
        public ImageRepository(ApplicationDbContext context) 
        { 
            _context = context;
        }
        public async Task<ImageModel> CreateImage(ImageModel image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public Task DeleteImage(string id)
        {
            throw new NotImplementedException();
        }
    }
}

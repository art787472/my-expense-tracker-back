using 記帳程式後端.DbAccess;
using 記帳程式後端.Models;

namespace 記帳程式後端.Repository
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
            return image;
        }

        public Task DeleteImage(string id)
        {
            throw new NotImplementedException();
        }
    }
}

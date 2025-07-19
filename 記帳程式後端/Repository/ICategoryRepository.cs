using 記帳程式後端.Dto;
using 記帳程式後端.Models;

namespace 記帳程式後端.Repository
{
    public interface ICategoryRepository
    {
        Task<List<CategoryDto>> GetCategories();
    }
}

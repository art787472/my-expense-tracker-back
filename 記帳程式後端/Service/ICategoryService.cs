using 記帳程式後端.Dto;
using 記帳程式後端.Models;

namespace 記帳程式後端.Service
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetCategories();
    }
}

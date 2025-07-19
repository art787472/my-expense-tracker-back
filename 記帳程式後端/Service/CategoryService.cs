using 記帳程式後端.Dto;
using 記帳程式後端.Models;
using 記帳程式後端.Repository;

namespace 記帳程式後端.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> GetCategories()
        {
            return await _categoryRepository.GetCategories();
        }
    }
}

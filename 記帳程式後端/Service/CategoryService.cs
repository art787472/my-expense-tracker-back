using 記帳程式後端.Contract.Cache;
using 記帳程式後端.Dto;
using 記帳程式後端.Models;
using 記帳程式後端.Repository;

namespace 記帳程式後端.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICacheService _cache;

        public CategoryService(ICategoryRepository categoryRepository, ICacheService cache)
        {
            this._categoryRepository = categoryRepository;
            _cache = cache;
        }

        public async Task<List<CategoryDto>> GetCategories()
        {
            var categories = await _cache.GetAsync<List<CategoryDto>>("categories");
            if (categories == null)
            {
                categories =  await _categoryRepository.GetCategories();
                await _cache.SetAsync("categories", categories, TimeSpan.FromHours(1));
            }
            return categories;
        }

        public async Task<List<CategoryDto>> GetIncomeCategories()
        {
            var categories = await _cache.GetAsync<List<CategoryDto>>("incomCategories");
            if (categories == null)
            {
                categories = await _categoryRepository.GetIncomeCategories();
                await _cache.SetAsync("incomCategories", categories, TimeSpan.FromHours(1));
            }
            return categories;
        }
    }
}

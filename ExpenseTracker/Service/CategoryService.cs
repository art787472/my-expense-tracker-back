using ExpenseTracker.Contract.Cache;
using ExpenseTracker.Dto;
using ExpenseTracker.Models;
using ExpenseTracker.Repository;

namespace ExpenseTracker.Service
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

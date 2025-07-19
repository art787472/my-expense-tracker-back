using Microsoft.EntityFrameworkCore;
using 記帳程式後端.DbAccess;
using 記帳程式後端.Dto;
using 記帳程式後端.Models;

namespace 記帳程式後端.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CategoryDto>> GetCategories()
        {
            // 將所有 Icons 撈出來快取起來避免重複查詢資料庫
            var iconMap = await _dbContext.Icons.ToDictionaryAsync(i => i.Id);

            // 先撈所有 SubCategories，並加上 Icon
            var subCategoryDtos = await _dbContext.SubCategories
                .Select(sub => new SubCategoryDto
                {
                    Id = sub.Id,
                    Name = sub.Name,
                    CategoryId = sub.CategoryId,
                    Icon = iconMap.ContainsKey(sub.Id) ? iconMap[sub.Id] : null
                })
                .ToListAsync();

            // 將 SubCategories 分組
            var subCategoryGroupMap = subCategoryDtos
                .GroupBy(s => s.CategoryId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 撈主分類 Category 並組成 DTO
            var categories = await _dbContext.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = iconMap.ContainsKey(c.IconId) ? iconMap[c.IconId] : null,
                    SubCategories = subCategoryGroupMap.ContainsKey(c.Id) ? subCategoryGroupMap[c.Id] : new List<SubCategoryDto>()
                })
                .ToListAsync();

            return categories;

        }
    }
}

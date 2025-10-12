using ExpenseTracker.Dto;
using ExpenseTracker.Models;

namespace ExpenseTracker.Repository
{
    public interface ICategoryRepository
    {
        Task<List<CategoryDto>> GetCategories();

        Task<List<CategoryDto>> GetIncomeCategories();
    }
}

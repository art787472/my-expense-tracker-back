using ExpenseTracker.Dto;
using ExpenseTracker.Models;

namespace ExpenseTracker.Service
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetCategories();

        Task<List<CategoryDto>> GetIncomeCategories();
    }
}

using ExpenseTracker.Dto;
using ExpenseTracker.Dto.Request;
using ExpenseTracker.Models;

namespace ExpenseTracker.Repository
{
    public interface IExpenseRepository
    {
        Task<ExpenseDto?> GetExpenseById(int id);
        Task<IEnumerable<ExpenseDto>> GetExpenses(QueryExpenseRequest query);
        Task EditExpense(int id, ExpenseRequest request);
        Task DeleteExpense(int id);
        Task<int> CreateExpense(Expense expense);
        Task<int> GetExpenseTotal(QueryExpenseRequest query);

        Task<int> GetExpenseMonthTotal(Guid userId);
    }
}

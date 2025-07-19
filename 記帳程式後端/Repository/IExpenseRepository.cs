using 記帳程式後端.Dto;
using 記帳程式後端.Models;

namespace 記帳程式後端.Repository
{
    public interface IExpenseRepository
    {
        Task<Expense> GetExpenseById(int id);
        Task<IEnumerable<Expense>> GetExpenses(QueryExpenseRequest query);
        Task EditExpense(int id, ExpenseRequest request);
        Task DeleteExpense(int id);
        Task<int> CreateExpense(Expense expense);
    }
}

using Microsoft.AspNetCore.Mvc;
using 記帳程式後端.Dto;
using 記帳程式後端.Dto.Request;
using 記帳程式後端.Models;

namespace 記帳程式後端.Service
{
    public interface IExpenseService
    {
        Task<ExpenseDto?> GetExpenseById(int id);
        Task<IEnumerable<ExpenseDto>> GetExpenses(QueryExpenseRequest query);
        Task EditExpense(int id, ExpenseRequest request);
        Task DeleteExpense(int id);
        Task<int> CreateExpense(Guid userId, ExpenseRequest request);
        Task<int> GetExpenseTotal(QueryExpenseRequest query);
        Task<int> GetExpenseMonthTotal(Guid userId);
        
    }
}

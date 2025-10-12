using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Dto;
using ExpenseTracker.Dto.Request;
using ExpenseTracker.Models;

namespace ExpenseTracker.Service
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

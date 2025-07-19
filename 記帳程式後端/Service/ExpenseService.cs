using Microsoft.EntityFrameworkCore;
using 記帳程式後端.DbAccess;
using 記帳程式後端.Dto;
using 記帳程式後端.Models;
using 記帳程式後端.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace 記帳程式後端.Service
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _repository;
        public ExpenseService(IExpenseRepository repository) 
        {
            _repository = repository;
        }
        public async Task<int> CreateExpense(Guid userId, ExpenseRequest request)
        {
            Expense expense = new Expense() 
            {
                dateTime = request.dateTime,
                account = request.account,
                category = request.category,
                reason = request.reason,
                isDelete = false,
                price = request.price,
                picPath1 = string.Empty,
                picPath2 = string.Empty,
                smallPicPath1 = string.Empty,
                smallPicPath2 = string.Empty,
                userId = userId,
            };

            return await _repository.CreateExpense(expense);


        }





        public async Task<Expense> GetExpenseById(int id)
        {
            var result =  await _repository.GetExpenseById(id);
            return result;
        }



        public async Task DeleteExpense(int id)
        {
            await _repository.DeleteExpense(id);
        }

        async Task IExpenseService.EditExpense(int id, ExpenseRequest request)
        {
           
            await _repository.EditExpense(id, request);
        }

        public async Task<IEnumerable<Expense>> GetExpenses(QueryExpenseRequest query)
        {
            return await _repository.GetExpenses(query);
        }
    }
}

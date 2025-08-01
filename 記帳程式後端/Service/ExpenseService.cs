using Microsoft.EntityFrameworkCore;
using 記帳程式後端.DbAccess;
using 記帳程式後端.Dto;
using 記帳程式後端.Dto.Request;
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
                accountId = request.accountId,
                categoryId = request.categoryId,
                subcategoryId = request.subcategoryId,
                isDelete = false,
                price = request.price,
                picPath1 = request.imageId,
                
                userId = userId,
                Name = request.name
            };

            return await _repository.CreateExpense(expense);


        }





        public async Task<ExpenseDto?> GetExpenseById(int id)
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

        public async Task<IEnumerable<ExpenseDto>> GetExpenses(QueryExpenseRequest query)
        {
            return await _repository.GetExpenses(query);
        }
    }
}

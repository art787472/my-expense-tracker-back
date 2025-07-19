using Microsoft.EntityFrameworkCore;
using 記帳程式後端.DbAccess;
using 記帳程式後端.Dto;
using 記帳程式後端.Models;
using 記帳程式後端.Service;

namespace 記帳程式後端.Repository
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ExpenseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> CreateExpense(Expense expense)
        {
            
            _dbContext.Expenses.Add(expense);
            await _dbContext.SaveChangesAsync();
            return expense.Id;

        }





        public async Task<Expense> GetExpenseById(int id)
        {
            var result = await _dbContext.Expenses.FindAsync(id);
            return result;
        }



        public async Task DeleteExpense(int id)
        {
            var deletedExpense = await GetExpenseById(id);
            if(deletedExpense == null)
            {
                return;
            }

            deletedExpense.isDelete = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditExpense(int id, ExpenseRequest request)
        {
            var expense = _dbContext.Expenses.Find(id);
            expense.price = request.price;
            expense.account = request.account;
            expense.reason = request.reason;
            expense.category = request.category;
            expense.dateTime = request.dateTime;
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Expense>> GetExpenses(QueryExpenseRequest query)
        {
            IQueryable<Expense> expenses = _dbContext.Expenses;
            if(!string.IsNullOrWhiteSpace(query.Category))
            {
                expenses = expenses.Where(x => x.category == query.Category);
            }
            if(!string.IsNullOrWhiteSpace(query.Account))
            {
                expenses = expenses.Where(x => x.account == query.Account);
            }
            if(!string.IsNullOrWhiteSpace(query.Reason))
            {
                expenses = expenses.Where(x => x.reason == query.Reason);
            }
            if (query.StartDate.HasValue)
            {
                // 注意：日期比較可能需要處理時間部分，這裡假設 DateTime 欄位包含時間
                // 如果你的 DateTime 欄位只儲存日期，可以比較 .Date
                expenses = expenses.Where(e => e.dateTime >= query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
               
                expenses = expenses.Where(e => e.dateTime <= query.EndDate.Value);
            }

            if (query.MinPrice.HasValue)
            {
                expenses = expenses.Where(e => e.price >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                expenses = expenses.Where(e => e.price <= query.MaxPrice.Value);
            }

            return await expenses.ToListAsync();

        }
    }
}

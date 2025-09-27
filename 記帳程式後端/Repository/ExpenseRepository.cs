using Microsoft.EntityFrameworkCore;
using 記帳程式後端.DbAccess;
using 記帳程式後端.Dto;
using 記帳程式後端.Dto.Request;
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





        public async Task<ExpenseDto?> GetExpenseById(int id)
        {
            var ex = await _dbContext.Expenses.FindAsync(id);
            if (ex == null || ex.isDelete)
                return null;

            // 檢查各個關聯是否存在
            var category = await _dbContext.Categories.FindAsync(ex.categoryId);
            var subCategory = await _dbContext.SubCategories.FindAsync(ex.subcategoryId);
            var account = await _dbContext.ExpenseAccounts.FindAsync(ex.accountId);
            var user = await _dbContext.Users.FindAsync(ex.userId);
            var img = await _dbContext.Images.FindAsync(ex.picPath1);

            return new ExpenseDto
            {
                Id = ex.Id,
                Name = ex.Name,
                dateTime = ex.dateTime,
                price = ex.price,
                CategoryId = category.Id,
                SubCategoryId = subCategory.Id,
                AccountId = account.Id,
                ImagePath = img.StorageKey,
                isDelete = ex.isDelete,
                User = user != null ? new UserDto { Account = user.Account, Id = user.Id } : null
            };
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
            expense.accountId = request.accountId;
            expense.subcategoryId = request.subcategoryId;
            expense.categoryId = request.categoryId;
            expense.dateTime = request.dateTime;
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ExpenseDto>> GetExpenses(QueryExpenseRequest query)
        {
            var expensesQuery = from e in _dbContext.Expenses
                                join c in _dbContext.Categories on e.categoryId equals c.Id
                                join sc in _dbContext.SubCategories on e.subcategoryId equals sc.Id
                                join acc in _dbContext.ExpenseAccounts on e.accountId equals acc.Id
                                join u in _dbContext.Users on e.userId equals u.Id
                                join img in _dbContext.Images
                        on e.picPath1.HasValue ? e.picPath1.Value : 0 equals img.Id into imgJoin
                                from img in imgJoin.DefaultIfEmpty()
                                where !e.isDelete
                                select new
                                {
                                    Expense = e,
                                    Category = c,
                                    SubCategory = sc,
                                    Account = acc,
                                    User = u,
                                    Image = img
                                };

            // 套用篩選條件
            if (query.CategoryId != null)
            {
                expensesQuery = expensesQuery.Where(x => x.Expense.categoryId == query.CategoryId);
            }

            if (query.AccountId != null)
            {
                expensesQuery = expensesQuery.Where(x => x.Expense.accountId == query.AccountId);
            }

            if (query.SubcategoryId != null)
            {
                expensesQuery = expensesQuery.Where(x => x.Expense.subcategoryId == query.SubcategoryId);
            }

            if (query.StartDate.HasValue)
            {
                expensesQuery = expensesQuery.Where(x => x.Expense.dateTime >= query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                expensesQuery = expensesQuery.Where(x => x.Expense.dateTime <= query.EndDate.Value);
            }

            if (query.MinPrice.HasValue)
            {
                expensesQuery = expensesQuery.Where(x => x.Expense.price >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                expensesQuery = expensesQuery.Where(x => x.Expense.price <= query.MaxPrice.Value);
            }

            if (query.UserId.HasValue)
            {
                expensesQuery = expensesQuery.Where(x => x.Expense.userId == query.UserId);
            }

            // 投影到 ExpenseDto 並執行查詢
            return await expensesQuery.Select(x => new ExpenseDto
            {
                Id = x.Expense.Id,
                Name = x.Expense.Name,
                dateTime = x.Expense.dateTime,
                price = x.Expense.price,
                CategoryId = x.Category.Id,
                SubCategoryId = x.SubCategory.Id,
                AccountId = x.Account.Id,
                ImagePath = x.Image.StorageKey,
                isDelete = x.Expense.isDelete,
                User = new UserDto { Account = x.User.Account, Id = x.User.Id }            }).ToListAsync();

        }

        public async Task<List<ExpenseDto>> GetExpensesWithPaging(QueryExpenseRequest query, int pageNumber = 1, int pageSize = 10)
        {
            // 先建立基本的 join 查詢
            var expensesQuery = from e in _dbContext.Expenses
                                join c in _dbContext.Categories on e.categoryId equals c.Id
                                join sc in _dbContext.SubCategories on e.subcategoryId equals sc.Id
                                join acc in _dbContext.ExpenseAccounts on e.accountId equals acc.Id
                                join u in _dbContext.Users on e.userId equals u.Id
                                join img in _dbContext.Images
                        on e.picPath1.HasValue ? e.picPath1.Value : 0 equals img.Id into imgJoin
                                from img in imgJoin.DefaultIfEmpty()
                                where !e.isDelete
                                select new
                                {
                                    Expense = e,
                                    Category = c,
                                    SubCategory = sc,
                                    Account = acc,
                                    User = new UserDto { Account = u.Account, Id = u.Id},
                                    Image = img
                                };

            // 套用篩選條件（同上）
            if (query.CategoryId != null)
                expensesQuery = expensesQuery.Where(x => x.Expense.categoryId == query.CategoryId);

            if (query.AccountId != null)
                expensesQuery = expensesQuery.Where(x => x.Expense.accountId == query.AccountId);

            if (query.SubcategoryId != null)
                expensesQuery = expensesQuery.Where(x => x.Expense.subcategoryId == query.SubcategoryId);

            if (query.StartDate.HasValue)
                expensesQuery = expensesQuery.Where(x => x.Expense.dateTime >= query.StartDate.Value);

            if (query.EndDate.HasValue)
                expensesQuery = expensesQuery.Where(x => x.Expense.dateTime <= query.EndDate.Value);

            if (query.MinPrice.HasValue)
                expensesQuery = expensesQuery.Where(x => x.Expense.price >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                expensesQuery = expensesQuery.Where(x => x.Expense.price <= query.MaxPrice.Value);

            if (query.UserId.HasValue)
                expensesQuery = expensesQuery.Where(x => x.Expense.userId == query.UserId);

            // 加入分頁和排序
            return await expensesQuery
                .OrderByDescending(x => x.Expense.dateTime) // 按時間降序排列
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ExpenseDto
                {
                    Id = x.Expense.Id,
                    Name = x.Expense.Name,
                    dateTime = x.Expense.dateTime,
                    price = x.Expense.price,
                    CategoryId = x.Category.Id,
                    SubCategoryId = x.SubCategory.Id,
                    AccountId = x.Account.Id,
                    ImagePath = x.Image.StorageKey,
                    isDelete = x.Expense.isDelete,
                    User = new UserDto { Account = x.User.Account, Id = x.User.Id },
                }).ToListAsync();
        }

        public async Task<int> GetExpenseTotal(QueryExpenseRequest query)
        {
            var expensesQuery = _dbContext.Expenses.AsQueryable();
            if (query.CategoryId != null)
            {
                expensesQuery = expensesQuery.Where(x => x.categoryId == query.CategoryId);
            }

            if (query.AccountId != null)
            {
                expensesQuery = expensesQuery.Where(x => x.accountId == query.AccountId);
            }

            if (query.SubcategoryId != null)
            {
                expensesQuery = expensesQuery.Where(x => x.subcategoryId == query.SubcategoryId);
            }

            if (query.StartDate.HasValue)
            {
                expensesQuery = expensesQuery.Where(x => x.dateTime >= query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                expensesQuery = expensesQuery.Where(x => x.dateTime <= query.EndDate.Value);
            }

            if (query.MinPrice.HasValue)
            {
                expensesQuery = expensesQuery.Where(x => x.price >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                expensesQuery = expensesQuery.Where(x => x.price <= query.MaxPrice.Value);
            }

            if (query.UserId.HasValue)
            {
                expensesQuery = expensesQuery.Where(x => x.userId == query.UserId);
            }

            int total = await expensesQuery.SumAsync(x => x.price);
            return total;
        }

        public async Task<int> GetExpenseMonthTotal(Guid userId)
        {
            QueryExpenseRequest query = new QueryExpenseRequest()
            {
                UserId = userId,
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now
            };
            return await GetExpenseTotal(query);
        }
    }
}

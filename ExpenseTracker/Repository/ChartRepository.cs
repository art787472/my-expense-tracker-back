using ExpenseTracker.DbAccess;
using ExpenseTracker.Models;
using System.Globalization;
using ExpenseTracker.Dto;
using ExpenseTracker.Dto.Request;
namespace ExpenseTracker.Repository
{
    public class ChartRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ChartRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public dynamic GetData(BarChartRequest request)
        {
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            var baseQuery = _dbContext.Expenses
            .Where(e => e.userId == request.UserId)  // 使用者篩選
            .Where(e => e.dateTime >= request.StartDate && e.dateTime <= request.EndDate);  // 時間範圍篩選
            // 在資料庫層面就 Join 類別名稱
            var expensesWithCategoryNames = baseQuery
                .Join(_dbContext.Categories,
                      expense => expense.categoryId,
                      category => category.Id,
                      (expense, category) => new
                      {
                          expense.dateTime,
                          expense.price,
                          expense.categoryId,
                          CategoryName = category.Name
                      });
            var allCategories =  _dbContext.Categories
           
           .ToDictionary(c => c.Id, c => c.Name);

            var result = expensesWithCategoryNames.AsEnumerable()
                .GroupBy(x => myCal.GetWeekOfYear(x.dateTime, myCWR, myFirstDOW))
                .Select(g => new BarChartDto
                {
                    Week = g.Key,
                    TotalAmount = g.Sum(e => e.price),
                    CategoryCounts = allCategories.Values.ToDictionary(
                        categoryName => categoryName,
                        categoryName => g.Count(e => e.CategoryName == categoryName)),
                    CategoryAmounts = allCategories.Values.ToDictionary(
                        categoryName => categoryName,
                        categoryName => g.Where(e => e.CategoryName == categoryName).Sum(e => e.price))
                });

            return result.ToList();
        }
    }
}

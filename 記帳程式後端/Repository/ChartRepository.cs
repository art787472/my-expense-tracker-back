using 記帳程式後端.DbAccess;
using 記帳程式後端.Models;
using System.Globalization;
namespace 記帳程式後端.Repository
{
    public class ChartRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ChartRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public dynamic GetData()
        {
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
            IQueryable<Expense> expenses = _dbContext.Expenses;

            var e = expenses.AsEnumerable().GroupBy(x => myCal.GetWeekOfYear(x.dateTime, myCWR, myFirstDOW)).Select(g => 
            new
            {
                Week = g.Key,
                TotalAmount = g.Sum(e => e.price),
                Count = g.Where(e => e.categoryId==1).Count()
            });

            return e.ToList();
        }
    }
}

using CsvHelper;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using 記帳程式後端.Dto.Request;
using 記帳程式後端.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace 記帳程式後端.Controllers
{
    public class DownloadController : Controller
    {
        private readonly IExpenseService _expenseService;
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] QueryExpenseRequest query)
        {
            var expenses = await _expenseService.GetExpenses(query);
            var stream = new MemoryStream();

            
            var bom = new byte[] { 0xEF, 0xBB, 0xBF };
            await stream.WriteAsync(bom, 0, bom.Length);

            
            using (var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(expenses);
            }

            
            stream.Position = 0;

            var fileName = $"expenses_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(stream, "text/csv", fileName);
        }
    }
}

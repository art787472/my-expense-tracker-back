using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Dto.Request;
using ExpenseTracker.Repository;

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : Controller
    {
        private readonly ChartRepository chartRepository;

        public ChartController(ChartRepository chartRepository)
        {
            this.chartRepository = chartRepository;
        }
        [HttpGet("barChart")]
        public IActionResult Index([FromQuery]BarChartRequest request)
        {
            return Ok(chartRepository.GetData(request));
        }
    }
}

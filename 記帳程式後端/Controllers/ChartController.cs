using Microsoft.AspNetCore.Mvc;
using 記帳程式後端.Dto.Request;
using 記帳程式後端.Repository;

namespace 記帳程式後端.Controllers
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

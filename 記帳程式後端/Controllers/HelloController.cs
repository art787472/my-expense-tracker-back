using Microsoft.AspNetCore.Mvc;
using Serilog;
using 記帳程式後端.Aspect;

namespace 記帳程式後端.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : Controller
    {
        [WriteLog]
        public IActionResult Index()
        {
            return Ok("Hello World");
        }

        [HttpGet("error")]
        public IActionResult Error()
        {
            int a = 0;
            int b = 1 / a;
            return Ok();

        }
    }
}

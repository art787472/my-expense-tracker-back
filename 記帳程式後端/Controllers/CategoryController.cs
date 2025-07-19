using Microsoft.AspNetCore.Mvc;
using 記帳程式後端.Dto;
using 記帳程式後端.Models;
using 記帳程式後端.Service;

namespace 記帳程式後端.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {

        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService) {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var res = await _categoryService.GetCategories();
                return Ok(new ResponseData<List<CategoryDto>>(res));
            }
            catch (Exception ex) {
                return BadRequest(ex.ToString());
            }
        }
    }
}

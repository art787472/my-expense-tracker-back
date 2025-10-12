using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Dto;
using ExpenseTracker.Service;
using ExpenseTracker.Utility;


namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
     [ApiController]
    public class ImageController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private IImageService _imageService;
        public ImageController(IWebHostEnvironment env, IImageService imageService)
        {
            _env = env;
            _imageService = imageService;
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(List<IFormFile> files)
        {
            if(files.Count == 0)
            {
                return BadRequest(new { message = "沒有檔案" });
            }
            long size = files.Sum(f => f.Length);
            string rootRoot = _env.ContentRootPath + @"\wwwroot\images\";
            var file = files.First();
            var filePath = Path.Combine(rootRoot, file.FileName);
            ImageUploadResponse reponse = null;
           
                    
                    

            var s = CompressImage.Compress(file, 20);
            reponse = await _imageService.UploadImageAsync(s, file.FileName);
                        
                
            
            if(reponse.Success)
            {
                return Ok(new { count = files.Count, size, path=reponse.Url, imageId = reponse.image.Id });
            }

            return BadRequest(new { message = reponse.ErrorMessage });
            
        }
    }
}

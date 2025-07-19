using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using 記帳程式後端.Auth;
using 記帳程式後端.Dto.Response;
using 記帳程式後端.Models;
using 記帳程式後端.Service;
using LoginRequest = 記帳程式後端.Dto.LoginRequest;
using RegisterRequest = 記帳程式後端.Dto.RegisterRequest;

namespace 記帳程式後端.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        public AccountController(IConfiguration configuration, IUserService userService, IRefreshTokenService refreshTokenService)
        {
            _configuration = configuration;
            _userService = userService;
            _refreshTokenService = refreshTokenService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByAccount(request.Account);
            if(user == null)
            {
                return NotFound(new { Message = "使用者不存在" });
            }

            if(!PwdCrypto.Verify(request.Password, user.password))
            {
                return Unauthorized(new { Message = "無效的密碼" });
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Account),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };

            

            var accessjwtToken = JWTAuth.GenerateJWTToken(claims, DateTime.Now.AddDays(1), _configuration);
            //var refreshJwtToken = JWTAuth.GenerateJWTToken(claims, DateTime.Now.AddDays(7), _configuration);
           
            return  Ok(new ResponseData<LogingResponse>
            (
                new LogingResponse() { accessToken = accessjwtToken }
            ));
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {

            await HttpContext.SignOutAsync("CookieScheme");
            return NoContent();
            
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = await _userService.GetUserByAccount(request.Account);
            if(user != null)
            {
                return Conflict("帳號已經存在");
            }
            var id = await _userService.CreateUser(request);
            var createdUser = _userService.GetUserById(id);
            if (createdUser == null)
            {
                return NoContent();
            }
            return CreatedAtAction(nameof(Register), id, createdUser);
        }

        [HttpPost("token")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshRequest request)
        {
            var refreshToken = await _refreshTokenService.GetRefreshTokenByToken(request.RefreshToken);

            if(refreshToken == null)
            {
                return Unauthorized();
            }
            if(refreshToken.ExpiryDate < DateTime.Now)
            {
                return Unauthorized();
            }
            
            //var refreshJwtToken = JWTAuth.GenerateJWTToken(claims, DateTime.Now.AddDays(7), _configuration);
            return Ok();
        }
    }
}

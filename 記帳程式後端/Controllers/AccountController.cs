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
using System.Security.Cryptography;
using 記帳程式後端.Dto;
using Azure.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.DataProtection;
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
            await _refreshTokenService.DeleteTokensByUserId(user.Id);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Account),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };

            

            var accessjwtToken = JWTAuth.GenerateJWTToken(claims, DateTime.Now.AddMinutes(5), _configuration);
            var refreshToken = RefreshTokenAuth.GenerateSecureRefreshToken();

            var refreshTokenModel = new RefreshToken()
            {
                AddedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(7),
                Token = refreshToken,
                UserId = user.Id
            };

            await _refreshTokenService.CreateToken(refreshTokenModel);

            
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions { 
                HttpOnly = true,
                Path = "/",
                MaxAge = TimeSpan.FromDays(7),
                Secure = false, // 開發環境設為 false
                SameSite = SameSiteMode.Lax // 改為 Lax 或 None

            });
            

            var userDto = new UserDto() { Account = user.Account, Id = user.Id };

            return  Ok(new ResponseData<AuthenticateResponse>
            (
                new AuthenticateResponse() { accessToken = accessjwtToken,  user = userDto }
            ));
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] LogoutRequest request)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if(refreshToken == null)
            {
                return Unauthorized();
            }
            await _refreshTokenService.DeleteToken(refreshToken);
           
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Path = "/",
                MaxAge = TimeSpan.FromDays(7),
                Secure = false, // 開發環境設為 false
                SameSite = SameSiteMode.Lax // 改為 Lax 或 None

            });
            Response.Cookies.Delete("token");

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
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var refreshTokenRequest = Request.Cookies["refreshToken"];
            if (refreshTokenRequest == null)
            {
                return Unauthorized("cookie 中沒有 refreshToken");
            }
            var refreshToken = await _refreshTokenService.GetRefreshTokenByToken(refreshTokenRequest);

            if (refreshToken == null)
            {
                return Unauthorized("refreshToken 不存在");
            }
            if (refreshToken.ExpiryDate < DateTime.Now)
            {
                return Unauthorized("refreshToken 已過期");
            }

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.ReadJwtToken(request.AccessToken);
            var expiryUnix = long.Parse(token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryUnix).UtcDateTime;

            if (expiryDate > DateTime.UtcNow)
            {
                return BadRequest("Access token 尚未過期");
            }

            var principal = RefreshTokenAuth.GetPrincipalFromExpiredToken(request.AccessToken, _configuration); //由原本的accessToken 取得user
            var username = principal.Identity.Name;

            var user = await _userService.GetUserByAccount(username);
            if (user.Id != refreshToken.UserId)
            {
                return BadRequest();
            }

            var newAccessToken = JWTAuth.GenerateJWTToken(principal.Claims, DateTime.Now.AddMinutes(5), _configuration);
            var newRefreshToken = RefreshTokenAuth.GenerateSecureRefreshToken();

            var refreshTokenModel = new RefreshToken()
            {
                AddedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(7),
                Token = newRefreshToken,
                UserId = user.Id

            };
            await _refreshTokenService.CreateToken(refreshTokenModel);

            //刪除舊的 refreshToken
            await _refreshTokenService.DeleteToken(refreshToken.Token);

            HttpContext.Response.Cookies.Append("refreshToken", newRefreshToken,new CookieOptions(){ HttpOnly = true,
                Path = "/",
            MaxAge = TimeSpan.FromDays(7),
                Secure = false, // 開發環境設為 false
                SameSite = SameSiteMode.Lax // 改為 Lax 或 None);

                });

            return Ok(new ResponseData<AuthenticateResponse>
            (
                new AuthenticateResponse() { accessToken = newAccessToken }
            ));
        }
    }
}

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


            return  Ok(new ResponseData<AuthenticateResponse>
            (
                new AuthenticateResponse() { accessToken = accessjwtToken, refreshToken = refreshToken }
            ));
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] LogoutRequest request)
        {
            await _refreshTokenService.DeleteToken(request.RefreshToken);
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
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
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

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.ReadJwtToken(request.AccessToke);
            var expiryUnix = long.Parse(token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryUnix).UtcDateTime;

            if (expiryDate > DateTime.UtcNow)
            {
                return BadRequest("Access token not expired yet");
            }

            var principal = RefreshTokenAuth.GetPrincipalFromExpiredToken(request.AccessToke, _configuration); //由原本的accessToken 取得user
            var username = principal.Identity.Name;

            var user = await _userService.GetUserByAccount(username);
            if(user.Id != refreshToken.UserId)
            {
                return BadRequest();
            }

            var newAccessToken =  JWTAuth.GenerateJWTToken(principal.Claims, DateTime.Now.AddMinutes(5), _configuration);
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

            return Ok(new ResponseData<AuthenticateResponse>
            (
                new AuthenticateResponse() { accessToken = newAccessToken, refreshToken = newRefreshToken }
            ));
        }
    }
}

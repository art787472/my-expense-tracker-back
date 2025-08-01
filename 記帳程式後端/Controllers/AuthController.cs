using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using 記帳程式後端.Auth;
using 記帳程式後端.Dto;
using 記帳程式後端.Dto.Response;
using 記帳程式後端.Models;
using 記帳程式後端.Service;
using RegisterRequest = 記帳程式後端.Dto.Request.RegisterRequest;

namespace 記帳程式後端.Controllers
{
    [Route("google")]
    [ApiController]
    public class AuthController : Controller

    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        public AuthController(IConfiguration configuration, IRefreshTokenService refreshTokenService, IUserService userService)
        {
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
            _userService = userService;
        }
        [HttpGet("callback")]
        public async Task<IActionResult> Index([FromQuery] string code)
        {
            HttpClient httpClient = new HttpClient();
            
                var formData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", _configuration["GoogleAuth:ClientId"]),
                    new KeyValuePair<string, string>("client_secret", _configuration["GoogleAuth:ClientSecret"]),
                    new KeyValuePair<string, string>("redirect_uri", _configuration["GoogleAuth:RedirectUri"])
                };

                var content = new FormUrlEncodedContent(formData);

                var response = await httpClient.PostAsync("https://www.googleapis.com/oauth2/v4/token", content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(jsonResponse);

                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.access_token))
                {
                    return BadRequest("Failed to get access token from Google");
                }
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.access_token);
            var userInfoResponse = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v1/userinfo?alt=json");
                userInfoResponse.EnsureSuccessStatusCode();
                var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(userInfoJson);
                if (userInfo == null)
                {
                    return BadRequest("Failed to get user info from Google");
                }

                var user = await FindOrCreateGoogleUser(userInfo);


                return await GenerateAuthResponse(user);
            
        }
            private async Task<User> FindOrCreateGoogleUser(GoogleUserInfo googleUserInfo)
        {
            // 1. 先用 GoogleId 查找
            var user = await _userService.GetUserByGoogleId(googleUserInfo.id);

            if (user != null)
            {
                // 更新用戶資訊
                user.Account = googleUserInfo.email; // 如果你的 Account 就是 email
                                                     // 可以更新其他欄位如 Name, PictureUrl 等
                await _userService.UpdateUser(user);
                return user;
            }

            // 2. 用 Email 查找是否已有帳號 (假設 Account 就是 email)
            user = await _userService.GetUserByAccount(googleUserInfo.email);

            if (user != null)
            {
                // 綁定 Google 帳號到現有用戶
                user.GoogleId = googleUserInfo.id;
                await _userService.UpdateUser(user);
                return user;
            }

            // 3. 建立新用戶
            user = new User
            {
                Id = new Guid(),
                Account = googleUserInfo.email,
                GoogleId = googleUserInfo.id,
                // 可以加其他 Google 用戶資訊
                Name = googleUserInfo.name,
                PictureUrl = googleUserInfo.picture,
                CreatedAt = DateTime.Now,
                IsEmailVerified = true // Google 用戶 email 已驗證
            };
            RegisterRequest registerRequest = new RegisterRequest
            { 
                Account = googleUserInfo.email, 
                Password = "GoogleLogin" 
            };
            await _userService.CreateUser(registerRequest);
            return user;
        }

        // 使用你原本的認證邏輯
        private async Task<IActionResult> GenerateAuthResponse(User user)
        {
            // 刪除舊的 refresh token
            await _refreshTokenService.DeleteTokensByUserId(user.Id);

            // 建立 Claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Account),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, "User")
    };

            // 生成 JWT Token
            var accessjwtToken = JWTAuth.GenerateJWTToken(claims, DateTime.Now.AddMinutes(5), _configuration);

            // 生成 Refresh Token
            var refreshToken = RefreshTokenAuth.GenerateSecureRefreshToken();
            var refreshTokenModel = new RefreshToken()
            {
                AddedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(7),
                Token = refreshToken,
                UserId = user.Id
            };
            await _refreshTokenService.CreateToken(refreshTokenModel);

            // 設定 Cookie
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Path = "/",
                MaxAge = TimeSpan.FromDays(7),
                Secure = false, // 開發環境設為 false
                SameSite = SameSiteMode.Lax
            });

            // 建立回應
            var userDto = new UserDto() { Account = user.Account, Id = user.Id };
            return Ok(new ResponseData<AuthenticateResponse>
            (
                new AuthenticateResponse() { accessToken = accessjwtToken, user = userDto }
            )
            {
                Message = "Google 登入成功"
            });
        }


    }
    }


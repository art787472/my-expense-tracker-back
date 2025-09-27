using System.Net.Http;
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
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller

    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly HttpClient _httpClient;
        public AuthController(IConfiguration configuration, IRefreshTokenService refreshTokenService, IUserService userService, HttpClient httpClient)
        {
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
            _userService = userService;
            _httpClient = httpClient;
        }

        [HttpGet("line/callback")]
        public async Task<IActionResult> LineLogin([FromQuery] string code)
        {
            HttpClient httpClient = new HttpClient();
            var tokenUrl = "https://api.line.me/oauth2/v2.1/token";
            var formData = new List<KeyValuePair<string, string>>
            {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", _configuration["LineAuth:RedirectUri"]),
            new KeyValuePair<string, string>("client_id", _configuration["LineAuth:ClientId"]),
            new KeyValuePair<string, string>("client_secret", _configuration["LineAuth:ClientSecret"])
            };
            var content = new FormUrlEncodedContent(formData);
            var response = await httpClient.PostAsync(tokenUrl, content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LineTokenResponse>(jsonResponse); // Fix: Use JsonConvert.DeserializeObject
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                return BadRequest("Failed to get access token from Google");
            }
            var accessToken = tokenResponse.AccessToken;
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var userInfoResponse = await httpClient.GetAsync("https://api.line.me/v2/profile");
            var userJsonResponse = await userInfoResponse.Content.ReadAsStringAsync();
            var lineUserInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<LineUserInfo>(userJsonResponse);

            
            var lineIdInfo = await VerifyIdTokenAsync(tokenResponse.IdToken, _configuration["LineAuth:ClientId"]);
            if (lineIdInfo == null)
            {
                return BadRequest("Failed to verify ID token from Line");
            }
            if(lineIdInfo.Email == null)
            {
                return BadRequest("Line ID token does not contain email");
            }

            var user = await FindOrCreateLineUser(lineUserInfo, lineIdInfo.Email);

            return await GenerateAuthResponse(user);

        }
        [HttpGet("github/callback")]
        public async Task<IActionResult> GithubLogin([FromQuery] string code)
        {
            var tokenUrl = "https://github.com/login/oauth/access_token";
            HttpClient httpClient = new HttpClient();
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", _configuration["GithubAuth:ClientId"]),
                new KeyValuePair<string, string>("client_secret", _configuration["GithubAuth:ClientSecret"]),
                new KeyValuePair<string, string>("redirect_uri", _configuration["GithubAuth:RedirectUri"])
            };

            var content = new FormUrlEncodedContent(formData);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await httpClient.PostAsync(tokenUrl, content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<GitHubTokenResponse>(jsonResponse); // Fix: Use JsonConvert.DeserializeObject

            var accessToken = tokenResponse.access_token;
            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest("Failed to get access token from GitHub");
            }
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0"); // 必須設定
            httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var userInfoResponse = await httpClient.GetAsync("https://api.github.com/user");
            var userJsonResponse = await userInfoResponse.Content.ReadAsStringAsync();
            var githubUserInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<GitHubUserInfo>(userJsonResponse); // Fix: Use JsonConvert.DeserializeObject
            
            var emailResponse = await httpClient.GetAsync("https://api.github.com/user/emails");
            var emailJsonResponse = await emailResponse.Content.ReadAsStringAsync();
            List<GitHubEmail> emailsInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GitHubEmail>>(emailJsonResponse);

            var user = await FindOrCreateGithubUser(githubUserInfo, emailsInfo);
            return await GenerateAuthResponse(user);
        }
            [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleLogin([FromQuery] string code)
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
                var tokenResponse =  JsonSerializer.Deserialize<GoogleTokenResponse>(jsonResponse);

                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.access_token))
                {
                    return BadRequest("Failed to get access token from Google");
                }
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.access_token);
                var userInfoResponse = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v1/userinfo?alt=json");
                userInfoResponse.EnsureSuccessStatusCode();
                var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
                var userInfo =  JsonSerializer.Deserialize<GoogleUserInfo>(userInfoJson);
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
                FixUserDateTimeKinds(user);
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
                FixUserDateTimeKinds(user);
                // 綁定 Google 帳號到現有用戶
                user.GoogleId = googleUserInfo.id;
                user.UpdatedAt = DateTime.UtcNow;
                await _userService.UpdateUser(user);
                return user;
            }

            // 3. 建立新用戶
            user = new User
            {
                Id = Guid.NewGuid(),
                Account = googleUserInfo.email,
                GoogleId = googleUserInfo.id,
                // 可以加其他 Google 用戶資訊
                Name = googleUserInfo.name,
                PictureUrl = googleUserInfo.picture,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
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

        private async Task<User> FindOrCreateGithubUser(GitHubUserInfo githubUser, List<GitHubEmail> gitHubEmails)
        {
            var user = await _userService.GetUserByGithubId(githubUser.Id);
            var email = gitHubEmails.First().Email;
            if (user != null)
            {
                FixUserDateTimeKinds(user);
                // 更新用戶資訊
                user.Account = email; // 如果你的 Account 就是 email
                user.UpdatedAt = DateTime.UtcNow;                      // 可以更新其他欄位如 Name, PictureUrl 等
                await _userService.UpdateUser(user);
                return user;
            }

            // 2. 用 Email 查找是否已有帳號 (假設 Account 就是 email)
            user = await _userService.GetUserByAccount(email);

            if (user != null)
            {
                FixUserDateTimeKinds(user);
                // 綁定 Google 帳號到現有用戶
                user.GithubId = githubUser.Id;
                await _userService.UpdateUser(user);
                return user;
            }

            user = new User
            {
                Id =  Guid.NewGuid(),
                Account = email,
                GithubId = githubUser.Id,
                // 可以加其他 Google 用戶資訊
                Name = githubUser.Login,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsEmailVerified = true // Google 用戶 email 已驗證
            };
            RegisterRequest registerRequest = new RegisterRequest
            {
                Account = email,
                Password = "GithubLogin"
            };
            await _userService.CreateUser(registerRequest);
            return user;

        }

        private async Task<User> FindOrCreateLineUser(LineUserInfo user, string email)
        {
            // 1. 先用 LineId 查找
            var lineId = user.UserId;
            var userModel = await _userService.GetUserByLineId(lineId);
            if (userModel != null)
            {
                FixUserDateTimeKinds(userModel);
                // 更新用戶資訊
                userModel.Account = email; // 如果你的 Account 就是 email
                // 可以更新其他欄位如 Name, PictureUrl 等
                await _userService.UpdateUser(userModel);
                return userModel;
            }
            // 2. 用 Email 查找是否已有帳號 (假設 Account 就是 email)
            
            userModel = await _userService.GetUserByAccount(email);
            if (userModel != null)
            {
                FixUserDateTimeKinds(userModel);
                // 綁定 Line 帳號到現有用戶
                userModel.LineId = lineId;
                await _userService.UpdateUser(userModel);
                return userModel;
            }
            // 3. 建立新用戶
            userModel = new User
            {
                Id = Guid.NewGuid(),
                Account = email,
                LineId = lineId,
                Name = user.DisplayName,
                PictureUrl = user.PictureUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsEmailVerified = true // Line 用戶 email 已驗證
            };
            RegisterRequest registerRequest = new RegisterRequest { Account = email, Password = "LineLogin" };
            await _userService.CreateUser(registerRequest);
            return userModel;
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
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
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
                Message = "登入成功"
            });
        }

        private async Task<LineIdTokenVerifyResponse> VerifyIdTokenAsync(string idToken, string clientId)
        {
            var requestUri = "https://api.line.me/oauth2/v2.1/verify";

            // 準備 form data
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id_token", idToken),
                new KeyValuePair<string, string>("client_id", clientId)
            };

            // 建立 form-urlencoded content
            var formContent = new FormUrlEncodedContent(formData);

            try
            {
                // 發送 POST 請求
                var response = await _httpClient.PostAsync(requestUri, formContent);

                // 確保請求成功
                response.EnsureSuccessStatusCode();

                // 讀取回應內容（包含使用者的 profile 和 email）
                var responseContent = await response.Content.ReadAsStringAsync();

                return Newtonsoft.Json.JsonConvert.DeserializeObject<LineIdTokenVerifyResponse>(responseContent);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"驗證 ID Token 失敗: {ex.Message}", ex);
            }
        }

        private void FixUserDateTimeKinds(User user)
        {
            // 修正 CreatedAt
            if (user.CreatedAt.Kind == DateTimeKind.Unspecified)
            {
                user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);
            }
            else if (user.CreatedAt.Kind == DateTimeKind.Local)
            {
                user.CreatedAt = user.CreatedAt.ToUniversalTime();
            }

            // 修正 UpdatedAt (如果有值)
            if (user.UpdatedAt != null)
            {
                if (user.UpdatedAt.Kind == DateTimeKind.Unspecified)
                {
                    user.UpdatedAt = DateTime.SpecifyKind(user.UpdatedAt, DateTimeKind.Utc);
                }
                else if (user.UpdatedAt.Kind == DateTimeKind.Local)
                {
                    user.UpdatedAt = user.UpdatedAt.ToUniversalTime();
                }
            }

            // 如果有其他 DateTime 欄位，也要在這裡處理
            // 例如：LastLoginAt, EmailVerifiedAt 等
        }
    }


}
    


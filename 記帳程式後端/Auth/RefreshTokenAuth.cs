using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace 記帳程式後端.Auth
{
    public class RefreshTokenAuth
    {
        public static string GenerateSecureRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration configuration)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["ClientAuthentication:SecurityKey"])),
                ValidateIssuer = true,
                ValidIssuer = configuration["ClientAuthentication:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["ClientAuthentication:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            //var jwtSecurityToken = securityToken as JwtSecurityToken;
            //if (jwtSecurityToken == null
            //    || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    throw new SecurityTokenException("Invalid token");
            //}

            return principal;
        }
    }
}

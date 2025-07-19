using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace 記帳程式後端.Auth
{
    public class JWTAuth
    {
        public static string GenerateJWTToken(IEnumerable<Claim> claims, DateTime expiresAt, IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["ClientAuthentication:SecurityKey"]));
            var signingCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claimsDict = new Dictionary<string, object>();
            foreach (var item in claims)
            {
                claimsDict[item.Type] = item.Value;
            }
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = claimsDict,
                Expires = expiresAt,
                SigningCredentials = signingCredential,
                Issuer = configuration["ClientAuthentication:Issuer"],
                Audience = configuration["ClientAuthentication:Audience"]
            };



            

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler(); // 建立一個JWT Token處理容器
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);  // 將Token內容實體放入JWT Token處理容器
            string serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }
    }
}

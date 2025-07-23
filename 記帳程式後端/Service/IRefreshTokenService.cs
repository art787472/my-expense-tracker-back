using 記帳程式後端.Dto;

namespace 記帳程式後端.Service
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GetRefreshTokenByToken(string token);
        Task DeleteToken(string token);
        Task DeleteTokensByUserId(Guid userId);
        Task<RefreshToken> CreateToken(RefreshToken refreshToken);
    }
}

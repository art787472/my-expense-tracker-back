using 記帳程式後端.Dto;

namespace 記帳程式後端.Repository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetRefreshTokenByToken(string token);
        Task DeleteToken(string token);
        Task DeleteTokensByUserId(Guid userId);

        Task<RefreshToken> CreateToken(RefreshToken refreshToken);
    }
}

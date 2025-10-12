using ExpenseTracker.Models;

namespace ExpenseTracker.Repository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetRefreshTokenByToken(string token);
        Task DeleteToken(string token);
        Task DeleteTokensByUserId(Guid userId);

        Task<RefreshToken> CreateToken(RefreshToken refreshToken);
    }
}

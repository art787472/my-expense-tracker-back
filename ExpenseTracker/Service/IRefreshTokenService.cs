using ExpenseTracker.Models;

namespace ExpenseTracker.Service
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GetRefreshTokenByToken(string token);
        Task DeleteToken(string token);
        Task DeleteTokensByUserId(Guid userId);
        Task<RefreshToken> CreateToken(RefreshToken refreshToken);
    }
}

using Microsoft.EntityFrameworkCore;
using 記帳程式後端.DbAccess;
using 記帳程式後端.Dto;

namespace 記帳程式後端.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public RefreshTokenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RefreshToken> CreateToken(RefreshToken refreshToken)
        {
            _dbContext.refreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync();
            return refreshToken;
        }

        public async Task DeleteToken(string token)
        {
            var refreshToken = await GetRefreshTokenByToken(token);
            if(refreshToken == null)
            {
                return;
            }
            _dbContext.refreshTokens.Remove(refreshToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenByToken(string token)
        {
            var refreshToken = await _dbContext.refreshTokens.FirstOrDefaultAsync(x => x.Token == token);
            return refreshToken;
        }
    }
}

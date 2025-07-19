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

        public Task<RefreshToken> CreateToken(RefreshToken refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteToken(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<RefreshToken> GetRefreshTokenByToken(string token)
        {
            var refreshToken = await _dbContext.refreshTokens.FirstOrDefaultAsync(x => x.Token == token);
            return refreshToken;
        }
    }
}

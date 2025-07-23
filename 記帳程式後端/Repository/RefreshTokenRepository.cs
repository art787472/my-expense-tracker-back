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
            var refreshToken = await _dbContext.refreshTokens.FirstOrDefaultAsync(x => x.Token == token); ;
            if(refreshToken == null)
            {
                return;
            }
            refreshToken.IsUsed = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteTokensByUserId(Guid userId)
        {
            var refreshTokens = await _dbContext.refreshTokens.Where(x => x.UserId == userId).ToListAsync();
            foreach (RefreshToken refreshToken in refreshTokens)
            {
                refreshToken.IsUsed = true ;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenByToken(string token)
        {
            var refreshToken = await _dbContext.refreshTokens.FirstOrDefaultAsync(x => x.Token == token);
            if(refreshToken.IsUsed == true)
            {
                return null;
            }
            return refreshToken;
        }
    }
}

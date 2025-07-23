using 記帳程式後端.Dto;
using 記帳程式後端.Repository;

namespace 記帳程式後端.Service
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _repository;
        public RefreshTokenService(IRefreshTokenRepository repository)
        {
            _repository = repository;
        }

        public async Task<RefreshToken> CreateToken(RefreshToken refreshToken)
        {
            return await _repository.CreateToken(refreshToken);
        }

        public async Task DeleteToken(string token)
        {
            await _repository.DeleteToken(token);
        }

        public async Task DeleteTokensByUserId(Guid userId)
        {
             await _repository.DeleteTokensByUserId(userId);
        }

        public async Task<RefreshToken> GetRefreshTokenByToken(string token)
        {
            return await _repository.GetRefreshTokenByToken(token);
        }
    }
}

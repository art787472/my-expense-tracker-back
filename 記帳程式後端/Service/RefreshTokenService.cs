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
            return await _repository.GetRefreshTokenByToken(token);
        }
    }
}

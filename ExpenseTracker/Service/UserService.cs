using Azure.Core;
using ExpenseTracker.Auth;
using ExpenseTracker.Dto.Request;
using ExpenseTracker.Models;
using ExpenseTracker.Repository;

namespace ExpenseTracker.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository) 
        {
            _repository = repository;
        }

        public async Task<Guid> CreateUser(RegisterRequest request)
        {
            var newUser = new User()
            {
                Account = request.Account,
                password = PwdCrypto.Hash(request.Password)
            };
            return await _repository.CreateUser(newUser);
        }

        public async Task<User> GetUser(LoginRequest request)
        {
            var user = new User()
            {
                Account = request.Account,
                password = request.Password
            };
            return await _repository.ValidateUser(user);
        }

        public async Task<User> GetUserByAccount(string account)
        {
            if(account == null)
            {
                return null;
            }
            return await _repository.GetUserByAccount(account);
        }

        public async Task<User> GetUserByGithubId(string githubId)
        {
            return await _repository.GetUserByGithubId(githubId);
        }

        public async Task<User> GetUserByGoogleId(string googleId)
        {
            return await _repository.GetUserByGoogleId(googleId);
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _repository.GetUserById(id);
        }

        public async Task<User> GetUserByLineId(string lineId)
        {
            return await _repository.GetUserByLineId(lineId);
        }

        public async Task UpdateUser(User user)
        {
            await _repository.UpdateUser(user);
        }
    }
}

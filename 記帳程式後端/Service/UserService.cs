using Azure.Core;
using 記帳程式後端.Auth;
using 記帳程式後端.Dto;
using 記帳程式後端.Models;
using 記帳程式後端.Repository;

namespace 記帳程式後端.Service
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
            return await _repository.GetUserByAccount(account);
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _repository.GetUserById(id);
        }
    }
}

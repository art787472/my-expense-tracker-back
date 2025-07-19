using 記帳程式後端.Dto;
using 記帳程式後端.Models;
using LoginRequest = 記帳程式後端.Dto.LoginRequest;

namespace 記帳程式後端.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserById(Guid id);
        Task<User> ValidateUser(User user);

        Task<Guid> CreateUser(User user);
        Task<User> GetUserByAccount(string account);
    }
}

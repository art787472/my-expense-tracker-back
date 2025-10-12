using ExpenseTracker.Models;
using LoginRequest = ExpenseTracker.Dto.Request.LoginRequest;

namespace ExpenseTracker.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserById(Guid id);
        Task<User> ValidateUser(User user);

        Task<Guid> CreateUser(User user);
        Task<User> GetUserByAccount(string account);
        Task UpdateUser(User user);
        Task<User> GetUserByGoogleId(string googleId);

        Task<User> GetUserByGithubId(string githubId);
        Task<User> GetUserByLineId(string lineId);
    }
}

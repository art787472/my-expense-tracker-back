using RegisterRequest = ExpenseTracker.Dto.Request.RegisterRequest;
using LoginRequest = ExpenseTracker.Dto.Request.LoginRequest;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace ExpenseTracker.Service
{
    public interface IUserService
    {
        Task<User> GetUser(LoginRequest request);
        Task<User> GetUserById(Guid id);
        Task<Guid> CreateUser(RegisterRequest registerRequest);

        Task<User> GetUserByAccount(string account);
        Task UpdateUser(User user);
        Task<User> GetUserByGoogleId(string googleId);
        Task<User> GetUserByGithubId(string githubId);
        Task<User> GetUserByLineId(string lineId);
    }
}

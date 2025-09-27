using RegisterRequest = 記帳程式後端.Dto.Request.RegisterRequest;
using LoginRequest = 記帳程式後端.Dto.Request.LoginRequest;
using 記帳程式後端.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace 記帳程式後端.Service
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

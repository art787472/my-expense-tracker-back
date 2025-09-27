using Microsoft.EntityFrameworkCore;
using 記帳程式後端.Auth;
using 記帳程式後端.DbAccess;
using 記帳程式後端.Dto;
using 記帳程式後端.Models;

namespace 記帳程式後端.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Guid> CreateUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user.Id;
        }

        public async Task<User> ValidateUser(User user)
        {


            bool isPwdValid = PwdCrypto.Verify(user.password, user.password);

            if(!isPwdValid)
            {
                return null;
            }
            return user;
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<User> GetUserByAccount(string account)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Account == account);
        }

        public async Task UpdateUser(User user)
        {
            var olduser = _dbContext.Users.Find(user.Id);
            if (olduser != null)
            {
                olduser.IsEmailVerified = user.IsEmailVerified;
                olduser.Email = user.Email;
            }

            _dbContext.Users.Update(olduser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> GetUserByGoogleId(string googleId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.GoogleId == googleId);
            return user;
        }

        public async Task<User> GetUserByGithubId(string githubId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.GithubId == githubId);
            return user;
        }

        public async Task<User> GetUserByLineId(string lineId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.LineId == lineId);
            return user;
        }
    }
}

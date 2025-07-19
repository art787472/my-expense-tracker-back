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
    }
}

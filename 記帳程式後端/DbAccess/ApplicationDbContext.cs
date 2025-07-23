using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using 記帳程式後端.Dto;
using 記帳程式後端.Models;

namespace 記帳程式後端.DbAccess
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ImageModel> Images { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
        public DbSet<Icon> Icons { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<ExpenseAccount> ExpenseAccounts { get; set; }

        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}

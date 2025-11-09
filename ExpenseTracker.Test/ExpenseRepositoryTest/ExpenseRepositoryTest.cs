using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.DbAccess;
using ExpenseTracker.Dto.Request;
using ExpenseTracker.Models;
using ExpenseTracker.Repository;
using FluentAssertions;
using k8s.KubeConfigModels;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Test.ExpenseRepositoryTest
{
    public class ExpenseRepositoryTest
    {
        [Fact]
        public async Task GetAllExpenses_ShouldReturnAllExpenses()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var userId = Guid.NewGuid();
            context.Expenses.AddRange(
                new Expense { Id = 1, Name = "Lunch", price = 120, accountId = 1, categoryId = 1, subcategoryId = 1, userId = userId, isDelete = false },
                new Expense { Id = 2, Name = "Coffee", price = 80, accountId = 1, categoryId = 1, subcategoryId = 1,userId = userId, isDelete = false }
            );
            context.Categories.Add(new Category { Id = 1, Name = "食" });
            context.SubCategories.Add(new SubCategory { CategoryId = 1, Name = "早餐" });
            context.ExpenseAccounts.Add(new ExpenseAccount { Id = 1, Name = "VISA" });
            context.Users.Add(new Models.User { Name = "Mock User", Id = userId, Account = "account", password="password" });
            context.SaveChanges();

            var repo = new ExpenseRepository(context);
            var query = new QueryExpenseRequest { UserId = userId  };
            // Act
            var result = await repo.GetExpenses(query);

            // Assert
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Lunch");
        }

        [Fact]
        public async Task GetExpenseById_ShouldReturnCorrectExpense()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var userId = Guid.NewGuid();
            context.Expenses.Add(
                new Expense { Id = 1, Name = "Dinner", price = 120, accountId = 1, categoryId = 1, subcategoryId = 1, userId = userId, picPath1 = 1,isDelete = false }
                
            );
            context.Categories.Add(new Category { Id = 1, Name = "食" });
            context.SubCategories.Add(new SubCategory { CategoryId = 1, Name = "早餐" });
            context.ExpenseAccounts.Add(new ExpenseAccount { Id = 1, Name = "VISA" });
            context.Users.Add(new Models.User { Name = "Mock User", Id = userId, Account = "account", password = "password" });
            context.Images.Add(new Models.ImageModel { Id =1, StorageKey="",StorageProvider ="", url = "https://hello.com" });
            context.SaveChanges();
            

            var repo = new ExpenseRepository(context);

            // Act
            var result = await repo.GetExpenseById(1);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Dinner");
        }

        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 每次測試都用新的DB
            .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureCreated();

            return dbContext;
        }
    }
}

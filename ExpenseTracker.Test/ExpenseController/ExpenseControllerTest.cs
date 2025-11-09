using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Controllers;
using ExpenseTracker.DbAccess;
using ExpenseTracker.Dto;
using ExpenseTracker.Repository;
using ExpenseTracker.Service;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Test.ExpenseControllerTest
{
    public class ExpenseControllerTest
    {
        
        private readonly IExpenseService _expenseService;
        private readonly ICurrentUserService _currentUser;

        public ExpenseControllerTest() 
        {
            _expenseService = A.Fake<IExpenseService>();
            _currentUser = A.Fake<ICurrentUserService>();
        }

        [Fact]
        public async void ExpenseController_GetExpenseById()
        {
            // mock database => in-momory db
            // mock other service

            
            var controller = new ExpenseController(_expenseService, _currentUser);
            int expenseId = 1;
            var fakeExpense = new ExpenseDto
            {
                Id = expenseId,
                Name = "Test Expense",
                price = 100
            };

            A.CallTo(() => _expenseService.GetExpenseById(expenseId))
                .Returns(Task.FromResult(fakeExpense));
            var result = await controller.GetExpenseById(expenseId);

            result.Should().NotBeNull();
        }

        


        
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using 記帳程式後端.Dto;
using 記帳程式後端.Models;
using 記帳程式後端.Service;

namespace 記帳程式後端.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly ICurrentUserService _currentUser;
        public ExpenseController(IExpenseService expenseService, ICurrentUserService currentUser)
        {
            _expenseService = expenseService;
            _currentUser = currentUser;

        }
        
        [HttpGet]
        public async Task<ActionResult<string>> GetExpensesAsync([FromQuery] QueryExpenseRequest query)
        {
            var expenses = await _expenseService.GetExpenses(query);
            return Ok(expenses);
        }
        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            Console.WriteLine("About to throw exception...");
            throw new Exception("This is a test exception");
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpenseById(int id)
        {
            
           Expense expense = await _expenseService.GetExpenseById(id);
           if(expense == null)
            {
                return NotFound();
            }
            return Ok(expense);
        }



        
       [HttpPost]
        public async Task<ActionResult> Create([FromBody] ExpenseRequest request)
        {
            var userId = _currentUser.UserId;
            int newId = await _expenseService.CreateExpense(userId, request);
            var createdExpense = await _expenseService.GetExpenseById(newId);



            return CreatedAtAction(
                actionName: nameof(GetExpenseById),
                routeValues: new { id = newId }, 
                value: createdExpense 
            );
        }

        

        
        [HttpPut("{id}")]
        public async Task<ActionResult<Expense>> EditExpense(int id, [FromBody] ExpenseRequest request)
        {
            var expense = await _expenseService.GetExpenseById(id);
            if (expense == null)
            {
                return NotFound();
            }
            if (expense.userId != _currentUser.UserId) 
            {
                return Unauthorized();
            }
            await _expenseService.EditExpense(id, request);
            var updatedExpense = await _expenseService.GetExpenseById(id);
            return Ok(updatedExpense);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var expense = await _expenseService.GetExpenseById(id);
            if (expense.userId != _currentUser.UserId)
            {
                return Unauthorized();
            }
            await _expenseService.DeleteExpense(id);

            return NoContent();
        }


    }
}

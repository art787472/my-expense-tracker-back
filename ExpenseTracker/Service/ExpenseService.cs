using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Contract.Cache;
using ExpenseTracker.DbAccess;
using ExpenseTracker.Dto;
using ExpenseTracker.Dto.Request;
using ExpenseTracker.Models;
using ExpenseTracker.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ExpenseTracker.Service
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _repository;
        private readonly ICacheService _cache;
        private readonly IAmazonS3 _client;
        private readonly IConfiguration _configuration;
        public ExpenseService(IExpenseRepository repository, ICacheService cacheService, IConfiguration configuration)
        {
            _repository = repository;
            _cache = cacheService;
            _client = new AmazonS3Client();
            _configuration = configuration;
        }
        public async Task<int> CreateExpense(Guid userId, ExpenseRequest request)
        {

           


            Expense expense = new Expense() 
            {
                dateTime = request.dateTime.ToUniversalTime(),
                accountId = request.accountId,
                categoryId = request.categoryId,
                subcategoryId = request.subcategoryId,
                isDelete = false,
                price = request.price,
                picPath1 = request.imageId,
                
                userId = userId,
                Name = request.name
            };

            return await _repository.CreateExpense(expense);


        }





        public async Task<ExpenseDto?> GetExpenseById(int id)
        {
            var result =  await _repository.GetExpenseById(id);

            if (result.ImagePath != null)
            {


                string imageUrl = await _client.GetPreSignedURLAsync(new GetPreSignedUrlRequest()
                {
                    BucketName = _configuration["AmazonImageBucket"],
                    Key = result.ImagePath,
                    Expires = DateTime.Now.AddMinutes(5)
                });

                result.ImagePath = result.ImagePath;
            }
            return result;
        }



        public async Task DeleteExpense(int id)
        {
            await _repository.DeleteExpense(id);
        }

        async Task IExpenseService.EditExpense(int id, ExpenseRequest request)
        {
           
            await _repository.EditExpense(id, request);
        }

        public async Task<IEnumerable<ExpenseDto>> GetExpenses(QueryExpenseRequest query)
        {
            var data =  await _repository.GetExpenses(query);

            foreach (var item in data)
            {
                if(item.ImagePath==null)
                {
                    continue;
                }
            string imageUrl = await _client.GetPreSignedURLAsync(new GetPreSignedUrlRequest()
            {
                BucketName = _configuration["AmazonImageBucket"],
                Key = item.ImagePath,
                Expires = DateTime.Now.AddMinutes(5)
            });
                item.ImagePath = imageUrl;
            }

            return data;
        }

        public async Task<int> GetExpenseTotal(QueryExpenseRequest query)
        {
            return await _repository.GetExpenseTotal(query);
        }

        public async Task<int> GetExpenseMonthTotal(Guid userId)
        {
            var cacheValue = await _cache.GetAsync<string>($"ExpenseMonthTotal_{userId}");
            if (cacheValue != null && int.TryParse(cacheValue, out int result))
            {
                return result;
            }
            var total = await _repository.GetExpenseMonthTotal(userId);
            await _cache.SetAsync($"ExpenseMonthTotal_{userId}", total.ToString(), TimeSpan.FromMinutes(10));
            return total;
        }
    }
}

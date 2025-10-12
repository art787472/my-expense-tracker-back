using System.ComponentModel;
using ExpenseTracker.Models;

namespace ExpenseTracker.Dto
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        
        public DateTime dateTime { get; set; }
        
        public int price { get; set; }
        
        public int CategoryId { get; set; }
        
        public int SubCategoryId { get; set; }
        
        public int AccountId { get; set; }
        
        public string? ImagePath { get; set; }

        public bool isDelete { get; set; }
        public UserDto User { get; set; }
    }
}

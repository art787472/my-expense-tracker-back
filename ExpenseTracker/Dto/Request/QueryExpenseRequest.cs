namespace ExpenseTracker.Dto.Request
{
    public class QueryExpenseRequest
    {
        public int? CategoryId { get; set; }
        public int? AccountId { get; set; } 
        public int? SubcategoryId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } 
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; } 

        public Guid? UserId { get; set; }
    }
}

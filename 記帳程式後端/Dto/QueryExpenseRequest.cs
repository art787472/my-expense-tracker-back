namespace 記帳程式後端.Dto
{
    public class QueryExpenseRequest
    {
        public string? Category { get; set; }
        public string? Account { get; set; } 
        public string? Reason { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } 
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; } 

        public Guid? UserId { get; set; }
    }
}

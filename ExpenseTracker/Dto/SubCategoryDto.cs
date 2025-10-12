using ExpenseTracker.Models;

namespace ExpenseTracker.Dto
{
    public class SubCategoryDto
    {
        public int Id { get; set; }
        public Icon Icon { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
    }
}

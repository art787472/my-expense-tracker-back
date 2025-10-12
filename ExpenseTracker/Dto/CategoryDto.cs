using ExpenseTracker.Models;

namespace ExpenseTracker.Dto
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public Icon Icon { get; set; }
        public string Name { get; set; }

        public IEnumerable<SubCategoryDto> SubCategories { get; set; }
    }
}

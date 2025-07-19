using 記帳程式後端.Models;

namespace 記帳程式後端.Dto
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public Icon Icon { get; set; }
        public string Name { get; set; }

        public IEnumerable<SubCategoryDto> SubCategories { get; set; }
    }
}

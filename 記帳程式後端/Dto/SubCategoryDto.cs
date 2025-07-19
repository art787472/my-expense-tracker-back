using 記帳程式後端.Models;

namespace 記帳程式後端.Dto
{
    public class SubCategoryDto
    {
        public int Id { get; set; }
        public Icon Icon { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
    }
}

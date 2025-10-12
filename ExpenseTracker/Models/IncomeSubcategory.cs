using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class IncomeSubcategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IconId { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public Guid? UserId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class SubCategory
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

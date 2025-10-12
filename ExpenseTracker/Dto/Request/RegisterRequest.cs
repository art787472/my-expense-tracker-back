using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ExpenseTracker.Dto.Request
{
    public class RegisterRequest
    {
        [Required]
        [DisplayName("帳號")]
        public string Account { get; set; }
        [Required]
        [DisplayName("密碼")]
        public string Password { get; set; }
    }
}

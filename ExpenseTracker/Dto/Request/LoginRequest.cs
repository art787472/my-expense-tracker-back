using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Dto.Request
{
    public class LoginRequest
    {
        
        [Required]
        [DisplayName("帳號")]
        public string Account {  get; set; }
        [Required]
        [DisplayName("密碼")]
        public string Password { get; set; }
    }
}

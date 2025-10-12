using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        
        [DisplayName("日期")]
        public DateTime dateTime { get; set; }
        [DisplayName("金額")]
        public int price { get; set; }
        [DisplayName("類別")]
        public int categoryId { get; set; }
        [DisplayName("消費目的")]
        public int subcategoryId { get; set; }
        [DisplayName("帳戶")]
        public int accountId { get; set; }
        [DisplayName("圖片一路徑")]
        public int? picPath1 { get; set; }
        [DisplayName("圖片二路徑")]
        public int? picPath2 { get; set; }
        [DisplayName("縮圖一路徑")]
        public int? smallPicPath1 { get; set; }
        [DisplayName("縮圖二路徑")]
        public int? smallPicPath2 { get; set; }

        public bool isDelete { get; set; }
        public Guid userId { get; set; }
        
    }
}

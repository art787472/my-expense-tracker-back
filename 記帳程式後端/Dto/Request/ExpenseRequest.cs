using System.ComponentModel;

namespace 記帳程式後端.Dto.Request
{
    public class ExpenseRequest
    {
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
        public int? imageId { get; set; }
        public string? name { get; set; }
    }
}

using System.ComponentModel;

namespace 記帳程式後端.Dto
{
    public class ExpenseRequest
    {
        [DisplayName("日期")]
        public DateTime dateTime { get; set; }
        [DisplayName("金額")]
        public int price { get; set; }
        [DisplayName("類別")]
        public string category { get; set; }
        [DisplayName("消費目的")]
        public string reason { get; set; }
        [DisplayName("帳戶")]
        public string account { get; set; }
        public string? picPath { get; set; }
        public string? name { get; set; }
    }
}

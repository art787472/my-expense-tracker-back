namespace ExpenseTracker.Dto
{
    public class BarChartDto
    {
        public int Week { get; set; }
        public decimal TotalAmount { get; set; }
        public Dictionary<string, int> CategoryCounts { get; set; }  // 改用 string 儲存類別名稱
        public Dictionary<string, int> CategoryAmounts { get; set; }  // 改用 string 儲存類別名稱
    }
}

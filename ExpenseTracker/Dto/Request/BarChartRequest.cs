using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dto.Request
{
    public class BarChartRequest
    {
        [Required(ErrorMessage = "開始時間為必填")]
        public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year, 7, 1); // 預設為 30 天前

        [Required(ErrorMessage = "結束時間為必填")]
        public DateTime EndDate { get; set; } = DateTime.UtcNow; // 預設為現在時間
        public Guid UserId { get; set; }
    }
}

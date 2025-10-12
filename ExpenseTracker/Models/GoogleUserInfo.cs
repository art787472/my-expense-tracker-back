namespace ExpenseTracker.Models
{
    public class GoogleUserInfo
    {
        public string id { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string picture { get; set; } = string.Empty;
        public bool verified_email { get; set; }
        public string given_name { get; set; } = string.Empty;
        public string family_name { get; set; } = string.Empty;
    }
}

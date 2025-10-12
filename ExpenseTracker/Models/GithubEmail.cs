using System.Text.Json.Serialization;

namespace ExpenseTracker.Models
{
    public class GitHubEmail
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("primary")]
        public bool Primary { get; set; }

        [JsonPropertyName("verified")]
        public bool Verified { get; set; }

        [JsonPropertyName("visibility")]
        public string Visibility { get; set; } // "private" 或 null
    }
}

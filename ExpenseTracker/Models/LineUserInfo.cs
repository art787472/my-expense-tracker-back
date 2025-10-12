using Newtonsoft.Json;

namespace ExpenseTracker.Models
{
    public class LineUserInfo
    {
       
            [JsonProperty("userId")]
            public string UserId { get; set; }

            [JsonProperty("displayName")]
            public string DisplayName { get; set; }

            [JsonProperty("pictureUrl")]
            public string PictureUrl { get; set; }

            [JsonProperty("statusMessage")]
            public string StatusMessage { get; set; }
        
    }
}

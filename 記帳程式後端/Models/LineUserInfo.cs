using Newtonsoft.Json;

namespace 記帳程式後端.Models
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

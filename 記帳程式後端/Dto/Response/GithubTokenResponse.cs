using Newtonsoft.Json;

namespace 記帳程式後端.Dto.Response
{
    public class GitHubTokenResponse
    {
        [JsonProperty("access_token")]
        public string access_token { get; set; }

        [JsonProperty("token_type")]
        public string token_type { get; set; }

        [JsonProperty("scope")]
        public string scope { get; set; }

        [JsonProperty("error")]
        public string error { get; set; }

        [JsonProperty("error_description")]
        public string error_description { get; set; }
    }
}

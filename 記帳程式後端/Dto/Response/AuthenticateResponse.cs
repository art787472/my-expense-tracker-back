namespace 記帳程式後端.Dto.Response
{
    public class AuthenticateResponse
    {
        
        public string accessToken { get; set; }
        public UserDto? user { get; set; }
    }
}

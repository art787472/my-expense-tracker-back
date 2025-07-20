namespace 記帳程式後端.Dto.Response
{
    public class AuthenticateResponse
    {
        public string refreshToken {  get; set; }
        public string accessToken { get; set; }
    }
}

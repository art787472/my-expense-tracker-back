namespace ExpenseTracker.Dto.Response
{
    public class AuthenticateResponse
    {
        
        public string accessToken { get; set; }
        public UserDto? user { get; set; }
    }
}

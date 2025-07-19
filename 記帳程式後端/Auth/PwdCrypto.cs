using static BCrypt.Net.BCrypt;
using static System.Net.Mime.MediaTypeNames;
using BCrypt.Net;


namespace 記帳程式後端.Auth
{
    public class PwdCrypto
    {
        public static string Hash(string password)
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            var hashedPwd = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hashedPwd;
        }

        public static bool Verify(string password, string hashedPwd)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPwd);
        }
    }
}

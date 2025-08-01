namespace 記帳程式後端.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Account { get; set; }
        public string password { get; set; }

        
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }

        // 一般註冊
        public string? PasswordHash { get; set; }

        // Google 登入
        public string? GoogleId { get; set; }

        // 共同欄位
        public bool IsEmailVerified { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public string AuthProvider { get; set; } = "local"; // local, google, facebook
    }
}

namespace 記帳程式後端.Models
{
    public class LoggingModel
    {
        public DateTime RequestTime { get; set; }

        public DateTime ReponseTime { get; set; }

        public TimeSpan Duration { get; set; }

        public string HttpMethod { get; set; }

        public string UserAgent { get; set; }

        public string Token { get; set; }
        public string ELKLogId { get; set; } // 這是Request的唯一識別碼 用來標示 整個Req-> Res 中間所有Log的紀錄，所以她的Logid必定相等

        public string ControllerName { get; set; }

        public string Email { get; set; }

        public Guid? UserId { get; set; }

        public string UserName { get; set; }

        public string MethodName { get; set; }

        public Object Body { get; set; }

        public string RequestParam { get; set; }
        public string? ErrorLog { get; set; }

        public string? ErrorMsg { get; set; }

        public string Environment { get; set; }

        public string? ResponseData { get; set; }

        public string Service { get; set; }

        public int? ReponseStatus { get; set; }
    }
}

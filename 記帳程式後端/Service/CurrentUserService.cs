using System.Security.Claims;
using CloudinaryDotNet;

namespace 記帳程式後端.Service
{
    public class CurrentUserService :ICurrentUserService
    {
        public Guid UserId => new Guid(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
        public string Account => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor accessor)
        {
            _httpContextAccessor = accessor;
        }
    }
}

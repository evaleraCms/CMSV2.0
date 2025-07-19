using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CMS.Infrastructure.Common
{
    public static class AppData
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;      
            
        }

        public static IConfiguration Configuration;

        public static HttpContext HttpContext => _httpContextAccessor?.HttpContext;
    }
}

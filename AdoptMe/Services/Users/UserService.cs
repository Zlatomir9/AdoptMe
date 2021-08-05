namespace AdoptMe.Services.Users
{
    using Microsoft.AspNetCore.Http;
    using System.Security.Claims;

    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
            => httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}

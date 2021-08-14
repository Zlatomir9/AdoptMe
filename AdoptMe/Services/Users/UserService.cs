namespace AdoptMe.Services.Users
{
    using Microsoft.AspNetCore.Http;
    using System.Security.Claims;

    using static Common.GlobalConstants.Roles;

    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
             => this.httpContextAccessor = httpContextAccessor;

        public string GetUserId()
            => httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public bool UserIsAdmin()
            => httpContextAccessor.HttpContext.User.IsInRole(AdminRoleName);
    }
}

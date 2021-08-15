namespace AdoptMe.Services.Users
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using AdoptMe.Data.Models;

    using static Common.GlobalConstants.Roles;

    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor; 
        private readonly UserManager<User> userManager;

        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public bool AddUserToRole(string userId, string role)
        {
            var user = this.GetUserById(userId);

            if (user == null)
            {
                return false;
            }

            this.userManager.AddToRoleAsync(user, role).GetAwaiter().GetResult();
            return true;
        }

        public bool RemoveUserFromRole(string userId, string role)
        {
            var user = this.GetUserById(userId);

            if (user == null)
            {
                return false;
            }

            this.userManager.RemoveFromRoleAsync(user, role).GetAwaiter().GetResult();
            return true;
        }

        public User GetUserById(string userId)
            => this.userManager.FindByIdAsync(userId).GetAwaiter().GetResult();

        public string GetUserId()
            => httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public bool UserIsAdmin()
            => httpContextAccessor.HttpContext.User.IsInRole(AdminRoleName);

        public string GetUserEmail()
            => httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.Email);
    }
}

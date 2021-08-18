namespace AdoptMe.Services.Users
{
    using Microsoft.AspNetCore.Identity;
    using AdoptMe.Data.Models;

    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;

        public UserService(UserManager<User> userManager)
            => this.userManager = userManager;

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

        public User GetUserById(string userId)
            => this.userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
    }
}

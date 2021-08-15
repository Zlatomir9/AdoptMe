namespace AdoptMe.Services.Users
{
    using AdoptMe.Data.Models;

    public interface IUserService
    {
        public string GetUserId();

        public string GetUserEmail();

        public bool UserIsAdmin();

        public bool AddUserToRole(string userId, string role);

        public bool RemoveUserFromRole(string userId, string role);

        public User GetUserById(string userId);
    }
}

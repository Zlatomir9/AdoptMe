namespace AdoptMe.Services.Users
{
    using AdoptMe.Data.Models;

    public interface IUserService
    {
        public bool AddUserToRole(string userId, string role);

        public User GetUserById(string userId);
    }
}
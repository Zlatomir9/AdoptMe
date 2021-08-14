namespace AdoptMe.Services.Users
{
    public interface IUserService
    {
        public string GetUserId();

        public bool UserIsAdmin();
    }
}

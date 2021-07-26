namespace AdoptMe.Services.Shelters
{
    public interface IShelterService
    {
        int Create(string userId, string name, string phoneNumber, string email);

        public bool IsShelter(string userId);

        public int IdByUser(string userId);

        public string EmailByUser(string userId);
    }
}

namespace AdoptMe.Services.Shelters
{
    public interface IShelterService
    {
        int Create(string name, string phoneNumber, string cityName, string streetName, string streetNumber);

        public bool IsShelter(string userId);

        public bool RegistrationIsSubmitted(string userId);

        public int IdByUser(string userId);

        public string GetShelterUserIdByPet(int id);
    }
}

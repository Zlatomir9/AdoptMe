namespace AdoptMe.Services.Shelters
{
    using System.Threading.Tasks;
    using AdoptMe.Data.Models;

    public interface IShelterService
    {
        int Create(string name, string phoneNumber, string cityName, string streetName, string streetNumber, string userId);

        public bool RegistrationIsSubmitted(string userId);

        public int IdByUser(string userId);

        Task<string> GetShelterUserIdByPet(int id);

        public Shelter GetShelterById(int id);
    }
}

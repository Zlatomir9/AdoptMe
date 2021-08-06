namespace AdoptMe.Services.Administration
{
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Pets;
    using AdoptMe.Models.Shelters;

    public interface IAdministrationService
    {
        RegistrationRequestsViewModel RegistrationRequests(int pageIndex);

        AllPetsViewModel AllPets(int pageIndex, string sortOrder);

        public void AcceptRequest(int id);

        public void DeclineRequest(int id);

        public Shelter GetShelterById(int id);
    }
}

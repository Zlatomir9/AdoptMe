namespace AdoptMe.Services.Administration
{
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Pets;
    using AdoptMe.Models.Shelters;
    using System.Threading.Tasks;

    public interface IAdministrationService
    {
        Task<RegistrationRequestsViewModel> RegistrationRequests(int pageIndex);

        Task<AllPetsViewModel> AllPets(int pageIndex, string sortOrder);

        Task AcceptRequest(int id);

        Task DeclineRequest(int id);

        Task<Shelter> GetShelterById(int id);
    }
}

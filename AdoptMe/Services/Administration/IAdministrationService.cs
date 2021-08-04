namespace AdoptMe.Services.Administration
{
    using AdoptMe.Models.Pets;
    using AdoptMe.Models.Shelters;

    public interface IAdministrationService
    {
        RegistrationRequestsViewModel RegistrationRequests(int pageIndex);

        AllPetsViewModel AllPets(int pageIndex, string sortOrder);
    }
}

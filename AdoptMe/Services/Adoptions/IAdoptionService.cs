namespace AdoptMe.Services.Adoptions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Adoptions;

    public interface IAdoptionService
    {
        Task<int> CreateAdoption(string firstQuestion, string secondQuestion, string thirdQuestion, string fourthQuestion, int id, string userId);

        Task<AdoptionApplicationsViewModel> AdoptionApplications(int pageIndex, string userId);

        Task<AdoptionDetailsViewModel> Details(int id);

        Task ApproveAdoption(int id);

        Task DeclineAdoption(int id);

        Task<bool> SentApplication(int id, string userId);

        Task<AdoptionApplication> GetAdoption(int id);

        Task<IEnumerable<AdoptionApplication>> SubmittedPetAdoptionApplications(int id);

        Task DeclineAdoptionWhenPetIsDeletedOrAdopted(int petId);

        Task<Adopter> GetAdopterByAdoptionId(int id);

        Task<Pet> GetPetByAdoptionId(int id);
    }
}

namespace AdoptMe.Services.Adoptions
{
    using System.Collections.Generic;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Adoptions;

    public interface IAdoptionService
    {
        int CreateAdoption(string firstQuestion, string secondQuestion, string thirdQuestion, string fourthQuestion, int id, string userId);

        public AdoptionApplicationsViewModel AdoptionApplications(int pageIndex, string userId);

        public AdoptionDetailsViewModel Details(int id);

        public void ApproveAdoption(int id);

        public void DeclineAdoption(int id);

        public bool SentApplication(int id, string userId);

        public AdoptionApplication GetAdoption(int id);

        public IEnumerable<AdoptionApplication> SubmittedPetAdoptionApplications(int id);

        public void DeclineAdoptionWhenPetIsDeletedOrAdopted(int petId);

        public Adopter GetAdopterByAdoptionId(int id);

        public Pet GetPetByAdoptionId(int id);
    }
}

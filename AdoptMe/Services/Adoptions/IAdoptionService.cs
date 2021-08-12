namespace AdoptMe.Services.Adoptions
{
    using System.Collections.Generic;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Adoptions;

    public interface IAdoptionService
    {
        int CreateAdoption(string firstName, string lastName, int Age,
            string firstQuestion, string secondQuestion, string thirdQuestion, string fourthQuestion, int id);

        public AdoptionApplicationsViewModel AdoptionApplications(int pageIndex);

        public AdoptionDetailsViewModel Details(int id);

        public void ApproveAdoption(int id);

        public void DeclineAdoption(int id);

        public bool SentApplication(int id);

        public AdoptionApplication GetApplication(int id);

        public IEnumerable<AdoptionApplication> SubmittedPetAdoptionApplications(int id);
    }
}

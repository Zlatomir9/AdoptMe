using AdoptMe.Models.Adoptions;

namespace AdoptMe.Services.Adoptions
{
    public interface IAdoptionService
    {
        int CreateAdoption(string firstName, string lastName, int Age,
            string firstQuestion, string secondQuestion, string thirdQuestion, string fourthQuestion, int id);

        public AdoptionApplicationsViewModel AdoptionApplications(int pageIndex);

        public bool SentApplication(int id);
    }
}

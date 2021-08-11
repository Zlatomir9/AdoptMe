namespace AdoptMe.Services.Adoptions
{
    using System;
    using System.Linq;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Adoptions;
    using AdoptMe.Services.Users;

    using static Data.Models.Enums.RequestStatus;
    using static Common.GlobalConstants.PageSizes;

    public class AdoptionService : IAdoptionService
    {
        private readonly IUserService userService;
        private readonly AdoptMeDbContext data;

        public AdoptionService(IUserService userService, AdoptMeDbContext data)
        {
            this.userService = userService;
            this.data = data;
        }

        public int CreateAdoption(string firstName, string lastName, int Age, string firstQuestion, 
            string secondQuestion, string thirdQuestion, string fourthQuestion, int petId)
        {
            var userId = this.userService.GetUserId();

            var adopter = this.data
                    .Adopters
                    .Where(x => x.UserId == userId)
                    .FirstOrDefault();

            if (adopter == null)
            {
                adopter = new Adopter
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Age = Age,
                    UserId = userId
                };

                this.data.Adopters.Add(adopter);
                this.data.SaveChanges();
            }

            var pet = this.data
                    .Pets
                    .Where(x => x.Id == petId && 
                           x.IsAdopted == false)
                    .FirstOrDefault();

            var adoptionData = new AdoptionApplication
            {
                AdopterId = adopter.Id,
                PetId = pet.Id,
                RequestStatus = Submitted,
                FirstAnswer = firstQuestion,
                SecondAnswer = secondQuestion,
                ThirdAnswer = thirdQuestion,
                FourthAnswer = firstQuestion,
                SubmittedOn = DateTime.UtcNow
            };

            this.data.AdoptionApplications.Add(adoptionData);
            this.data.SaveChanges();

            return adoptionData.PetId;
        }

        public AdoptionApplicationsViewModel AdoptionApplications(int pageIndex)
        {
            var adoptionsQuery = this.data
                .AdoptionApplications
                .Where(s => s.RequestStatus == Submitted)
                .AsQueryable();

            var adoptions = adoptionsQuery
                .Select(x => new AdoptionViewModel
                {
                    AdopterFullName = x.Adopter.FirstName + " " + x.Adopter.LastName,
                    PetName = x.Pet.Name,
                    SubmittedOn = x.SubmittedOn
                })
                .Skip((pageIndex - 1) * AdoptionApplicationsPageSize)
                .Take(AdoptionApplicationsPageSize)
                .ToList();

            return new AdoptionApplicationsViewModel
            {
                Adoptions = adoptions,
                TotalAdoptionApplications = adoptionsQuery.Count()
            };
        }

        public bool SentApplication(int id)
            => this.data
                .AdoptionApplications
                .Any(x => x.Adopter.UserId == userService.GetUserId() &&
                    x.PetId == id);
    }
}

namespace AdoptMe.Services.Adoptions
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
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
            var userId = this.userService.GetUserId();

            var adoptionsQuery = this.data
                .AdoptionApplications
                .Where(s => s.RequestStatus == Submitted && s.Pet.Shelter.UserId == userId)
                .AsQueryable();

            var adoptions = adoptionsQuery
                .Select(x => new AdoptionViewModel
                {
                    Id = x.Id,
                    AdopterFullName = x.Adopter.FirstName + " " + x.Adopter.LastName,
                    PetName = x.Pet.Name,
                    SubmittedOn = x.SubmittedOn,
                    PetId = x.PetId
                })
                .OrderBy(x => x.PetName)
                .ThenBy(x => x.SubmittedOn)
                .Skip((pageIndex - 1) * AdoptionApplicationsPageSize)
                .Take(AdoptionApplicationsPageSize)
                .ToList();

            return new AdoptionApplicationsViewModel
            {
                Adoptions = adoptions,
                TotalAdoptionApplications = adoptionsQuery.Count()
            };
        }

        public AdoptionDetailsViewModel Details(int id)
            => this.data
                   .AdoptionApplications
                   .Where(a => a.Id == id)
                   .Select(a => new AdoptionDetailsViewModel
                   {
                       Id = a.Id,
                       AdopterFullName = a.Adopter.FirstName + " " + a.Adopter.LastName,
                       AdopterAge = a.Adopter.Age,
                       PetName = a.Pet.Name,
                       SubmittedOn = a.SubmittedOn,
                       FirstAnswer = a.FirstAnswer,
                       SecondAnswer = a.SecondAnswer,
                       ThirdAnswer = a.ThirdAnswer,
                       FourthAnswer = a.FourthAnswer,
                       PetId = a.PetId
                   })
                   .FirstOrDefault();

        public void ApproveAdoption(int id)
        {
            var adoptionApplication = GetApplication(id);

            adoptionApplication.RequestStatus = Аccepted;

            var petData = this.data
                    .Pets
                    .FirstOrDefault(x => x.Id == adoptionApplication.PetId);

            petData.IsAdopted = true;

            var submittedApplications = SubmittedPetAdoptionApplications(id);

            if (submittedApplications.Any())
            {
                foreach (var adoptionApplicaton in submittedApplications)
                {
                    adoptionApplication.RequestStatus = Declined;
                }
            }

            this.data.SaveChanges();
        }

        public void DeclineAdoption(int id)
        {
            var adoptionApplication = GetApplication(id);

            adoptionApplication.RequestStatus = Declined;

            this.data.SaveChanges();
        }

        public bool SentApplication(int id)
            => this.data
                .AdoptionApplications
                .Any(x => x.Adopter.UserId == userService.GetUserId() &&
                          x.PetId == id);

        public AdoptionApplication GetApplication(int id)
            => this.data
                    .AdoptionApplications
                    .Where(x => x.Id == id)
                    .FirstOrDefault();

        public IEnumerable<AdoptionApplication> SubmittedPetAdoptionApplications(int id)
            => this.data
                .AdoptionApplications
                .Where(x => x.Id == id && x.RequestStatus == Submitted)
                .ToList();
    }
}

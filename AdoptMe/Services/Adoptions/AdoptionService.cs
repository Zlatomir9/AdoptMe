namespace AdoptMe.Services.Adoptions
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Adoptions;
    using AdoptMe.Services.Users;
    using AdoptMe.Services.Notifications;

    using static Data.Models.Enums.RequestStatus;
    using static Common.GlobalConstants.PageSizes;

    public class AdoptionService : IAdoptionService
    {
        private readonly INotificationService notificationService;
        private readonly AdoptMeDbContext data;

        public AdoptionService(INotificationService notificationService, AdoptMeDbContext data)
        {
            this.data = data;
            this.notificationService = notificationService;
        }

        public int CreateAdoption(string firstQuestion, string secondQuestion, string thirdQuestion, string fourthQuestion, int petId, string userId)
        {
            var adopter = this.data
                    .Adopters
                    .Where(x => x.UserId == userId)
                    .FirstOrDefault();

            var adoptionData = new AdoptionApplication
            {
                AdopterId = adopter.Id,
                Adopter = adopter,
                PetId = petId,
                RequestStatus = Submitted,
                FirstAnswer = firstQuestion,
                SecondAnswer = secondQuestion,
                ThirdAnswer = thirdQuestion,
                FourthAnswer = fourthQuestion,
                SubmittedOn = DateTime.UtcNow
            };

            this.data.AdoptionApplications.Add(adoptionData);
            this.data.SaveChanges();

            return adoptionData.Id;
        }

        public AdoptionApplicationsViewModel AdoptionApplications(int pageIndex, string userId)
        {
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
            var adoptionApplication = GetAdoption(id);

            adoptionApplication.RequestStatus = Аccepted;

            this.data.SaveChanges();
        }

        public void DeclineAdoption(int id)
        {
            var adoptionApplication = GetAdoption(id);

            adoptionApplication.RequestStatus = Declined;

            this.data.SaveChanges();
        }

        public void DeclineAdoptionWhenPetIsDeletedOrAdopted(int petId)
        {
            var petAdoptionApplications = this.SubmittedPetAdoptionApplications(petId);

            if (petAdoptionApplications.Any())
            {
                foreach (var application in petAdoptionApplications)
                {
                    var adopter = this.GetAdopterByAdoptionId(application.Id);

                    application.RequestStatus = Declined;
                    notificationService.DeclineAdoptionNotification(application.Pet.Name,
                                                                    adopter.UserId);

                    this.data.SaveChanges();
                }
            }
        }

        public bool SentApplication(int id, string userId)
            => this.data
                .AdoptionApplications
                .Any(x => x.Adopter.UserId == userId
                          && x.PetId == id
                          && x.RequestStatus == Submitted);

        public AdoptionApplication GetAdoption(int id)
            => this.data
                    .AdoptionApplications
                    .Where(x => x.Id == id)
                    .FirstOrDefault();

        public IEnumerable<AdoptionApplication> SubmittedPetAdoptionApplications(int petId)
            => this.data
                .AdoptionApplications
                .Where(x => x.PetId == petId && x.RequestStatus == Submitted)
                .ToList();

        public Adopter GetAdopterByAdoptionId(int id)
            => this.data
                   .Adopters
                   .FirstOrDefault(x => x.AdoptionApplications.Any(x => x.Id == id));

        public Pet GetPetByAdoptionId(int id)
            => this.data
                   .Pets
                   .FirstOrDefault(x => x.AdoptionApplications.Any(x => x.Id == id));
    }
}
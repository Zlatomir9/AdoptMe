namespace AdoptMe.Services.Adoptions
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Adoptions;
    using AdoptMe.Services.Users;
    using AdoptMe.Services.Notifications;

    using static Data.Models.Enums.RequestStatus;
    using static Common.GlobalConstants.PageSizes;
    using static Common.GlobalConstants.Roles;

    public class AdoptionService : IAdoptionService
    {
        private readonly IUserService userService;
        private readonly INotificationService notificationService;
        private readonly UserManager<User> userManager;
        private readonly AdoptMeDbContext data;

        public AdoptionService(IUserService userService,
            INotificationService notificationService,
            AdoptMeDbContext data,
            UserManager<User> userManager)
        {
            this.userService = userService;
            this.data = data;
            this.notificationService = notificationService;
            this.userManager = userManager;
        }

        public int CreateAdoption(string firstName, string lastName, int аge, string firstQuestion,
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
                    Age = аge,
                    UserId = userId
                };

                this.data.Adopters.Add(adopter);
                this.data.SaveChanges();
            }

            Task
                .Run(async () =>
                {
                    var user = this.userManager.FindByIdAsync(adopter.UserId).Result;
                    await userManager.AddToRoleAsync(user, AdopterRoleName);
                })
                .GetAwaiter()
                .GetResult();

            var pet = this.data
                    .Pets
                    .Where(x => x.Id == petId &&
                           x.IsAdopted == false)
                    .FirstOrDefault();

            var shelter = this.data
                    .Shelters
                    .FirstOrDefault(x => x.Id == pet.ShelterId);

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

            this.notificationService.SentAdoptionNotification(pet.Name, shelter.UserId);

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

            this.data.SaveChanges();

            var adopterData = GetAdopter(adoptionApplication.AdopterId);

            this.notificationService.ApprovedAdoptionNotification(petData.Name, adopterData.UserId);

            var submittedApplications = SubmittedPetAdoptionApplications(petData.Id);

            if (submittedApplications.Any())
            {
                foreach (var application in submittedApplications)
                {
                    application.RequestStatus = Declined;
                    var adopter = GetAdopter(application.AdopterId);
                    this.notificationService.DeclinedAdoptionNotification(petData.Name, adopter.UserId);

                    this.data.SaveChanges();
                }
            }
        }

        public void DeclineAdoption(int id)
        {
            var adoptionApplication = GetApplication(id);

            var petData = this.data
                    .Pets
                    .FirstOrDefault(x => x.Id == adoptionApplication.PetId);

            adoptionApplication.RequestStatus = Declined;

            var adopterData = GetAdopter(adoptionApplication.AdopterId);

            this.notificationService.DeclinedAdoptionNotification(petData.Name, adopterData.UserId);

            this.data.SaveChanges();
        }

        public void DeclineAdoptionWhenPetIsDeleted(int petId)
        {
            var petAdoptionApplications = this.data
                    .AdoptionApplications
                    .Where(x => x.PetId == petId && x.RequestStatus == Submitted);

            if (petAdoptionApplications.Any())
            {
                foreach (var application in petAdoptionApplications)
                {
                    application.RequestStatus = Declined;
                    notificationService.DeclinedAdoptionNotification(
                        application.Pet.Name, application.Adopter.UserId);

                    this.data.SaveChanges();
                }
            }
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

        public IEnumerable<AdoptionApplication> SubmittedPetAdoptionApplications(int petId)
            => this.data
                .AdoptionApplications
                .Where(x => x.PetId == petId && x.RequestStatus == Submitted)
                .ToList();

        public Adopter GetAdopter(int id)
            => this.data
                   .Adopters
                   .FirstOrDefault(x => x.Id == id);
    }
}
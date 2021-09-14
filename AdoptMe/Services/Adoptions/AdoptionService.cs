namespace AdoptMe.Services.Adoptions
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Adoptions;
    using AdoptMe.Services.Notifications;

    using static Data.Models.Enums.RequestStatus;
    using static Common.GlobalConstants.PageSizes;

    public class AdoptionService : IAdoptionService
    {
        private readonly AdoptMeDbContext data;
        private readonly INotificationService notificationService;
        private readonly IConfigurationProvider mapper;

        public AdoptionService(INotificationService notificationService, 
            IConfigurationProvider mapper,
            AdoptMeDbContext data)
        {
            this.data = data;
            this.notificationService = notificationService;
            this.mapper = mapper;
        }

        public async Task<int> CreateAdoption(string firstQuestion, string secondQuestion, string thirdQuestion, string fourthQuestion, int petId, string userId)
        {
            var adopter = await this.data
                    .Adopters
                    .Where(x => x.UserId == userId)
                    .FirstOrDefaultAsync();

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

            await this.data.AdoptionApplications.AddAsync(adoptionData);
            await this.data.SaveChangesAsync();

            return adoptionData.Id;
        }

        public async Task<AdoptionApplicationsViewModel> AdoptionApplications(int pageIndex, string userId)
        {
            var adoptionsQuery = this.data
                .AdoptionApplications
                .Where(s => s.RequestStatus == Submitted && s.Pet.Shelter.UserId == userId)
                .AsQueryable();

            var adoptions = await adoptionsQuery
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
                .ToListAsync();

            return new AdoptionApplicationsViewModel
            {
                Adoptions = adoptions,
                TotalAdoptionApplications = adoptionsQuery.Count()
            };
        }

        public async Task<AdoptionDetailsViewModel> Details(int id)
            => await this.data
                   .AdoptionApplications
                   .Where(a => a.Id == id)
                   .ProjectTo<AdoptionDetailsViewModel>(this.mapper)
                   .FirstOrDefaultAsync();

        public async Task ApproveAdoption(int id)
        {
            var adoptionApplication = await GetAdoption(id);

            adoptionApplication.RequestStatus = Аccepted;

            await this.data.SaveChangesAsync();
        }

        public async Task DeclineAdoption(int id)
        {
            var adoptionApplication = await GetAdoption(id);

            adoptionApplication.RequestStatus = Declined;

            await this.data.SaveChangesAsync();
        }

        public async Task DeclineAdoptionWhenPetIsDeletedOrAdopted(int petId)
        {
            var petAdoptionApplications = await this.SubmittedPetAdoptionApplications(petId);

            if (petAdoptionApplications.Any())
            {
                foreach (var application in petAdoptionApplications)
                {
                    var adopter = await this.GetAdopterByAdoptionId(application.Id);

                    application.RequestStatus = Declined;
                    await notificationService.DeclineAdoptionNotification(application.Pet.Name, adopter.UserId);

                    await this.data.SaveChangesAsync();
                }
            }
        }

        public async Task<bool> SentApplication(int id, string userId)
            => await this.data
                .AdoptionApplications
                .AnyAsync(x => x.Adopter.UserId == userId
                          && x.PetId == id
                          && x.RequestStatus == Submitted);

        public async Task<AdoptionApplication> GetAdoption(int id)
            => await this.data
                    .AdoptionApplications
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

        public async Task<IEnumerable<AdoptionApplication>> SubmittedPetAdoptionApplications(int petId)
            => await this.data
                .AdoptionApplications
                .Where(x => x.PetId == petId && x.RequestStatus == Submitted)
                .ToListAsync();

        public async Task<Adopter> GetAdopterByAdoptionId(int id)
            => await this.data
                   .Adopters
                   .FirstOrDefaultAsync(x => x.AdoptionApplications.Any(x => x.Id == id));

        public async Task<Pet> GetPetByAdoptionId(int id)
            => await this.data
                   .Pets
                   .FirstOrDefaultAsync(x => x.AdoptionApplications.Any(x => x.Id == id));
    }
}
namespace AdoptMe.Services.Administration
{
    using System.Linq;
    using AdoptMe.Data;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using AdoptMe.Models.Shelters;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Pets;
    using AdoptMe.Services.Notifications;
    using AdoptMe.Services.Users;

    using static Common.GlobalConstants.PageSizes;
    using static Common.GlobalConstants.Roles;

    public class AdministrationService : IAdministrationService
    {
        private readonly AdoptMeDbContext data;
        private readonly INotificationService notificationService;
        private readonly IUserService userService;

        public AdministrationService(AdoptMeDbContext data, 
            INotificationService notificationService,
            IUserService userService)
        {
            this.data = data;
            this.notificationService = notificationService;
            this.userService = userService;
        }

        public async Task<RegistrationRequestsViewModel> RegistrationRequests(int pageIndex)
        {
            var sheltersQuery = this.data
                .Shelters
                .Where(s => s.RegistrationStatus == RequestStatus.Submitted)
                .AsQueryable();

            var shelters = await sheltersQuery
                .Select(x => new ShelterDetailsViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber,
                    Email = this.data.Users
                                     .Where(s => s.Id == x.UserId)
                                     .Select(x => x.Email)
                                     .FirstOrDefault(),
                    Address = this.data.Addresses
                                .Where(a => a.Id == x.AddressId)
                                .Select(a => new AddressViewModel
                                {
                                    CityName = a.City.Name,
                                    StreetName = a.StreetName,
                                    StreetNumber = a.StreetNumber
                                })
                                .FirstOrDefault()
                })
                .Skip((pageIndex - 1) * AdminPanelPagesSize)
                .Take(AdminPanelPagesSize)
                .ToListAsync();

            return new RegistrationRequestsViewModel
            {
                Shelters = shelters,
                TotalShelters = sheltersQuery.Count()
            };
        }

        public async Task<AllPetsViewModel> AllPets(int pageIndex, string sortOrder)
        {
            var petsQuery = this.data.Pets.AsQueryable();

            petsQuery = sortOrder switch
            {
                "Date" => petsQuery.OrderBy(p => p.DateAdded),
                "Shelter" => petsQuery.OrderBy(p => p.Shelter.Name),
                "shelter_desc" => petsQuery.OrderByDescending(p => p.Shelter.Name),
                _ => petsQuery.OrderByDescending(p => p.DateAdded),
            };

            var pets = await petsQuery
                .Select(x => new PetDetailsViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Species = x.Species.Name,
                    Breed = x.Breed,
                    ImageUrl = x.ImageUrl,
                    DateAdded = x.DateAdded,
                    ShelterName = x.Shelter.Name,
                    IsAdopted = x.IsAdopted,
                    IsDeleted = x.IsDeleted
                })
                .Skip((pageIndex - 1) * AdminPanelPagesSize)
                .Take(AdminPanelPagesSize)
                .ToListAsync();

            return new AllPetsViewModel
            {
                Pets = pets,
                TotalPets = petsQuery.Count()
            };
        }

        public async Task AcceptRequest(int id)
        {
            var shelter = await this.GetShelterById(id);
            shelter.RegistrationStatus = RequestStatus.Аccepted;
            this.userService.AddUserToRole(shelter.UserId, ShelterRoleName);

            await this.data.SaveChangesAsync();
        }

        public async Task DeclineRequest(int id)
        {
            var shelter = await this.GetShelterById(id);

            this.data.Shelters.Remove(shelter);
            await this.data.SaveChangesAsync();
        }

        public async Task<Shelter> GetShelterById(int id)
            => await this.data
                .Shelters
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();
    }
}
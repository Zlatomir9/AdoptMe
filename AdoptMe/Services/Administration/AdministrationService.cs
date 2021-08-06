namespace AdoptMe.Services.Administration
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using AdoptMe.Data;
    using AdoptMe.Models.Shelters;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Pets;

    using static Common.GlobalConstants.PageSizes;
    using static Common.GlobalConstants.Roles;

    public class AdministrationService : IAdministrationService
    {
        private readonly AdoptMeDbContext data;
        private readonly UserManager<IdentityUser> userManager;

        public AdministrationService(AdoptMeDbContext data, UserManager<IdentityUser> userManager)
        {
            this.data = data;
            this.userManager = userManager;
        }

        public RegistrationRequestsViewModel RegistrationRequests(int pageIndex)
        {
            var sheltersQuery = this.data
                .Shelters
                .Where(s => s.RegistrationStatus == RegistrationStatus.Submitted)
                .AsQueryable();

            var shelters = sheltersQuery
                .Select(x => new ShelterDetailsViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
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
                .ToList();

            return new RegistrationRequestsViewModel
            {
                Shelters = shelters,
                TotalShelters = sheltersQuery.Count()
            };
        }

        public AllPetsViewModel AllPets(int pageIndex, string sortOrder)
        {
            var petsQuery = this.data.Pets.AsQueryable();

            petsQuery = sortOrder switch
            {
                "Date" => petsQuery.OrderBy(p => p.DateAdded),
                "Shelter" => petsQuery.OrderBy(p => p.Shelter.Name),
                "shelter_desc" => petsQuery.OrderByDescending(p => p.Shelter.Name),
                _ => petsQuery.OrderByDescending(p => p.DateAdded),
            };

            var pets = petsQuery
                .Select(x => new PetDetailsViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Species = x.Species.Name,
                    Breed = x.Breed,
                    ImageUrl = x.ImageUrl,
                    DateAdded = x.DateAdded,
                    ShelterName = x.Shelter.Name
                })
                .Skip((pageIndex - 1) * AdminPanelPagesSize)
                .Take(AdminPanelPagesSize)
                .ToList();

            return new AllPetsViewModel
            {
                Pets = pets,
                TotalPets = petsQuery.Count()
            };
        }

        public void AcceptRequest(int id)
        {
            var shelter = this.GetShelterById(id);

            shelter.RegistrationStatus = RegistrationStatus.Аccepted;

            Task
                .Run(async () =>
                {
                    var user = this.userManager.FindByIdAsync(shelter.UserId).Result;
                    await userManager.AddToRoleAsync(user, ShelterRoleName);
                })
                .GetAwaiter()
                .GetResult();

            this.data.SaveChanges();
        }

        public void DeclineRequest(int id)
        {
            var shelter = this.GetShelterById(id);

            shelter.RegistrationStatus = RegistrationStatus.Declined;

            this.data.SaveChanges();
        }

        public Shelter GetShelterById(int id)
            => this.data
                .Shelters
                .Where(s => s.Id == id)
                .FirstOrDefault();
    }
}
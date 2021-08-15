namespace AdoptMe.Services.Administration
{
    using System.Linq;
    using AdoptMe.Data;
    using AdoptMe.Models.Shelters;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Pets;
    using AdoptMe.Services.Notifications;

    using static Common.GlobalConstants.PageSizes;
    using static Common.GlobalConstants.Roles;
    using AdoptMe.Services.Users;

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

        public RegistrationRequestsViewModel RegistrationRequests(int pageIndex)
        {
            var sheltersQuery = this.data
                .Shelters
                .Where(s => s.RegistrationStatus == RequestStatus.Submitted)
                .AsQueryable();

            var shelters = sheltersQuery
                .Select(x => new ShelterDetailsViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
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
                    ShelterName = x.Shelter.Name,
                    IsAdopted = x.IsAdopted,
                    IsDeleted = x.IsDeleted
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
            shelter.RegistrationStatus = RequestStatus.Аccepted;

            string message = $"Your request for registrating as {shelter.Name} shelter has been approved.";
            var notification = notificationService.Create(message);
            notificationService.AddNotificationToUser(notification.Id, shelter.UserId);

            this.data.SaveChanges();
        }

        public void DeclineRequest(int id)
        {
            var shelter = this.GetShelterById(id);
            this.userService.RemoveUserFromRole(shelter.UserId, ShelterRoleName);

            string message = $"Your request for registrating as {shelter.Name} shelter has been declined. You can send new request.";
            var notification = notificationService.Create(message);
            notificationService.AddNotificationToUser(notification.Id, shelter.UserId);

            this.data.Shelters.Remove(shelter);
            this.data.SaveChanges();
        }

        public Shelter GetShelterById(int id)
            => this.data
                .Shelters
                .Where(s => s.Id == id)
                .FirstOrDefault();
    }
}
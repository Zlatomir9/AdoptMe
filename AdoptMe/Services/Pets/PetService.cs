namespace AdoptMe.Services.Pets
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using AutoMapper;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Models.Pets;
    using AdoptMe.Services.Users;
    using AdoptMe.Services.Adoptions;

    using static Common.GlobalConstants.PageSizes;
    using AdoptMe.Services.Notifications;

    public class PetService : IPetService
    {
        private readonly AdoptMeDbContext data;
        private readonly IUserService userService;
        private readonly IAdoptionService adoptionService;
        private readonly INotificationService notificationService;
        private readonly IMapper mapper;

        public PetService(AdoptMeDbContext data, 
            IMapper mapper,
            IUserService userService, 
            IAdoptionService adoptionService, 
            INotificationService notificationService)
        {
            this.data = data;
            this.mapper = mapper;
            this.userService = userService;
            this.adoptionService = adoptionService;
            this.notificationService = notificationService;
        }

        public AllPetsViewModel All(string species, string searchString, int pageIndex)
        {
            var petsQuery = this.data
                    .Pets
                    .Where(p => p.IsAdopted == false && p.IsDeleted == false)
                    .AsQueryable();

            if (!string.IsNullOrEmpty(species))
            {
                petsQuery = petsQuery.Where(s => s.Species.Name == species);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                petsQuery = petsQuery.Where(s => s.Breed.Contains(searchString));
            }

            var totalPets = petsQuery.Count();

            var pets = petsQuery
                .Select(x => new PetDetailsViewModel
                {
                    Id = x.Id,
                    Species = x.Species.ToString(),
                    Breed = x.Breed,
                    ImageUrl = x.ImageUrl,
                    Name = x.Name,
                    Age = x.Age.ToString(),
                    Gender = x.Gender.ToString(),
                    DateAdded = x.DateAdded
                })
                .OrderBy(d => d.DateAdded)
                .Skip((pageIndex - 1) * AllPetsPageSize)
                .Take(AllPetsPageSize)
                .ToList();

            return new AllPetsViewModel
            {
                Pets = pets,
                SearchString = searchString,
                PageIndex = pageIndex,
                TotalPets = totalPets
            };
        }

        public AllPetsViewModel MyPets(int pageIndex, string sortOrder) 
        {
            var petsQuery = this.data.Pets.AsQueryable();

            petsQuery = sortOrder switch
            {
                "Date" => petsQuery.OrderBy(p => p.DateAdded),
                "Name" => petsQuery.OrderBy(p => p.Name),
                "name_desc" => petsQuery.OrderByDescending(p => p.Name),
                _ => petsQuery.OrderByDescending(p => p.DateAdded),
            };
            
            var userId = this.userService.GetUserId();

            var pets = petsQuery
                .Where(x => x.Shelter.UserId == userId)
                .Select(x => new PetDetailsViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Species = x.Species.Name,
                    Breed = x.Breed,
                    ImageUrl = x.ImageUrl,
                    DateAdded = x.DateAdded,
                    IsAdopted = x.IsAdopted,
                    IsDeleted = x.IsDeleted
                })
                .Skip((pageIndex - 1) * MyPetsPageSize)
                .Take(MyPetsPageSize)
                .ToList();

            return new AllPetsViewModel
            {
                Pets = pets,
                TotalPets = petsQuery.Count()
            };
        }

        public PetDetailsViewModel Details(int id)
            => this.data
                   .Pets
                   .Where(p => p.Id == id)
                   .Select(p => new PetDetailsViewModel
                   {
                       Id = p.Id,
                       Age = p.Age.ToString(),
                       Breed = p.Breed,
                       Color = p.Color,
                       Gender = p.Gender.ToString(),
                       MyStory = p.MyStory,
                       Name = p.Name,
                       ImageUrl = p.ImageUrl,
                       Species = p.Species.ToString(),
                       ShelterName = p.Shelter.Name,
                       ShelterPhoneNumber = p.Shelter.PhoneNumber,
                       ShelterEmail = p.Shelter.Email,
                       UserId = p.Shelter.UserId,
                       SpeciesId = p.SpeciesId,
                       IsAdopted = p.IsAdopted,
                       IsDeleted = p.IsDeleted
                   })
                   .FirstOrDefault();

        public int Add(string name, Age age, string breed, string color, Gender gender,
            string myStory, string imageUrl, int speciesId, int shelterId)
        {
            var petData = new Pet
            {
                Name = name,
                Age = age,
                Breed = breed,
                Color = color,
                Gender = gender,
                MyStory = myStory,
                ImageUrl = imageUrl,
                SpeciesId = speciesId,
                ShelterId = shelterId,
                DateAdded = DateTime.UtcNow
            };

            this.data.Pets.Add(petData);
            this.data.SaveChanges();

            return petData.Id;
        }

        public bool Edit(int id, string name, Age age, string breed, string color, Gender gender,
                    string myStory, string imageUrl, int speciesId)
        {
            var petData = this.data.Pets.FirstOrDefault(x => x.Id == id);

            if (petData == null)
            {
                return false;
            }

            petData.Name = name;
            petData.Age = age;
            petData.Breed = breed;
            petData.Color = color;
            petData.Gender = gender;
            petData.MyStory = myStory;
            petData.ImageUrl = imageUrl;
            petData.SpeciesId = speciesId;

            var shelterData = this.data.Shelters.FirstOrDefault(x => x.Id == petData.ShelterId);

            if (userService.UserIsAdmin())
            {
                PetEditByAdminNotification(petData.Name, shelterData.UserId);
            }

            this.data.SaveChanges();

            return true;
        }

        public void Delete(int id)
        {
            var petData = this.data
                    .Pets
                    .FirstOrDefault(x => x.Id == id);

            var shelterData = this.data
                    .Shelters
                    .FirstOrDefault(x => x.Id == petData.ShelterId);

            petData.IsDeleted = true;

            if (userService.UserIsAdmin())
            {
                PetDeletedByAdminNotification(petData.Name, shelterData.UserId);
            }

            this.data.SaveChanges();

            var petAdoptionApplications = this.adoptionService.SubmittedPetAdoptionApplications(id);

            if (petData.AdoptionApplications.Any())
            {
                foreach (var application in petAdoptionApplications)
                {
                    application.RequestStatus = RequestStatus.Declined;
                    var adopter = adoptionService.GetAdopter(application.AdopterId);
                    adoptionService.DeclinedAdoptionNotification(petData.Name, adopter.UserId);

                    this.data.SaveChanges();
                }
            }            
        }

        public void PetEditByAdminNotification(string petName, string userId)
        {
            var message = $"Your advertisment about {petName} has been edited by administrator.";

            var notification = this.notificationService.Create(message);

            this.notificationService
                .AddNotificationToUser(notification.Id, userId);
        }

        public void PetDeletedByAdminNotification(string petName, string userId)
        {
            var message = $"Your advertisment about {petName} has been deleted by administrator.";

            var notification = this.notificationService.Create(message);

            this.notificationService
                .AddNotificationToUser(notification.Id, userId);
        }

        public IEnumerable<PetSpeciesModel> AllSpecies()
           => this.data
               .Species
               .Select(c => new PetSpeciesModel
               {
                   Id = c.Id,
                   Name = c.Name
               })
               .ToList();

        public bool IsByShelter(int petId, int shelterId)
            => this.data
                .Pets
                .Any(c => c.Id == petId && c.ShelterId == shelterId);

        public bool SpeciesExists(int speciesId)
            => this.data
                .Species
                .Any(c => c.Id == speciesId);

        private static IEnumerable<PetDetailsViewModel> GetPets(IQueryable<Pet> petQuery)
            => petQuery
                .Select(c => new PetDetailsViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Age = c.Age.ToString(),
                    Species = c.Species.Name,
                    Breed = c.Breed,
                    Gender = c.Gender.ToString(),
                    ImageUrl = c.ImageUrl
                })
                .ToList();
    }
}
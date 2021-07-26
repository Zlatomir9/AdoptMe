namespace AdoptMe.Services.Pets
{
    using System.Linq;
    using System.Collections.Generic;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Data.Models.Enums;

    public class PetService : IPetService
    {
        private readonly AdoptMeDbContext data;

        public PetService(AdoptMeDbContext data)
            => this.data = data;


        public PetsQueryServiceModel All(string species, string searchString, int pageIndex, int pageSize)
        {
            var petsQuery = this.data.Pets.AsQueryable();

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
                .Select(x => new PetServiceModel
                {
                    Id = x.Id,
                    Species = x.Species.ToString(),
                    Breed = x.Breed,
                    ImageUrl = x.ImageUrl,
                    Name = x.Name,
                    Age = x.Age.ToString(),
                    Gender = x.Gender.ToString()
                })
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PetsQueryServiceModel
            {
                SearchString = searchString,
                Pets = pets,
                TotalPets = totalPets,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public PetDetailsServiceModel Details(int id)
            => this.data
                   .Pets
                   .Where(p => p.Id == id)
                   .Select(p => new PetDetailsServiceModel
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
                       SpeciesId = p.SpeciesId
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
                ShelterId = shelterId
            };

            this.data.Pets.Add(petData);
            this.data.SaveChanges();

            return petData.Id;
        }

        public bool Edit(int id, string name, Age age, string breed, string color, Gender gender,
                    string myStory, string imageUrl, int speciesId)
        {
            var petData = this.data.Pets.Find(id);

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

            this.data.SaveChanges();

            return true;
        }

        public IEnumerable<PetSpeciesServiceModel> AllSpecies()
           => this.data
               .Species
               .Select(c => new PetSpeciesServiceModel
               {
                   Id = c.Id,
                   Name = c.Name
               })
               .ToList();

        public IEnumerable<PetServiceModel> ByUser(string userId)
            => GetPets(this.data
                .Pets
                .Where(c => c.Shelter.UserId == userId));

        public bool IsByShelter(int petId, int shelterId)
            => this.data
                .Pets
                .Any(c => c.Id == petId && c.ShelterId == shelterId);

        public bool SpeciesExists(int speciesId)
            => this.data
                .Species
                .Any(c => c.Id == speciesId);

        private static IEnumerable<PetServiceModel> GetPets(IQueryable<Pet> petQuery)
            => petQuery
                .Select(c => new PetServiceModel
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

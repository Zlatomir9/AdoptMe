namespace AdoptMe.Services.Pets
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Models.Pets;

    using static Common.GlobalConstants.PageSizes;
    using System.Threading.Tasks;

    public class PetService : IPetService
    {
        private readonly AdoptMeDbContext data;
        private readonly IConfigurationProvider mapper;

        public PetService(AdoptMeDbContext data, IConfigurationProvider mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public async Task<AllPetsViewModel> All(string species, string searchString, int pageIndex)
        {
            var petsQuery = this.data
                    .Pets
                    .Where(p => p.IsAdopted == false && p.IsDeleted == false)
                    .AsQueryable();

            if (!string.IsNullOrEmpty(species))
            {
                petsQuery = petsQuery
                    .Where(s => s.Species.Name == species);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                petsQuery = petsQuery
                    .Where(s => s.Breed.Contains(searchString));
            }

            var totalPets = petsQuery.Count();

            var pets = await petsQuery
                .ProjectTo<PetDetailsViewModel>(this.mapper)
                .OrderBy(d => d.DateAdded)
                .Skip((pageIndex - 1) * AllPetsPageSize)
                .Take(AllPetsPageSize)
                .ToListAsync();

            return new AllPetsViewModel
            {
                Pets = pets,
                SearchString = searchString,
                PageIndex = pageIndex,
                TotalPets = totalPets
            };
        }

        public AllPetsViewModel MyPets(int pageIndex, string sortOrder, string userId)
        {
            var petsQuery = this.data.Pets.AsQueryable();

            petsQuery = sortOrder switch
            {
                "Date" => petsQuery.OrderBy(p => p.DateAdded),
                "Name" => petsQuery.OrderBy(p => p.Name),
                "name_desc" => petsQuery.OrderByDescending(p => p.Name),
                _ => petsQuery.OrderByDescending(p => p.DateAdded),
            };

            var pets = petsQuery
                .Where(x => x.Shelter.UserId == userId)
                .ProjectTo<PetDetailsViewModel>(this.mapper)
                .Skip((pageIndex - 1) * MyPetsPageSize)
                .Take(MyPetsPageSize)
                .ToList();

            return new AllPetsViewModel
            {
                Pets = pets,
                TotalPets = pets.Count()
            };
        }

        public async Task<PetDetailsViewModel> Details(int id)
            => await this.data
                   .Pets
                   .Where(p => p.Id == id)
                   .ProjectTo<PetDetailsViewModel>(this.mapper)
                   .FirstOrDefaultAsync();

        public async Task<int> Add(string name, Age age, string breed, string color, Gender gender,
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

            await this.data.Pets.AddAsync(petData);
            await this.data.SaveChangesAsync();

            return petData.Id;
        }

        public async Task<bool> Edit(int id, string name, Age age, string breed, string color, Gender gender,
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

            await this.data.SaveChangesAsync();

            return true;
        }

        public void Delete(int id)
        {
            var petData = this.data
                    .Pets
                    .FirstOrDefault(x => x.Id == id);

            petData.IsDeleted = true;

            this.data.SaveChanges();
        }

        public void IsAdopted(int id)
        {
            var petData = this.data
                    .Pets
                    .FirstOrDefault(x => x.Id == id);

            petData.IsAdopted = true;

            this.data.SaveChanges();
        }

        public async Task<IEnumerable<PetSpeciesModel>> AllSpecies()
           => await this.data
               .Species
               .ProjectTo<PetSpeciesModel>(this.mapper)
               .ToListAsync();

        public bool AddedByShelter(int petId, string userId)
            => this.data
                .Pets
                .Any(c => c.Id == petId && c.Shelter.UserId == userId);

        public bool SpeciesExists(int speciesId)
            => this.data
                .Species
                .Any(c => c.Id == speciesId);

        public Pet GetPetById(int id)
            => this.data
                .Pets
                .FirstOrDefault(x => x.Id == id);
    }
}
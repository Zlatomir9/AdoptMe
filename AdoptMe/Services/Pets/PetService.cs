﻿namespace AdoptMe.Services.Pets
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

    using static Common.GlobalConstants.PageSizes;

    public class PetService : IPetService
    {
        private readonly AdoptMeDbContext data;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public PetService(AdoptMeDbContext data, IMapper mapper, IUserService userService)
        {
            this.data = data;
            this.mapper = mapper;
            this.userService = userService;
        }

        public AllPetsViewModel All(string species, string searchString, int pageIndex)
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

            

            var pets = petsQuery
                .Select(x => new PetDetailsViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Species = x.Species.Name,
                    Breed = x.Breed,
                    ImageUrl = x.ImageUrl,
                    DateAdded = x.DateAdded
                })
                .Skip((pageIndex - 1) * MyPetsPageSize)
                .Take(MyPetsPageSize)
                .ToList();

            var userId = this.userService.GetUserId();
            pets = ByUser(userId).ToList();

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

        public IEnumerable<PetSpeciesModel> AllSpecies()
           => this.data
               .Species
               .Select(c => new PetSpeciesModel
               {
                   Id = c.Id,
                   Name = c.Name
               })
               .ToList();

        public IEnumerable<PetDetailsViewModel> ByUser(string userId)
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
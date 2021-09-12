namespace AdoptMe.Tests.Services
{
    using Xunit;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using FluentAssertions;
    using AutoMapper;
    using AdoptMe.Services.Pets;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Models.Pets;
    using AdoptMe.Infrastructure;

    public class PetServiceTest
    {
        [Theory]
        [InlineData("Name1", Age.Adult, "Labrador", "Yellow", Gender.Male,
                    "Behold 12-year-old SIR NEKO! Like any true gentleman, he knows his manners, Sit, Stay, Come. His beard may be frosted, but he still enjoys a good game of fetch or a nicely paced stroll on the trail",
                    "https://upload.wikimedia.org/wikipedia/commons/thumb/2/26/YellowLabradorLooking_new.jpg/250px-YellowLabradorLooking_new.jpg",
                    1, 1)]
        public async Task AddMethodShouldAddPetInDatabase(string name,
            Age age, string breed,
            string color,
            Gender gender,
            string myStory,
            string imageUrl,
            int speciesId,
            int shelterId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var petService = new PetService(db, null);

            var pet = await petService.Add(name, age, breed, color, gender, myStory, imageUrl, speciesId, shelterId);

            var actualPet = await db.Pets.FirstOrDefaultAsync(x => x.Id == pet);

            pet.Should().Be(1);
            db.Pets.Should().HaveCount(1);

            Assert.True(pet == actualPet.Id);
        }

        [Theory]
        [InlineData(2, "Name2", Age.Adult, "Pit bull", "Yellow", Gender.Female,
            "Behold 12-year-old SIR NEKO! Like any true gentleman, he knows his manners, Sit, Stay, Come. His beard may be frosted, but he still enjoys a good game of fetch or a nicely paced stroll on the trail",
            "https://upload.wikimedia.org/wikipedia/commons/thumb/2/26/YellowLabradorLooking_new.jpg/250px-YellowLabradorLooking_new.jpg",
            1)]
        public async Task EditMethodShouldChangePetData(int id,
                    string name,
                    Age age,
                    string breed,
                    string color,
                    Gender gender,
                    string myStory,
                    string imageUrl,
                    int speciesId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var pet = new Pet
            {
                Id = id,
                Name = name,
                Age = age,
                Breed = breed,
                Color = color,
                Gender = gender,
                MyStory = myStory,
                ImageUrl = imageUrl,
                SpeciesId = speciesId
            };

            db.Pets.Add(pet);
            db.SaveChanges();

            var petService = new PetService(db, null);

            await petService.Edit(id, name, Age.Young, breed, color, gender, myStory, imageUrl, speciesId);

            var result = db.Pets.FirstOrDefault(x => x.Id == pet.Id);

            var expected = new Pet
            {
                Id = id,
                Name = name,
                Age = Age.Young,
                Breed = breed,
                Color = color,
                Gender = gender,
                MyStory = myStory,
                ImageUrl = imageUrl,
                SpeciesId = speciesId
            };

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(3)]
        public async Task DeleteShouldSetIsDeletedToTrue(int id)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var pet = new Pet
            {
                Id = id,
                Name = "Name",
                Age = Age.Adult,
                Breed = "Breed",
                Color = "Color",
                Gender = Gender.Female,
                MyStory = "Behold 12-year-old SIR NEKO! Like any true gentleman, he knows his manners, Sit, Stay, Come. His beard may be frosted, but he still enjoys a good game of fetch or a nicely paced stroll on the trail",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/26/YellowLabradorLooking_new.jpg/250px-YellowLabradorLooking_new.jpg",
                SpeciesId = 1
            };

            db.Pets.Add(pet);
            db.SaveChanges();

            var petService = new PetService(db, null);

            await petService.Delete(3);

            var result = db.Pets.FirstOrDefault(x => x.Id == id);

            result.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllSpeciesMethodShouldReturnAllSpeciesFromDb()
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            var autoMapper = new MapperConfiguration(
                mc => mc.AddProfile(new MappingProfile()))
                .CreateMapper()
                .ConfigurationProvider;

            var species = new List<Species>
            {
                new Species
                {
                    Id = 1,
                    Name = "Dog"
                },
                new Species
                {
                    Id = 2,
                    Name = "Cat"
                },
                new Species
                {
                    Id = 3,
                    Name = "Rabbit"
                }
            };

            await db.Species.AddRangeAsync(species);
            await db.SaveChangesAsync();

            var petService = new PetService(db, autoMapper);
            var result = await petService.AllSpecies();

            var expected = new List<PetSpeciesModel>
            {
                new PetSpeciesModel
                {
                    Id = 1,
                    Name = "Dog"
                },
                new PetSpeciesModel
                {
                    Id = 2,
                    Name = "Cat"
                },
                new PetSpeciesModel
                {
                    Id = 3,
                    Name = "Rabbit"
                }
            };

            result.Count().Should().Be(3);
            result.Should().BeEquivalentTo(expected);
        }


        [Theory]
        [InlineData(22, "shelter")]
        public async Task AddedByShelterMethodShouldReturnTrueIfPetIsAddedByShelter(int petId, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var shelter = new Shelter
            {
                Id = 1,
                UserId = userId
            };

            var pet = new Pet
            {
                Id = petId,
                ShelterId = shelter.Id
            };
            
            db.Shelters.Add(shelter);
            db.Pets.Add(pet);

            db.SaveChanges();

            var petService = new PetService(db, null);
            var result = await petService.AddedByShelter(petId, userId);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(4)]
        [InlineData(6)]
        [InlineData(12)]
        public void SpeciesExistsMethodShouldReturnTrueIfSpeciesIsAddedToDb(int speciesId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var species = new Species
            {
                Id = speciesId
            };

            db.Species.Add(species);
            db.SaveChanges();

            var petService = new PetService(db, null);
            var result = petService.SpeciesExists(speciesId);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        public async Task GetPetByIdShouldReturnPet(int petId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var pet = new Pet
            {
                Id = petId
            };

            await db.Pets.AddAsync(pet);
            await db.SaveChangesAsync();

            var petService = new PetService(db, null);
            var result = await petService.GetPetById(petId);

            var expected = await db.Pets.FirstOrDefaultAsync(x => x.Id == petId);

            result.Should().BeEquivalentTo(expected);
        }
    }
}

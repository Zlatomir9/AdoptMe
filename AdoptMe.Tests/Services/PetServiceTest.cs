namespace AdoptMe.Tests.Services
{
    using Xunit;
    using Moq;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using FluentAssertions;
    using AdoptMe.Services.Pets;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Data.Models.Enums;

    public class PetServiceTest
    {
        [Theory]
        [InlineData("Name1", Age.Adult, "Labrador", "Yellow", Gender.Male,
                    "Behold 12-year-old SIR NEKO! Like any true gentleman, he knows his manners, Sit, Stay, Come. His beard may be frosted, but he still enjoys a good game of fetch or a nicely paced stroll on the trail", 
                    "https://upload.wikimedia.org/wikipedia/commons/thumb/2/26/YellowLabradorLooking_new.jpg/250px-YellowLabradorLooking_new.jpg", 
                    1, 1)]
        public void AddMethodShouldAddPetInDatabase(string name, 
            Age age, string breed, 
            string color, 
            Gender gender, 
            string myStory, 
            string imageUrl, 
            int speciesId,
            int shelterId)
        {
            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase("data")
                        .Options;

            var db = new AdoptMeDbContext(options);

            var petsService = new PetService(db, null, null, null, null, null);

            var pet = petsService.Add(name, age, breed, color, gender, myStory, imageUrl, speciesId, shelterId);

            var actualPet = db.Pets.FirstOrDefault();

            pet.Should().Be(1);
            db.Pets.Should().HaveCount(1);

            Assert.True(pet == actualPet.Id);
        }

        [Theory]
        [InlineData(10, "Name2", Age.Adult, "Pit bull", "Yellow", Gender.Female,
            "Behold 12-year-old SIR NEKO! Like any true gentleman, he knows his manners, Sit, Stay, Come. His beard may be frosted, but he still enjoys a good game of fetch or a nicely paced stroll on the trail", 
            "https://upload.wikimedia.org/wikipedia/commons/thumb/2/26/YellowLabradorLooking_new.jpg/250px-YellowLabradorLooking_new.jpg", 
            1)]
        public void EditMethodShouldChangePetData(int id, 
                    string name, 
                    Age age, 
                    string breed, 
                    string color, 
                    Gender gender, 
                    string myStory, 
                    string imageUrl, 
                    int speciesId)
        {
            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                .UseInMemoryDatabase("data")
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

            var petService = new PetService(db, null, null, null, null, null);

            petService.Edit(id, name, age, breed, color, gender, myStory, imageUrl, 3);

            var actual = db.Pets.FirstOrDefault();

            var expected = new Pet
            {
                Id = 10,
                Name = name,
                Age = age,
                Breed = breed,
                Color = color,
                Gender = gender,
                MyStory = myStory,
                ImageUrl = imageUrl,
                SpeciesId = 3
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(3)]
        public void DeleteShouldSetIsDeletedToTrue(int id)
        {
            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                .UseInMemoryDatabase("data")
                .Options;

            var db = new AdoptMeDbContext(options);

            var pet = new Pet
            {
                Id = id,
                Name = "Name",
                Age = Age.Senior,
                Breed = "Breed",
                Color = "Color",
                Gender = Gender.Female,
                MyStory = "Behold 12-year-old SIR NEKO! Like any true gentleman, he knows his manners, Sit, Stay, Come. His beard may be frosted, but he still enjoys a good game of fetch or a nicely paced stroll on the trail",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/26/YellowLabradorLooking_new.jpg/250px-YellowLabradorLooking_new.jpg",
                SpeciesId = 1
            };

            db.Pets.Add(pet);
            db.SaveChanges();

            var petService = new PetService(db, null, null, null, null, null);

            petService.Delete(1);

            var result = db.Pets.FirstOrDefault();

            result.IsDeleted.Should().BeTrue();
        }
    }
}

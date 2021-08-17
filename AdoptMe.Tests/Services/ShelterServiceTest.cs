namespace AdoptMe.Tests.Services
{
    using System;
    using Xunit;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using FluentAssertions;
    using AdoptMe.Data;
    using AdoptMe.Services.Shelters;
    using AdoptMe.Data.Models;

    using static Data.Models.Enums.RequestStatus;
    using System.Collections.Generic;

    public class ShelterServiceTest
    {
        [Theory]
        [InlineData("Name", "+35988888888", "City", "Street", "14A", "userId")]
        public void CreateShouldAddShelterInTheDatabase(string name, string phoneNumber, string cityName, string streetName, string streetNumber, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var shelterService = new ShelterService(db);

            var shelter = shelterService.Create(name, phoneNumber, cityName, streetName, streetNumber, userId);

            var actualShelter = db.Shelters.FirstOrDefault(x => x.Id == shelter);

            shelter.Should().Be(1);
            db.Shelters.Should().NotBeEmpty();
            db.Addresses.Should().Contain(a => a.StreetName == streetName);
            db.Cities.Should().Contain(c => c.Name == cityName);

            Assert.True(shelter == actualShelter.Id);
        }

        [Theory]
        [InlineData("userId")]
        public void GetIdByUserShouldReturnShelterId(string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var shelter = new Shelter
            {
                Id = 100,
                UserId = userId
            };

            db.Shelters.Add(shelter);
            db.SaveChanges();

            var shelterService = new ShelterService(db);
            var result = shelterService.IdByUser(userId);

            result.Should().Be(100);
        }

        [Theory]
        [InlineData("userId")]
        public void RegistrationIsSubmittedShouldReturnTrue(string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var shelter = new Shelter
            {
                RegistrationStatus = Submitted,
                UserId = userId
            };

            db.Shelters.Add(shelter);
            db.SaveChanges();

            var shelterService = new ShelterService(db);
            var result = shelterService.RegistrationIsSubmitted(userId);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("userId")]
        public void RegistrationIsSubmittedShouldReturnFalse(string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var shelter = new Shelter
            {
                RegistrationStatus = Declined,
                UserId = userId
            };

            db.Shelters.Add(shelter);
            db.SaveChanges();

            var shelterService = new ShelterService(db);
            var result = shelterService.RegistrationIsSubmitted(userId);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(1, "shelterId")]
        [InlineData(100, "id")]
        public void GetShelterUserIdByPetShouldReturnWorkCorrectly(int petId, string userId)
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

            var shelter = new Shelter
            {
                UserId = userId,
                Pets = new List<Pet>() { pet }
            };

            db.Pets.Add(pet);
            db.Shelters.Add(shelter);

            db.SaveChanges();

            var shelterService = new ShelterService(db);
            var result = shelterService.GetShelterUserIdByPet(petId);

            result.Should().Be(userId);
        }
    }
}

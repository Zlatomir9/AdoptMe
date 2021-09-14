namespace AdoptMe.Tests.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;
    using FluentAssertions;
    using AutoMapper;
    using AdoptMe.Infrastructure;
    using AdoptMe.Data;
    using AdoptMe.Services.Adoptions;
    using AdoptMe.Data.Models;
    using AdoptMe.Services.Notifications;

    using static Data.Models.Enums.RequestStatus;

    public class AdoptionServiceTest
    {
        [Theory]
        [InlineData("FirstQuestion", "SecondQuestion", "ThirdQuestion", "FourthQuestion", 1, "userId")]
        public async Task CreateAdoptionShouldAddAdoptionInTheDatabase(string firstQuestion, string secondQuestion, string thirdQuestion, string fourthQuestion, int petId, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var autoMapper = new MapperConfiguration(
                mc => mc.AddProfile(new MappingProfile()))
                .CreateMapper()
                .ConfigurationProvider;

            var adopter = new Adopter
            {
                UserId = userId
            };

            await db.Adopters.AddAsync(adopter);
            await db.SaveChangesAsync();

            var adoptionService = new AdoptionService(null, autoMapper, db);

            var adoption = await adoptionService.CreateAdoption(firstQuestion, secondQuestion, thirdQuestion, fourthQuestion, petId, userId);

            adoption.Should().Be(1);
            db.AdoptionApplications.Should().NotBeEmpty();
            db.AdoptionApplications.Should().Contain(a => a.PetId == petId);
            db.AdoptionApplications.Should().Contain(c => c.Adopter.UserId == userId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task ApproveAdoptionShouldChangeAdoptionStatus(int adoptionId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var adoptionAplication = new AdoptionApplication
            {
                Id = adoptionId,
                RequestStatus = Submitted
            };

            await db.AdoptionApplications.AddAsync(adoptionAplication);
            await db.SaveChangesAsync();

            
            var autoMapper = new MapperConfiguration(
                mc => mc.AddProfile(new MappingProfile()))
                .CreateMapper()
                .ConfigurationProvider;

            var adoptionService = new AdoptionService(null, autoMapper, db);

            await adoptionService.ApproveAdoption(adoptionId);

            var expected = new AdoptionApplication
            {
                Id = adoptionId,
                RequestStatus = Аccepted
            };

            adoptionAplication.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task DeclineAdoptionShouldChangeAdoptionStatus(int adoptionId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var adoptionAplication = new AdoptionApplication
            {
                Id = adoptionId,
                RequestStatus = Submitted
            };

            await db.AdoptionApplications.AddAsync(adoptionAplication);
            await db.SaveChangesAsync();

            var autoMapper = new MapperConfiguration(
                mc => mc.AddProfile(new MappingProfile()))
                .CreateMapper()
                .ConfigurationProvider;

            var adoptionService = new AdoptionService(null, autoMapper, db);

            await adoptionService.DeclineAdoption(adoptionId);

            var expected = new AdoptionApplication
            {
                Id = adoptionId,
                RequestStatus = Declined
            };

            adoptionAplication.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(1, "user1")]
        public async Task SentApplicationShouldReturnTrueIfAdopterSentApplicationForPet(int petId, string userId)
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

            var adopter = new Adopter
            {
                UserId = userId
            };

            var adoptionAplication = new AdoptionApplication
            {
                PetId = pet.Id,
                Adopter = adopter,
                RequestStatus = Submitted
            };

            await db.Pets.AddAsync(pet);
            await db.Adopters.AddAsync(adopter);
            await db.AdoptionApplications.AddAsync(adoptionAplication);
            await db.SaveChangesAsync();

            var autoMapper = new MapperConfiguration(
                mc => mc.AddProfile(new MappingProfile()))
                .CreateMapper()
                .ConfigurationProvider;

            var adoptionService = new AdoptionService(null, autoMapper, db);

            var result = await adoptionService.SentApplication(adoptionAplication.PetId, adoptionAplication.Adopter.UserId);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(1, "user1")]
        public async Task SentApplicationShouldReturnFalse(int petId, string userId)
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

            var adopter = new Adopter
            {
                UserId = userId
            };

            var adoptionAplication = new AdoptionApplication
            {
                PetId = pet.Id,
                Adopter = adopter,
                RequestStatus = Аccepted
            };

            await db.Pets.AddAsync(pet);
            await db.Adopters.AddAsync(adopter);
            await db.AdoptionApplications.AddAsync(adoptionAplication);
            await db.SaveChangesAsync();

            var autoMapper = new MapperConfiguration(
                mc => mc.AddProfile(new MappingProfile()))
                .CreateMapper()
                .ConfigurationProvider;

            var adoptionService = new AdoptionService(null, autoMapper, db);

            var result = await adoptionService.SentApplication(adoptionAplication.PetId, adoptionAplication.Adopter.UserId);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        public async Task GetAdoptionShouldReturnAdoption(int adoptionId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var adoptionAplication = new AdoptionApplication
            {
                Id = adoptionId
            };

            await db.AdoptionApplications.AddAsync(adoptionAplication);
            await db.SaveChangesAsync();

            var autoMapper = new MapperConfiguration(
                mc => mc.AddProfile(new MappingProfile()))
                .CreateMapper()
                .ConfigurationProvider;

            var adoptionService = new AdoptionService(null, autoMapper, db);

            var result = await adoptionService.GetAdoption(adoptionId);

            var expected = new AdoptionApplication
            {
                Id = adoptionId
            };

            result.Should().BeEquivalentTo(expected);
            result.Id.Should().Be(adoptionId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(21)]
        public async Task SubmittedAdoptionsShouldWorkCorrectly(int petId)
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

            var firstAplication = new AdoptionApplication
            {
                PetId = petId,
                RequestStatus = Submitted
            }; 
            
            var secondAplication = new AdoptionApplication
            {
                PetId = petId,
                RequestStatus = Submitted
            };

            await db.Pets.AddAsync(pet);
            await db.AdoptionApplications.AddRangeAsync(firstAplication, secondAplication);
            await db.SaveChangesAsync();

            var autoMapper = new MapperConfiguration(
                mc => mc.AddProfile(new MappingProfile()))
                .CreateMapper()
                .ConfigurationProvider;

            var adoptionService = new AdoptionService(null, autoMapper, db);

            var result = await adoptionService.SubmittedPetAdoptionApplications(petId);

            result.Should().HaveCount(2);
        }

        [Theory]
        [InlineData(1, "firstName", 1)]
        [InlineData(21, "firstNameee", 1)]
        public async Task GetAdopterIdByAdoptionIdShouldWorkCorrectly(int adoptionId, string firstName, int adopterId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var adopter = new Adopter
            {
                Id = adopterId,
                FirstName = firstName
            };

            var aplication = new AdoptionApplication
            {
                Id = adoptionId,
                AdopterId = adopterId
            };

            await db.Adopters.AddAsync(adopter);
            await db.AdoptionApplications.AddAsync(aplication);
            await db.SaveChangesAsync();

            var autoMapper = new MapperConfiguration(
                mc => mc.AddProfile(new MappingProfile()))
                .CreateMapper()
                .ConfigurationProvider;

            var adoptionService = new AdoptionService(null, autoMapper, db);

            var result = await adoptionService.GetAdopterByAdoptionId(adoptionId);

            var expected = db.Adopters.FirstOrDefault();

            expected.Should().BeEquivalentTo(result);
        }

        [Theory]
        [InlineData(1, "firstName", 1)]
        [InlineData(21, "firstNameee", 1)]
        public async Task GetPetIdByAdoptionIdShouldWorkCorrectly(int adoptionId, string petName, int petId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var pet = new Pet
            {
                Id = petId,
                Name = petName
            };

            var aplication = new AdoptionApplication
            {
                Id = adoptionId,
                PetId = petId
            };

            await db.Pets.AddAsync(pet);
            await db.AdoptionApplications.AddAsync(aplication);
            await db.SaveChangesAsync();

            var autoMapper = new MapperConfiguration(
                mc => mc.AddProfile(new MappingProfile()))
                .CreateMapper()
                .ConfigurationProvider;

            var adoptionService = new AdoptionService(null, autoMapper, db);

            var result = await adoptionService.GetPetByAdoptionId(adoptionId);

            var expected = db.Pets.FirstOrDefault();

            expected.Should().BeEquivalentTo(result);
        }

        [Theory]
        [InlineData(1, "PetName", "userId", 2, 3)]
        public async Task DeclineAdoptionWhenPetIsDeletedOrAdoptedShouldDeclineAllSubmittedAdoptions(int petId, string petName, string userId, int firstAdopterId, int secondAdopterId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var pet = new Pet
            {
                Id = petId,
                Name = petName
            };

            var firstAdopter = new Adopter
            {
                Id = firstAdopterId,
                UserId = userId
            };

            var secondAdopter = new Adopter
            {
                Id = secondAdopterId,
                UserId = userId
            };

            var firstApplication = new AdoptionApplication
            {
                PetId = petId,
                RequestStatus = Submitted,
                AdopterId = firstAdopterId
            };

            var secondApplication = new AdoptionApplication
            {
                PetId = petId,
                RequestStatus = Submitted,
                AdopterId = secondAdopterId
            };

            await db.Pets.AddAsync(pet);
            await db.Adopters.AddRangeAsync(firstAdopter, secondAdopter);
            await db.AdoptionApplications.AddRangeAsync(firstApplication, secondApplication);
            await db.SaveChangesAsync();

            var notificationService = new Mock<INotificationService>();
            notificationService.Setup(u => u.DeclineAdoptionNotification(petName, firstAdopter.UserId));

            var autoMapper = new MapperConfiguration(
                mc => mc.AddProfile(new MappingProfile()))
                .CreateMapper()
                .ConfigurationProvider;

            var adoptionService = new AdoptionService(notificationService.Object, autoMapper, db);

            await adoptionService.DeclineAdoptionWhenPetIsDeletedOrAdopted(petId);

            firstApplication.RequestStatus.Should().Be(Declined);
            secondApplication.RequestStatus.Should().Be(Declined);
        }
    }
}

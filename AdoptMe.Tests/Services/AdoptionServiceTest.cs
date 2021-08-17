namespace AdoptMe.Tests.Services
{
    using System;
    using System.Linq;
    using Moq;
    using Xunit;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using AdoptMe.Data;
    using AdoptMe.Services.Adoptions;
    using AdoptMe.Data.Models;
    using AdoptMe.Services.Notifications;

    using static Data.Models.Enums.RequestStatus;

    public class AdoptionServiceTest
    {
        [Theory]
        [InlineData("FirstQuestion", "SecondQuestion", "ThirdQuestion", "FourthQuestion", 1, "userId")]
        public void CreateAdoptionShouldAddAdoptionInTheDatabase(string firstQuestion, string secondQuestion, string thirdQuestion, string fourthQuestion, int petId, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var adopter = new Adopter
            {
                UserId = userId
            };

            db.Adopters.Add(adopter);
            db.SaveChanges();

            var adoptionService = new AdoptionService(null, db);

            var adoption = adoptionService.CreateAdoption(firstQuestion, secondQuestion, thirdQuestion, fourthQuestion, petId, userId);

            adoption.Should().Be(1);
            db.AdoptionApplications.Should().NotBeEmpty();
            db.AdoptionApplications.Should().Contain(a => a.PetId == petId);
            db.AdoptionApplications.Should().Contain(c => c.Adopter.UserId == userId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ApproveAdoptionShouldChangeAdoptionStatus(int adoptionId)
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

            db.AdoptionApplications.Add(adoptionAplication);
            db.SaveChanges();

            var adoptionService = new AdoptionService(null, db);

            adoptionService.ApproveAdoption(adoptionId);

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
        public void DeclineAdoptionShouldChangeAdoptionStatus(int adoptionId)
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

            db.AdoptionApplications.Add(adoptionAplication);
            db.SaveChanges();

            var adoptionService = new AdoptionService(null, db);

            adoptionService.DeclineAdoption(adoptionId);

            var expected = new AdoptionApplication
            {
                Id = adoptionId,
                RequestStatus = Declined
            };

            adoptionAplication.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(1, "user1")]
        public void SentApplicationShouldReturnTrueIfAdopterSentApplicationForPet(int petId, string userId)
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

            db.Pets.Add(pet);
            db.Adopters.Add(adopter);
            db.AdoptionApplications.Add(adoptionAplication);
            db.SaveChanges();

            var adoptionService = new AdoptionService(null, db);

            var result = adoptionService.SentApplication(adoptionAplication.PetId, adoptionAplication.Adopter.UserId);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(1, "user1")]
        public void SentApplicationShouldReturnFalse(int petId, string userId)
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

            db.Pets.Add(pet);
            db.Adopters.Add(adopter);
            db.AdoptionApplications.Add(adoptionAplication);
            db.SaveChanges();

            var adoptionService = new AdoptionService(null, db);

            var result = adoptionService.SentApplication(adoptionAplication.PetId, adoptionAplication.Adopter.UserId);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        public void GetAdoptionShouldReturnAdoption(int adoptionId)
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

            db.AdoptionApplications.Add(adoptionAplication);
            db.SaveChanges();

            var adoptionService = new AdoptionService(null, db);

            var result = adoptionService.GetAdoption(adoptionId);

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
        public void SubmittedAdoptionsShouldWorkCorrectly(int petId)
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

            db.Pets.Add(pet);
            db.AdoptionApplications.AddRange(firstAplication, secondAplication);
            db.SaveChanges();

            var adoptionService = new AdoptionService(null, db);

            var result = adoptionService.SubmittedPetAdoptionApplications(petId);

            result.Should().HaveCount(2);
        }

        [Theory]
        [InlineData(1, "firstName", 1)]
        [InlineData(21, "firstNameee", 1)]
        public void GetAdopterIdByAdoptionIdShouldWorkCorrectly(int adoptionId, string firstName, int adopterId)
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

            db.Adopters.Add(adopter);
            db.AdoptionApplications.Add(aplication);
            db.SaveChanges();

            var adoptionService = new AdoptionService(null, db);

            var result = adoptionService.GetAdopterByAdoptionId(adoptionId);

            var expected = db.Adopters.FirstOrDefault();

            expected.Should().BeEquivalentTo(result);
        }

        [Theory]
        [InlineData(1, "firstName", 1)]
        [InlineData(21, "firstNameee", 1)]
        public void GetPetIdByAdoptionIdShouldWorkCorrectly(int adoptionId, string petName, int petId)
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

            db.Pets.Add(pet);
            db.AdoptionApplications.Add(aplication);
            db.SaveChanges();

            var adoptionService = new AdoptionService(null, db);

            var result = adoptionService.GetPetByAdoptionId(adoptionId);

            var expected = db.Pets.FirstOrDefault();

            expected.Should().BeEquivalentTo(result);
        }

        [Theory]
        [InlineData(1, "PetName", "userId", 2, 3)]
        public void DeclineAdoptionWhenPetIsDeletedOrAdoptedShouldDeclineAllSubmittedAdoptions(int petId, string petName, string userId, int firstAdopterId, int secondAdopterId)
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

            db.Pets.Add(pet);
            db.Adopters.AddRange(firstAdopter, secondAdopter);
            db.AdoptionApplications.AddRange(firstApplication, secondApplication);
            db.SaveChanges();

            var notificationService = new Mock<INotificationService>();
            notificationService.Setup(u => u.DeclineAdoptionNotification(petName, firstAdopter.UserId));

            var adoptionService = new AdoptionService(notificationService.Object, db);

            adoptionService.DeclineAdoptionWhenPetIsDeletedOrAdopted(petId);

            firstApplication.RequestStatus.Should().Be(Declined);
            secondApplication.RequestStatus.Should().Be(Declined);
        }
    }
}

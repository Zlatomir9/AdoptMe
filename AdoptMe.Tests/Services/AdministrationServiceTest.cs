namespace AdoptMe.Tests.Services
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Xunit;
    using Moq;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Services.Administration;
    using AdoptMe.Services.Users;

    using static Data.Models.Enums.RequestStatus;
    using static Common.GlobalConstants.Roles;
    using FluentAssertions;
    using AdoptMe.Services.Notifications;

    public class AdministrationServiceTest
    {
        [Theory]
        [InlineData(1, "userId")]
        public void AcceptRequestShouldAcceptShelterRequest(int shelterId, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var shelter = new Shelter
            {
                Id = shelterId,
                RegistrationStatus = Submitted,
                UserId = userId
            };

            db.Shelters.Add(shelter);
            db.SaveChanges();

            var userService = new Mock<IUserService>();
            userService.Setup(u => u.AddUserToRole(userId, ShelterRoleName));

            var administrationService = new AdministrationService(db, null, userService.Object);

            administrationService.AcceptRequest(shelterId);

            var expected = new Shelter
            {
                Id = shelterId,
                RegistrationStatus = Аccepted,
                UserId = userId
            };

            shelter.RegistrationStatus.Should().Be(Аccepted);
            shelter.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(1, "userId")]
        public void DeclineRequestShouldDeclineShelterRequestAndRemoveShelterFromSheltersTable(int shelterId, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var shelter = new Shelter
            {
                Id = shelterId,
                RegistrationStatus = Submitted,
                UserId = userId
            };

            db.Shelters.Add(shelter);
            db.SaveChanges();

            var administrationService = new AdministrationService(db, null, null);

            administrationService.DeclineRequest(shelterId);

            db.Shelters.Should().BeEmpty();
        }

        [Theory]
        [InlineData(1, "Shelter")]
        public void GetShelterByIdShouldReturnShelter(int shelterId, string name)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var shelter = new Shelter
            {
                Id = shelterId,
                Name = name
            };

            db.Shelters.Add(shelter);
            db.SaveChanges();

            var administrationService = new AdministrationService(db, null, null);

            var result = administrationService.GetShelterById(shelterId);

            var expected = new Shelter
            {
                Id = shelterId,
                Name = name
            };

            expected.Should().BeEquivalentTo(result);
        }
    }
}

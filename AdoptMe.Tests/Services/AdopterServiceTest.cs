namespace AdoptMe.Tests.Services
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using Moq;
    using Xunit;
    using FluentAssertions;
    using AdoptMe.Data;
    using AdoptMe.Services.Adopters;
    using AdoptMe.Services.Users;
    using AdoptMe.Data.Models;

    public class AdopterServiceTest
    {
        [Theory]
        [InlineData("firstName", "lastName", 20, "userId")]
        [InlineData("Name", "Name2", 28, "user")]
        public void CreateShouldAddAdopterInDb(string firstName, string lastName, int age, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var userService = new Mock<IUserService>();

            var adopterService = new AdopterService(db, userService.Object);

            var result = adopterService.Create(firstName, lastName, age, userId);

            var actual = db.Adopters.FirstOrDefault();

            result.Should().Be(1);
            actual.FirstName.Should().BeEquivalentTo(firstName);
            actual.LastName.Should().BeEquivalentTo(lastName);
            actual.Age.Should().Be(age);
            actual.UserId.Should().BeEquivalentTo(userId);
        }

        [Theory]
        [InlineData("userId")]
        [InlineData("userId2")]
        public void IsAdopterShouldReturnTrue(string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var adopterService = new AdopterService(db, null);

            var adopter = new Adopter
            {
                UserId = userId
            };

            db.Adopters.Add(adopter);
            db.SaveChanges();

            var result = adopterService.IsAdopter(userId);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("userId")]
        [InlineData("userId2")]
        public void IsAdopterShouldReturnFalse(string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var adopterService = new AdopterService(db, null);

            var adopter = new Adopter
            {
                UserId = "otherId"
            };

            db.Adopters.Add(adopter);
            db.SaveChanges();

            var result = adopterService.IsAdopter(userId);

            result.Should().BeFalse();
        }
    }
}

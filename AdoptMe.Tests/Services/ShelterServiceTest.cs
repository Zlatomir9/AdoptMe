namespace AdoptMe.Tests.Services
{
    using System;
    using Xunit;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using FluentAssertions;
    using AdoptMe.Data;
    using AdoptMe.Services.Shelters;

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

            var shelterService = new ShelterService(db, null);

            var shelter = shelterService.Create(name, phoneNumber, cityName, streetName, streetNumber, userId);

            var actualShelter = db.Shelters.FirstOrDefault(x => x.Id == shelter);

            shelter.Should().Be(1);
            db.Shelters.Should().HaveCount(1);

            Assert.True(shelter == actualShelter.Id);
        }
    }
}

namespace AdoptMe.Tests.Services
{
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Services.Statistics;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using System;
    using Xunit;

    using static Data.Models.Enums.RequestStatus;

    public class StatisticServiceTest
    {
        [Fact]
        public void TotalShouldReturnCorrectData()
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var statisticService = new StatisticsService(db);

            var pet = new Pet { };
            
            var shelter = new Shelter
            {
                RegistrationStatus = Аccepted
            };
            
            var adoption = new AdoptionApplication
            {
                RequestStatus = Аccepted
            };

            var secondAdoption = new AdoptionApplication
            {
                RequestStatus = Аccepted
            };

            db.Pets.Add(pet);
            db.Shelters.Add(shelter);
            db.AdoptionApplications.Add(adoption);
            db.AdoptionApplications.Add(secondAdoption);

            db.SaveChanges();

            var result = statisticService.Total();

            result.TotalAdoptions.Should().Be(2);
            result.TotalPets.Should().Be(1);
            result.TotalShelters.Should().Be(1);
        }
    }
}

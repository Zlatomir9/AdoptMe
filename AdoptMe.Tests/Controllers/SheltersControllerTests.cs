namespace AdoptMe.Tests.Controllers
{
    using Xunit;
    using MyTested.AspNetCore.Mvc;
    using System.Linq;
    using AdoptMe.Controllers;
    using AdoptMe.Models.Shelters;
    using AdoptMe.Data.Models;

    public class SheltersControllerTests
    {
        [Fact]
        public void GetCreateShelterShouldBeForAuthorizedUsersAndReturnView()
            => MyController<SheltersController>
                .Instance()
                .Calling(c => c.Create())
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .View();

        [Theory]
        [InlineData("Shelter", "+359111111111", "Plovdiv", "Tsarevets", "10")]
        public void PostCreateShouldBeForAuthorizedUsersAndReturnRedirectWithValidModel(
            string shelterName,
            string shelterPhoneNumber,
            string shelterCityName,
            string shelterStreetName,
            string shelterStreetNumber)
            => MyController<SheltersController>
                .Instance(controller => controller
                    .WithUser())
                .Calling(c => c.Create(new CreateShelterFormModel
                {
                    Name = shelterName,
                    PhoneNumber = shelterPhoneNumber,
                    CityName = shelterCityName,
                    StreetName = shelterStreetName,
                    StreetNumber = shelterStreetNumber
                }))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests())
                .ValidModelState()
                .Data(data => data
                    .WithSet<Shelter>(shelters => shelters
                        .Any(s =>
                            s.Name == shelterName &&
                            s.UserId == TestUser.Identifier)))
                .AndAlso()
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .To<HomeController>(c => c.Index()));
    }
}

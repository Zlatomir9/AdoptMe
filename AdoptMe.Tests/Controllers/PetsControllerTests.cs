namespace AdoptMe.Tests.Controllers
{
    using Xunit;
    using MyTested.AspNetCore.Mvc;
    using AdoptMe.Controllers;
    using AdoptMe.Models.Pets;
    
    public class PetsControllerTests
    {
        [Fact]
        public void AllShouldReturnViewWithModel()
            => MyController<PetsController>
                .Instance()
                .Calling(c => c.All(new AllPetsViewModel()
                {
                    SearchString = "dog",
                    PageIndex = 1,
                    Species = "dog"
                }))
                .ShouldReturn()
                .View(view => view.WithModelOfType<AllPetsViewModel>()
                                  .Passing(c => c.SearchString == "dog" &&
                                                c.Species == "dog" &&
                                                c.PageIndex == 1));

        [Fact]
        public void GetAddShoulBeForSheltersdReturnViewWithModel()
            => MyController<PetsController>
                .Instance()
                    .WithUser(x => x.InRole("Shelter"))
                .Calling(c => c.Add())
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .View(view => view.WithModelOfType<PetFormModel>());

        [Fact]
        public void MyPetsShouldReturnViewWithModel()
            => MyController<PetsController>
                .Instance(controller => controller
                    .WithUser())
                .Calling(c => c.MyPets(new AllPetsViewModel(), "Name"))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .View(view => view.WithModelOfType<AllPetsViewModel>());

        [Fact]
        public void DetailsShouldReturnCorrectView()
            => MyController<PetsController>
                .Instance()
                .Calling(c => c.Details(new PetDetailsViewModel()))
                .ShouldReturn()
                .View();

    }
}

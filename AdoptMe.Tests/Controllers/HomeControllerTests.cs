namespace AdoptMe.Tests.Controllers
{
    using MyTested.AspNetCore.Mvc;
    using Xunit;
    using AdoptMe.Controllers;

    public class HomeControllerTests
    {
        //[Fact]
        //public void IndexShouldReturnCorrectViewWithModel()
        //    => MyController<HomeController>
        //        .Instance(controller => controller
        //            .WithData())
        //        .Calling(c => c.Index())
        //        .ShouldReturn()
        //        .View(view => view
        //            .WithModelOfType<StatisticsViewModel>()
        //            .Passing(m => m.TotalPets == 1 &&
        //                          m.TotalShelters == 1 &&
        //                          m.TotalAdoptions == 1));

        [Fact]
        public void ErrorShouldReturnView()
            => MyController<HomeController>
                .Instance()
                .Calling(c => c.Error())
                .ShouldReturn()
                .View();
    }
}
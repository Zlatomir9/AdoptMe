namespace AdoptMe.Tests.Controllers
{
    using MyTested.AspNetCore.Mvc;
    using Xunit;
    using AdoptMe.Controllers;

    public class HomeControllerTests
    {
        [Fact]
        public void ErrorShouldReturnView()
            => MyController<HomeController>
                .Instance()
                .Calling(c => c.Error())
                .ShouldReturn()
                .View();
    }
}
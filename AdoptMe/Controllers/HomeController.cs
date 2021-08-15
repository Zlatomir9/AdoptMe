namespace AdoptMe.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using AdoptMe.Services.Statistics;

    public class HomeController : Controller
    {
        private readonly IStatisticsService statisticsService;

        public HomeController(IStatisticsService statisticsService)
            => this.statisticsService = statisticsService;

        public IActionResult Index()
        {
            var result = statisticsService.Total();

            return View(result);
        }

        public IActionResult Error() => View();
    }
}

namespace AdoptMe.Controllers
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using AdoptMe.Models;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

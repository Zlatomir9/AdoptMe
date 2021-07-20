namespace AdoptMe.Controllers
{
    using AdoptMe.Data;
    using AdoptMe.Models;
    using AdoptMe.Models.Home;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using System.Linq;

    public class HomeController : Controller
    {
        private readonly AdoptMeDbContext data;

        public HomeController(AdoptMeDbContext data)
            => this.data = data;

        public IActionResult Index()
        {
            var totalPets = this.data.Pets.Count();
            var totalShelters = this.data.Shelters.Count();

            var result = new IndexViewModel
            {
                TotalPets = totalPets,
                TotalShelters = totalShelters
            };

            return View(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

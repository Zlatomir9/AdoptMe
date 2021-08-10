namespace AdoptMe.Controllers
{
    using System.Linq;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using AdoptMe.Data;
    using AdoptMe.Models;
    using AdoptMe.Models.Home;

    using static Data.Models.Enums.RequestStatus;

    public class HomeController : Controller
    {
        private readonly AdoptMeDbContext data;

        public HomeController(AdoptMeDbContext data)
            => this.data = data;

        public IActionResult Index()
        {
            var totalPets = this.data.Pets.Count();
            var totalShelters = this.data.Shelters.Where(s => s.RegistrationStatus == Аccepted).Count();

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

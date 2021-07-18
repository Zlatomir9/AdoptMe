namespace AdoptMe.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Infrastructure;
    using AdoptMe.Models.Shelters;

    public class SheltersController : Controller
    {
        private readonly AdoptMeDbContext data;

        public SheltersController(AdoptMeDbContext data)
        {
            this.data = data;
        }

        [Authorize]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize]
        public IActionResult Create(CreateShelterFormModel shelter)
        {
            var userId = this.User.GetId();

            if (this.data.Shelters.Any(s => s.UserId == userId))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(shelter);
            }

            var shelterData = new Shelter
            {
                UserId = userId,
                Name = shelter.Name,
                PhoneNumber = shelter.PhoneNumber
            };

            this.data.Shelters.Add(shelterData);
            this.data.SaveChanges();

            return RedirectToAction("All", "Pets");
        }
    }
}

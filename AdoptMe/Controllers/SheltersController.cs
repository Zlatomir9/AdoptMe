namespace AdoptMe.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using AdoptMe.Data;
    using AdoptMe.Infrastructure;
    using AdoptMe.Models.Shelters;
    using AdoptMe.Services.Shelters;

    public class SheltersController : Controller
    {
        private readonly AdoptMeDbContext data;
        private readonly IShelterService shelter;

        public SheltersController(AdoptMeDbContext data, IShelterService shelter)
        {
            this.data = data;
            this.shelter = shelter;
        }

        [Authorize]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize]
        public IActionResult Create(CreateShelterFormModel shelter)
        {
            var userId = this.User.GetId();
            var shelterEmail = this.User.GetEmail();

            if (this.data.Shelters.Any(s => s.UserId == userId))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(shelter);
            }

            this.shelter.Create(
                userId,
                shelter.Name,
                shelter.PhoneNumber,
                shelterEmail);

            return RedirectToAction("All", "Pets");
        }
    }
}

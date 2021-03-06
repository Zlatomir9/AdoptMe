namespace AdoptMe.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using AdoptMe.Models.Shelters;
    using AdoptMe.Services.Shelters;
    using AdoptMe.Data.Models;

    using static Common.GlobalConstants.Roles;

    public class SheltersController : Controller
    {
        private readonly IShelterService shelterService;
        private readonly UserManager<User> userManager;


        public SheltersController(IShelterService shelterService, UserManager<User> userManager)
        {
            this.shelterService = shelterService;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult Create()
        {
            if (User.IsInRole(ShelterRoleName)
                || User.IsInRole(AdopterRoleName)
                || User.IsInRole(AdminRoleName))
            {
                return BadRequest();
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(CreateShelterFormModel shelter)
        {
            if (!ModelState.IsValid)
            {
                return View(shelter);
            }

            var userId = this.userManager.GetUserId(this.User);

            this.shelterService.Create(
                shelter.Name,
                shelter.PhoneNumber,
                shelter.CityName,
                shelter.StreetName,
                shelter.StreetNumber,
                userId);

            return RedirectToAction("Index", "Home");
        }
    }
}
